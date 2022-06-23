namespace EyeTracking
{
	using UnityEngine;

	public struct GazeRay
	{
		/// <summary>
		/// Unit vector describing the direction of the eye.
		/// </summary>
		public Vector3 direction;

		/// <summary>
		/// True when the origin and direction of the gaze ray is valid.
		/// </summary>
		public bool isValid;

		/// <summary>
		/// The 3D position of the origin of the gaze ray given in meters.
		/// </summary>
		public Vector3 origin;

		/// <summary>
		/// The distance position from the origin of the gaze ray given in meters.
		/// </summary>
		public float distance;
	}
}
