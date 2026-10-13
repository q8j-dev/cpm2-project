using CPMEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking.Scripts.Scopes;

public class TriggerScriptTrafficCar : MonoBehaviour
{
	public CarAIController CarAIController;

	public int count;

	public bool noOverTake;

	public TrafficCarTriger trafficCarTrigger;

	private int _trafficCarHash;

	private int _carHash;

	private int _pedHash;

	private int _trailerHash;

	private int _driverPersonHash;

	private Powertrain _dr;

	private Sort2 _sort2;

	private TrafficCarControl _trafficCarControl;

	[Inject]
	public void Construct(Sort2 sort2, OptionalDependency<TrafficCarControl> trafficCar)
	{
		_trafficCarControl = trafficCar.Value;
		_sort2 = sort2;
	}

	private void Start()
	{
		_trafficCarHash = "TrafficCar".GetHashCode();
		_carHash = "Car".GetHashCode();
		_pedHash = "ped".GetHashCode();
		_trailerHash = "Trailer".GetHashCode();
		_driverPersonHash = "DriverPerson".GetHashCode();
		trafficCarTrigger = CarAIController.GetComponent<TrafficCarTriger>();
		_dr = CarAIController.GetComponent<Powertrain>();
	}

	public void Reset()
	{
		CarAIController.StopCar = false;
		count = 0;
	}

	private void ChangeRow()
	{
		if (noOverTake)
		{
			return;
		}
		int row = CarAIController.Row;
		if (row == 2 || row == 5 || row == 3 || row == 4)
		{
			int currentIndex = CarAIController.CurrentTarIndex;
			if (CarAIController.Row != 3 && CarAIController.Row != 4)
			{
				currentIndex = PlayerDatas.getEqualTargetIndexToMainRows(CarAIController.CurrentTarIndex, 2);
			}
			if (PlayerDatas.GetRowNumber(currentIndex) > 3)
			{
				int nextRowIndex = PlayerDatas.getNextRowIndex(currentIndex, CarAIController.Row);
				ChangeRow(nextRowIndex);
			}
		}
	}

	private void ChangeRow(int NextRow)
	{
		int num;
		switch (NextRow)
		{
		case 2:
		case 5:
			num = PlayerDatas.getEqualTargetIndex(CarAIController.CurrentTarIndex, NextRow);
			break;
		case 3:
		case 4:
			num = PlayerDatas.getEqualTargetIndexToMainRows(CarAIController.CurrentTarIndex, 2);
			break;
		default:
			num = CarAIController.CurrentTarIndex;
			break;
		}
		int currentTarIndex = num;
		if (CarAIController.targets.Length - 25 > CarAIController.CurrentTarIndex)
		{
			CarAIController.Row = NextRow;
			CarAIController.targets = _sort2.rowTrafficArrays[NextRow];
			CarAIController.CurrentTarIndex = currentTarIndex;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == CarAIController.gameObject || ((bool)_trafficCarControl && CarAIController.gameObject == _trafficCarControl.CarToFollow))
		{
			return;
		}
		int hashCode = other.tag.GetHashCode();
		if (hashCode != _trafficCarHash && hashCode != _carHash && hashCode != _pedHash && hashCode != _trailerHash && hashCode != _driverPersonHash)
		{
			return;
		}
		count++;
		if (count <= 0)
		{
			return;
		}
		if ((hashCode == _trafficCarHash || hashCode == _carHash || hashCode == _trailerHash) && SceneManager.GetActiveScene().buildIndex == 13 && CarAIController.TargetSpeed > 20f)
		{
			ChangeRow();
			CarAIController.StopCar = true;
			if (hashCode == _carHash && (bool)V.Car && V.Car.GetComponent<Powertrain>().SpeedMps - 10f < _dr.SpeedMps && (bool)trafficCarTrigger)
			{
				trafficCarTrigger.PlayHorn();
			}
		}
		else
		{
			CarAIController.StopCar = true;
			if ((hashCode == _carHash || hashCode == _trailerHash) && (bool)trafficCarTrigger)
			{
				trafficCarTrigger.PlayHorn();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == CarAIController.gameObject)
		{
			return;
		}
		int hashCode = other.tag.GetHashCode();
		if (hashCode == _trafficCarHash || hashCode == _carHash || hashCode == _pedHash || hashCode == _trailerHash || hashCode == _driverPersonHash)
		{
			count--;
			if (count <= 0)
			{
				CarAIController.StopCar = false;
				count = 0;
				CancelInvoke("Reset");
			}
		}
	}
}
