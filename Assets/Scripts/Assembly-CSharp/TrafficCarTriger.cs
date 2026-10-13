using System;
using CPMEngine;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Providers;

public class TrafficCarTriger : MonoBehaviour
{
	public bool JustENtered;

	public TriggerScriptTrafficCar triggerScriptTrafficCar;

	private CarAIController _mcc;

	private Rigidbody _rb;

	private VehicleData _vehicleData;

	private TargetControll _targetControll;

	private Powertrain _dr;

	private SoundTrafficCar soundController;

	public TraffiCarDataBody trafficCarDataBody;

	private bool _hornPlayed;

	public bool enabledCar = true;

	private IDisposable _disposableResetJustEntered;

	private IDisposable _disposableCheckDistanceWithCar;

	private Sort2 _sort2;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	[Inject]
	public void Construct(Sort2 sort2, OnlineCollectionsProvider onlineCollectionsProvider)
	{
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_sort2 = sort2;
	}

	private void Awake()
	{
		enabledCar = true;
		TryGetComponent<CarAIController>(out _mcc);
		TryGetComponent<Rigidbody>(out _rb);
		TryGetComponent<VehicleData>(out _vehicleData);
		TryGetComponent<Powertrain>(out _dr);
		TryGetComponent<SoundTrafficCar>(out soundController);
		_targetControll = ((SceneManager.GetActiveScene().buildIndex != 1) ? V.CoreGameManager.LevelsProvider.LevelScript.targetControll : V.CoreGameManager.transform.gameObject.GetComponent<TargetControll>());
	}

	private void OnEndReached(int indexPos)
	{
		Vector3 localPosition = _mcc.targets[indexPos];
		Vector3 vector = _mcc.targets[indexPos + 1];
		localPosition.y += 1.3f;
		vector.y += 1.3f;
		base.transform.localPosition = localPosition;
		base.transform.LookAt(vector + V.GlobalOffset);
		_mcc.CurrentTarIndex = indexPos + 1;
		_mcc.StopCar = false;
		_vehicleData.trigerScriptTraffic.Reset();
		_dr.Gear = 1;
		int num = UnityEngine.Random.Range(1, 10);
		if (SceneManager.GetActiveScene().buildIndex == 3 && (num == 1 || num == 2))
		{
			num = 5;
		}
		if (SceneManager.GetActiveScene().buildIndex != 6 && SceneManager.GetActiveScene().buildIndex != 7 && SceneManager.GetActiveScene().buildIndex != 14)
		{
			_rb.velocity = Vector3.zero;
		}
		if (SceneManager.GetActiveScene().buildIndex != 13)
		{
			TrafficCarsService.RemoveOldBodyAndSet(base.gameObject, num, WaypointCar: false);
		}
		_rb.angularVelocity = Vector3.zero;
	}

