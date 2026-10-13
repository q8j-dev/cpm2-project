using System;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Providers;

public class FlagTrigger : MonoBehaviour, IFlagTrigger
{
	private const float TriggerCoolDown = 3f;

	private const float TimerResetCoolDown = 30f;

	private float _radius;

	[SerializeField]
	private AnimationBlinkerGO Flag = default;

	[SerializeField]
	private Transform Checker = default;

	private LocationFlagsController _flagController;

	private FlagNode<IFlagTrigger> _flagNode;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private IDisposable _timerTrigger;

	private float _lastPassTime;

	[UnityEngine.Scripting.Preserve]
	[Inject]
	public void Construct(OnlineCollectionsProvider onlineCollectionsProvider)
	{
		_onlineCollectionsProvider = onlineCollectionsProvider;
	}

	public void SetBlinker(GameObject blinker)
	{
		Flag.Blinker = blinker;
	}

	public void Init(LocationFlagsController locationFlagsController, FlagNode<IFlagTrigger> flagNode)
	{
		_flagController = locationFlagsController;
		_flagNode = flagNode;
	}

	public void SetRadius(float radius)
	{
		_radius = radius;
	}

	public void CheckTrigger()
	{
		bool flag = false;
		MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			if ((bool)multiPlayerPosControl && (!V.Car || !(V.Car.gameObject == multiPlayerPosControl.gameObject)))
			{
				Vector3 a = new Vector3(Checker.position.x, 0f, Checker.position.z);
				Vector3 b = new Vector3(multiPlayerPosControl.gameObject.transform.position.x, 0f, multiPlayerPosControl.gameObject.transform.position.z);
				if (Vector3.Distance(a, b) <= _radius && multiPlayerPosControl.PredictRawVelocity() < 2f)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag && _flagNode.Previous != null)
		{
			_flagNode.Previous.Value.ChangeState(state: true);
			ChangeState(state: true);
		}
		else
		{
			ChangeState(state: false);
		}
	}

	public void ChangeState(bool state)
	{
		Flag.ChangeBlinker(state);
		if (state)
		{
			_timerTrigger?.Dispose();
			_timerTrigger = Observable.Timer(TimeSpan.FromSeconds(30.0)).Subscribe((long _) =>
			{
				ChangeState(state: false);
			}).AddTo(this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerViaName.CarLayer && (bool)V.Car && V.Car == other.transform.parent.gameObject && !(Time.time - _lastPassTime <= 3f))
		{
			_flagController.OnFlagEntered(_flagNode);
			_lastPassTime = Time.time;
		}
	}

	private void OnDisable()
	{
		_timerTrigger?.Dispose();
	}
}
