using AK.Wwise;
using UnityEngine;

namespace Wwise
{
	public class PaintSoundEnable : MonoBehaviour
	{
		public RTPC MenuAmbOff;

		private void OnEnable()
		{
			MenuAmbOff.SetGlobalValue(0f);
		}

		private void OnDisable()
		{
			MenuAmbOff.SetGlobalValue(1f);
		}
	}
}
