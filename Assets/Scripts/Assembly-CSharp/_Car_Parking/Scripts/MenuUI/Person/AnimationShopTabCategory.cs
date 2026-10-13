using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.MenuUI.Person
{
	public class AnimationShopTabCategory : MonoBehaviour
	{
		public Toggle Toggle;

		[field: SerializeField]
		public AnimationShopTab Tab { get; private set; }

		public event Action<AnimationShopTab> OnSelectedTab;

		public void Init()
		{
			Toggle.onValueChanged.AddListener(OnToggle);
		}

		public void SetSelectedSilent(bool selected)
		{
			if ((bool)Toggle)
			{
				Toggle.SetIsOnWithoutNotify(selected);
			}
		}

		private void OnToggle(bool state)
		{
			if (state)
			{
				OnSelectedTab?.Invoke(Tab);
			}
		}

		private void OnDestroy()
		{
			if ((bool)Toggle)
			{
				Toggle.onValueChanged.RemoveAllListeners();
			}
			OnSelectedTab = null;
		}
	}
}
