using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public abstract class SlotManagerUIButtonBase : MonoBehaviour
	{
		[SerializeField]
		protected ButtonWithState Button;

		protected SlotManagerSelectController _slotManagerSelectController;

		protected abstract int MinSelectedCountToWork { get; }

		public virtual void Init(SlotManagerSelectController slotManagerSelectController)
		{
			_slotManagerSelectController = slotManagerSelectController;
			if (_slotManagerSelectController != null)
			{
				_slotManagerSelectController.OnSelectCountUpdatedEvent += SelectCountUpdatedCallback;
				SelectCountUpdatedCallback(_slotManagerSelectController.SelectedCount);
			}
		}

		protected virtual void ButtonClickCallback(ButtonWithState button)
		{
		}

		private void OnEnable()
		{
			if ((bool)Button)
			{
				Button.ClickedEvent += ButtonClickCallback;
			}
			if (_slotManagerSelectController != null)
			{
				SelectCountUpdatedCallback(_slotManagerSelectController.SelectedCount);
			}
		}

		private void OnDisable()
		{
			if ((bool)Button)
			{
				Button.ClickedEvent -= ButtonClickCallback;
			}
		}

		private void OnDestroy()
		{
			if (_slotManagerSelectController != null)
			{
				_slotManagerSelectController.OnSelectCountUpdatedEvent -= SelectCountUpdatedCallback;
			}
		}

		private void SelectCountUpdatedCallback(int count)
		{
			if ((bool)Button)
			{
				Button.Interactable = count >= MinSelectedCountToWork;
			}
		}
	}
}
