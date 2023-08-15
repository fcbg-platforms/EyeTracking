namespace EyeTracking
{
	using UnityEngine;

	/// <summary>
	/// Contains the data of the head position/orientation.
	/// </summary>
	public struct HeadData
	{
		/// <summary>
		/// The position in the world, of the head.
		/// </summary>
		public Vector3 positionWorld;

		/// <summary>
		/// Unit vector of the forward direction of the head.
		/// </summary>
		public Vector3 forwardDirectionWorld;
	}
}
