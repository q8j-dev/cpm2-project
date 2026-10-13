using System;
using System.Collections.Generic;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using ElectronicRoadSign;
using Photon.Pun;
using Photon.Realtime;
using Service;
using Service.Abstract;
using Service.Delivery;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Economics;

public class WeightTollStation : MonoBehaviour
{
	public ElectronicRoadSignScript display;

	[SerializeField]
	private List<GameObject> _objectsList = default;

	private int _totalMass;

	private Transform _tollGate;

	private Vector3 openGate = new Vector3(90f, -180f, -90f);

	private Vector3 closeGate = new Vector3(0f, -180f, -90f);

	private CurrencyManagerController _currencyManagerController;

	private IPricesService _pricesService;

	private IDisposable _closeGateDisposable;

	[Inject]
	public void Construct(CurrencyManagerController currencyManagerController, IPricesService pricesService)
	{
		_pricesService = pricesService;
		_currencyManagerController = currencyManagerController;
	}

	public void Awake()
	{
		_tollGate = base.transform.GetChild(1);
		_totalMass = 0;
		V.OnPlayerDisconnected += RemovePlayerFromWeights;
	}

	private void OnDestroy()
	{
		V.OnPlayerDisconnected -= RemovePlayerFromWeights;
		_closeGateDisposable?.Dispose();
		foreach (GameObject objects in _objectsList)
		{
			if ((bool)objects && objects.TryGetComponent<Powertrain>(out var component))
			{
				component.OnMassChangedEvent -= OnCarMassChanged;
			}
		}
	}

	private void OnValidate()
	{
		if (display == null)
		{
			throw new Exception("ATTACH ElectronicRoadSign to WeightTollStation");
		}
	}

	private void SetDisplayText(int mass)
	{
		mass = Mathf.Min(99999, mass);
		display.MessageText = Utils.GetMassNoLoc(mass, round: true);
	}

	private void RecalculateMass()
	{
		_totalMass = 0;
		for (int num = _objectsList.Count - 1; num >= 0; num--)
		{
			Rigidbody component;
			MultiPersonControl component2;
			VehicleData component3;
			if (_objectsList[num] == null)
			{
				_objectsList.RemoveAt(num);
			}
			else if (_objectsList[num].TryGetComponent<Rigidbody>(out component) && !_objectsList[num].TryGetComponent<MultiPersonControl>(out component2) && (!_objectsList[num].TryGetComponent<VehicleData>(out component3) || !component3.Towing))
			{
				_totalMass += Mathf.RoundToInt(component.mass);
			}
		}
	}

	private void RemovePlayerFromWeights(Player player)
	{
		Dictionary<int, ClientController> clients = Global.TaskManager.DeliveryManager.clients;
		if (clients.ContainsKey(player.ActorNumber) && clients[player.ActorNumber] != null)
		{
			foreach (ItemController item in clients[player.ActorNumber].GetComponent<TrailerController>().containerController.items)
			{
				_objectsList.Remove(item.gameObject);
			}
		}
		for (int num = _objectsList.Count - 1; num >= 0; num--)
		{
			if (_objectsList[num] == null)
			{
				_objectsList.RemoveAt(num);
			}
			else
			{
				bool flag = false;
				PhotonView component = _objectsList[num].GetComponent<PhotonView>();
				MultiPersonControl component2;
				if ((bool)component && component.CreatorActorNr == player.ActorNumber)
				{
					flag = true;
				}
				else if (_objectsList[num].TryGetComponent<MultiPersonControl>(out component2) && component2.currentCar != null)
				{
					component = component2.currentCar.GetComponent<PhotonView>();
					if (component.CreatorActorNr == player.ActorNumber)
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (_objectsList[num].TryGetComponent<Powertrain>(out var component3))
					{
						component3.OnMassChangedEvent -= OnCarMassChanged;
					}
					_objectsList.RemoveAt(num);
				}
			}
		}
		RecalculateMass();
		SetDisplayText(_totalMass);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 23 || other.attachedRigidbody == null)
		{
			return;
		}
		GameObject gameObject = other.attachedRigidbody.gameObject;
		if (!_objectsList.Contains(gameObject))
		{
			_objectsList.Add(gameObject);
			if (gameObject.TryGetComponent<Powertrain>(out var component))
			{
				component.OnMassChangedEvent += OnCarMassChanged;
			}
			if (gameObject.TryGetComponent<VehicleData>(out var _))
			{
				OpenTollGate();
			}
			RecalculateMass();
			SetDisplayText(_totalMass);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 23 || other.attachedRigidbody == null)
		{
			return;
		}
		GameObject gameObject = other.attachedRigidbody.gameObject;
		if (!_objectsList.Contains(gameObject))
		{
			return;
		}
		if (gameObject.TryGetComponent<Powertrain>(out var component))
		{
			component.OnMassChangedEvent -= OnCarMassChanged;
		}
		_objectsList.Remove(gameObject);
		if (V.Car == gameObject && gameObject.TryGetComponent<VehicleData>(out var component2) && !component2.Towing)
		{
			PayForTransit();
		}
		RecalculateMass();
		SetDisplayText(_totalMass);
		if (_totalMass == 0)
		{
			_closeGateDisposable?.Dispose();
			_closeGateDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				CloseTollGate();
			}).AddTo(this);
		}
	}

	private void OnCarMassChanged(Powertrain _)
	{
		RecalculateMass();
		SetDisplayText(_totalMass);
	}

	private void OpenTollGate()
	{
		_closeGateDisposable?.Dispose();
		_tollGate.DOLocalRotate(openGate, 3f);
	}

	private void CloseTollGate()
	{
		_tollGate.DOLocalRotate(closeGate, 3f);
	}

	private void PayForTransit()
	{
		ObscuredInt obscuredInt = (int)_pricesService.WeightTallDefault + _totalMass / (int)_pricesService.WeightTallWeightCoef;
		V.CoreGameManager.SetMessage($"-{obscuredInt}$", 1f);
		AdvancedInt price = new AdvancedInt(obscuredInt);
		_currencyManagerController.TryMoneySpend(out var _, price);
	}
}
