namespace EyeTracking
{
	using System;
	using UnityEngine;
	using ViveSR.anipal.Eye;

	/// <summary>
	/// Interface that must be implemented for gaze managers, to have a standardized output.
	/// </summary>
	public interface IGazeManager
	{
		bool isUserDetected { get; }

		/// <summary>
		/// the data from the aqcuisition
		/// </summary>
		/// <value></value>
		AcquisitionData acquisitionData { get; }

		EyesPhysiologicalData eyePhysiologicalData { get; }

		HeadData headData { get; }

		GazeData gazeData { get; }

		GameObject objectLookedAt { get; }
		string objectLookedAtLabel { get; }
		Action<GameObject> objectLookedChanged { get; set; }

		Action eyeDataUpdated { get; set; }

		// Debug
		bool gazeVisualFeedback { get; set; }
	}
}
