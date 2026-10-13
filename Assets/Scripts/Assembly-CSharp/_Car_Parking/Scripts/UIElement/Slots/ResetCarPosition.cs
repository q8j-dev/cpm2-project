using System;
using System.Threading;
using CPM.Database;
using CPM.Scripts.Garage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class ResetCarPosition : MonoBehaviour
	{
		private enum StateView
		{
			None = 0,
			InActive = 1,
			Drag = 2,
			RemoveCarAction = 3
		}

		private const string MarketInventoryPopupConfirm = "MarketInventoryPopup.Confirm";

		private const string GarageMenuWindowResetCarPosition = "GarageMenuWindow.ResetCarPosition";

		private const string GarageMenuWindowResetAllCarsPositions = "GarageMenuWindow.ResetAllCarsPositions";

		private const string GarageMenuWindowAreYouSureYouWantToResetAllC = "GarageMenuWindow.AreYouSureYouWantToResetAllC";

		private const string GarageMenuWindowAreYouSureYouWantToResetThisC = "GarageMenuWindow.AreYouSureYouWantToResetThisC";

		[SerializeField]
		private Button ButtonResetCarPositions;

		[SerializeField]
		private GameObject ResetCarCenterText;

		[SerializeField]
		private GameObjectStateUpdater[] StateUpdaters;

		[SerializeField]
		private ScrollViewSlots[] ScrollViewSlots;

		[SerializeField]
		protected RectTransformInCanvasSpace RectTransformInCanvasSpace;

		private ISlotService _slotService;

		private ManagerUI _managerUI;

		private ICarInfoService _carInfoService;

		private WindowScript _windowScript;

		private CancellationTokenSource _cancellationTokenSourceCheck;

		private IGarageEntity _currentGarage;

		private StateView _stateView;

		private readonly Color _backgroundColor = new Color(0f, 0f, 0f, 0.75f);

		private RectTransform CanvasRectTransform
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
		public void Construct(ISlotService slotService, ManagerUI managerUI, ICarInfoService carInfoService, IGarageEntity currentGarage)
		{
			_slotService = slotService;
			_managerUI = managerUI;
			_carInfoService = carInfoService;
			_currentGarage = currentGarage;
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
			ButtonResetCarPositions.onClick.AddListener(ResetAllCarsPositionsClickCallback);
		}

		private void OnDisable()
		{
			ScrollViewSlots[] scrollViewSlots = ScrollViewSlots;
			foreach (ScrollViewSlots obj in scrollViewSlots)
			{
				obj.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Remove(obj.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(ScrollViewSlotsStateUpdatedCallback));
			}
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheck);
			ButtonResetCarPositions.onClick.RemoveListener(ResetAllCarsPositionsClickCallback);
		}

		private void ScrollViewSlotsStateUpdatedCallback(ScrollViewBase scrollView, ScrollViewBase.ScrollViewStateChangeData scrollViewStateChangeData)
		{
			if (scrollViewStateChangeData.OldState != null && scrollViewStateChangeData.OldState.State == ScrollViewState.StateType.DragItem && MousePositionContainsSelf)
			{
				ResetSelectedCarPosition((ItemContentDraggableAnimation)scrollView.ActiveItem);
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
				GameObjectStateUpdater[] stateUpdaters = StateUpdaters;
				for (int i = 0; i < stateUpdaters.Length; i++)
				{
					stateUpdaters[i].UpdateState(stateView == StateView.Drag || stateView == StateView.RemoveCarAction);
				}
				ResetCarCenterText.SetActive(_stateView == StateView.RemoveCarAction);
			}
		}

		private void ResetAllCarsPositionsClickCallback()
		{
			ShowUI(state: false);
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(_backgroundColor);
			_managerUI.ShowConfirmWindow(() =>
			{
				_currentGarage.ResetAllCarsPositionsInCurrentGarage();
				ShowUI(state: true);
			}, new LocalizedString("GarageMenuWindow.ResetAllCarsPositions"), new LocalizedString("GarageMenuWindow.AreYouSureYouWantToResetAllC"), () =>
			{
				ShowUI(state: true);
			}, new LocalizedString("MarketInventoryPopup.Confirm"), null, additionalData).Forget();
		}

		private void ResetSelectedCarPosition(ItemContentDraggableAnimation item)
		{
			SlotItemContent component = item.GetComponent<SlotItemContent>();
			if (!component || !item)
			{
				return;
			}
			CarSlot slot = _slotService.GetSlot(component.CarInstanceId);
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
			Texture texture = Resources.Load(PathGetterHelper.GetCarPreviewPath(_carInfoService.GetFromSlot(slot.ID).CarID), typeof(Texture)) as Texture;
			VehicleData vehicleData = _currentGarage.GetCarBySpot(slot.ID);
			ShowUI(state: false);
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(_backgroundColor);
			_managerUI.ShowConfirmWindow(() =>
			{
				if (item.IsSelected)
				{
					ScrollViewBase componentInParent = item.GetComponentInParent<ScrollViewBase>();
					item.TryRevertSelect();
					componentInParent.OnItemSelectChanged(item);
				}
				_currentGarage.ResetSavedCarPosition(vehicleData);
				ShowUI(state: true);
			}, new LocalizedString("GarageMenuWindow.ResetCarPosition"), new LocalizedString("GarageMenuWindow.AreYouSureYouWantToResetThisC"), () =>
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
