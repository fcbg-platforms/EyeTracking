namespace EyeTracking
{
	using System;
	using UnityEngine;
	using ViveSR.anipal.Eye;

	public interface IGazeManager
	{
		bool gazeVisualFeedback { get; set; }
		EyeData_v2 eyeData { get; }
		Vector3 gazeCombinedPosition { get; }
		GazeRay gazeRay { get; }
		float opennessLeft { get; }
		float opennessRight { get; }
		float pupilDiameterLeft { get; }
		float pupilDiameterRight { get; }
		GameObject objectLookedAt { get; }
		Action<GameObject> objectLookedChanged { get; set; }
	}
}
