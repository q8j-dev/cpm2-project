using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public class IdleWheelScrollModule : StateModule
	{
		private const float DefaultSensitivity = 1f;

		private const float DefaultSmoothSpeed = 16f;

		private const float FallbackCellSize = 100f;

		private const float SnapDistance = 0.5f;

		private float _target;

		private bool _active;

		private bool IsVertical => scrollViewBase.TypeMoveAxis == ScrollViewBase.MoveAxisType.Vertical;

		private float Sensitivity
		{
			get
			{
				if (!base.ScrollViewConfig)
				{
					return 1f;
				}
				return base.ScrollViewConfig.WheelScrollSensitivity;
			}
		}

		private float SmoothSpeed
		{
			get
			{
				if (!base.ScrollViewConfig)
				{
					return 16f;
				}
				return base.ScrollViewConfig.WheelSmoothSpeed;
			}
		}

		public IdleWheelScrollModule(ScrollViewBase scrollViewBase, ScrollViewState scrollViewState)
			: base(scrollViewBase, scrollViewState)
		{
		}

		public override void OnScroll(Vector2 scrollDelta)
		{
			float num = (IsVertical ? scrollDelta.y : scrollDelta.x);
			if (!Mathf.Approximately(num, 0f))
			{
				float num2 = (IsVertical ? (-1f) : 1f);
				float num3 = GetCellSize() * Sensitivity * Mathf.Sign(num) * num2;
				float num4 = (_active ? _target : GetCurrentAxisPosition());
				_target = scrollViewBase.ClampContentPositionAxis(num4 + num3);
				_active = true;
				IdleInertiaModule idleInertiaModule = ((scrollViewState != null) ? scrollViewState.GetModule<IdleInertiaModule>() : null);
				if (idleInertiaModule != null)
				{
					idleInertiaModule.Velocity = Vector2.zero;
				}
			}
		}

		public override void Update()
		{
			base.Update();
			if (_active)
			{
				float currentAxisPosition = GetCurrentAxisPosition();
				float t = 1f - Mathf.Exp((0f - SmoothSpeed) * Time.unscaledDeltaTime);
				float num = Mathf.Lerp(currentAxisPosition, _target, t);
				scrollViewBase.Move(AxisVector(num - currentAxisPosition));
				if (Mathf.Abs(_target - num) < 0.5f)
				{
					scrollViewBase.Move(AxisVector(_target - GetCurrentAxisPosition()));
					_active = false;
				}
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			_active = false;
		}

		private float GetCurrentAxisPosition()
		{
			RectTransform contentRectTransform = scrollViewBase.ContentRectTransform;
			if (!contentRectTransform)
			{
				return 0f;
			}
			if (!IsVertical)
			{
				return contentRectTransform.anchoredPosition.x;
			}
			return contentRectTransform.anchoredPosition.y;
		}

		private Vector2 AxisVector(float value)
		{
			if (!IsVertical)
			{
				return new Vector2(value, 0f);
			}
			return new Vector2(0f, value);
		}

		private float GetCellSize()
		{
			ScrollViewItemBase[] items = scrollViewBase.Items;
			if (items != null && items.Length != 0 && (bool)items[0] && items[0].transform is RectTransform rectTransform)
			{
				float num = (IsVertical ? rectTransform.rect.height : rectTransform.rect.width);
				if (num > 0f)
				{
					return num + scrollViewBase.ContentSpaceBetweenItems;
				}
			}
			return 100f;
		}
	}
}
