using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace CPM.Scripts.Garage
{
	public interface IGarageSelectorService
	{
		int CurrentSceneIndex { get; }

		int CurrentGarageIndex { get; }

		float LoadProgress { get; }

		bool IsGarageSwitching { get; }

		event Action<int> OnCurrentGarageIndexChanged;

		UniTask<bool> ChangeMenuGarage(int id, bool withScreen = false, bool checkCurrentIndex = true);

		UniTask LoadCurrentGarageAsync(params object[] instances);

		UniTask ActivateWarmedGarageAsync(AsyncOperation warmedOperation, params object[] instances);

		void ChangeGarageIndex(int garageIndex);

		int GetGarageSceneIndex(int garageId);

		void Register(ProgressContainer progressContainer);
	}
}
