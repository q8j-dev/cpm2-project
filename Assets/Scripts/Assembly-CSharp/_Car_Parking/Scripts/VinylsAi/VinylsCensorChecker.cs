using System;
using CPM.Scripts.VinylDrawer;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.VinylsAi
{
	public class VinylsCensorChecker
	{
		private bool _isInProgress;

		private VinylsCheckApi _vinylsCheckApi;

		private VehicleData _vehicleData;

		public VinylsCensorChecker(VinylsCheckApi vinylsCheckApi, VehicleData vehicleData)
		{
			_vinylsCheckApi = vinylsCheckApi;
			_vehicleData = vehicleData;
		}

		public async UniTask<bool> IsVinylSafe(VinylsType vinylType)
		{
			if (_vehicleData == null)
			{
				return true;
			}
			return vinylType switch
			{
				VinylsType.CarVinyls => await CheckTexture(_vehicleData.VinylsComposition.RenderTexture), 
				VinylsType.WindowVinyls => await CheckTexture(_vehicleData.WindowComposition.WindowRT), 
				_ => true, 
			};
		}

		public async UniTask<bool> CheckTexture(RenderTexture rt)
		{
			if (rt == null)
			{
				return true;
			}
			while (_isInProgress)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.800000011920929));
			}
			_isInProgress = true;
			bool isSafe = false;
			try
			{
				isSafe = await _vinylsCheckApi.GetProbability(rt).Timeout(TimeSpan.FromSeconds(4.0)) < _vinylsCheckApi.UnsafeLimitValue;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			_isInProgress = false;
			return isSafe;
		}
	}
}
