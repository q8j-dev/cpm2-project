using UnityEngine;

public class FreeDriveTraffic : MonoBehaviour
{
	public GameObject[] trafficCars;

	private void Awake()
	{
		InitAwake();
	}

	public void InitAwake()
	{
	}

	public void EnableAllCars(bool enable)
	{
		GameObject[] array = trafficCars;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				if (enable)
				{
					gameObject.GetComponent<TrafficCarTriger>().EnableCar();
				}
				else
				{
					gameObject.GetComponent<TrafficCarTriger>().DisabelCar();
				}
			}
		}
	}
}
