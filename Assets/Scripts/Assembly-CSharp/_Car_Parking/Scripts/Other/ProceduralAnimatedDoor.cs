using System;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.Other
{
	public class ProceduralAnimatedDoor : ProceduralDoor
	{
		private static readonly int Open = Animator.StringToHash("Open");

		public Animator Animator;

		public bool IsFoldingDoor;

		protected override void InteractInternal()
		{
			if (IsFoldingDoor)
			{
				AudioWrapper.PostEvent(base.IsOpen ? "Play_Folding_Doors_Close" : "Play_Folding_Door_Open", base.gameObject);
			}
			else
			{
				AudioWrapper.PostEvent("Play_garage_door", base.gameObject);
				Observable.Timer(TimeSpan.FromSeconds(0.8330000042915344)).Subscribe((long _) =>
				{
					AudioWrapper.PostEvent("Stop_garage_door", base.gameObject);
				}).AddTo(this);
			}
			Animator.SetBool(Open, !base.IsOpen);
		}
	}
}
