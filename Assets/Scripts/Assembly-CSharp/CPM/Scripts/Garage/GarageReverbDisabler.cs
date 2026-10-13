using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace CPM.Scripts.Garage
{
	public class GarageReverbDisabler : MonoBehaviour
	{
		private const string RtpcGarageReverb = "RTPC_Garage_Rever_Off";

		private void OnEnable()
		{
			AudioWrapper.SetRTPCValue("RTPC_Garage_Rever_Off", 1f);
		}

		private void OnDisable()
		{
			AudioWrapper.SetRTPCValue("RTPC_Garage_Rever_Off", 0f);
		}

		private void OnDestroy()
		{
			AudioWrapper.SetRTPCValue("RTPC_Garage_Rever_Off", 0f);
		}
	}
}
