using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Persistence.Systems;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.EventSystem.View.Elements;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts.EventSystem.View.ButtonActions
{
	[Preserve]
	public class ClaimRewardAction : IBannerButtonAction
	{
		private const string AccountSectionName = "Account";

		private const string WaitingWindowName = "WaitingWindow";

		private const int StylePullAttempts = 5;

		private const int StylePullDelayMs = 1500;

		private readonly IEventService _eventService;

		private readonly CommonProvider _commonProvider;

		private readonly ICarInfoService _carInfoService;

		public BannerButtonKind Kind => BannerButtonKind.ClaimReward;

		public ClaimRewardAction(IEventService eventService, CommonProvider commonProvider, ICarInfoService carInfoService)
		{
			_eventService = eventService;
			_commonProvider = commonProvider;
			_carInfoService = carInfoService;
		}

		public void Execute(BannerActionContext context)
		{
			if (!ManagerKeysForPrefs.IsSteamLinked)
			{
				OpenAccountSettings(context);
			}
			else if (IsCurrentGarageFull())
			{
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("SlotManager.GarageIsFull"), 3f, NotificationType.Error).Forget();
			}
			else
			{
				ClaimAsync(context).Forget();
			}
		}

		private bool IsCurrentGarageFull()
		{
			SlotCollection slotCollection = _commonProvider?.SlotService?.SlotCollection;
			if (slotCollection == null || slotCollection.TryGetEmptySlot(out var _))
			{
				return false;
			}
			return slotCollection.IsFull;
		}

		private bool HasEmptySlot()
		{
			CarSlot res;
			return (_commonProvider?.SlotService?.SlotCollection)?.TryGetEmptySlot(out res) ?? true;
		}

		private HashSet<string> SnapshotCarPaths()
		{
			HashSet<string> hashSet = new HashSet<string>();
			ISlotService slotService = _commonProvider?.SlotService;
			if (slotService?.AllSlotsInfos == null)
			{
				return hashSet;
			}
			foreach (CarSlot allSlotsInfo in slotService.AllSlotsInfos)
			{
				if (allSlotsInfo != null && !allSlotsInfo.IsEmpty && !string.IsNullOrEmpty(allSlotsInfo.CarFilePath))
				{
					hashSet.Add(allSlotsInfo.CarFilePath);
				}
			}
			return hashSet;
		}

		private async UniTask PullGrantedCarFromDatabase(HashSet<string> knownCarPaths)
		{
			if (_commonProvider?.DataBase == null)
			{
				return;
			}
			for (int attempt = 0; attempt < 5; attempt++)
			{
				if (attempt > 0)
				{
					await UniTask.Delay(1500);
				}
				_commonProvider.DataBase.DatabaseCarsService.DeleteData();
				await _commonProvider.DataBase.LoadCarSlots();
				await _commonProvider.DataBase.LoadCars();
				_carInfoService?.ClearPool();
				if (GrantedCarHasStyle(knownCarPaths))
				{
					break;
				}
			}
		}

		private static async UniTaskVoid HoldWaitingWindow(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI && !V.CoreGameManager.ManagerUI.IsWindowActiveOrOpening("WaitingWindow"))
				{
					V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true).Forget();
				}
				await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			}
		}

		private static void RepaintGrantedCar()
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component) && component.carInfo != null)
			{
				if ((bool)component.CarMat)
				{
					ColorsService.SetCarPaintColor(component.CarMat, component.carInfo.Colors.CarColor);
				}
				component.VinylsComposition?.RenderVinyls(VinylsType.CarVinyls, isFrontPlate: false);
				component.VinylsComposition?.RenderVinyls(VinylsType.WindowVinyls, isFrontPlate: false);
			}
		}

		private string DescribeGrantedCar(HashSet<string> knownCarPaths)
		{
			ISlotService slotService = _commonProvider?.SlotService;
			if (slotService?.AllSlotsInfos == null)
			{
				return "no slot service";
			}
			foreach (CarSlot allSlotsInfo in slotService.AllSlotsInfos)
			{
				if (allSlotsInfo != null && !allSlotsInfo.IsEmpty && !string.IsNullOrEmpty(allSlotsInfo.CarFilePath) && !knownCarPaths.Contains(allSlotsInfo.CarFilePath))
				{
					return allSlotsInfo.CarFilePath;
				}
			}
			return "not in slots yet";
		}

		private bool GrantedCarHasStyle(HashSet<string> knownCarPaths)
		{
			ISlotService slotService = _commonProvider?.SlotService;
			if (slotService?.AllSlotsInfos == null)
			{
				return true;
			}
			foreach (CarSlot allSlotsInfo in slotService.AllSlotsInfos)
			{
				if (allSlotsInfo != null && !allSlotsInfo.IsEmpty && !string.IsNullOrEmpty(allSlotsInfo.CarFilePath) && !knownCarPaths.Contains(allSlotsInfo.CarFilePath))
				{
					return EasySaveDataHandler.FileExists(DirectoryHelper.GetCarVinylsPath(allSlotsInfo.CarFilePath));
				}
			}
			return false;
		}

		private static string FailureMessageKey(FunctionStatus status)
		{
			return status switch
			{
				FunctionStatus.EventsSteamIdNotLinked => "EventError.SteamNotLinked", 
				FunctionStatus.EventsSteamAwardNotAvailable => "EventError.RewardNotAvailable", 
				FunctionStatus.EventsWrongPlatform => "CarShop.AvailableOnlyOnSteam", 
				_ => "DebugAccountData.SomethingWentWrong", 
			};
		}

		private static void OpenAccountSettings(BannerActionContext context)
		{
			context?.CloseBanner?.Invoke();
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI)
			{
				V.CoreGameManager.ManagerUI.CloseWindow("MainMenuWindow");
				V.CoreGameManager.ManagerUI.ShowSettingsWindow("Account");
			}
		}

		private async UniTaskVoid ClaimAsync(BannerActionContext context)
		{
			if (context?.Event == null)
			{
				return;
			}
			bool hadEmptySlot = HasEmptySlot();
			HashSet<string> carsBeforeClaim = SnapshotCarPaths();
			ClaimRewardResult claimRewardResult = await _eventService.ClaimReward(context.Event.Type);
			if (!claimRewardResult.Success)
			{
				FunctionStatus statusCode = (FunctionStatus)claimRewardResult.StatusCode;
				if (hadEmptySlot)
				{
					V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString(FailureMessageKey(statusCode)), 3f, NotificationType.Error).Forget();
				}
				return;
			}
			CancellationTokenSource waitingWindowCts = new CancellationTokenSource();
			HoldWaitingWindow(waitingWindowCts.Token).Forget();
			await PullGrantedCarFromDatabase(carsBeforeClaim);
			RepaintGrantedCar();
			waitingWindowCts.Cancel();
			waitingWindowCts.Dispose();
			V.CoreGameManager.ManagerUI.CloseWaitingWindow();
			EventView.Claimed?.Invoke();
			context.CloseBanner?.Invoke();
		}
	}
}
