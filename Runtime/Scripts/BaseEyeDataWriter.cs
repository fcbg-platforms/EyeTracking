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

			float time { get; set; }
			//  float luminance,
			bool isUserDetected { get; set; }

			int acquisitionDataSensorFrameSequence { get; set; }
			float acquisitionDatasensorTimestamp { get; set; }

			float leftEyeOpenness { get; set; }
			float rightEyeOpenness { get; set; }

			float leftEyeWide { get; set; }
			float rightEyeWide { get; set; }
			float leftEyeSqueeze { get; set; }
			float rightEyeSqueeze { get; set; }
			float leftEyeFrown { get; set; }
			float rightEyeFrown { get; set; }

			float leftEyePupilDiameter { get; set; }
			float rightEyePupilDiameter { get; set; }

			Vector2 leftEyePupilPositionInSensorArea { get; set; }
			Vector2 rightEyePupilPositionInSensorArea { get; set; }

			bool gazeDataIsValid { get; set; }
			Vector3 gazeDataOriginLocal { get; set; }
			Vector3 gazeDataDirectionLocal { get; set; }
			Vector3 gazeDataOriginWorld { get; set; }
			Vector3 gazeDataDirectionWorld { get; set; }
			float gazeDataDistance { get; set; }
			Vector3 gazeDataHitPoint { get; set; }

			string objectLookedAtLabel { get; set; }
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
