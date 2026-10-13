using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class ScrollViewStateDrag : ScrollViewStateDragBase
	{
		private ScrollViewBase _newScrollView;

		private DragStateConditionController _conditionController;

		private DragStateExitExtraController _exitExtraController;

		private bool _isSwapItem;

		public override StateType State => StateType.DragItem;

		public ScrollViewBase OriginalScrollView { get; private set; }

		public override bool CanChangeActiveItem => false;

		public ScrollViewBase NewScrollView
		{
			get
			{
				return _newScrollView;
			}
			set
			{
				_newScrollView = value;
				_conditionController?.Init((ScrollViewDragAndDropItemContent)_newScrollView);
			}
		}

		public ItemContentDraggable NewItemContentDraggable { get; set; }

		public bool IsSwapItem
		{
			get
			{
				return _isSwapItem;
			}
			set
			{
				if (_isSwapItem != value)
				{
					_isSwapItem = value;
					_conditionController?.OnDataUpdated();
				}
			}
		}

		public ItemContentDraggable ActiveItemScrollViewItem
		{
			get
			{
				if (!base.ActiveItem)
				{
					return null;
				}
				return (ItemContentDraggable)base.ActiveItem;
			}
		}

		private DragAndDropItemContentConfig DragAndDropItemContentConfig => (DragAndDropItemContentConfig)base.Config;

		public event Action<ScrollViewStateDrag> OnSwapEvent;

		public ScrollViewStateDrag(ScrollViewDragAndDropItemContent scrollView)
			: base(scrollView)
		{
			AddModule(new DragItemModuleMoveItem(scrollView, this));
			AddModule(new DragItemModuleScrollScrollView(scrollView, this));
			AddModule(new DragItemModuleSetNewItem(scrollView, this));
			AddModule(new DragItemModuleClearSelectItems(scrollView, this));
		}

		public void SetConditionController(DragStateConditionController conditionController)
		{
			_conditionController = conditionController;
		}

		public void SetExitExtraController(DragStateExitExtraController exitExtraController)
		{
			_exitExtraController = exitExtraController;
		}

		public override void OnEnter()
		{
			IsSwapItem = false;
			_conditionController?.Enter();
			if ((bool)base.ActiveItem)
			{
				OriginalScrollView = scrollView;
				ActiveItemScrollViewItem.StartDrag();
				ActiveItemScrollViewItem.ContentRectTransform.transform.SetParent(scrollView.TransformTop);
			}
			NewScrollView = null;
			NewItemContentDraggable = null;
			base.OnEnter();
		}

		public override void OnExit()
		{
			if ((bool)base.ActiveItem)
			{
				ActiveItemScrollViewItem.StopDragBegin();
				bool flag = true;
				bool flag2 = false;
				if ((bool)NewScrollView && (bool)NewItemContentDraggable && NewItemContentDraggable.IsEmpty && NewItemContentDraggable != ActiveItemScrollViewItem)
				{
					if (_conditionController == null || _conditionController.CanAddToEmpty)
					{
						NewItemContentDraggable.CopyContent(ActiveItemScrollViewItem);
						NewItemContentDraggable.ContentVisibleUpdate(visible: true);
						NewItemContentDraggable.EmptyVisibleUpdate(visible: false);
						ActiveItemScrollViewItem.ContentVisibleUpdate(visible: false);
						ActiveItemScrollViewItem.EmptyVisibleUpdate(visible: true);
						_exitExtraController?.MoveToEmpty(ActiveItemScrollViewItem, NewItemContentDraggable);
						flag = false;
					}
					else
					{
						_conditionController?.FailToDragEmpty();
					}
				}
				else if ((bool)NewScrollView && (bool)NewItemContentDraggable && !NewItemContentDraggable.IsEmpty)
				{
					if (IsSwapItem)
					{
						if (NewItemContentDraggable.CantSwap)
						{
							NewItemContentDraggable.SelectContentVisibleUpdate(visible: false);
							flag = true;
							flag2 = true;
						}
						else
						{
							ScrollViewDragAndDropItemContentHelper.SwapItems(NewItemContentDraggable, ActiveItemScrollViewItem);
							OnSwapEvent?.Invoke(this);
						}
					}
					else if (_conditionController == null || _conditionController.CanAddNewItem)
					{
						int num = NewItemContentDraggable.transform.GetSiblingIndex();
						if (NewItemContentDraggable.AddItemView.CurrentAddType == AddItemViewController.AddType.Front)
						{
							num++;
						}
						int num2 = num;
						ItemContentDraggable[] componentsInChildren = NewScrollView.ContentRectTransform.GetComponentsInChildren<ItemContentDraggable>();
						for (int i = 0; i < componentsInChildren.Length * 2; i++)
						{
							int num3 = num + ((i % 2 != 0) ? 1 : (-1)) * (i / 2);
							ItemContentDraggable itemContentDraggable = componentsInChildren[Mathf.Clamp(num3, 0, componentsInChildren.Length - 1)];
							if (itemContentDraggable.IsEmpty || itemContentDraggable == ActiveItemScrollViewItem)
							{
								num2 = num3;
								break;
							}
						}
						int num4 = num;
						if (num4 > num2)
						{
							num4--;
						}
						flag = false;
						for (int j = 0; j < Mathf.Abs(num4 - num2); j++)
						{
							int value = num4 + j * ((num4 <= num2) ? 1 : (-1));
							ItemContentDraggable itemContentDraggable2 = componentsInChildren[Mathf.Clamp(value, 0, componentsInChildren.Length - 1)];
							if ((bool)itemContentDraggable2 && itemContentDraggable2.CantSwap)
							{
								flag = true;
								flag2 = true;
							}
						}
						if (!flag)
						{
							ItemContentDraggable itemContentDraggable3 = UnityEngine.Object.Instantiate(DragAndDropItemContentConfig.ItemContentDraggablePrefab, ActiveItemScrollViewItem.ContentRectTransform.position, ActiveItemScrollViewItem.ContentRectTransform.rotation, NewScrollView.ContentRectTransform);
							itemContentDraggable3.transform.SetSiblingIndex(num);
							itemContentDraggable3.CopyContent(ActiveItemScrollViewItem);
							itemContentDraggable3.ContentVisibleUpdate(visible: true);
							itemContentDraggable3.EmptyVisibleUpdate(visible: false);
							itemContentDraggable3.NewItemAnimation();
							ActiveItemScrollViewItem.ContentVisibleUpdate(visible: false);
							ActiveItemScrollViewItem.EmptyVisibleUpdate(visible: true);
							componentsInChildren = NewScrollView.ContentRectTransform.GetComponentsInChildren<ItemContentDraggable>();
							ItemContentDraggable removedItem = null;
							for (int k = 0; k < componentsInChildren.Length * 2; k++)
							{
								int value2 = num + ((k % 2 == 0) ? 1 : (-1)) * (k / 2);
								ItemContentDraggable itemContentDraggable4 = componentsInChildren[Mathf.Clamp(value2, 0, componentsInChildren.Length - 1)];
								if (itemContentDraggable4.IsEmpty)
								{
									removedItem = itemContentDraggable4;
									itemContentDraggable4.DestroyAnimation();
									break;
								}
							}
							_exitExtraController?.MoveToNewPlace(ActiveItemScrollViewItem, itemContentDraggable3, NewScrollView, removedItem);
						}
					}
					else
					{
						_conditionController?.FailToDragNewPlace();
					}
				}
				if (flag)
				{
					ActiveItemScrollViewItem.EmptyVisibleUpdate(visible: false);
				}
				if (flag2 && (bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI)
				{
					V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("CarBlock.CarInService"), 3f, NotificationType.Error).Forget();
				}
				ActiveItemScrollViewItem.StopDragEnd();
			}
			base.OnExit();
			_conditionController?.Exit();
			OriginalScrollView = null;
			NewScrollView = null;
			NewItemContentDraggable = null;
			IsSwapItem = false;
		}

		public override void OnItemUp()
		{
			base.OnItemUp();
			scrollView.SetState(StateType.Idle);
		}

		public override void OnPointerUp()
		{
			base.OnPointerUp();
			scrollView.SetState(StateType.Idle);
		}
	}
}
