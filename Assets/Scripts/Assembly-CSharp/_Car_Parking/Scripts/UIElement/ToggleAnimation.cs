using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ToggleAnimation : MonoBehaviour
	{
		[SerializeField]
		private Toggle TargetToggle = default;

		[SerializeField]
		private Transform ObjectToMove = default;

		[SerializeField]
		private bool Horizontal = default;

		[SerializeField]
		private bool Vertical = default;

		[SerializeField]
		private float OffsetToEnd = default;

		[SerializeField]
		private float AnimDuration = default;

		[SerializeField]
		private bool SwitchImageColor = default;

		[SerializeField]
		private ToggleSwitchHandler.ToggleImageElements ColorSwitchElements = default;

		private Vector3 _offPos;

		private Tween _tween;

		private void Start()
		{
			Vector3 localPosition = ObjectToMove.localPosition;
			_offPos = (TargetToggle.isOn ? new Vector3(localPosition.x - OffsetToEnd, localPosition.y - OffsetToEnd, localPosition.z) : localPosition);
			SetState(TargetToggle.isOn);
			TargetToggle.onValueChanged.AddListener(OnToggleSwitch);
		}

		private void SetState(bool isOn)
		{
			Vector3 localPosition = CalculateTargetPos(isOn);
			ObjectToMove.localPosition = localPosition;
			if (SwitchImageColor && (bool)ColorSwitchElements.ImageToColorChange)
			{
				Color color = (isOn ? ColorSwitchElements.On : ColorSwitchElements.Off);
				ColorSwitchElements.ImageToColorChange.color = color;
			}
		}

		private void OnToggleSwitch(bool isOn)
		{
			MoveAnimation(isOn);
			if (SwitchImageColor)
			{
				ColorAnimation(isOn);
			}
		}

		private void MoveAnimation(bool isOn)
		{
			Vector3 endValue = CalculateTargetPos(isOn);
			_tween?.Kill();
			_tween = ObjectToMove.DOLocalMove(endValue, AnimDuration).SetUpdate(isIndependentUpdate: true);
		}

		private Vector3 CalculateTargetPos(bool isOn)
		{
			float x = (Horizontal ? (_offPos.x + OffsetToEnd) : _offPos.x);
			float y = (Vertical ? (_offPos.y + OffsetToEnd) : _offPos.y);
			float z = _offPos.z;
			if (!isOn)
			{
				return _offPos;
			}
			return new Vector3(x, y, z);
		}

		private void ColorAnimation(bool isOn)
		{
			if ((bool)ColorSwitchElements.ImageToColorChange)
			{
				Color endValue = (isOn ? ColorSwitchElements.On : ColorSwitchElements.Off);
				ColorSwitchElements.ImageToColorChange.DOColor(endValue, AnimDuration).SetUpdate(isIndependentUpdate: true);
			}
		}
	}
}
