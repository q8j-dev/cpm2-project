using System;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.EngineTuning.DataTypes
{
	public interface IPaintDataService
	{
		event Action<CarMatType> OnBought;

		bool CheckPaintBought(CarMatType paintType);

		void SetPaintBought(CarMatType paintType);
	}
}
