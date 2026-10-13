using UnityEngine;

public class InjectCar : MonoBehaviour
{
	private void Start()
	{
	}

	private void Inject()
	{
		GetComponent<AkTriggerEnter>().enabled = true;
		GetComponent<AkTriggerExit>().enabled = true;
		GetComponent<AkTriggerEnter>().triggerObject = V.Car;
		GetComponent<AkTriggerExit>().triggerObject = V.Car;
	}

	private void Update()
	{
	}
}
