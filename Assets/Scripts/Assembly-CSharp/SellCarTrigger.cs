using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Free_drive.Scenes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Trade;

public class SellCarTrigger : MonoBehaviour
{
	private const int MaxCarSpeedToShowWindow = 1;

	public const string ColliderName = "Collider";

	public GameObject edges;

	public Collider carCollider;

	private Collider _triggerCollider;

	private Bounds _fakeSellingCarBounds;

	private IDisposable _carSpeedCheck;

	private GameObject _sellingCar;

	private SellCarTriggerHelperNoMove _sellCarTriggerHelperNoMove;

	private ISellExchangeFreeService _sellExchangeFreeService;

	private ITradeService _tradeService;

	private ManagerUI _managerUI;

	private IPrefabFactory _prefabFactory;

	private FreeDriveProvider _freeDriveProvider;

	private CarSellTriggersCollection _carSellTriggersCollection;

	private MultiPlayer _multiPlayer;

	private CarSellTriggerExitHandler _carSellTriggerExitHandler;

	[field: SerializeField]
	public int Index { get; private set; } = -1;

	public BuyCarWorldWindow BuyCarWorldWindow { get; set; }

	public string TradeOfferId => SellCarTriggerData?.TradeOfferId ?? "";

	public AdvancedInt PriceCar => SellCarTriggerData?.PriceCar ?? new AdvancedInt(-1);

	private SellCarTriggerData SellCarTriggerData => _carSellTriggersCollection.GetSellCarTriggerData(Index);

	public GameObject SellingCar
	{
		get
		{
			return _sellingCar;
		}
		set
		{
			_sellingCar = value;
			_sellCarTriggerHelperNoMove.Init(_sellingCar);
			edges.SetActive(!_sellingCar);
			if (!_sellingCar)
			{
				carCollider = null;
				CloseBuyCarWorldWindow();
			}
		}
	}

	private static bool IsCarSaleAvailable
	{
		get
		{
			if (FlagsStorage.IsMultiFree)
			{
				return PhotonNetwork.InRoom;
			}
			return false;
		}
	}

	[Inject]
	public void Construct(ISellExchangeFreeService sellExchangeFreeService, ManagerUI managerUI, IPrefabFactory prefabFactory, ITradeService tradeService, FreeDriveProvider freeDriveProvider, CarSellTriggersCollection carSellTriggersCollection, MultiPlayer multiPlayer, CarSellTriggerExitHandler carSellTriggerExitHandler)
	{
		_sellExchangeFreeService = sellExchangeFreeService;
		_managerUI = managerUI;
		_prefabFactory = prefabFactory;
		_tradeService = tradeService;
		_freeDriveProvider = freeDriveProvider;
		_carSellTriggersCollection = carSellTriggersCollection;
		_multiPlayer = multiPlayer;
		_carSellTriggerExitHandler = carSellTriggerExitHandler;
	}

	private void Awake()
	{
		_triggerCollider = GetComponent<Collider>();
		edges.gameObject.SetActive(value: true);
		PlaceLabelTopOfCarAsync(base.destroyCancellationToken).Forget();
		_sellCarTriggerHelperNoMove = base.gameObject.AddComponent<SellCarTriggerHelperNoMoveV2>();
	}

	private void Start()
	{
		_ = Index;
		_ = 0;
		if (_carSellTriggersCollection != null)
		{
			SellCarTriggerData.OnDataChanged += OnSellCarTriggerDataChanged;
			OnSellCarTriggerDataChanged(SellCarTriggerData);
		}
	}

	private void OnDestroy()
	{
		if (_carSellTriggersCollection != null)
		{
			SellCarTriggerData.OnDataChanged -= OnSellCarTriggerDataChanged;
		}
		_carSpeedCheck?.Dispose();
	}

