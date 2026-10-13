using System;
using UnityEngine;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Infrastructure
{
	public static class GlobalEventSystem
	{
		public static Action<bool> OnCleanUI;

		public static Action<bool> OnGamePaused;

		public static Action OnBuyThisCarWindowClosed;

		public static Action<TradeOffer> OnNewOffer;

		public static Action OnHudClicked;

		public static event Action<VehicleData> OnCarForcePositioned;

		public static event Action<VehicleData> OnCarWheelsInstalled;

		public static event Action<TesterType, float> OnTesterCollisionIgnored;

		public static event Func<SunAndSkyController> OnSunControllerRequested;

		public static event Func<TimeAndWeatherController> OnTimeControllerRequested;

		public static event Action<int, VehicleData, bool> OnPlateCreated;

		public static event Action<bool> OnOnlineVinylsVisibilityChanged;

		public static event Action<CPMGraphicsSettings.AdditionalEffects> OnGraphicsChanged;

		public static event Func<string, Shader> OnShaderRequired;

		public static event Action<bool> OnActivateInSceneProps;

		public static event Action<bool, int, bool> OnCity2PartActivated;

		public static event Action<TimeAttackContentData> OnTimeAttackDataUpdate;

		public static event Action<bool, int, string> OnDragRacingPropsActivate;

		public static event Action<bool> OnSpeedUnitChanged;

		public static void InvokeCarForcePositioned(VehicleData vehicleData)
		{
			OnCarForcePositioned?.Invoke(vehicleData);
		}

		public static void InvokeCarWheelsInstalled(VehicleData vehicleData)
		{
			OnCarWheelsInstalled?.Invoke(vehicleData);
		}

		public static void InvokeTesterCollisionIgnore(TesterType type, float duration)
		{
			OnTesterCollisionIgnored?.Invoke(type, duration);
		}

		public static SunAndSkyController RequestSunController()
		{
			return OnSunControllerRequested?.Invoke();
		}

		public static TimeAndWeatherController RequestTimeController()
		{
			return OnTimeControllerRequested?.Invoke();
		}

		public static void InvokePlateCreated(int plateId, VehicleData vehicleData, bool isFront)
		{
			OnPlateCreated?.Invoke(plateId, vehicleData, isFront);
		}

		public static void InvokeOnlineVinylsVisibilityChanged(bool show)
		{
			OnOnlineVinylsVisibilityChanged?.Invoke(show);
		}

		public static void InvokeGraphicsChanged(CPMGraphicsSettings.AdditionalEffects additionalEffects)
		{
			OnGraphicsChanged?.Invoke(additionalEffects);
		}

		public static void ClearGraphicsChangedEvent()
		{
			OnGraphicsChanged = null;
		}

		public static Shader RequireShader(string shaderName)
		{
			return OnShaderRequired?.Invoke(shaderName);
		}

		public static void ActivateInSceneProps(bool show)
		{
			OnActivateInSceneProps?.Invoke(show);
		}

		public static void ActivateCity2Part(bool isPart, int partId, bool show)
		{
			OnCity2PartActivated?.Invoke(isPart, partId, show);
		}

		public static void UpdateTimeAttackContentData(TimeAttackContentData data)
		{
			OnTimeAttackDataUpdate?.Invoke(data);
		}

		public static void InvokeDragRacingPropsActivate(bool state, int distanceIndex, string racingTreeName = "")
		{
			OnDragRacingPropsActivate?.Invoke(state, distanceIndex, racingTreeName);
		}

		public static void InvokeSpeedUnitChanged(bool isKph)
		{
			OnSpeedUnitChanged?.Invoke(isKph);
		}
	}
}
