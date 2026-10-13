using System;
using UnityEngine;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public interface IColorPicker
	{
		event Action<Color> ColorChanged;

		void Select(Color newColor);

		void ChangeColorName(string newName);
	}
}
