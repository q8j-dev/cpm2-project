using UnityEngine;

public class CarServiceAudioTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Car"))
		{
			AudioManager.instance.EnterCarService();
		}
	}
}
