using UnityEngine;

public class LoadingImageRotate : MonoBehaviour
{
	public float degrees;

	private Vector3 rotationEuler;

	private void Update()
	{
		rotationEuler += Vector3.forward * degrees * Time.deltaTime;
		base.transform.rotation = Quaternion.Euler(rotationEuler);
	}
}
