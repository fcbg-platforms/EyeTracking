namespace EyeTracking
{
	using GameLibrary.SOWorkflowCommon.Variables;
	using UnityEngine;

	public class GazeManagerSetter : MonoBehaviour
	{
		[SerializeField]
		private GazeManagerAnchor _gazeManagerAnchor;

		[SerializeField]
		private BoolVariableSO _useEyeTrackingVariableSO;

		[Header("Prefabs")]
		[SerializeField]
		private ViveGazeManager _gazeManagerPrefab;
		[SerializeField]
		private FakeGazeManager _fakeGazeManagerPrefab;

		public void Awake()
		{
			GameObject gm = Instantiate(
				_useEyeTrackingVariableSO.runtimeValue ? _gazeManagerPrefab.gameObject : _fakeGazeManagerPrefab.gameObject,
				default(Vector3),
				default(Quaternion),
				transform);
			_gazeManagerAnchor.Provide(gm.GetComponent<IGazeManager>());
		}
	}
}
