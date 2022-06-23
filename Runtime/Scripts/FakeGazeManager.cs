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
		[SerializeField, Tooltip("Enable the gaze visualizer to highlight the gaze position.")]
		private bool _gazeVisualFeedback;
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
		private Vector3 _gazeCombinedPosition;
		private GazeRay _gazeRay;
		private RaycastHit _gazeHit;

		private GameObject currentObjectLookedAt = null;

		#region Getters
		public bool gazeVisualFeedback { get => _gazeVisualFeedback; set => _gazeVisualFeedback = value; }
		public EyeData_v2 eyeData { get { return default(EyeData_v2); } }
		public Vector3 gazeCombinedPosition { get { return _gazeCombinedPosition; } }
		public GazeRay gazeRay { get { return _gazeRay; } }
		public float opennessLeft { get { return 1f; } }
		public float opennessRight { get { return 1f; } }
		public float pupilDiameterLeft { get { return 1f; } }
		public float pupilDiameterRight { get { return 1f; } }

		/// <summary>
		/// The object currently looked at, if any.
		/// Note: if a parent has a rigidbody, that gameObject will be returned instead of the one having the collider.
		/// </summary>
		public GameObject objectLookedAt { get { return currentObjectLookedAt; } }
		public Action<GameObject> objectLookedChanged { get; set; }

		#endregion

		private void Start()
		{
			_mainCamera = Camera.main;

			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void Update()
		{
			if (_updateMainCameraEachFrame)
			{
				_mainCamera = Camera.main;
			}
			SetGazeRayAndEyeOpenness();
		}

		private void SetGazeRayAndEyeOpenness()
		{
			Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			_gazeRay.origin = _mainCamera.transform.position;
			_gazeRay.direction = ray.direction;
			_gazeRay.isValid = true;

			if (Physics.Raycast(ray, out _gazeHit, maxDistance: _raycastMaxDistance, layerMask: _raycastLayerMask))
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

		private void SetPositionAndScale(GazeRay gazeRay)
		{
			Vector3 interpolatedGazeDirection = Vector3.Lerp(_lastGazeDirection, gazeRay.direction, _smoothMoveSpeed * Time.unscaledDeltaTime);

			Vector3 usedDirection = _smoothMove ? interpolatedGazeDirection.normalized : gazeRay.direction.normalized;
			transform.position = gazeRay.origin + usedDirection * gazeRay.distance;

			transform.localScale = Vector3.one * gazeRay.distance * _scaleFactor;

			transform.forward = usedDirection.normalized;

			_lastGazeDirection = usedDirection;
		}
	}
}
