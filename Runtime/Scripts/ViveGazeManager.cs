namespace EyeTracking
{
	using System;
	using System.Runtime.InteropServices;
	using UnityEngine;
	using ViveSR.anipal.Eye;

	[RequireComponent(typeof(SpriteRenderer))]
	public class ViveGazeManager : BaseGazeManager
	{
		// private const float OffsetFromFarClipPlane = 10f;
		// private const float PrecisionAngleScaleFactor = 5f;

		//make a method to adapte his value based on sprite resolution

		[Header("Debugging:")]
		[SerializeField] private bool _skipCalibration = false;
		private EyeData_v2 _eyeData = new EyeData_v2();

		public EyeData_v2 eyeData { get => _eyeData; }

		protected override void Start()
		{
			base.Start();

			if (!SRanipal_Eye_Framework.Instance.EnableEye)
			{
				enabled = false;
				return;
			}
			else if (!_skipCalibration)
			{
				SRanipal_Eye_v2.LaunchEyeCalibration();     // Perform calibration for eye tracking.
			}

			EyeFramework();
		}

		protected virtual void FixedUpdate()
		{
			if (eyeCallbackRegistered)
			{
				UpdateEyePhysiologicalAndGazeData();
			}

			if (_gazeData.isValid)
			{
				if (Physics.Raycast(_gazeData.originWorld, _gazeData.directionWorld, out _gazeData.gazeHit, maxDistance: _raycastMaxDistance, layerMask: _raycastLayerMask))
				{
					_gazeData.distance = _gazeData.gazeHit.distance;
					if (_gazeData.gazeHit.rigidbody != null && _gazeData.gazeHit.rigidbody.gameObject != _currentObjectLookedAt || _gazeData.gazeHit.collider.gameObject != _currentObjectLookedAt)
					{
						UpdateObjectLookedAt(_gazeData.gazeHit.rigidbody != null ? _gazeData.gazeHit.rigidbody.gameObject : _gazeData.gazeHit.collider.gameObject);
					}
				}
				else
				{
					_gazeData.distance = _raycastMaxDistance;
					UpdateObjectLookedAt(null);
				}

				if (gazeVisualFeedback)
				{
					_spriteRenderer.enabled = _gazeData.isValid;

					if (_spriteRenderer.enabled)
					{
						SetPositionAndScale(_gazeData);
					}
				}
			}
		}

		protected virtual void EyeFramework()
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
					Debug.LogWarning(string.Format("SRanipal status not recognized: {0}.", SRanipal_Eye_Framework.Status));
					return;
			}

			if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback && !eyeCallbackRegistered)
			{
				SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
				eyeCallbackRegistered = true;
			}
			else if (!SRanipal_Eye_Framework.Instance.EnableEyeDataCallback && eyeCallbackRegistered)
			{
				SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
				eyeCallbackRegistered = false;
			}
		}

		public override void UpdateEyePhysiologicalAndGazeData()
		{
			// -- EYE PHYSIOLOGICAL data --
			_gazeData.isValid = false;

			if (eyeCallbackRegistered || SRanipal_Eye_API.GetEyeData_v2(ref _eyeData) == ViveSR.Error.WORK)
			{
				_eyePhysiologicalData.leftEyePhysiologicalData.pupilDiameter = _eyeData.verbose_data.left.pupil_diameter_mm;
				_eyePhysiologicalData.rightEyePhysiologicalData.pupilDiameter = _eyeData.verbose_data.right.pupil_diameter_mm;

				_eyePhysiologicalData.leftEyePhysiologicalData.pupilPositionInSensorArea = _eyeData.verbose_data.left.pupil_position_in_sensor_area;
				_eyePhysiologicalData.rightEyePhysiologicalData.pupilPositionInSensorArea = _eyeData.verbose_data.right.pupil_position_in_sensor_area;

				_eyePhysiologicalData.leftEyePhysiologicalData.eyeOpenness = _eyeData.verbose_data.left.eye_openness;
				_eyePhysiologicalData.rightEyePhysiologicalData.eyeOpenness = _eyeData.verbose_data.right.eye_openness;

				_eyePhysiologicalData.leftEyePhysiologicalData.eyeFrown = _eyeData.expression_data.left.eye_frown;
				_eyePhysiologicalData.leftEyePhysiologicalData.eyeSqueeze = _eyeData.expression_data.left.eye_wide;
				_eyePhysiologicalData.leftEyePhysiologicalData.eyeWide = _eyeData.expression_data.left.eye_frown;
				_eyePhysiologicalData.rightEyePhysiologicalData.eyeFrown = _eyeData.expression_data.right.eye_frown;
				_eyePhysiologicalData.rightEyePhysiologicalData.eyeSqueeze = _eyeData.expression_data.right.eye_wide;
				_eyePhysiologicalData.rightEyePhysiologicalData.eyeWide = _eyeData.expression_data.right.eye_frown;
			}
			else
			{
				_eyePhysiologicalData.leftEyePhysiologicalData.pupilDiameter = -1f;
				_eyePhysiologicalData.rightEyePhysiologicalData.pupilDiameter = -1f;

				_eyePhysiologicalData.leftEyePhysiologicalData.pupilPositionInSensorArea = new Vector2(-1f, -1f);
				_eyePhysiologicalData.rightEyePhysiologicalData.pupilPositionInSensorArea = new Vector2(-1f, -1f);

				_eyePhysiologicalData.leftEyePhysiologicalData.eyeOpenness = -1f;
				_eyePhysiologicalData.rightEyePhysiologicalData.eyeOpenness = -1f;
			}


			// -- EYE DIRECTION --

			// TODO: Improve with a loop here iterating with a GazeIndex array made of combine, left and right

			// if (eye_callback_registered)
			// {

			// EYE DATA already updated in the previous section (eye physiological), so no need for another fetch
			if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out _gazeData.originLocal, out _gazeData.directionLocal, eyeData))
			{ }
			else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out _gazeData.originLocal, out _gazeData.directionLocal, eyeData))
			{ }
			else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out _gazeData.originLocal, out _gazeData.directionLocal, eyeData))
			{ }
			else
			{
				return;
			}
			// }
			// else
			// {
			// 	if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out _gazeOriginCombinedLocal, out _gazeDirectionCombinedLocal))
			// 	{ }
			// 	else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out _gazeOriginCombinedLocal, out _gazeDirectionCombinedLocal))
			// 	{ }
			// 	else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out _gazeOriginCombinedLocal, out _gazeDirectionCombinedLocal))
			// 	{ }
			// 	else
			// 	{
			// 		_gazeRay.isValid = false;
			// 		return;
			// 	}
			// }

			if (_mainCamera == null)
			{
				_mainCamera = Camera.main;
			}
			_gazeData.originWorld = _mainCamera.transform.position;
			_gazeData.directionWorld = _mainCamera.transform.TransformDirection(_gazeData.directionLocal);
			_gazeData.isValid = true;
			_isUserDetected = _eyeData.no_user;

			_acquisitionData.sensorFrameSequence = _eyeData.frame_sequence;
			_acquisitionData.sensorTimestamp = _eyeData.timestamp;

			if (eyeDataUpdated != null)
			{
				eyeDataUpdated();
			}
		}

		// private void UpdatePrecisionScale(float maxPrecisionAngleDegrees)
		// {
		// 	transform.localScale *= (1f + GetScaleAffectedByPrecisionAngle(maxPrecisionAngleDegrees));
		// }

		// private static float GetScaleAffectedByPrecisionAngle(float maxPrecisionAngleDegrees)
		// {
		// 	return maxPrecisionAngleDegrees * Mathf.Sin(maxPrecisionAngleDegrees * Mathf.Deg2Rad) * PrecisionAngleScaleFactor;
		// }

		protected virtual void Release()
		{
			if (eyeCallbackRegistered)
			{
				SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
				eyeCallbackRegistered = false;
			}
		}

		protected virtual void EyeCallback(ref EyeData_v2 eyeData)
		{
			_eyeData = eyeData;

			UpdateEyePhysiologicalAndGazeData();
		}
	}
}
