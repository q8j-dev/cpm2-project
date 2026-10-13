using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Free_drive.Scenes
{
	public class CarSellTriggersCollection
	{
		private readonly Dictionary<int, SellCarTriggerData> _sellCarTriggersData = new Dictionary<int, SellCarTriggerData>();

		private readonly CommonProvider _commonProvider;

		[Inject]
		public CarSellTriggersCollection(CommonProvider commonProvider)
		{
			_commonProvider = commonProvider;
		}

		public SellCarTriggerData GetSellCarTriggerData(int index)
		{
			if (!_sellCarTriggersData.TryGetValue(index, out var value))
			{
				value = new SellCarTriggerData(index, _commonProvider);
				_sellCarTriggersData[index] = value;
			}
			return value;
		}
	}
}
