using System.Collections.Generic;
using System.Text;
using CPM.Scripts.Garage;
using UnityEngine;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;
using _Car_Parking.Scripts.CarSlots;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class GarageInfoEvent : DefaultEvent<DefaultContext>
	{
		private readonly ISlotService _slotService;

		private readonly IGarageSelectorService _garageSelectorService;

		protected sealed override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"car_ids",
				ConvertCarsToString()
			},
			{
				"active_car_id",
				PlayerPrefs.GetInt("LastCarID").ToString()
			},
			{ "total_slots", _slotService.SlotCount },
			{ "used_slots", _slotService.OccupiedSlotCount },
			{
				"free_slots",
				_slotService.SlotCount - _slotService.OccupiedSlotCount
			},
			{ "car_count", _slotService.OccupiedSlotCountInAllGarages },
			{ "garage_id", _garageSelectorService.CurrentGarageIndex }
		};

		protected override bool CanSend
		{
			get
			{
				if (_slotService != null)
				{
					return _garageSelectorService != null;
				}
				return false;
			}
		}

		protected override string Name => "garage_info";

		public GarageInfoEvent(CPMAnalytics analytics, DefaultContext context)
			: base(analytics, context)
		{
			AnalyticsDependencies.TryGet<ISlotService>(out _slotService);
			AnalyticsDependencies.TryGet<IGarageSelectorService>(out _garageSelectorService);
		}

		private string ConvertCarsToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CarSlot allSlotsInfo in _slotService.AllSlotsInfos)
			{
				if (allSlotsInfo != null && !allSlotsInfo.IsEmpty)
				{
					stringBuilder.Append(allSlotsInfo.CarFilePath);
					stringBuilder.Append(",");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
