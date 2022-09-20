namespace EyeTracking
{
	using System;
	using System.Runtime.InteropServices;
	using UnityEngine;
	using ViveSR.anipal.Eye;

	[RequireComponent(typeof(SpriteRenderer))]
	public class ViveGazeManager : MonoBehaviour, IGazeManager
	{
		[Header("Debugging:")]
		[SerializeField] private bool _skipCalibration = false;

		[SerializeField, Tooltip("Enable the gaze visualizer to highlight the gaze position.")]
		private bool _gazeVisualFeedback;
		public bool gazeVisualFeedback { get => _gazeVisualFeedback; set => _gazeVisualFeedback = value; }

		[Header("Settings")]
		public bool ScaleAffectedByPrecision;

		[SerializeField] private bool _smoothMove = true;

		[SerializeField][Range(1, 30)] private int _smoothMoveSpeed = 30;

		[Header("Eye Raycast")]
		[SerializeField] private int _raycastMaxDistance;
		[SerializeField] private LayerMask _raycastLayerMask;

		private Camera _mainCamera;

		private SpriteRenderer _spriteRenderer;
		private Vector3 _lastGazeDirection;

		// private const float OffsetFromFarClipPlane = 10f;
		// private const float PrecisionAngleScaleFactor = 5f;

		//make a method to adapte his value based on sprite resolution
		private const float _scaleFactor = 0.03f;

		private bool eye_callback_registered = false;

		private static EyeData_v2 _eyeData = new EyeData_v2();
		private Vector3 _gazeCombinedPosition;
		private GazeRay _gazeRay;
		private RaycastHit _gazeHit;

		private float _opennessLeft;
		private float _opennessRight;
		private float _pupilDiameterLeft;
		private float _pupilDiameterRight;

		private GameObject currentObjectLookedAt = null;

		#region Getters
		public EyeData_v2 eyeData { get { return _eyeData; } }
		public Vector3 gazeCombinedPosition { get { return _gazeCombinedPosition; } }
		public GazeRay gazeRay { get { return _gazeRay; } }
		public RaycastHit gazeHit { get { return _gazeHit; } }
		public float opennessLeft { get { return _opennessLeft; } }
		public float opennessRight { get { return _opennessRight; } }
		public float pupilDiameterLeft { get { return _pupilDiameterLeft; } }
		public float pupilDiameterRight { get { return _pupilDiameterRight; } }
		public GameObject objectLookedAt { get { return currentObjectLookedAt; } }
		public Action<GameObject> objectLookedChanged { get; set; }

		#endregion


		// Singleton
		public static ViveGazeManager instance = null;
		private void Awake()
		{
			if (instance == null)
				instance = this;
			else if (instance != this)
				Destroy(gameObject);
		}

		private void Start()
		{
			_mainCamera = Camera.main;

			_spriteRenderer = GetComponent<SpriteRenderer>();
			if (!gazeVisualFeedback)
			{
				_spriteRenderer.enabled = _gazeRay.isValid;
			}

			if (!SRanipal_Eye_Framework.Instance.EnableEye)
			{
				enabled = false;
				return;
			}
			else if (!_skipCalibration)
			{
				SRanipal_Eye_v2.LaunchEyeCalibration();     // Perform calibration for eye tracking.
			}
		}

		private void FixedUpdate()
		{
			EyeFramework();

			SetGazeRayAndEyeOpenness();

			//SetGaze(_gazeRay);

			//if (ScaleAffectedByPrecision && gazeModifierFilter != null)
			//{
			//    UpdatePrecisionScale(gazeModifierFilter.GetMaxPrecisionAngleDegrees(eyeTrackingData.GazeRay.Direction, worldForward));
			//}
		}

		private void EyeFramework()
		{
			switch (SRanipal_Eye_Framework.Status)
			{
				case SRanipal_Eye_Framework.FrameworkStatus.WORKING:
					break;
				case SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT:
					Debug.LogWarning("Eye Tracking not supported.");
					return;
				case SRanipal_Eye_Framework.FrameworkStatus.ERROR:
					Debug.LogWarning("Error with Eye Tracking.");
					return;
				default:
					return;
			}

			if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback && !eye_callback_registered)
			{
				SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
				eye_callback_registered = true;
			}
			else if (!SRanipal_Eye_Framework.Instance.EnableEyeDataCallback && eye_callback_registered)
			{
				SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
				eye_callback_registered = false;
			}
		}


		private void SetGazeRayAndEyeOpenness()
		{
			// -- EYE OPENNESS --

			if (eye_callback_registered)
			{
				if (!SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.LEFT, out _opennessLeft, _eyeData))
				{
					_opennessLeft = -1f;
				}
				if (!SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.RIGHT, out _opennessRight, _eyeData))
				{
					_opennessRight = -1f;
				}

				VerboseData verboseData;
				if (SRanipal_Eye_v2.GetVerboseData(out verboseData, _eyeData))
				{
					_pupilDiameterLeft = verboseData.left.pupil_diameter_mm;
					_pupilDiameterRight = verboseData.right.pupil_diameter_mm;
				}
				else
				{
					_pupilDiameterLeft = -1f;
					_pupilDiameterRight = -1f;
				}
			}
			else
			{
				if (!SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.LEFT, out _opennessLeft))
				{
					_opennessLeft = -1f;
				}
				if (!SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.RIGHT, out _opennessRight))
				{
					_opennessRight = -1f;
				}

				VerboseData verboseData;
				if (SRanipal_Eye_v2.GetVerboseData(out verboseData))
				{
					_pupilDiameterLeft = verboseData.left.pupil_diameter_mm;
					_pupilDiameterRight = verboseData.right.pupil_diameter_mm;
				}
				else
				{
					_pupilDiameterLeft = -1f;
					_pupilDiameterRight = -1f;
				}
			}


			// -- EYE DIRECTION --

			Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

			// Improve with a loop here iterating with a GazeIndex array made of combine, left and right
			if (eye_callback_registered)
			{
				if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, _eyeData))
				{ }
				else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, _eyeData))
				{ }
				else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, _eyeData))
				{ }
				else
				{
					_gazeRay.isValid = false;
					return;
				}
			}
			else
			{
				if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
				{ }
				else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
				{ }
				else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
				{ }
				else
				{
					_gazeRay.isValid = false;
					return;
				}
			}

			Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
			//_gazeRay.Origin = Camera.main.transform.position - Camera.main.transform.up * 0.05f;
			_gazeRay.origin = Camera.main.transform.position;
			_gazeRay.direction = GazeDirectionCombined;
			_gazeRay.isValid = true;

			if (Physics.Raycast(_gazeRay.origin, _gazeRay.direction, out _gazeHit, maxDistance: _raycastMaxDistance, layerMask: _raycastLayerMask))
			{
				_gazeRay.distance = _gazeHit.distance;
				if (_gazeHit.rigidbody != null && _gazeHit.rigidbody.gameObject != currentObjectLookedAt || _gazeHit.collider.gameObject != currentObjectLookedAt)
				{
					currentObjectLookedAt = _gazeHit.rigidbody != null ? _gazeHit.rigidbody.gameObject : _gazeHit.collider.gameObject;
					if (objectLookedChanged != null)
					{
						objectLookedChanged(currentObjectLookedAt);
					}
				}
			}
			else
			{
				_gazeRay.distance = _raycastMaxDistance;
				if (currentObjectLookedAt != null)
				{
					currentObjectLookedAt = null;
					if (objectLookedChanged != null)
					{
						objectLookedChanged(currentObjectLookedAt);
					}
				}
			}

			SetGaze(_gazeRay);

			if (gazeVisualFeedback)
			{
				_spriteRenderer.enabled = _gazeRay.isValid;

				if (_spriteRenderer.enabled)
				{
					SetPositionAndScale(_gazeRay);
				}
			}
			else
			{
				_spriteRenderer.enabled = false;
			}
		}

		private void SetGaze(GazeRay gazeRay)
		{
			Vector3 usedDirection = gazeRay.direction.normalized;
			_gazeCombinedPosition = gazeRay.origin + usedDirection * gazeRay.distance;
		}

		private void SetPositionAndScale(GazeRay gazeRay)
		{
			Vector3 interpolatedGazeDirection = Vector3.Lerp(_lastGazeDirection, gazeRay.direction, _smoothMoveSpeed * Time.unscaledDeltaTime);

			Vector3 usedDirection = _smoothMove ? interpolatedGazeDirection.normalized : gazeRay.direction.normalized;
			transform.position = gazeRay.origin + usedDirection * gazeRay.distance;

			transform.localScale = Vector3.one * gazeRay.distance * _scaleFactor;

			transform.forward = usedDirection.normalized;

			_lastGazeDirection = usedDirection;
		}

		// private void UpdatePrecisionScale(float maxPrecisionAngleDegrees)
		// {
		// 	transform.localScale *= (1f + GetScaleAffectedByPrecisionAngle(maxPrecisionAngleDegrees));
		// }

		// private static float GetScaleAffectedByPrecisionAngle(float maxPrecisionAngleDegrees)
		// {
		// 	return maxPrecisionAngleDegrees * Mathf.Sin(maxPrecisionAngleDegrees * Mathf.Deg2Rad) * PrecisionAngleScaleFactor;
		// }

		// private void Release()
		// {
		// 	if (eye_callback_registered == true)
		// 	{
		// 		SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
		// 		eye_callback_registered = false;
		// 	}
		// }

		private static void EyeCallback(ref EyeData_v2 eye_data)
		{
			_eyeData = eye_data;
		}
	}
}
