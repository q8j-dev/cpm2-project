using System;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class ButtonRemoveCarV2 : MonoBehaviour
	{
		private enum StateView
		{
			None = 0,
			InActive = 1,
			Drag = 2,
			RemoveCarAction = 3
		}

		private static float TimerToDeleteCar = 5f;

		private ISlotService _slotService;

		private ManagerUI _managerUI;

		private ICarInfoService _carInfoService;

		private WindowScript _windowScript;

		private CancellationTokenSource _cancellationTokenSourceCheck;

		[SerializeField]
		private GameObject RemoveCarCenterText;

		[SerializeField]
		private GameObjectStateUpdater StateUpdater;

		[SerializeField]
		private ScrollViewSlots[] ScrollViewSlots;

		[SerializeField]
		protected RectTransformInCanvasSpace RectTransformInCanvasSpace;

		private StateView _stateView;

		protected RectTransform CanvasRectTransform
		{
			get
			{
				if (ScrollViewSlots.Length == 0 || !ScrollViewSlots[0])
				{
					return null;
				}
				return ScrollViewSlots[0].CanvasRectTransform;
			}
		}

		private bool MousePositionContainsSelf => RectTransformInCanvasSpace.CanvasRect.Contains(GetMousePosition());

		private bool HaveDragState
		{
			get
			{
				ScrollViewSlots[] scrollViewSlots = ScrollViewSlots;
				for (int i = 0; i < scrollViewSlots.Length; i++)
				{
					ScrollViewState currentState = scrollViewSlots[i].CurrentState;
					if (currentState != null && currentState.State == ScrollViewState.StateType.DragItem)
					{
						return true;
					}
				}
				return false;
			}
		}

		[Inject]
		public void Construct(ISlotService slotService, ManagerUI managerUI, ICarInfoService carInfoService)
		{
			_slotService = slotService;
			_managerUI = managerUI;
			_carInfoService = carInfoService;
		}

		private void OnEnable()
		{
			_stateView = StateView.None;
			ScrollViewSlots[] scrollViewSlots = ScrollViewSlots;
			foreach (ScrollViewSlots obj in scrollViewSlots)
			{
				obj.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Combine(obj.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(ScrollViewSlotsStateUpdatedCallback));
			}
			RecalculateStateView();
		}

		private void OnDisable()
		{
			ScrollViewSlots[] scrollViewSlots = ScrollViewSlots;
			foreach (ScrollViewSlots obj in scrollViewSlots)
			{
				obj.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Remove(obj.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(ScrollViewSlotsStateUpdatedCallback));
			}
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheck);
		}

		private void ScrollViewSlotsStateUpdatedCallback(ScrollViewBase scrollView, ScrollViewBase.ScrollViewStateChangeData scrollViewStateChangeData)
		{
			if (scrollViewStateChangeData.OldState != null && scrollViewStateChangeData.OldState.State == ScrollViewState.StateType.DragItem && MousePositionContainsSelf)
			{
				RemoveCar((ItemContentDraggableAnimation)scrollView.ActiveItem);
			}
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheck);
			if (HaveDragState)
			{
				_cancellationTokenSourceCheck = new CancellationTokenSource();
				RecalculateStateViewAsync(_cancellationTokenSourceCheck.Token).Forget();
			}
			RecalculateStateView();
		}

		private void RecalculateStateView()
		{
			StateView stateView = StateView.None;
			stateView = ((!HaveDragState) ? StateView.InActive : ((!MousePositionContainsSelf) ? StateView.Drag : StateView.RemoveCarAction));
			if (stateView != _stateView)
			{
				_stateView = stateView;
				StateUpdater.UpdateState(stateView == StateView.Drag || stateView == StateView.RemoveCarAction);
				RemoveCarCenterText.SetActive(_stateView == StateView.RemoveCarAction);
			}
		}

		private void RemoveCar(ItemContentDraggableAnimation item)
		{
			SlotItemContent slotItemContent = item.GetComponent<SlotItemContent>();
			if (!slotItemContent || !item)
			{
				return;
			}
			CarSlot slot = _slotService.GetSlot(slotItemContent.CarInstanceId);
			if (slot == null)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < ScrollViewSlots.Length; i++)
			{
				SlotItemContent[] componentsInChildren = ScrollViewSlots[i].GetComponentsInChildren<SlotItemContent>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (!string.IsNullOrEmpty(componentsInChildren[j].CarInstanceId))
					{
						num++;
					}
				}
			}
			if (num <= 1)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("GarageMenuWindow.Can'tremovelastcarfromgarage"), 3f).Forget();
				return;
			}
			Texture texture = Resources.Load(PathGetterHelper.GetCarPreviewPath(_carInfoService.GetFromSlot(slot.ID).CarID), typeof(Texture)) as Texture;
			ShowUI(state: false);
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(new Color(0f, 0f, 0f, 0.75f));
			additionalData.ConfirmTimer = new ConfirmWindow.AdditionalData.ConfirmTimerData(isEnabled: true, TimerToDeleteCar);
			_managerUI.ShowConfirmWindow(() =>
			{
				if (item.IsSelected)
				{
					ScrollViewBase componentInParent = item.GetComponentInParent<ScrollViewBase>();
					item.TryRevertSelect();
					componentInParent.OnItemSelectChanged(item);
				}
				item.ContentVisibleUpdate(visible: false);
				item.EmptyVisibleUpdate(visible: true);
				slotItemContent.ClearCarInstanceId();
				SlotsViewColumn componentInParent2 = slotItemContent.GetComponentInParent<SlotsViewColumn>();
				if ((bool)componentInParent2)
				{
					componentInParent2.UpdateOccupiedFromUI();
				}
				ShowUI(state: true);
			}, new LocalizedString("GarageMenuWindow.RemoveCar"), new LocalizedString("GarageMenuWindow.AreYouSureYouWantToRemoveThisC"), () =>
			{
				ShowUI(state: true);
			}, new LocalizedString("MarketInventoryPopup.Confirm"), texture, additionalData).Forget();
		}

		private void ShowUI(bool state)
		{
			if (!_windowScript)
			{
				_windowScript = GetComponentInParent<WindowScript>();
			}
			if ((bool)_windowScript)
			{
				_windowScript.ShowUI(state);
			}
		}

		private async UniTask RecalculateStateViewAsync(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				RecalculateStateView();
				await UniTask.Yield();
			}
		}

		private Vector2 GetMousePosition()
		{
			if (!CanvasRectTransform)
			{
				return Vector2.zero;
			}
			return Vector2.Scale(new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height), CanvasRectTransform.sizeDelta);
		}
	}
}
