namespace EyeTracking
{
	using UnityEngine;

	public struct SingleEyePhysiologicalData
	{
		/// <summary>
		/// The eye openness, exprimed between 0 (closed) and 1 (open).
		/// Results from eyeWide and eyeSqueeze
		/// </summary>
		public float eyeOpenness;

		/// <summary>
		/// How the eye is open wide, exprimed between 0 and 1.
		/// </summary>
		public float eyeWide;

		/// <summary>
		/// How the eye is closed tighlty, exprimed between 0 and 1.
		/// </summary>
		public float eyeSqueeze;

		/// <summary>
		/// How the eye is frown, exprimed between 0 and 1.
		/// </summary>
		public float eyeFrown;

		/// <summary>
		/// The pupil diameter, exprimed in mm.
		/// </summary>
		public float pupilDiameter;

		/// <summary>
		/// The normalized position of a pupil inthe sensor area, exprimed as a Vector2 between 0 and 1.
		/// </summary>
		public Vector2 pupilPositionInSensorArea;
	}

	public struct EyesPhysiologicalData
	{
		public SingleEyePhysiologicalData leftEyePhysiologicalData;
		public SingleEyePhysiologicalData rightEyePhysiologicalData;
	}
}
