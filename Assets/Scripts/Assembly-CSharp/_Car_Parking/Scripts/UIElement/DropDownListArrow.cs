using DG.Tweening;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.UIElement
{
	public class DropDownListArrow : MonoBehaviour
	{
		[SerializeField]
		private Transform OpenPosition = default;

		[SerializeField]
		private MoverByTween Mover = default;

		[SerializeField]
		private RotatorByTween Rotator = default;

		public void UpdateState(bool isOpen)
		{
			if (isOpen)
			{
				Mover.Move(OpenPosition.position, 0.5f, Ease.OutSine, isLocal: false);
				Rotator.Rotate(OpenPosition.localEulerAngles, 0.5f, Ease.OutSine, isLocal: true);
			}
			else
			{
				Mover.MoveToZero(0.5f, Ease.OutSine, isLocal: true);
				Rotator.Rotate(new Vector3(0f, 0f, 0f), 0.5f, Ease.OutSine, isLocal: true);
			}
		}
	}
}
