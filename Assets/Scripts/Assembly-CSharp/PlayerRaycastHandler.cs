using UnityEngine;

public class PlayerRaycastHandler : MonoBehaviour
{
	public float distance;

	private int? downHash;

	private int? upHash;

	private RaycastHit hit;

	private LayerMask layerMask;

	private GameObject player { get; set; }

	private void Start()
	{
		layerMask = 131072;
	}

	public GameObject GetPlayer()
	{
		return player;
	}
}
