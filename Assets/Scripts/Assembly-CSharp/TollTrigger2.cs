using System;
using Gley.TrafficSystem;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Providers;

public class TollTrigger2 : MonoBehaviour
{
	public enum TYPE
	{
		LEFT = 0,
		RIGHT = 1
	}

	public bool OpenNow;

	private Transform TollP_pivot;

	public TallPerson TallPerson;

	public GameObject Gate;

	public GameObject GateLod;

	[SerializeField]
	private TYPE type;

	[SerializeField]
	private int angle;

	[SerializeField]
	private int dir;

	private readonly Vector3 _openGate = new Vector3(0f, -90f, 0f);

	private readonly Vector3 _closeGate = new Vector3(0f, 90f, 0f);

	private IEarnMoneyService _earnMoneyService;

	private FreeDriveProvider _freeDriveProvider;

	private IDisposable _disposableClose;

	private Rigidbody _lastGleyCar;

	[Inject]
	public void Construct(IEarnMoneyService earnMoneyService, FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
		_earnMoneyService = earnMoneyService;
	}

	private void Start()
	{
		if (!TryInitTallPerson())
		{
			GameObject gameObject = new GameObject("TollP");
			gameObject.transform.parent = base.transform.parent.Find("TollP_pivot");
			if (!gameObject.transform.parent)
			{
				UnityEngine.Object.Destroy(gameObject);
				return;
			}
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.eulerAngles = Vector3.zero;
			gameObject.transform.localScale = new Vector3(3f, 2f, 3f);
			gameObject.AddComponent<BoxCollider>().isTrigger = true;
			TallPerson = gameObject.AddComponent<TallPerson>();
			TryInitTallPerson();
		}
	}

	private bool TryInitTallPerson()
	{
		if (!TallPerson)
		{
			return false;
		}
		TallPerson.Init(_earnMoneyService, _freeDriveProvider);
		Observable.Timer(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			CheckDistance();
		}).AddTo(this);
		base.enabled = false;
		return true;
	}

	private void CheckDistance()
	{
		if ((bool)V.CoreGameManager)
		{
			float num = Vector3.Distance(V.CoreGameManager.transform.position, base.transform.position);
			base.enabled = num < 100f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null && other.attachedRigidbody != _lastGleyCar && other.attachedRigidbody.TryGetComponent<VehicleComponent>(out var _))
		{
			_lastGleyCar = other.attachedRigidbody;
			Open(other.attachedRigidbody.gameObject);
		}
	}

	public void Open(GameObject car)
	{
		base.enabled = true;
		dir = 1;
		angle += ((angle < 90) ? dir : 0);
		if ((bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null && (bool)V.CoreGameManager.FreeDriveProvider.TallPerson && Vector3.Distance(car.transform.position, V.DriverPerson.transform.position) < 50f)
		{
			_freeDriveProvider.TallPerson.OnCarPass(car);
		}
	}

	private void Update()
	{
		Vector3 eulerAngles = Gate.transform.localRotation.eulerAngles;
		if (Mathf.Abs(angle) != 90 && angle != 0)
		{
			angle += dir;
		}
		else if (angle == 0)
		{
			dir = 0;
		}
		if (Mathf.Abs(angle) == 90 && dir != 0)
		{
			dir = 0;
			_disposableClose?.Dispose();
			_disposableClose = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				Close();
			}).AddTo(this);
		}
		eulerAngles.z = ((type == TYPE.RIGHT) ? angle : (-angle));
		Gate.transform.localRotation = Quaternion.Euler(eulerAngles);
		if ((bool)GateLod)
		{
			GateLod.transform.localRotation = Quaternion.Euler(eulerAngles);
		}
	}

	public void Close()
	{
		if (dir == 0)
		{
			_lastGleyCar = null;
			dir = -1;
			angle += ((angle > 0) ? dir : 0);
		}
	}
}
