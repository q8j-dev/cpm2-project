using UnityEngine;

public class MoveBodyOnExterior : MonoBehaviour
{
	public Vector3 offset;

	public Transform target;

	public int carId;

	private void Update()
	{
		if ((bool)target)
		{
			base.transform.position = target.position + offset;
		}
	}
}
