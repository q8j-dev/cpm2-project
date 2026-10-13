using System.Collections.Generic;
using CPM.Database;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts
{
	public static class NewSaveCarSystemExtension
	{
		private class AreCarsDesynchronizedData
		{
			[JsonProperty("cars")]
			public string Cars;

			[JsonProperty("count")]
			public int Count;
		}

		public const string DatabaseInquiryFn = "AreCarsDesynchronized24_1";

		public static async UniTask<bool> DataBaseNeedsToSyncAsync(ISlotService slotService, ICarInfoService carInfoService)
		{
			if (slotService == null || carInfoService == null)
			{
				return false;
			}
			carInfoService.CheckAllSlots();
			List<string> list = new List<string>();
			List<int> allSlots = slotService.GetAllSlots();
			for (int i = 0; i < allSlots.Count; i++)
			{
				list.Add(slotService.GetSlot(allSlots[i]).CarFilePath);
			}
			string cars = MemoryPackHelper.SerializeObject(list, ManagerKeysForPrefs.AccountId, useBrotli: true);
			FirebaseWrappedFunctionFactory firebaseWrappedFunctionFactory = new FirebaseWrappedFunctionFactory();
			AreCarsDesynchronizedData data = new AreCarsDesynchronizedData
			{
				Cars = cars,
				Count = slotService.SlotsCountInAllGarages
			};
			FunctionCallResult<int> functionCallResult = await firebaseWrappedFunctionFactory.Create("AreCarsDesynchronized24_1").CallAsyncResult<int>(data);
			return functionCallResult.IsSuccess && functionCallResult.Data == 1;
		}
	}
}