	private void OnSellCarTriggerDataChanged(SellCarTriggerData sellCarTriggerData)
	{
		if (sellCarTriggerData.IsOccupied)
		{
			TradeOffer offer = _tradeService.GetOffer(sellCarTriggerData.TradeOfferId);
			if (offer == null)
			{
				return;
			}
			GameObject carByOwnerID = _multiPlayer.GetCarByOwnerID(offer.OfferActorNr, needToCheckFirstCar: true);
			if ((bool)carByOwnerID)
			{
				VehicleData component = carByOwnerID.GetComponent<VehicleData>();
				if ((bool)component)
				{
					InitSellCar(sellCarTriggerData.CarModel, carByOwnerID, component.Collider);
				}
			}
		}
		else
		{
			SellingCar = null;
			CloseBuyCarWorldWindow();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!IsCarSaleAvailable || FlagsStorage.IsPhotoMode || !other.transform.parent || (bool)carCollider)
		{
			return;
		}
		GameObject car = other.transform.parent.gameObject;
		if (!(other.name == "Collider") || !car.TryGetComponent<VehicleData>(out var carData) || !_freeDriveProvider.FirstCarMultiPlayer || car != _freeDriveProvider.FirstCarMultiPlayer)
		{
			return;
		}
		_carSpeedCheck?.Dispose();
		_carSpeedCheck = Observable.Timer(TimeSpan.FromSeconds(0.5)).Repeat().Subscribe((long _) =>
		{
			if ((bool)carData && (bool)carData.Collider && !_triggerCollider.bounds.Intersects(carData.Collider.bounds))
			{
				_carSpeedCheck?.Dispose();
			}
			else if (carData.GetPowertrain.SpeedKmh < 1f)
			{
				OnCarEnter(car);
			}
		})
			.AddTo(carData.gameObject);
	}

	private async UniTask PlaceLabelTopOfCarAsync(CancellationToken ct)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		while (!ct.IsCancellationRequested)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			if ((bool)SellingCar && (bool)carCollider && (bool)BuyCarWorldWindow)
			{
				BuyCarWorldWindow.transform.position = GetBuyCarWindowPosition();
				BuyCarWorldWindow.gameObject.SetActive(carCollider.gameObject.activeInHierarchy);
			}
		}
	}

	public Vector3 GetBuyCarWindowPosition()
	{
		Vector3 position = base.transform.position;
		if ((bool)carCollider)
		{
			position = carCollider.transform.position;
			position.y = carCollider.bounds.max.y + 1.8f;
		}
		return position;
	}

	public void OnCarEnter(GameObject car)
	{
		_carSpeedCheck?.Dispose();
		_sellExchangeFreeService.TrySetCarOnSellTriggerAsync(this, car).Forget();
	}

	public void BuyThisCar()
	{
		if ((bool)SellingCar && SellingCar.TryGetComponent<VehicleData>(out var component) && component.СurrentSellCarTrigger != null)
		{
			_tradeService.TryTradeAsync(TradeOfferId).Forget();
		}
	}

	private void InitSellCar(string carModel, GameObject car, Collider carCollider)
	{
		SellingCar = car;
		this.carCollider = carCollider;
		CreateBuyCarWorldWindow(carModel, base.destroyCancellationToken).Forget();
		_carSellTriggerExitHandler.Add(car, Index, carCollider, _triggerCollider.bounds);
	}

	private async UniTask CreateBuyCarWorldWindow(string carModel, CancellationToken ct)
	{
		if (!BuyCarWorldWindow)
		{
			GameObject obj = await _managerUI.LoadWindowPrefab("BuyCarWorldWindow");
			if (ct.IsCancellationRequested || !this)
			{
				return;
			}
			BuyCarWorldWindow component = _prefabFactory.Instantiate(obj).GetComponent<BuyCarWorldWindow>();
			BuyCarWorldWindow = component;
		}
		PriceCar.Get(out int value);
		BuyCarWorldWindow.Init(carModel, value, this);
		BuyCarWorldWindow.transform.position = GetBuyCarWindowPosition();
		if (FlagsStorage.IsPhotoMode)
		{
			BuyCarWorldWindow.SetActive(active: false);
		}
	}

	private void CloseBuyCarWorldWindow()
	{
		if ((bool)BuyCarWorldWindow)
		{
			BuyCarWorldWindow.Close();
		}
		BuyCarWorldWindow = null;
	}
}
