using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public class IdleInertiaModule : StateModule
	{
		private Vector2 _velocity;

		public Vector2 Velocity
		{
			get
			{
				return _velocity;
			}
			set
			{
				_velocity = value;
			}
		}

		private ScrollViewConfig.InertiaData InertiaData => base.ScrollViewConfig.Inertia;

		public IdleInertiaModule(ScrollViewBase scrollViewBase, ScrollViewState scrollViewState)
			: base(scrollViewBase, scrollViewState)
		{
		}

		public override void Update()
		{
			base.Update();
			if (InertiaData.Enabled)
			{
				Velocity *= Mathf.Pow(InertiaData.DecelerationRate, Time.deltaTime);
				if (Mathf.Abs(_velocity.x) < 1f)
				{
					_velocity.x = 0f;
				}
				if (Mathf.Abs(_velocity.y) < 1f)
				{
					_velocity.y = 0f;
				}
				scrollViewBase.Move(_velocity * Time.deltaTime);
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			Velocity = Vector2.zero;
		}
	}
}
