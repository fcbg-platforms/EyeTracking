namespace EyeTracking
{
	using System;
	using UnityEngine;
	using UnityEngine.InputSystem;
	using ViveSR.anipal.Eye;

	/// <summary>
	/// A Fake Gaze Manager that can be used on a desktop.
	/// The gaze is controlled by the Mouse.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class FakeGazeManager : BaseGazeManager
	{
		[SerializeField] private bool _updateMainCameraEachFrame;

		protected override void Update()
		{
			if (_updateMainCameraEachFrame)
			{
				_mainCamera = Camera.main;
			}
			base.Update();
			UpdateEyePhysiologicalAndGazeData();
		}

		/// <inheritdoc/>
		public override void UpdateEyePhysiologicalAndGazeData()
		{
			Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
			_gazeData.originWorld = _mainCamera.transform.position;
			_gazeData.directionWorld = ray.direction;
			_gazeData.isValid = true;
			_isUserDetected = true;

			_acquisitionData.sensorFrameSequence = Time.frameCount;
			_acquisitionData.sensorTimestamp = Time.time;

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

			if (eyeDataUpdated != null)
			{
				eyeDataUpdated();
			}
		}
	}
}
