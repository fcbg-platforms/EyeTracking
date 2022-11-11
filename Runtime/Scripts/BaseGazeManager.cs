namespace EyeTracking
{
	using System;
	using System.Runtime.InteropServices;
	using UnityEngine;

	[RequireComponent(typeof(SpriteRenderer))]
	public abstract class BaseGazeManager : MonoBehaviour, IGazeManager
	{
		[Header("Sprite movement")]

		[SerializeField] protected bool _smoothMove = true;

		[SerializeField][Range(1, 30)] protected int _smoothMoveSpeed = 30;

		[Header("Eye Raycast")]
		[SerializeField] protected int _raycastMaxDistance;
		[SerializeField] protected LayerMask _raycastLayerMask;

		protected Camera _mainCamera;

		protected SpriteRenderer _spriteRenderer;
		protected Vector3 _lastGazeDirection;

		// private const float OffsetFromFarClipPlane = 10f;
		// private const float PrecisionAngleScaleFactor = 5f;

		//make a method to adapte his value based on sprite resolution
		protected const float _scaleFactor = 0.03f;

		protected bool eyeCallbackRegistered = false;

		protected bool _isUserDetected;

		protected EyesPhysiologicalData _eyePhysiologicalData;

		protected GazeData _gazeData;

		protected GameObject _currentObjectLookedAt = null;
		protected string _currentObjectLookedAtLabel;

		protected AcquisitionData _acquisitionData;

		#region interface implementation

		public bool isUserDetected { get { return _isUserDetected; } }

		public EyesPhysiologicalData eyePhysiologicalData { get { return _eyePhysiologicalData; } }

		public GazeData gazeData { get { return _gazeData; } }
		public GameObject objectLookedAt { get { return _currentObjectLookedAt; } }
		public string objectLookedAtLabel { get { return _currentObjectLookedAtLabel; } }
		public Action<GameObject> objectLookedChanged { get; set; }

		public AcquisitionData acquisitionData { get { return _acquisitionData; } }

		public Action eyeDataUpdated { get; set; }

		[Header("Debugging:")]

		[SerializeField, Tooltip("Enable the gaze visualizer to highlight the gaze position.")]
		protected bool _gazeVisualFeedback;
		public bool gazeVisualFeedback { get => _gazeVisualFeedback; set => _gazeVisualFeedback = value; }

		#endregion

		#region Unity callbacks

		protected virtual void Start()
		{
			_mainCamera = Camera.main;

			_spriteRenderer = GetComponent<SpriteRenderer>();
			_spriteRenderer.enabled = gazeVisualFeedback && _gazeData.isValid;
		}

		#endregion

		public abstract void UpdateEyePhysiologicalAndGazeData();

		protected virtual void SetPositionAndScale(GazeData gazeData)
		{
			Vector3 interpolatedGazeDirection = Vector3.Lerp(_lastGazeDirection, gazeData.directionWorld, _smoothMoveSpeed * Time.unscaledDeltaTime);

			Vector3 usedDirection = _smoothMove ? interpolatedGazeDirection.normalized : gazeData.directionWorld.normalized;
			transform.position = gazeData.originWorld + usedDirection * gazeData.distance;

			transform.localScale = Vector3.one * gazeData.distance * _scaleFactor;

			transform.forward = usedDirection.normalized;

			_lastGazeDirection = usedDirection;
		}

		protected void UpdateObjectLookedAt(GameObject newObject)
		{
			if (_currentObjectLookedAt != newObject)
			{
				_currentObjectLookedAt = newObject;
				_currentObjectLookedAtLabel = newObject != null ? newObject.name : string.Empty;
				if (objectLookedChanged != null)
				{
					objectLookedChanged(_currentObjectLookedAt);
				}
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Called when the script is loaded or a value is changed in the
		/// inspector (Called in the editor only).
		/// </summary>
		protected virtual void OnValidate()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			if (gazeVisualFeedback)
			{
				_spriteRenderer.enabled = _gazeData.isValid;

				if (_spriteRenderer.enabled)
				{
					SetPositionAndScale(_gazeData);
				}
			}
			else
			{
				_spriteRenderer.enabled = false;
			}
		}
#endif
	}
}
