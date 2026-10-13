using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.MenuUI
{
	[Preserve]
	public class NewCarsMarkController : IDisposable, IAllPrefsSaver
	{
		private const string PlayerPrefsKey = "IsOldCarMarkV3_";

		private const float TimeToSetOldCar = 0.75f;

		private CancellationTokenSource _cancellationTokenSourceTrySetOldCar;

		private List<int> _oldCarsSaved = new List<int>();

		private int[] _oldCars = new int[174]
		{
			59, 132, 13, 99, 152, 2, 74, 137, 77, 149,
			176, 102, 140, 146, 139, 190, 57, 133, 55, 100,
			186, 45, 29, 197, 89, 179, 106, 158, 184, 23,
			66, 53, 157, 131, 120, 163, 208, 187, 9, 21,
			39, 54, 60, 62, 121, 126, 147, 148, 151, 154,
			161, 168, 177, 180, 185, 196, 200, 206, 209, 0,
			1, 6, 8, 12, 30, 43, 81, 85, 112, 113,
			150, 160, 175, 181, 182, 183, 210, 220, 5, 11,
			17, 19, 20, 28, 35, 47, 49, 51, 56, 61,
			76, 82, 86, 88, 108, 110, 116, 127, 128, 134,
			145, 156, 189, 192, 199, 201, 205, 213, 219, 223,
			224, 14, 15, 22, 24, 31, 40, 87, 105, 111,
			118, 123, 136, 215, 3, 103, 107, 109, 114, 117,
			124, 141, 144, 153, 166, 169, 193, 211, 214, 4,
			104, 115, 130, 135, 142, 143, 172, 188, 194, 212,
			217, 221, 7, 10, 27, 32, 41, 58, 162, 178,
			198, 202, 203, 216, 167, 173, 228, 231, 225, 232,
			226, 229, 233, 230
		};

		public event Action<int> OnOldCarUpdatedEvent;

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceTrySetOldCar);
		}

		public bool IsCarOld(int carID)
		{
			if (!_oldCars.Contains(carID))
			{
				return PlayerPrefs.GetInt(GetPlayerPrefsOldCarKey(carID)) == 1;
			}
			return true;
		}

		public bool IsCarNew(int carID)
		{
			return !IsCarOld(carID);
		}

		public void TrySetOldCar(int carID)
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceTrySetOldCar);
			if (IsCarNew(carID))
			{
				_cancellationTokenSourceTrySetOldCar = new CancellationTokenSource();
				_ = SetOldCarAsync(carID, _cancellationTokenSourceTrySetOldCar.Token);
			}
		}

		public void StopTryOldCar()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceTrySetOldCar);
		}

		private async UniTaskVoid SetOldCarAsync(int carID, CancellationToken cancellationToken)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.75), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested)
			{
				PlayerPrefs.SetInt(GetPlayerPrefsOldCarKey(carID), 1);
				OnOldCarUpdatedEvent?.Invoke(carID);
			}
		}

		private string GetPlayerPrefsOldCarKey(int carID)
		{
			return "IsOldCarMarkV3_" + carID;
		}

		public void Save()
		{
			_oldCarsSaved.Clear();
			for (int i = 0; i <= (int)CarIdStorage.MAX_CAR_ID; i++)
			{
				if (PlayerPrefs.GetInt(GetPlayerPrefsOldCarKey(i)) == 1)
				{
					_oldCarsSaved.Add(i);
				}
			}
		}

		public void Load()
		{
			for (int i = 0; i < _oldCarsSaved.Count; i++)
			{
				PlayerPrefs.SetInt(GetPlayerPrefsOldCarKey(_oldCarsSaved[i]), 1);
			}
			_oldCarsSaved.Clear();
		}
	}
}
