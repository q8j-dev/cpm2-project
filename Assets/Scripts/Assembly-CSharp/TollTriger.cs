using System;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Providers;

public class TollTriger : MonoBehaviour
{
	public Animation GateAnimation;

	public AnimationClip OpenClip;

	public AnimationClip closeClip;

	public Transform RedLight;

	public Transform GreenLight;

	public bool OpenNow;

	public TallPerson TallPerson;

	private IEarnMoneyService _earnMoneyService;

	private FreeDriveProvider _freeDriveProvider;

	private IDisposable _closeDisposable;

	[Inject]
	public void Construct(IEarnMoneyService earnMoneyService, FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
		_earnMoneyService = earnMoneyService;
	}

	private void Start()
	{
		OpenNow = false;
		GameObject gameObject = new GameObject("TollP");
		gameObject.transform.parent = base.transform.parent;
		gameObject.transform.localPosition = new Vector3(0f, 1.8f, 0f);
		gameObject.transform.eulerAngles = Vector3.zero;
		gameObject.transform.localScale = new Vector3(1f, 2f, 3f);
		gameObject.AddComponent<BoxCollider>().isTrigger = true;
		TallPerson = gameObject.AddComponent<TallPerson>();
		TallPerson.Init(_earnMoneyService, _freeDriveProvider);
		GateAnimation = base.transform.parent.Find("Gate").GetComponent<Animation>();
	}

	public void Open(GameObject car)
	{
		if (!OpenNow)
		{
			GateAnimation.clip = OpenClip;
			GateAnimation.Play();
			OpenNow = true;
			_closeDisposable = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
			{
				Close();
			}).AddTo(this);
			RedLight.gameObject.SetActive(value: false);
			GreenLight.gameObject.SetActive(value: true);
			if ((bool)_freeDriveProvider.TallPerson && Vector3.Distance(car.transform.position, V.DriverPerson.transform.position) < 50f)
			{
				_freeDriveProvider.TallPerson.OnCarPass(car);
			}
		}
	}

	public void Close()
	{
		if (OpenNow)
		{
			GateAnimation.clip = closeClip;
			GateAnimation.Play();
			OpenNow = false;
			RedLight.gameObject.SetActive(value: true);
			GreenLight.gameObject.SetActive(value: false);
		}
	}
}
