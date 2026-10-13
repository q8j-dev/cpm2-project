using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class ButtonBuyGarage : MonoBehaviour
	{
		[SerializeField]
		private Button Button;

		private ManagerUI _managerUI;

		private int _garageIndex = 1;

		private Action<int> _clickCallback;

		[Inject]
		public void Construct(ManagerUI managerUI)
		{
			_managerUI = managerUI;
		}

		public void Init(int index, Action<int> clickCallback)
		{
			_garageIndex = index;
			_clickCallback = clickCallback;
		}

		private void OnEnable()
		{
			Button.onClick.AddListener(() =>
			{
				_clickCallback(_garageIndex);
			});
		}

		private void OnDisable()
		{
			Button.onClick.RemoveAllListeners();
		}
	}
}
