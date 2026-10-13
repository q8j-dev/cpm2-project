using SWS;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

public class TrafficMainMenuTrigger : MonoBehaviour
{
	public static readonly string PlayNpcCar = "Play_Npc_Car_";

	public static readonly string StopNpcCar = "Stop_Npc_Car_";

	private static readonly uint Endtraffic = AudioWrapper.GetId("endTraffic");

	public TrafficMainMenu _trafficMainMenu;

	private int soundId;

	private bool isPlaying;

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "endTraffic")
		{
			_trafficMainMenu.UpdateRow(GetComponent<splineMove>());
		}
	}

	private void OnEnable()
	{
		soundId = WiseHelper.SoundIdForNpc();
		if (!isPlaying)
		{
			AudioWrapper.PostEvent(PlayNpcCar + soundId, base.gameObject);
			isPlaying = true;
		}
	}

	private void OnDisable()
	{
		if (isPlaying)
		{
			AudioWrapper.PostEvent(StopNpcCar + soundId, base.gameObject);
			isPlaying = false;
		}
	}
}
