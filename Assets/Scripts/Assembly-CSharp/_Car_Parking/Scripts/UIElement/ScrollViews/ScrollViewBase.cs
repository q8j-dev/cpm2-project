using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public abstract class ScrollViewBase : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IScrollHandler
	{
		public enum MoveAxisType
		{
			Horizontal = 0,
			Vertical = 1
		}

		private class CashedItems
		{
			private ScrollViewItemBase[] _items;

			public ScrollViewItemBase[] GetItems(Transform content)
			{
				if (!content)
				{
					_items = new ScrollViewItemBase[0];
					return _items;
				}
				bool flag = _items == null || _items.Length != content.childCount;
				if (!flag)
				{
					for (int i = 0; i < _items.Length; i++)
					{
						if (!_items[i])
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					_items = content.GetComponentsInChildren<ScrollViewItemBase>();
				}
				return _items;
			}
		}

		public struct ScrollViewStateChangeData
		{
			public ScrollViewState OldState;

			public ScrollViewState NewState;
		}

		public Action<ScrollViewBase, ScrollViewItemInteractiveBase, bool> OnItemSelectChangedEvent;

		public Action<ScrollViewBase, ScrollViewStateChangeData> OnStateChangedEvent;

		[SerializeField]
		protected RectTransform RectTransformSelf;

		[SerializeField]
		protected RectTransform Content;

		[SerializeField]
		protected HorizontalOrVerticalLayoutGroup ContentGroup;

		[SerializeField]
		protected MoveAxisType MoveAxis;

		[SerializeField]
		protected RectTransformInCanvasSpace RectTransformInCanvasSpace;

		[SerializeField]
		private ScrollViewConfig ScrollViewConfig = default;

		[SerializeField]
		private RectTransform TopTransform;

		private List<ScrollViewState> _states = new List<ScrollViewState>();

		private ScrollViewState _currentState;

		private ScrollViewItemInteractiveBase _activeItem;

		private CashedItems _cashedItems = new CashedItems();

		public MoveAxisType TypeMoveAxis => MoveAxis;

		public ScrollViewConfig Config => ScrollViewConfig;

		public RectTransform ContentRectTransform => Content;

		public RectTransformInCanvasSpace TransformInCanvasSpace => RectTransformInCanvasSpace;

		public RectTransform CanvasRectTransform
		{
			get
			{
				if (!RectTransformInCanvasSpace)
				{
					return null;
				}
				return RectTransformInCanvasSpace.CanvasRectTransform;
			}
		}

		public RectTransform TransformTop
		{
			get
			{
				if (!TopTransform)
				{
					return CanvasRectTransform;
				}
				return TopTransform;
			}
		}

		public float ContentSpaceBetweenItems
		{
			get
			{
				if (!ContentGroup)
				{
					return 0f;
				}
				return ContentGroup.spacing;
			}
		}

		public Rect CanvasRect
		{
			get
			{
				if (!RectTransformInCanvasSpace)
				{
					return Rect.zero;
				}
				return RectTransformInCanvasSpace.CanvasRect;
			}
		}

		public ScrollViewItemInteractiveBase ActiveItem => _activeItem;

		public ScrollViewItemBase[] Items => _cashedItems.GetItems(Content);

		public ScrollViewState CurrentState => _currentState;

		public void SetTopTransform(RectTransform topTransform)
		{
			TopTransform = topTransform;
		}

		public void OnItemSelectChanged(ScrollViewItemInteractiveBase item)
		{
			OnItemSelectChangedEvent?.Invoke(this, item, item.IsSelected);
		}

		public void OnItemDown(ScrollViewItemInteractiveBase item)
		{
			if (_currentState == null || _currentState.CanChangeActiveItem)
			{
				_activeItem = item;
			}
			_currentState?.OnItemDown();
		}

		public void OnItemUp(ScrollViewItemInteractiveBase item)
		{
			_currentState?.OnItemUp();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_currentState?.OnPointerDown();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_currentState?.OnPointerUp();
		}

		public void OnScroll(PointerEventData eventData)
		{
			_currentState?.OnScroll(eventData.scrollDelta);
		}

		public virtual void SetState(ScrollViewState.StateType stateType)
		{
			ScrollViewStateChangeData arg = new ScrollViewStateChangeData
			{
				OldState = _currentState
			};
			_currentState?.OnExit();
			_currentState = GetState(stateType);
			_currentState?.OnEnter();
			arg.NewState = _currentState;
			OnStateChangedEvent?.Invoke(this, arg);
		}

		public ScrollViewState GetState(ScrollViewState.StateType stateType)
		{
			for (int i = 0; i < _states.Count; i++)
			{
				if (_states[i] != null && _states[i].State == stateType)
				{
					return _states[i];
				}
			}
			return null;
		}

		public T GetState<T>() where T : ScrollViewState
		{
			for (int i = 0; i < _states.Count; i++)
			{
				if (_states[i] != null && _states[i] is T)
				{
					return _states[i] as T;
				}
			}
			return null;
		}

		public void Move(Vector2 delta)
		{
			switch (MoveAxis)
			{
			case MoveAxisType.Horizontal:
				delta.y = 0f;
				break;
			case MoveAxisType.Vertical:
				delta.x = 0f;
				break;
			}
			if (!Content)
			{
				return;
			}
			Content.anchoredPosition += delta;
			if ((bool)RectTransformSelf)
			{
				Vector2 anchoredPosition = Content.anchoredPosition;
				switch (MoveAxis)
				{
				case MoveAxisType.Horizontal:
					anchoredPosition.x = ClampContentPositionAxis(anchoredPosition.x);
					break;
				case MoveAxisType.Vertical:
					anchoredPosition.y = ClampContentPositionAxis(anchoredPosition.y);
					break;
				}
				Content.anchoredPosition = anchoredPosition;
			}
		}

		public float ClampContentPositionAxis(float axisValue)
		{
			if (!Content || !RectTransformSelf)
			{
				return axisValue;
			}
			return MoveAxis switch
			{
				MoveAxisType.Horizontal => Mathf.Clamp(axisValue, RectTransformSelf.rect.width - Content.rect.width, 0f), 
				MoveAxisType.Vertical => Mathf.Clamp(axisValue, 0f, Content.rect.height - RectTransformSelf.rect.height), 
				_ => axisValue, 
			};
		}

		protected virtual void OnEnable()
		{
			if (_states.Count == 0)
			{
				InitStates();
			}
			SetState(ScrollViewState.StateType.Idle);
			ScrollViewContainer.AddScrollView(this);
		}

		protected virtual void OnDisable()
		{
			SetState(ScrollViewState.StateType.None);
			ScrollViewContainer.RemoveScrollView(this);
		}

		protected void Update()
		{
			_currentState?.Update();
		}

		private void LateUpdate()
		{
			_currentState?.LateUpdate();
		}

		protected void AddState(ScrollViewState state)
		{
			_states.Add(state);
		}

		protected abstract void InitStates();
	}
}
