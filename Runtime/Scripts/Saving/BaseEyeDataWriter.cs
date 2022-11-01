namespace EyeTracking.Saving
{
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class BaseEyeDataWriter : MonoBehaviour
	{
		[SerializeField]
		protected float _deltaTimeWritingFile = 1f;

		protected List<EyeDataOutput> _pendingEyeDataToWrite;

		protected float _lastTimeWrite;

		protected virtual void Awake()
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
