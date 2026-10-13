using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class ScrollViewSlots : ScrollViewDragAndDropItemContent
	{
		[SerializeField]
		private ButtonBuySlot ButtonBuySlotPrefab = default;

		[SerializeField]
		private GameObjectStateUpdaterContainer AddNewItemErrorStateUpdater = default;

		[SerializeField]
		private GameObject SwapCarTip = default;

		private int _garageIndex;

		private SlotsViewColumn.PositionType _positionType;

		private ISlotService _slotService;

		private IPrefabFactory _prefabFactory;

		private ButtonBuySlot _buttonBuySlot;

		private ManagerUI _managerUI;

		private ITimerTuningService _timerTuningService;

		[Inject]
		public void Construct(ISlotService slotService, IPrefabFactory prefabFactory, ManagerUI managerUI, ITimerTuningService timerTuningService)
		{
			_slotService = slotService;
			_prefabFactory = prefabFactory;
			_managerUI = managerUI;
			_timerTuningService = timerTuningService;
		}

		public void Init(int garageIndex, SlotsViewColumn.PositionType positionType)
		{
			_garageIndex = garageIndex;
			_positionType = positionType;
			base.ContentRectTransform.RemoveAllChild();
			SlotCollection slotCollection = _slotService.GetSlotCollection(garageIndex);
			ItemContentDraggable itemContentDraggable = null;
			for (int i = 0; i < (int)slotCollection.Count; i++)
			{
				ItemContentDraggable itemContentDraggable2 = InstantiateSlotItem(i);
				if (!itemContentDraggable && !slotCollection[i].IsEmpty)
				{
					itemContentDraggable = itemContentDraggable2;
				}
				if (slotCollection[i].ID == (int)V.SlotID)
				{
					itemContentDraggable = itemContentDraggable2;
				}
				if (_timerTuningService.IsCarBlocked(slotCollection[i].ID))
				{
					itemContentDraggable2.CantSwap = true;
					if (itemContentDraggable2.TryGetComponent<SlotIsBusySetter>(out var component))
					{
						component.LockSlot(state: true);
					}
				}
			}
			MoveScrollViewToCurrent(itemContentDraggable).Forget();
			_buttonBuySlot = Object.Instantiate(ButtonBuySlotPrefab, base.ContentRectTransform);
			_buttonBuySlot.OnClickEvent += ButtonBuySlotClickCallback;
			UpdateButtonBuySlot();
		}

		public void UpdateShowAddNewItemError(bool isOn)
		{
			if ((bool)AddNewItemErrorStateUpdater)
			{
				AddNewItemErrorStateUpdater.UpdateState(isOn);
			}
		}

		public void TryShowBuySlotMessage()
		{
			SlotCollection slotCollection = _slotService.GetSlotCollection(_garageIndex);
			if ((int)slotCollection.Count >= slotCollection.MaxCarSpotCount)
			{
				_ = _managerUI.ShowNotificationWindow(new LocalizedString("SlotManager.GarageIsFull"), 3f, NotificationType.Error);
				return;
			}
			string text = new LocalizedString("SlotUI.DoYouWantToBuyANewSlotForSlots");
			string newValue = VehicleSlotPaymentService.Price.ToString();
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(new CombinedPrice(VehicleSlotPaymentService.Price, 0));
			additionalData.SetBackground = true;
			additionalData.Background = new Color(0f, 0f, 0f, 0.7f);
			_ = _managerUI.ShowConfirmWindow(() =>
			{
				TryBuySlot().Forget();
			}, new LocalizedString("SlotUI.SlotPurchase"), text.Replace("{0}", newValue), () =>
			{
			}, "Confirm", null, additionalData);
		}

		protected override void InitStates()
		{
			base.InitStates();
			ScrollViewStateDrag obj = GetState(ScrollViewState.StateType.DragItem) as ScrollViewStateDrag;
			obj.SetExitExtraController(new SlotDragExitExtraController());
			obj.SetConditionController(new DragStateConditionControllerSlots(this, SwapCarTip));
		}

		private ItemContentDraggable InstantiateSlotItem(int index)
		{
			ItemContentDraggable itemContentDraggable = _prefabFactory.Instantiate(base.DragAndDropItemContentConfig.ItemContentDraggablePrefab, base.ContentRectTransform);
			if (itemContentDraggable.TryGetComponent<SlotItemContent>(out var component))
			{
				component.Init(_garageIndex, index, _positionType);
			}
			return itemContentDraggable;
		}

		private void UpdateButtonBuySlot()
		{
			SlotCollection slotCollection = _slotService.GetSlotCollection(_garageIndex);
			_buttonBuySlot.Init(slotCollection.Count, _positionType);
			_buttonBuySlot.gameObject.SetActive((int)slotCollection.Count < slotCollection.MaxCarSpotCount);
			_buttonBuySlot.transform.SetSiblingIndex(base.ContentRectTransform.childCount - 1);
		}

		private void ButtonBuySlotClickCallback()
		{
			TryShowBuySlotMessage();
		}

		private async UniTaskVoid TryBuySlot()
		{
			if (await _slotService.TryBuySlot(_garageIndex))
			{
				SlotCollection slotCollection = _slotService.GetSlotCollection(_garageIndex);
				InstantiateSlotItem((int)slotCollection.Count - 1);
				UpdateButtonBuySlot();
			}
		}

		private async UniTaskVoid MoveScrollViewToCurrent(ItemContentDraggable currentItem)
		{
			await UniTask.Yield();
			if ((bool)currentItem)
			{
				base.ContentRectTransform.anchoredPosition = new Vector2(base.ContentRectTransform.anchoredPosition.x, 0f - currentItem.SelfRectTransform.anchoredPosition.y - RectTransformSelf.rect.size.y / 2f);
			}
		}
	}
}
