namespace EyeTracking
{
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class BaseEyeDataWriter : MonoBehaviour
	{
		public class EyeDataOutput
		{
			public EyeDataOutput(
				float time,
				//  float luminance,
				bool isUserDetected,

				int acquisitionDataSensorFrameSequence,
				float acquisitionDatasensorTimestamp,

				float leftEyeOpenness,
				float rightEyeOpenness,

				float leftEyeWide,
				float rightEyeWide,
				float leftEyeSqueeze,
				float rightEyeSqueeze,
				float leftEyeFrown,
				float rightEyeFrown,

				float leftEyePupilDiameter,
				float rightEyePupilDiameter,

				Vector2 leftEyePupilPositionInSensorArea,
				Vector2 rightEyePupilPositionInSensorArea,

				bool gazeDataIsValid,
				Vector3 gazeDataOriginLocal,
				Vector3 gazeDataDirectionLocal,
				Vector3 gazeDataOriginWorld,
				Vector3 gazeDataDirectionWorld,
				float gazeDataDistance,
				Vector3 gazeDataHitPoint,

				string objectLookedAtLabel)
			{
				this.time = time;
				//  float luminance,
				this.isUserDetected = isUserDetected;

				this.acquisitionDataSensorFrameSequence = acquisitionDataSensorFrameSequence;
				this.acquisitionDatasensorTimestamp = acquisitionDatasensorTimestamp;

				this.leftEyeOpenness = leftEyeOpenness;
				this.rightEyeOpenness = rightEyeOpenness;

				this.leftEyeWide = leftEyeWide;
				this.rightEyeWide = rightEyeWide;
				this.leftEyeSqueeze = leftEyeSqueeze;
				this.rightEyeSqueeze = rightEyeSqueeze;
				this.leftEyeFrown = leftEyeFrown;
				this.rightEyeFrown = rightEyeFrown;

				this.leftEyePupilDiameter = leftEyePupilDiameter;
				this.rightEyePupilDiameter = rightEyePupilDiameter;

				this.leftEyePupilPositionInSensorArea = leftEyePupilPositionInSensorArea;
				this.rightEyePupilPositionInSensorArea = rightEyePupilPositionInSensorArea;

				this.gazeDataIsValid = gazeDataIsValid;
				this.gazeDataOriginLocal = gazeDataOriginLocal;
				this.gazeDataDirectionLocal = gazeDataDirectionLocal;
				this.gazeDataOriginWorld = gazeDataOriginWorld;
				this.gazeDataDirectionWorld = gazeDataDirectionWorld;
				this.gazeDataDistance = gazeDataDistance;
				this.gazeDataHitPoint = gazeDataHitPoint;

				this.objectLookedAtLabel = objectLookedAtLabel;
			}

			public EyeDataOutput(float time, IGazeManager iGazeManager)
			{
				this.time = time;
				//  float luminance,
				this.isUserDetected = iGazeManager.isUserDetected;

				this.acquisitionDataSensorFrameSequence = iGazeManager.acquisitionData.sensorFrameSequence;
				this.acquisitionDatasensorTimestamp = iGazeManager.acquisitionData.sensorTimestamp;

				this.leftEyeOpenness = iGazeManager.eyePhysiologicalData.leftEyePhysiologicalData.eyeOpenness;
				this.rightEyeOpenness = iGazeManager.eyePhysiologicalData.rightEyePhysiologicalData.eyeOpenness;

				this.leftEyeWide = leftEyeWide;
				this.rightEyeWide = rightEyeWide;
				this.leftEyeSqueeze = leftEyeSqueeze;
				this.rightEyeSqueeze = rightEyeSqueeze;
				this.leftEyeFrown = leftEyeFrown;
				this.rightEyeFrown = rightEyeFrown;

				this.leftEyePupilDiameter = leftEyePupilDiameter;
				this.rightEyePupilDiameter = rightEyePupilDiameter;

				this.leftEyePupilPositionInSensorArea = leftEyePupilPositionInSensorArea;
				this.rightEyePupilPositionInSensorArea = rightEyePupilPositionInSensorArea;

				this.gazeDataIsValid = iGazeManager.gazeData.isValid;
				this.gazeDataOriginLocal = iGazeManager.gazeData.originLocal;
				this.gazeDataDirectionLocal = iGazeManager.gazeData.directionLocal;
				this.gazeDataOriginWorld = iGazeManager.gazeData.originWorld;
				this.gazeDataDirectionWorld = iGazeManager.gazeData.directionWorld;
				this.gazeDataDistance = iGazeManager.gazeData.distance;
				this.gazeDataHitPoint = iGazeManager.gazeData.gazeHit.point;

				this.objectLookedAtLabel = iGazeManager.objectLookedAt.name;
			}

			public float time { get; set; }
			//  float luminance,
			public bool isUserDetected { get; set; }

			public int acquisitionDataSensorFrameSequence { get; set; }
			public float acquisitionDatasensorTimestamp { get; set; }

			public float leftEyeOpenness { get; set; }
			public float rightEyeOpenness { get; set; }

			public float leftEyeWide { get; set; }
			public float rightEyeWide { get; set; }
			public float leftEyeSqueeze { get; set; }
			public float rightEyeSqueeze { get; set; }
			public float leftEyeFrown { get; set; }
			public float rightEyeFrown { get; set; }

			public float leftEyePupilDiameter { get; set; }
			public float rightEyePupilDiameter { get; set; }

			public Vector2 leftEyePupilPositionInSensorArea { get; set; }
			public Vector2 rightEyePupilPositionInSensorArea { get; set; }

			public bool gazeDataIsValid { get; set; }
			public Vector3 gazeDataOriginLocal { get; set; }
			public Vector3 gazeDataDirectionLocal { get; set; }
			public Vector3 gazeDataOriginWorld { get; set; }
			public Vector3 gazeDataDirectionWorld { get; set; }
			public float gazeDataDistance { get; set; }
			public Vector3 gazeDataHitPoint { get; set; }

			public string objectLookedAtLabel { get; set; }
		}

		[SerializeField]
		protected float _deltaTimeWritingFile = 1f;

		protected List<EyeDataOutput> _pendingEyeDataToWrite;

		protected float _lastTimeWrite;

		protected virtual void Start()
		{
			_pendingEyeDataToWrite = new List<EyeDataOutput>();
			_lastTimeWrite = Time.time;
		}

		protected virtual void Update()
		{
			// We write the data if needed
			if (Time.time > _lastTimeWrite + _deltaTimeWritingFile)
			{
				WritePendingDataToDisk();
			}
		}

		void OnApplicationQuit()
		{
			Debug.LogWarning("BaseEyeDataWriter: ApplicationQuit detected, we write the pending data.");
			WritePendingDataToDisk();
		}

		public abstract void AddEyeData(EyeDataOutput eyeData);
		public abstract void WritePendingDataToDisk();
	}
}
