using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class ButtonOpenSlotManager : MonoBehaviour
	{
		[SerializeField]
		private Button Button = default;

		private ManagerUI _managerUI;

		[Inject]
		public void Construct(ManagerUI managerUI)
		{
			_managerUI = managerUI;
		}

		private void OnEnable()
		{
			Button.onClick.AddListener(ButtonClickCallback);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(ButtonClickCallback);
		}

		private void ButtonClickCallback()
		{
			_managerUI.CloseWindow("UserBalanceWindow");
			_managerUI.ShowWindow("SlotManagerWindow").Forget();
		}
	}
}
