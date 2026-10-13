using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio;

public class RainStateZone : MonoBehaviour
{
	[SerializeField]
	private RainZoneType zoneType = RainZoneType.None;

	private RainSoundState _rainSoundState;

	private bool _inside;

	[Inject]
	public void Construct(RainSoundState rainSoundState)
	{
		_rainSoundState = rainSoundState;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_inside && zoneType != RainZoneType.None && other.CompareTag("MainCamera"))
		{
			if (_rainSoundState == null && V.CoreGameManager != null)
			{
				_rainSoundState = V.CoreGameManager.RainSoundState;
			}
			if (_rainSoundState != null)
			{
				_inside = true;
				_rainSoundState.ZoneEntered(zoneType);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_inside && other.CompareTag("MainCamera"))
		{
			Exit();
		}
	}

	private void OnDisable()
	{
		if (_inside)
		{
			Exit();
		}
	}

	private void Exit()
	{
		_inside = false;
		_rainSoundState?.ZoneExited(zoneType);
	}
}
