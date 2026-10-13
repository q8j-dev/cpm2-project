using DG.Tweening;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.Other
{
	public class ProceduralDoorOpen : ProceduralDoor
	{
		public float Degrees = 90f;

		private Vector3 _startRotation;

		protected override void Start()
		{
			base.Start();
			_startRotation = base.transform.localEulerAngles;
		}

		protected override void InteractInternal()
		{
			float num = Degrees;
			if (!base.IsFront)
			{
				num *= -1f;
			}
			AudioWrapper.PostEvent(base.IsOpen ? "Play_close_door_houses" : "Play_open_door_houses", base.gameObject);
			base.Tween = base.transform.DOLocalRotate(base.IsOpen ? _startRotation : (_startRotation + Vector3.up * num), Duration).SetEase(Ease.OutQuart);
		}
	}
}
