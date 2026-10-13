using UnityEngine;

public class wwiseambientemitter : MonoBehaviour
{
	public bool Enable_debug;

	public GameObject AudioEmitter;

	public GameObject Character;

	public AkAudioListener Listener;

	private BoxCollider AmbientCollider;

	private Vector3 ListnerePosition;

	private Vector3 ColliderSizeMax;

	private Vector3 ColliderSizeMin;

	private float emitterX;

	private float emitterY;

	private float emitterZ;

	public bool IsInArea;

	private void Start()
	{
		AmbientCollider = GetComponent<BoxCollider>();
		ColliderSizeMax = AmbientCollider.transform.position;
		ColliderSizeMin = AmbientCollider.transform.position;
		ColliderSizeMax += base.transform.localScale / 2f;
		ColliderSizeMin -= base.transform.localScale / 2f;
	}

	private void Update()
	{
		ListnerePosition = Character.transform.position;
		if (ListnerePosition.x > ColliderSizeMin.x && ListnerePosition.x < ColliderSizeMax.x)
		{
			emitterX = ListnerePosition.x;
		}
		if (ListnerePosition.x < ColliderSizeMin.x)
		{
			emitterX = ColliderSizeMin.x;
		}
		if (ListnerePosition.x > ColliderSizeMax.x)
		{
			emitterX = ListnerePosition.x;
		}
		ListnerePosition = Character.transform.position;
		if (ListnerePosition.y > ColliderSizeMin.y && ListnerePosition.y < ColliderSizeMax.y)
		{
			emitterY = ListnerePosition.y;
		}
		if (ListnerePosition.y < ColliderSizeMin.y)
		{
			emitterY = ColliderSizeMin.y;
		}
		if (ListnerePosition.y > ColliderSizeMax.y)
		{
			emitterY = ListnerePosition.y;
		}
		ListnerePosition = Character.transform.position;
		if (ListnerePosition.z > ColliderSizeMin.z && ListnerePosition.z < ColliderSizeMax.z)
		{
			emitterZ = ListnerePosition.z;
		}
		if (ListnerePosition.z < ColliderSizeMin.z)
		{
			emitterZ = ColliderSizeMin.z;
		}
		if (ListnerePosition.z > ColliderSizeMax.z)
		{
			emitterZ = ListnerePosition.z;
		}
		if (!IsInArea)
		{
			AudioEmitter.transform.position = new Vector3(emitterX, emitterY, emitterZ);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other.TryGetComponent<WWiseCameraTrigger>(out var _))
		{
			if (!IsInArea)
			{
				IsInArea = true;
			}
			AudioEmitter.transform.position = ListnerePosition;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.TryGetComponent<WWiseCameraTrigger>(out var _))
		{
			IsInArea = false;
		}
	}
}
