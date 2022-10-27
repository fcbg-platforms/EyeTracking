namespace EyeTracking
{
	using System;
	using UnityEngine;
	using UnityEngine.InputSystem;
	using ViveSR.anipal.Eye;

	/// <summary>
	/// A Fake Gaze Manager that is controlled by the Mouse.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class FakeGazeManager : MonoBehaviour, IGazeManager
	{
		[Header("Settings")]
		public bool ScaleAffectedByPrecision;
		[SerializeField] private bool _smoothMove = true;

		[SerializeField][Range(1, 30)] private int _smoothMoveSpeed = 30;
		[SerializeField] private bool _updateMainCameraEachFrame;

		[Header("Eye Raycast")]
		[SerializeField] private int _raycastMaxDistance;
		[SerializeField] private LayerMask _raycastLayerMask;

		private Camera _mainCamera;

		private SpriteRenderer _spriteRenderer;
		private Vector3 _lastGazeDirection;

		//make a method to adapte his value based on sprite resolution
		private const float _scaleFactor = 0.03f;

		//these are get
		private RaycastHit _gazeHit;

		private bool _isUserDetected;

		private EyesPhysiologicalData _eyePhysiologicalData;

		private GazeData _gazeData;


		private GameObject _currentObjectLookedAt = null;

		#region interface implementation

		public bool isUserDetected { get { return _isUserDetected; } }

		public EyesPhysiologicalData eyePhysiologicalData { get { return _eyePhysiologicalData; } }

		public GazeData gazeData { get { return _gazeData; } }

		/// <summary>
		/// The object currently looked at, if any.
		/// Note: if a parent has a rigidbody, that gameObject will be returned instead of the one having the collider.
		/// </summary>
		public GameObject objectLookedAt { get { return _currentObjectLookedAt; } }
		public Action<GameObject> objectLookedChanged { get; set; }

		[Header("Debugging:")]
		[SerializeField] private bool _skipCalibration = false;

		[SerializeField, Tooltip("Enable the gaze visualizer to highlight the gaze position.")]
		private bool _gazeVisualFeedback;
		public bool gazeVisualFeedback { get => _gazeVisualFeedback; set => _gazeVisualFeedback = value; }

		#endregion

		private void Start()
		{
			_mainCamera = Camera.main;

			_spriteRenderer = GetComponent<SpriteRenderer>();
			_spriteRenderer.enabled = gazeVisualFeedback && _gazeData.isValid;
		}

		private void Update()
		{
			if (_updateMainCameraEachFrame)
			{
				_mainCamera = Camera.main;
			}
			GetEyePhysiologicalAndGazeData();
		}

		private void GetEyePhysiologicalAndGazeData()
		{
			Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			_gazeData.originWorld = _mainCamera.transform.position;
			_gazeData.directionWorld = ray.direction;
			_gazeData.isValid = true;
			_isUserDetected = true;

			if (Physics.Raycast(ray, out _gazeData.gazeHit, maxDistance: _raycastMaxDistance, layerMask: _raycastLayerMask))
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

		private void SetPositionAndScale(GazeData gazeData)
		{
			Vector3 interpolatedGazeDirection = Vector3.Lerp(_lastGazeDirection, gazeData.directionWorld, _smoothMoveSpeed * Time.unscaledDeltaTime);

			Vector3 usedDirection = _smoothMove ? interpolatedGazeDirection.normalized : gazeData.directionWorld.normalized;
			transform.position = gazeData.originWorld + usedDirection * gazeData.distance;

			transform.localScale = Vector3.one * gazeData.distance * _scaleFactor;

			transform.forward = usedDirection.normalized;

			_lastGazeDirection = usedDirection;
		}

		private void UpdateObjectLookedAt(GameObject newObject)
		{
			if (_currentObjectLookedAt != newObject)
			{
				_currentObjectLookedAt = newObject;
				if (objectLookedChanged != null)
				{
					objectLookedChanged(_currentObjectLookedAt);
				}
			}
		}
	}
}
