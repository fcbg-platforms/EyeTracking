namespace EyeTracking
{
	using System;
	using UnityEngine;
	using ViveSR.anipal.Eye;

	public interface IGazeManager
	{
		bool isUserDetected { get; }

		EyesPhysiologicalData eyePhysiologicalData { get; }

		GazeData gazeData { get; }

		GameObject objectLookedAt { get; }
		Action<GameObject> objectLookedChanged { get; }

		/// <summary>
		/// the data from the aqcuisition
		/// </summary>
		/// <value></value>
		AcquisitionData acquisitionData { get; }

		// Debug
		bool gazeVisualFeedback { get; set; }

	}
}
