using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Police
{
	public abstract class PoliceButtonBase : PoliceUIBase
	{
		[SerializeField]
		protected Button Button;

		protected virtual void OnEnable()
		{
			Button.onClick.AddListener(ButtonOnClick);
		}

		protected virtual void OnDisable()
		{
			Button.onClick.RemoveListener(ButtonOnClick);
		}

		protected virtual void ButtonOnClick()
		{
		}
	}
}
