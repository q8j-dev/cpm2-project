using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.MenuUI.Person
{
	public class PackCategory : MonoBehaviour
	{
		public Toggle Toggle;

		[field: SerializeField]
		public int PackId { get; private set; }

		public event Action<int> OnSelectedPack;

		public void Init()
		{
			Toggle.onValueChanged.AddListener(OnToggle);
		}

		private void OnToggle(bool state)
		{
			if (state)
			{
				OnSelectedPack?.Invoke(PackId);
			}
		}

		private void OnDisable()
		{
			if ((bool)Toggle)
			{
				Toggle.isOn = false;
			}
		}

		private void OnDestroy()
		{
			if ((bool)Toggle)
			{
				Toggle.onValueChanged.RemoveAllListeners();
			}
			OnSelectedPack = null;
		}
	}
}
