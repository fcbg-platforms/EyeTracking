namespace EyeTracking
{
	using UnityEngine;

	public struct GazeData
	{
		/// <summary>
		/// The frame sequence.
		/// </summary>
		public int frame_sequence;

		/// <summary>
		/// The time in ms when the frame was capturing.
		/// </summary>
		public int timestamp;
	}
}