	private void OnEndReached2(int row, int indexPos, int sign)
	{
		ChangeRow(row);
		Vector3 localPosition = _mcc.targets[indexPos];
		if (localPosition.y >= 100f)
		{
			localPosition.y = _mcc.targets[indexPos + 1].y;
		}
		Vector3 vector = _mcc.targets[indexPos + sign];
		if (vector.y >= 100f)
		{
			vector.y = _mcc.targets[indexPos + sign * 2].y;
		}
		localPosition.y += 1.1f;
		vector.y += 1.1f;
		base.transform.localPosition = localPosition;
		base.transform.LookAt(vector + V.GlobalOffset);
		_mcc.CurrentTarIndex = indexPos + sign;
		_mcc.StopCar = false;
		_mcc.Sign = sign;
		_mcc.TargetSpeed = UnityEngine.Random.Range(15, 45);
		_vehicleData.trigerScriptTraffic.Reset();
		int i = UnityEngine.Random.Range(1, 10);
		_rb.velocity = Vector3.zero;
		_rb.AddForce(base.transform.forward * 10f);
		if (SceneManager.GetActiveScene().buildIndex != 13)
		{
			TrafficCarsService.RemoveOldBodyAndSet(base.gameObject, i, WaypointCar: false);
		}
		else if ((bool)trafficCarDataBody)
		{
			trafficCarDataBody.ChangeColor();
		}
		MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			if ((bool)multiPlayerPosControl)
			{
				VehicleData component = multiPlayerPosControl.GetComponent<VehicleData>();
				if ((bool)component.Collider && (bool)_vehicleData.Collider)
				{
					CarColliders.SetIgnore(component, _vehicleData, ignore: true);
				}
			}
		}
		_rb.angularVelocity = Vector3.zero;
		_rb.ResetInertiaTensor();
	}

	private void MoveTowards()
	{
	}

	private void ChangeTarget(int target, int CurrentCarIndex)
	{
		_mcc.targets = _targetControll.targets1V3[target];
		_mcc.CurrentWay = target;
		_mcc.CurrentTarIndex = CurrentCarIndex;
	}

	public void DisabelCar()
	{
		soundController.SoundParent.transform.parent = null;
		soundController.SoundParent.transform.localPosition = Vector3.zero;
		base.gameObject.SetActive(value: false);
		base.enabled = true;
		enabledCar = false;
	}

	public void EnableCar()
	{
		soundController.SoundParent.transform.parent = base.gameObject.transform;
		soundController.SoundParent.transform.localPosition = Vector3.zero;
		base.gameObject.SetActive(value: true);
		base.enabled = true;
		enabledCar = true;
	}

	public void SetCarNewRow(int row, int index)
	{
		int roadDirection = PlayerDatas.GetRoadDirection(row);
		Vector3[] array = _sort2.rowTrafficArrays[row];
		if (index <= array.Length - 2 && index > 0 && !Physics.CheckBox(new Vector3(array[index].x, array[index].y + 1.3f, array[index].z) + V.GlobalOffset, new Vector3(0.4f, 0.4f, 4f), Quaternion.identity, -1, QueryTriggerInteraction.Ignore))
		{
			OnEndReached2(row, index, roadDirection);
		}
	}

	private void ChangeRow(int row)
	{
		_mcc.Row = row;
		_mcc.targets = _sort2.rowTrafficArrays[row];
	}

	private void ChangeRow(int row, int IndexToSet, Vector3[] targetToSet)
	{
		_mcc.Row = row;
		_mcc.CurrentTarIndex = IndexToSet;
		_mcc.targets = targetToSet;
	}

	private void ResetCar()
	{
		int num = 0;
		int num2 = UnityEngine.Random.Range(22, 25);
		int num3 = V.CurrentIndexArrayPositionCar + num2 * V.DirectionCar;
		if (!FlagsStorage.CloserMountain)
		{
			if (V.CurrentIndexArrayPositionCar > 20 && V.CurrentIndexArrayPositionCar < 1120)
			{
				num = PlayerDatas.GetRandomRow(num3);
				num3 = PlayerDatas.getEqualTargetIndex(num3, num);
				if (_sort2.rowTrafficArrays[num].Length - 30 < num3 || num3 < 10)
				{
					num = 0;
				}
			}
		}
		else if (V.CurrentIndexArrayPositionCar > 20 && V.CurrentIndexArrayPositionCar < 180)
		{
			num = UnityEngine.Random.Range(8, 12);
		}
		if (num != 0)
		{
			SetCarNewRow(num, num3);
		}
	}

	private void CheckDistacneWithCar()
	{
		if ((bool)V.DriverPerson)
		{
			float num = Vector3.Distance(base.gameObject.transform.position, V.DriverPerson.transform.position);
			Camera component = V.CoreGameManager.GetComponent<Camera>();
			if (num > component.farClipPlane + 60f)
			{
				ResetCar();
			}
		}
	}

	public void StartCheckingDistance()
	{
		_disposableCheckDistanceWithCar?.Dispose();
		_disposableCheckDistanceWithCar = Observable.Timer(TimeSpan.FromSeconds(1.0)).Concat(Observable.Interval(TimeSpan.FromSeconds(1.0))).Subscribe((long _) =>
		{
			CheckDistacneWithCar();
		})
			.AddTo(this);
	}

	private void ResetJustEntered()
	{
		JustENtered = false;
		_mcc.WillTurn = false;
	}

	private void Reset()
	{
		_mcc.StopCar = false;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!other.gameObject.CompareTag("ped") && (other.gameObject.CompareTag("Car") || other.gameObject.CompareTag("Trailer")))
		{
			PlayHorn();
		}
		if (SceneManager.GetActiveScene().buildIndex == 13 && other.gameObject.name == "Toll parent")
		{
			ResetCar();
		}
	}

	public void PlayHorn()
	{
		if (!_hornPlayed)
		{
			AudioWrapper.SetSwitch("Horns_Switch", "horn_{0}", base.gameObject);
			AudioWrapper.PostEvent(SoundController.PlayHornsSwitch, base.gameObject);
			_hornPlayed = true;
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				ResetHornPlayed();
			}).AddTo(this);
		}
	}

	private void ResetHornPlayed()
	{
		if (_hornPlayed)
		{
			AudioWrapper.PostEvent("Stop_Horns_Switch", base.gameObject);
		}
		_hornPlayed = false;
	}

	private void OnDestroy()
	{
		ResetHornPlayed();
	}

	private void SlowDown()
	{
		_mcc.TargetSpeed = 5f;
	}

	private void Accelerate()
	{
		_mcc.TargetSpeed = UnityEngine.Random.Range(20, 40);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (FlagsStorage.IsFreeDriveMode)
		{
			if (!(other.name != "Tall"))
			{
				TollTrigger2 component2;
				if (other.gameObject.TryGetComponent<TollTriger>(out var component))
				{
					component.Open(base.gameObject);
				}
				else if (other.gameObject.TryGetComponent<TollTrigger2>(out component2))
				{
					component2.Open(base.gameObject);
				}
			}
		}
		else if (other.CompareTag("TrafficRestrict"))
		{
			OnEndReached(2);
		}
		else if (other.CompareTag("cross") && !JustENtered)
		{
			JustENtered = true;
			_disposableResetJustEntered?.Dispose();
			_disposableResetJustEntered = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				ResetJustEntered();
			}).AddTo(this);
			if (_dr.SpeedMps > 20f)
			{
				_mcc.WillTurn = true;
			}
			if (other.name == "cross1")
			{
				if (_mcc.CurrentWay == 0 && _mcc.CurrentTarIndex < 11)
				{
					switch (UnityEngine.Random.Range(0, 3))
					{
					case 0:
						ChangeTarget(0, 151);
						break;
					case 1:
						ChangeTarget(2, 1);
						break;
					default:
						_ = 2;
						break;
					}
				}
				else if (_mcc.CurrentWay == 1 && _mcc.CurrentTarIndex < 140 && _mcc.CurrentTarIndex > 130)
				{
					switch (UnityEngine.Random.Range(0, 3))
					{
					case 0:
						ChangeTarget(0, 151);
						break;
					case 1:
						ChangeTarget(2, 1);
						break;
					default:
						_ = 2;
						break;
					}
				}
				else if (_mcc.CurrentWay == 2)
				{
					switch (UnityEngine.Random.Range(0, 3))
					{
					case 0:
						ChangeTarget(0, 7);
						break;
					case 1:
						ChangeTarget(1, 136);
						break;
					case 2:
						ChangeTarget(2, 1);
						break;
					}
				}
			}
			else if (other.name == "cross2")
			{
				if (_mcc.CurrentWay == 0 && _mcc.CurrentTarIndex < 22)
				{
					switch (UnityEngine.Random.Range(0, 3))
					{
					case 0:
						ChangeTarget(1, 24);
						break;
					case 1:
						ChangeTarget(0, 271);
						break;
					default:
						_ = 2;
						break;
					}
				}
				else if (_mcc.CurrentWay == 0 && _mcc.CurrentTarIndex > 262)
				{
					switch (UnityEngine.Random.Range(0, 3))
					{
					case 0:
						ChangeTarget(1, 119);
						break;
					case 1:
						ChangeTarget(0, 16);
						break;
					default:
						_ = 2;
						break;
					}
				}
				else if (_mcc.CurrentWay == 1)
				{
					switch (UnityEngine.Random.Range(0, 3))
					{
					case 0:
						ChangeTarget(1, 121);
						break;
					case 1:
						ChangeTarget(0, 14);
						break;
					default:
						_ = 2;
						break;
					}
				}
			}
			else if (other.name == "cross3")
			{
				if (_mcc.CurrentWay == 0 && _mcc.CurrentTarIndex < 25)
				{
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						ChangeTarget(0, 225);
					}
					else
					{
						_ = 1;
					}
				}
				else if (_mcc.CurrentWay == 1)
				{
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						ChangeTarget(0, 35);
					}
					else
					{
						_ = 1;
					}
				}
			}
			else if (other.name == "cross4")
			{
				if (_mcc.CurrentWay == 0 && _mcc.CurrentTarIndex > 280)
				{
					switch (UnityEngine.Random.Range(0, 2))
					{
					case 0:
						ChangeTarget(0, 183);
						break;
					case 1:
						ChangeTarget(0, 79);
						break;
					}
				}
				else if (_mcc.CurrentWay == 0)
				{
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						ChangeTarget(1, 1);
					}
					else
					{
						_ = 1;
					}
				}
			}
			else if (other.name == "cross5")
			{
				if (_mcc.CurrentWay == 0)
				{
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						ChangeTarget(0, 87);
					}
					else
					{
						_ = 1;
					}
				}
				else if (_mcc.CurrentWay == 2)
				{
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						ChangeTarget(0, 170);
					}
					else
					{
						_ = 1;
					}
				}
			}
			else
			{
				if (!(other.name == "cross6"))
				{
					return;
				}
				if (_mcc.CurrentWay == 0)
				{
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						ChangeTarget(3, 1);
					}
					else
					{
						_ = 1;
					}
				}
				else if (_mcc.CurrentWay == 1)
				{
					if (UnityEngine.Random.Range(0, 2) == 0)
					{
						ChangeTarget(3, 1);
					}
					else
					{
						_ = 1;
					}
				}
				else if (_mcc.CurrentWay == 4)
				{
					switch (UnityEngine.Random.Range(0, 2))
					{
					case 0:
						ChangeTarget(1, 70);
						break;
					case 1:
						ChangeTarget(0, 253);
						break;
					}
				}
			}
		}
		else if (other.name == "toBridge1")
		{
			ChangeTarget(0, 1);
		}
		else if (other.name == "toBridge2")
		{
			ChangeTarget(4, 1);
		}
	}
}
