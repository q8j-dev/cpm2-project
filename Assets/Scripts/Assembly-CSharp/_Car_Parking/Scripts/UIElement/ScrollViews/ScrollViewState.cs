using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public abstract class ScrollViewState
	{
		public enum StateType
		{
			None = 0,
			Idle = 1,
			Scroll = 2,
			ItemDown = 3,
			DragItem = 4
		}

		protected ScrollViewBase scrollView;

		private List<StateModule> _stateModules = new List<StateModule>();

		public abstract StateType State { get; }

		public virtual bool CanChangeActiveItem => true;

		protected RectTransform CanvasRectTransform
		{
			get
			{
				if (!scrollView)
				{
					return null;
				}
				return scrollView.CanvasRectTransform;
			}
		}

		protected ScrollViewConfig Config
		{
			get
			{
				if (!scrollView)
				{
					return null;
				}
				return scrollView.Config;
			}
		}

		protected ScrollViewItemInteractiveBase ActiveItem
		{
			get
			{
				if (!scrollView)
				{
					return null;
				}
				return scrollView.ActiveItem;
			}
		}

		public ScrollViewState(ScrollViewBase scrollView)
		{
			this.scrollView = scrollView;
		}

		public virtual void OnEnter()
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				_stateModules[i]?.OnEnter();
			}
		}

		public virtual void Update()
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				_stateModules[i]?.Update();
			}
		}

		public virtual void LateUpdate()
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				_stateModules[i]?.LateUpdate();
			}
		}

		public virtual void OnExit()
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				_stateModules[i]?.OnExit();
			}
		}

		public void AddModule(StateModule module)
		{
			_stateModules.Add(module);
		}

		public void RemoveModule<T>() where T : StateModule
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				if (_stateModules[i] != null && _stateModules[i] is T)
				{
					_stateModules.RemoveAt(i);
					break;
				}
			}
		}

		public T GetModule<T>() where T : StateModule
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				if (_stateModules[i] != null && _stateModules[i] is T)
				{
					return _stateModules[i] as T;
				}
			}
			return null;
		}

		public virtual void OnPointerDown()
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				_stateModules[i]?.OnPointerDown();
			}
		}

		public virtual void OnPointerUp()
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				_stateModules[i]?.OnPointerUp();
			}
		}

		public virtual void OnItemDown()
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				_stateModules[i]?.OnItemDown();
			}
		}

		public virtual void OnItemUp()
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				_stateModules[i]?.OnItemUp();
			}
		}

		public virtual void OnScroll(Vector2 scrollDelta)
		{
			for (int i = 0; i < _stateModules.Count; i++)
			{
				_stateModules[i]?.OnScroll(scrollDelta);
			}
		}

		public Vector2 GetMousePosition()
		{
			if (!CanvasRectTransform)
			{
				return Vector2.zero;
			}
			return Vector2.Scale(new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height), CanvasRectTransform.sizeDelta);
		}
	}
}
