using System;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public interface IVisualTuningUpdateAction
	{
		event Action OnValuesChanged;
	}
}
