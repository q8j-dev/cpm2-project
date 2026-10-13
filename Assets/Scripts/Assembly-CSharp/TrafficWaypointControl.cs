using System;
using SWS;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Car.CarSetter;

public class TrafficWaypointControl : MonoBehaviour
{
	public splineMove sm;

	public int currentRow;

	public int direction;

	public bool Reversed;

	public Transform[] wheels;

	public Transform wheelParent;

	public float targetSpeed;

	public float acc = 2f;

	public Transform COG;

	public Rigidbody rb;

	private bool ReCreateBody = default;

	private AudioSource audioSource = default;

	public TraffiCarDataBody trafficCarDataBody;

	public Collider[] colliders;

	public bool HornPlayed;

	public TriggerScriptTrafficCarWaypoint tstcw;

	private IDisposable _resetCarDisposable;

	private IDisposable _checkDistanceWithCarDisposable;

	private Sort2 _sort2;

	[Inject]
	public void Construct(Sort2 sort2)
	{
		_sort2 = sort2;
	}

	private void Start()
	{
		sm = GetComponent<splineMove>();
		rb = GetComponent<Rigidbody>();
		StartCheckingDistance();
	}

	private void StartCheckingDistance()
	{
		_checkDistanceWithCarDisposable?.Dispose();
		_checkDistanceWithCarDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Repeat().Subscribe((long _) =>
		{
			CheckDistanceWithCar();
		})
			.AddTo(this);
	}

	private void CheckDistanceWithCar()
	{
		if ((bool)V.DriverPerson && Vector3.Distance(V.DriverPerson.transform.position, base.transform.position) > 400f)
		{
			ResetCar();
		}
		if (rb.isKinematic)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.z = 0f;
			base.transform.eulerAngles = eulerAngles;
			float num = 0f;
			if ((bool)COG)
			{
				COG.transform.position = base.transform.position;
			}
			if (Physics.Raycast(base.transform.position + base.transform.right * 0.6f + new Vector3(0f, 0.6f, 0f), -base.transform.up, out var hitInfo))
			{
				num = hitInfo.distance;
			}
			float y = 0.6f - num;
			float x = 0.6f;
			float value = Mathf.Atan2(y, x) * 57.29578f;
			value = Mathf.Clamp(value, -10f, 10f);
			eulerAngles = base.transform.eulerAngles;
			eulerAngles.z = value;
			base.transform.eulerAngles = eulerAngles;
		}
	}

	private void ResetCar()
	{
		int num2;
		if (FlagsStorage.RacingGameMode)
		{
			int num = currentRow;
			num2 = ((num == 2 || num == 3) ? 1 : 0);
		}
		else
		{
			num2 = 0;
		}
		bool flag = (byte)num2 != 0;
		if ((V.CurrentIndexArrayPositionCar > 20 && V.CurrentIndexArrayPositionCar < 870) || flag)
		{
			int row = ((V.CurrentIndexArrayPositionCar > 530) ? UnityEngine.Random.Range(3, 6) : UnityEngine.Random.Range(0, 8));
			int num3 = UnityEngine.Random.Range(14, 17);
			if (FlagsStorage.RacingGameMode)
			{
				row = UnityEngine.Random.Range(3, 5);
			}
			SetCarNewRow(row, V.CurrentIndexArrayPositionCar + num3 * V.DirectionCar);
		}
	}

	private void SetCarNewRow(int row, int index)
	{
		if (_sort2.rowTrafficArrays == null || row >= _sort2.rowTrafficArrays.Length)
		{
			return;
		}
		Vector3[] array = _sort2.rowTrafficArrays[row];
		if (array == null || index >= array.Length)
		{
			return;
		}
		int sign = 1;
		if (row > 3)
		{
			sign = -1;
		}
		direction = sign;
		if (Physics.CheckBox(new Vector3(array[index].x, array[index].y + 1f, array[index].z), new Vector3(0.7f, 0.5f, 0.7f)))
		{
			_resetCarDisposable?.Dispose();
			_resetCarDisposable = Observable.Interval(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				ResetCar();
			}).AddTo(this);
		}
		else
		{
			currentRow = row;
			OnEndReached2(index, sign);
		}
	}

	private void OnEndReached2(int indexPos, int sign)
	{
		if ((bool)tstcw)
		{
			tstcw.count = 0;
		}
		direction = sign;
		if (direction > 0)
		{
			MakeForward();
		}
		else
		{
			MakeReverse();
		}
		targetSpeed = ((SceneManager.GetActiveScene().buildIndex == 13) ? UnityEngine.Random.Range(15, 45) : UnityEngine.Random.Range(35, 45));
		acc = 2f + targetSpeed / 5f;
		sm.GoToWaypoint(indexPos);
		sm.startPoint = indexPos;
		rb.isKinematic = true;
		int num = UnityEngine.Random.Range(1, 10);
		if (SceneManager.GetActiveScene().buildIndex == 3 && (num == 1 || num == 2))
		{
			num = 5;
		}
		if (ReCreateBody)
		{
			TrafficCarsService.RemoveOldBodyAndSet(base.gameObject, num, WaypointCar: true);
		}
		else if ((bool)trafficCarDataBody)
		{
			trafficCarDataBody.ChangeColor();
		}
	}

	private void MakeReverse()
	{
		if (!Reversed)
		{
			sm.Reverse();
			Reversed = true;
		}
	}

	private void MakeForward()
	{
		if (Reversed)
		{
			sm.Reverse();
			Reversed = false;
		}
	}

	private void OnCollided(Vector3 impulse)
	{
		if (rb.isKinematic)
		{
			sm.Stop();
			sm.ChangeSpeed(0f);
			targetSpeed = 0f;
			rb.isKinematic = false;
			rb.AddForce(impulse / 3f, ForceMode.Impulse);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (SceneManager.GetActiveScene().buildIndex == 13 && other.gameObject.CompareTag("Car"))
		{
			OnCollided(other.impulse);
		}
	}

	private void Update()
	{
		sm.ChangeSpeed(Mathf.Lerp(sm.speed, targetSpeed, Time.deltaTime * acc));
		if ((bool)audioSource)
		{
			audioSource.pitch = (rb.isKinematic ? (sm.speed / 30f) : 0.3f);
		}
	}
}
