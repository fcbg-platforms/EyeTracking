namespace EyeTracking
{
	using UnityEngine;

	public struct GazeData
	{
		/// <summary>
		/// True when the origin and direction of the gaze ray is valid.
		/// </summary>
		public bool isValid;

		/// <summary>
		/// The 3D position of the origin of the gaze ray given in meters.
		/// </summary>
		public Vector3 originLocal;

		/// <summary>
		/// Unit vector describing the direction of the eye.
		/// </summary>
		public Vector3 directionLocal;

		/// <summary>
		/// The 3D position of the origin of the gaze ray given in meters.
		/// </summary>
		public Vector3 originWorld;

		/// <summary>
		/// Unit vector describing the direction of the eye.
		/// </summary>
		public Vector3 directionWorld;

		/// <summary>
		/// The global coordinates of the raycastHit.
		/// </summary>
		public RaycastHit gazeHit;

		/// <summary>
		/// The distance position from the origin of the gaze ray given in meters.
		/// </summary>
		public float distance;
	}
}
