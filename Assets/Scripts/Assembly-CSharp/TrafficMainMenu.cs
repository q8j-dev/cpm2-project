using SWS;
using UnityEngine;

public class TrafficMainMenu : MonoBehaviour
{
	public splineMove[] AllTrafficCars;

	public void UpdateRow(splineMove Move)
	{
		splineMove nextCar = GetNextCar();
		nextCar.transform.position = Move.transform.position;
		nextCar.enabled = true;
		nextCar.pathContainer = Move.pathContainer;
		nextCar.speed = Random.Range(30, 45);
		nextCar.transform.LookAt(nextCar.pathContainer.waypoints[5]);
		nextCar.gameObject.SetActive(value: true);
		Move.gameObject.SetActive(value: false);
		nextCar.StartMove();
	}

	private splineMove GetNextCar()
	{
		splineMove[] allTrafficCars = AllTrafficCars;
		foreach (splineMove splineMove2 in allTrafficCars)
		{
			if (!splineMove2.gameObject.activeSelf)
			{
				return splineMove2;
			}
		}
		return null;
	}

	private void Update()
	{
	}
}
