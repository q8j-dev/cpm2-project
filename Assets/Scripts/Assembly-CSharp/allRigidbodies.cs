using UnityEngine;

public class allRigidbodies : MonoBehaviour
{
	public Rigidbody[] allRigid;

	private void Start()
	{
		MakeAllKinematic(make: false);
	}

	public void MakeAllKinematic(bool make)
	{
		Rigidbody[] array = allRigid;
		foreach (Rigidbody rigidbody in array)
		{
			if ((bool)rigidbody)
			{
				rigidbody.isKinematic = make;
			}
		}
	}
}
