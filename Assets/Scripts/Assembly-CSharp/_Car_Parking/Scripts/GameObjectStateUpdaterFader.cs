using UnityEngine;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts
{
	public class GameObjectStateUpdaterFader : GameObjectStateUpdater
	{
		[SerializeField]
		private Fader Fader = default;

		[SerializeField]
		private float AlphaOn = default;

		[SerializeField]
		private float TimeOn = default;

		[SerializeField]
		private float AlphaOff = default;

		[SerializeField]
		private float TimeOff = default;

		public override bool IsOn
		{
			get
			{
				if ((bool)Fader)
				{
					return Mathf.Abs(Fader.Alpha - AlphaOn) < Mathf.Abs(Fader.Alpha - AlphaOff);
				}
				return false;
			}
		}

		public override void UpdateState(bool isOn)
		{
			if ((bool)Fader)
			{
				Fader.Fade(isOn ? AlphaOn : AlphaOff, isOn ? TimeOn : TimeOff);
			}
		}
	}
}
