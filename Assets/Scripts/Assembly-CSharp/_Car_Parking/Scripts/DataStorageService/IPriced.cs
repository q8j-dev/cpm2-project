using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.DataStorageService
{
	public interface IPriced
	{
		CostType PriceType { get; }

		ObscuredInt GetPrice { get; }
	}
}
