using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public abstract class StateModule
	{
		protected ScrollViewBase scrollViewBase;

		protected ScrollViewState scrollViewState;

		protected ScrollViewConfig ScrollViewConfig => scrollViewBase.Config;

		protected ScrollViewItemInteractiveBase ActiveItem => scrollViewBase.ActiveItem;

		public StateModule(ScrollViewBase scrollViewBase, ScrollViewState scrollViewState)
		{
			this.scrollViewBase = scrollViewBase;
			this.scrollViewState = scrollViewState;
		}

		public virtual void OnEnter()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void LateUpdate()
		{
		}

		public virtual void OnExit()
		{
		}

		public virtual void OnPointerDown()
		{
		}

		public virtual void OnPointerUp()
		{
		}

		public virtual void OnItemDown()
		{
		}

		public virtual void OnItemUp()
		{
		}

		public virtual void OnScroll(Vector2 scrollDelta)
		{
		}
	}
}
