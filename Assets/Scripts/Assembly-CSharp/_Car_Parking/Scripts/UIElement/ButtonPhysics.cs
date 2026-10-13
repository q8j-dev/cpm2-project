using System;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class ButtonPhysics : MonoBehaviour
	{
		public event Action<ButtonPhysics> Clicked;

		public void OnClick()
		{
			Clicked?.Invoke(this);
		}
	}
}
