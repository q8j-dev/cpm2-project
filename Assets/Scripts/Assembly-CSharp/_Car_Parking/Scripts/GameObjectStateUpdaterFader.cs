using UnityEngine;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts
{
	public class GameObjectStateUpdaterFader : GameObjectStateUpdater
	{
		[SerializeField]
		private Fader Fader;

		[SerializeField]
		private float AlphaOn;

		[SerializeField]
		private float TimeOn;

		[SerializeField]
		private float AlphaOff;

		[SerializeField]
		private float TimeOff;

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
