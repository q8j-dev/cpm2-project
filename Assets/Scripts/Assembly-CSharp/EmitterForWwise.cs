using UnityEngine;
using _Car_Parking.Scripts.Audio;

[RequireComponent(typeof(SphereCollider))]
public class EmitterForWwise : MonoBehaviour
{
	public string Ambient_Event = "Amb_OceanBridge";

	private AkGameObj GameObjectID;

	private bool IsInCollider;

	private void Start()
	{
		AkSoundEngine.RegisterGameObj(base.gameObject);
		GameObjectID = GetComponent<AkGameObj>();
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent<WWiseCameraTrigger>(out var _) && !IsInCollider && Ambient_Event != null)
		{
			AudioWrapper.PostEvent(Ambient_Event, base.gameObject);
			IsInCollider = true;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other.TryGetComponent<WWiseCameraTrigger>(out var _) && !IsInCollider && Ambient_Event != null)
		{
			IsInCollider = true;
			AudioWrapper.PostEvent(Ambient_Event, base.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.TryGetComponent<WWiseCameraTrigger>(out var _) && !IsInCollider && Ambient_Event != null)
		{
			AudioWrapper.PostEvent(Ambient_Event, base.gameObject);
			IsInCollider = false;
		}
	}
}
