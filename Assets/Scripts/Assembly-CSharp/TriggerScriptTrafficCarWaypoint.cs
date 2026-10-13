using UnityEngine;

public class TriggerScriptTrafficCarWaypoint : MonoBehaviour
{
	public TrafficWaypointControl twc;

	public int count;

	private void Start()
	{
	}

	private void ChangeRow(int row)
	{
		_ = twc.currentRow;
		int currentPoint = twc.sm.currentPoint;
		twc.sm.moveToPath = true;
		twc.sm.startPoint = currentPoint + 2 * twc.direction;
		twc.currentRow = row;
	}

	private void EnteredToCity()
	{
	}

	private void ExitFromCity()
	{
	}

	private void UTurnHighWay()
	{
	}

	private void StopCar()
	{
		twc.targetSpeed = 0f;
	}

	private void MoveCar()
	{
		if (twc.rb.isKinematic)
		{
			if (Application.loadedLevel == 13)
			{
				twc.targetSpeed = Random.Range(15, 45);
			}
			else
			{
				twc.targetSpeed = Random.Range(35, 45);
			}
			twc.acc = 2f + twc.targetSpeed / 7f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.gameObject != twc.gameObject))
		{
			return;
		}
		if (other.CompareTag("TrafficCar") || other.CompareTag("Car") || other.CompareTag("ped") || other.CompareTag("Trailer") || other.CompareTag("DriverPerson"))
		{
			count++;
			if (count > 0)
			{
				if (other.CompareTag("TrafficCar"))
				{
					StopCar();
				}
				else
				{
					StopCar();
				}
			}
		}
		if (other.name == "ChangeRowTo3")
		{
			ChangeRow(3);
		}
		else if (other.name == "ChangeRowTo2")
		{
			ChangeRow(2);
		}
		else if (other.name == "ChangeRowTo7")
		{
			ChangeRow(7);
		}
		else if (other.name == "ToCity")
		{
			EnteredToCity();
		}
		else if (other.name == "FromCity")
		{
			ExitFromCity();
		}
		else if (other.name == "UTurn")
		{
			UTurnHighWay();
		}
		else if (other.name == "Slow")
		{
			SlowDown();
		}
		else if (other.name == "UnSlow")
		{
			MoveCar();
		}
	}

	private void SlowDown()
	{
		twc.targetSpeed = 6f;
		twc.acc = 2f + twc.targetSpeed / 5f;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject != twc.gameObject && (other.CompareTag("TrafficCar") || other.CompareTag("Car") || other.CompareTag("Trailer") || other.CompareTag("ped") || other.CompareTag("DriverPerson")))
		{
			count--;
			if (count <= 0)
			{
				MoveCar();
				count = 0;
			}
		}
	}
}
