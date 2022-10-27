namespace EyeTracking
{
	using UnityEngine;

	public struct AcquisitionData
	{
		/// <summary>
		/// The frame sequence.
		/// </summary>
		public int frameSequence;

		/// <summary>
		/// The time in ms when the frame was capturing.
		/// </summary>
		public float timestamp;
	}
}
