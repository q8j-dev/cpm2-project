using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class FirstTimeFadeOn : MonoBehaviour
	{
		[SerializeField]
		private Fader Fader;

		[SerializeField]
		private float Duration;

		private void Start()
		{
			if ((bool)Fader)
			{
				Fader.Fade(0f, -1f);
				Fader.Fade(1f, Duration);
			}
		}
	}
}
