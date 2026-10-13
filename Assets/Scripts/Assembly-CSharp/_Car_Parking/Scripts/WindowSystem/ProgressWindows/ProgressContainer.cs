using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.WindowSystem.ProgressWindows
{
	[Preserve]
	public class ProgressContainer
	{
		public const float DelayInSecBeforeRealEnd = 2f;

		private const float CarValueSize = 0.2f;

		public float Value { get; private set; }

		public string Status { get; private set; }

		public bool IsEnded { get; private set; }

		private float CarsValue { get; set; }

		public void SetProgress(float value, string status = "")
		{
			Value = value;
			if (status != "")
			{
				Status = status;
			}
		}

		public void SetSavingProgress(float value, string status = "")
		{
			Value = value;
			if (status != "")
			{
				Status = status;
			}
		}

		public async UniTask EndAfterDelay(string status = "", bool success = true)
		{
			if (status != "")
			{
				Status = status;
			}
			Value = (success ? 1f : 0f);
			await UniTask.Delay(TimeSpan.FromSeconds(2.0));
			End();
		}

		private void End()
		{
			IsEnded = true;
		}

		public void Reset()
		{
			Value = 0f;
			IsEnded = false;
			Status = "";
		}

		public void SetCarsProgress(float progressProgressValue)
		{
			CarsValue = progressProgressValue;
			Value += CarsValue * 0.2f / 0.2f;
		}
	}
}
