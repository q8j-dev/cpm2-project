using System;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public interface IVisualTuningViewModel
	{
		event Action OnValuesChanged;

		void Execute(string scope);
	}
}
