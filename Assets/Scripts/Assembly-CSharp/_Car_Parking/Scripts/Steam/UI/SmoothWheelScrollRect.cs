using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Steam.UI
{
	public class SmoothWheelScrollRect : ScrollRect
	{
		[SerializeField]
		private float wheelStepPixels = 90f;

		[SerializeField]
		private float wheelSmoothSpeed = 16f;

		private const float SnapNormalized = 0.0005f;

		private float _targetNorm;

		private bool _active;

		public override void OnScroll(PointerEventData data)
		{
			float num = ((base.vertical && (bool)base.content && (bool)base.viewport) ? (base.content.rect.height - base.viewport.rect.height) : 0f);
			if (!base.vertical || num <= 0f)
			{
				base.OnScroll(data);
			}
			else if (!Mathf.Approximately(data.scrollDelta.y, 0f))
			{
				float num2 = Mathf.Sign(data.scrollDelta.y);
				float num3 = wheelStepPixels / num;
				float num4 = (_active ? _targetNorm : base.verticalNormalizedPosition);
				_targetNorm = Mathf.Clamp01(num4 + num2 * num3);
				_active = true;
				StopMovement();
			}
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{
			_active = false;
			base.OnBeginDrag(eventData);
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (_active)
			{
				float num = (base.verticalNormalizedPosition = Mathf.Lerp(base.verticalNormalizedPosition, t: 1f - Mathf.Exp((0f - wheelSmoothSpeed) * Time.unscaledDeltaTime), b: _targetNorm));
				if (Mathf.Abs(_targetNorm - num) < 0.0005f)
				{
					base.verticalNormalizedPosition = _targetNorm;
					_active = false;
				}
			}
		}
	}
}
