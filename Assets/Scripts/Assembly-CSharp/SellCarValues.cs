using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using CPM.CarVisualModel;
using CPM.Database;
using CPM.Database.Constructors;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using Car_Parking.Scripts.Drag.CarCompositions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.BuyNewCar;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;

public class SellCarValues : MonoBehaviour, IVinylDrawPerformer, IBodyKitValuesParent
{
	private const string GlassWindowGameObjectName = "glass window";

	public int indexStyle;

	public int maxStyleCount;

	public GameObject body;

	public Material mainMat;

	public Material windowMat;

	public Vector3 pos;

	public Vector3 rot;

	public RenderTexture renderTexture;

	public RenderTexture windowRT;

	public AllVinylPreset allVinylsPreset;

	public CarPaintColor PurchasedColor;

	private SellCarWheel[] _wheels;

	public BuyBodyKitsUIButtonPositionGetter BuyBodyKitsUIButtonPositionGetter;

	public float WheelAngle = 33f;

	[NonSerialized]
	public bool UseDefaultTuningWheels;

	private WheelData _frontWheelData;

	private WheelData _rearWheelData;

	private VehicleStockTuningData _sellCarData;

	private DrawQueue _drawQueue;

	private OneVinylsPreset _currentVinylPreset;

	private VinylDrawer _vinylDrawer;

	private PlayerDatas.VinylsDataUtility _vinylsDataUtility;

	private Transform _lightProbeAnchor;

	public bool pauseSaving;

	public Collider Collider;

	private BuyCarBodyKits _buyCarBodyKits;

	private DefaultMaterialsConfig _defaultMaterialsConfig;

	private CarPriceHelperV2 _carPriceHelperV2;

	private EngineSound _engineSound;

	private float _redlineRpm = 8000f;

	private float _peakNm = 400f;

	private float _fakeInertia;

	private TurboType _turboType;

	private float _fakeMaxTqRpm;

	private float _fakeDisplacementCc;

	private bool _fakeIsRotary;

	private float[] _fakeTorqueCurve;

	private float _fakeIdleRpm = 800f;

	private AsyncOperationHandle<GameObject>? _handle;

	private CancellationTokenSource _swapCts;

	private LoadedKits _presetKits;

	private IPrefabFactory _prefabFactory;

	public AdvancedInt CarID { get; private set; }

	public Material CarMaterial => mainMat;

	public LightsComposition LightsComposition => null;

	public DefaultMaterialsConfig DefaultMaterialsConfig => _defaultMaterialsConfig;

	public BuyCarBodyKits BuyCarBodyKits => _buyCarBodyKits;

	public CarPriceHelperV2 CarPriceHelperV2 => _carPriceHelperV2;

	public bool IsFrontFloorChecked { get; set; }

	public bool IsRearFloorChecked { get; set; }

	public float FrontHeight { get; set; }

	public float RearHeight { get; set; }

	public MonoBehaviour ComponentToCheck => this;

	Transform IBodyKitValuesParent.transform => base.transform;

	public event Action<Color> CarColorChanged;

	public event Action<SellCarValues> OnChanged;

	public static string GetCarVinylPresetPath(int carID)
	{
		return $"LiveryPresets/PresetLiveryCar{carID}";
	}

	public static string GetVehicleStockTuningDataPath(int carID)
	{
		return string.Format("{0}car{1}", "Configs/CarTuningData/", carID);
	}

	public EngineSound GetEngineSound()
	{
		return _engineSound;
	}

	private void Awake()
	{
		_wheels = GetComponentsInChildren<SellCarWheel>();
		Match match = Regex.Match(base.gameObject.name, "\\d+");
		CarID = new AdvancedInt(-1);
		if (match.Success)
		{
			int num = int.Parse(match.Value);
			CarID = new AdvancedInt(num);
			_sellCarData = Resources.Load<VehicleStockTuningData>(GetVehicleStockTuningDataPath(num));
		}
	}

	[Inject]
	public void Construct(DrawQueue drawQueue, VinylDrawer vinylDrawer, PlayerDatas.VinylsDataUtility vinylsDataUtility, ICarTuningFactory carTuningFactory, DefaultMaterialsConfig defaultMaterialsConfig, CarPriceHelperV2 carPriceHelperV2, IPrefabFactory prefabFactory)
	{
		_vinylsDataUtility = vinylsDataUtility;
		_drawQueue = drawQueue;
		_vinylDrawer = vinylDrawer;
		_defaultMaterialsConfig = defaultMaterialsConfig;
		_carPriceHelperV2 = carPriceHelperV2;
		_prefabFactory = prefabFactory;
	}

	public void SetFake(float redlineRpm, float peakRpm, float inertia, TurboType aspirated, float maxTqRpm, float displacementCc, bool isRotary, float[] torqueCurve, float idleRpm)
	{
		_redlineRpm = redlineRpm;
		_peakNm = peakRpm;
		_turboType = aspirated;
		_fakeInertia = inertia;
		_fakeMaxTqRpm = maxTqRpm;
		_fakeDisplacementCc = displacementCc;
		_fakeIsRotary = isRotary;
		_fakeTorqueCurve = torqueCurve;
		_fakeIdleRpm = idleRpm;
	}

	private async UniTask SwapEngineSound(int id)
	{
		if (id == -1)
		{
			return;
		}
		_swapCts?.Cancel();
		_swapCts?.Dispose();
		_swapCts = new CancellationTokenSource();
		CancellationToken token = _swapCts.Token;
		try
		{
			if (_handle.HasValue && _handle.Value.IsValid())
			{
				Addressables.Release(_handle.Value);
				_handle = null;
			}
			AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>($"EngineSound_{id}");
			_handle = asyncOperationHandle;
			GameObject gameObject = await asyncOperationHandle.WithCancellation(token);
			if (token.IsCancellationRequested || !this)
			{
				return;
			}
			if (!gameObject)
			{
				Debug.LogWarning($"[SellCarValues] EngineSound_{id} не загрузился (нет Addressables-записи?) — car{id} без звука двигателя");
				return;
			}
			if ((bool)_engineSound)
			{
				UnityEngine.Object.Destroy(_engineSound.gameObject);
				_engineSound = null;
			}
			if (_prefabFactory != null)
			{
				GameObject gameObject2 = _prefabFactory.Instantiate(gameObject, base.transform);
				gameObject2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				_engineSound = gameObject2.GetComponent<EngineSound>();
			}
			if ((bool)_engineSound)
			{
				_engineSound.GetPositionGO(out var turboPos, out var exhaustPos);
				_engineSound.RepositionGO(turboPos, exhaustPos);
				_engineSound.SetSellCar(_redlineRpm, _peakNm, _fakeInertia, _turboType, _fakeMaxTqRpm, _fakeDisplacementCc, _fakeIsRotary, _fakeTorqueCurve, _fakeIdleRpm);
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public async UniTask LoadPresetBodyKits(bool stockOnly, CancellationToken token = default(CancellationToken))
	{
		ReleasePresetKits();
		if (stockOnly || !_sellCarData || !body)
		{
			return;
		}
		VehiclePresetTuningData[] presets = _sellCarData.Presets;
		VehiclePresetTuningData vehiclePresetTuningData = ((presets != null && presets.Length > 0) ? _sellCarData.Presets[0] : null);
		InstalledBodyKits installedBodyKits = ((!vehiclePresetTuningData) ? null : vehiclePresetTuningData.CarInfoMain?.InstalledBodyKits);
		if (installedBodyKits != null)
		{
			CarID.Get(out int value);
			CarModelLibrary carModelLibrary = Resources.Load<CarModelLibrary>("Configs/CarModels/CarModelLibrary");
			if (!carModelLibrary || !carModelLibrary.TryGet(value, out var entry))
			{
				Debug.LogWarning($"[SellCarValues] car{value}: нет CarModelLibrary-записи — киты пресета не поставлены");
				return;
			}
			Transform transform = new GameObject("PresetKitFrame").transform;
			transform.SetParent(base.transform, worldPositionStays: false);
			transform.localPosition = pos - entry.BodyAnchorLocal;
			_presetKits = await InstalledBodyKitLoader.LoadAsync(value, transform.gameObject, body, entry, installedBodyKits, token);
		}
	}

	private void ReleasePresetKits()
	{
		if (_presetKits.Handles == null)
		{
			return;
		}
		foreach (AsyncOperationHandle handle in _presetKits.Handles)
		{
			if (handle.IsValid())
			{
				Addressables.Release(handle);
			}
		}
		_presetKits = default;
	}

	public int StyleDefault()
	{
		if (!_sellCarData)
		{
			return -1;
		}
		return _sellCarData.DefaultStyle;
	}

	private CombinedPrice GetDefaultPrice()
	{
		CarID.Get(out int value);
		return _carPriceHelperV2.GetCarPrice(value);
	}

	public CombinedPrice GetPrice()
	{
		CombinedPrice defaultPrice = GetDefaultPrice();
		if (indexStyle > 0 && indexStyle <= maxStyleCount && (bool)allVinylsPreset && indexStyle <= allVinylsPreset.allPresetList.Count)
		{
			defaultPrice.AddCoin(allVinylsPreset.allPresetList[indexStyle - 1].price);
		}
		if ((bool)_buyCarBodyKits)
		{
			defaultPrice += _buyCarBodyKits.GetPrice();
		}
		return defaultPrice;
	}

	public void SetBuyBodyKits(BuyCarBodyKits buyCarBodyKits)
	{
		if (_buyCarBodyKits != null)
		{
			_buyCarBodyKits.OnChanged -= OnBodyKitsChanged;
		}
		_buyCarBodyKits = buyCarBodyKits;
		if ((bool)_buyCarBodyKits)
		{
			_buyCarBodyKits.OnChanged += OnBodyKitsChanged;
		}
	}

	private void CreateDebugText()
	{
	}

	private void Start()
	{
		InstallWheels();
		CreateDebugText();
		SetLightProbeAnchor();
	}

	private void OnEnable()
	{
		CarID.Get(out int value);
		SwapEngineSound(value).Forget();
		SetBuyBodyKits(_buyCarBodyKits);
	}

	private void OnDisable()
	{
		if ((bool)_buyCarBodyKits)
		{
			_buyCarBodyKits.OnChanged -= OnBodyKitsChanged;
		}
	}

	private void OnBodyKitsChanged(BuyCarBodyKits buyCarBodyKits)
	{
		OnChanged?.Invoke(this);
	}

	private void InstallWheels()
	{
		if (_wheels.Length == 0 || !_sellCarData)
		{
			return;
		}
		_frontWheelData = _sellCarData.Tuning.FrontWheelData;
		_rearWheelData = _sellCarData.Tuning.RearWheelData;
		if (!UseDefaultTuningWheels)
		{
			VehiclePresetTuningData[] presets = _sellCarData.Presets;
			if (presets != null && presets.Length > 0 && (bool)_sellCarData.Presets[0])
			{
				_frontWheelData = _sellCarData.Presets[0].FrontWheelData;
				_rearWheelData = _sellCarData.Presets[0].RearWheelData;
			}
		}
		SellCarWheel[] wheels = _wheels;
		foreach (SellCarWheel sellCarWheel in wheels)
		{
			if ((bool)sellCarWheel)
			{
				if (base.transform.InverseTransformPoint(sellCarWheel.transform.position).z > 0f)
				{
					sellCarWheel.transform.Rotate(0f, WheelAngle, 0f, Space.Self);
					WheelConstructor.InstallSellCarWheel(sellCarWheel, _frontWheelData, this);
				}
				else
				{
					WheelConstructor.InstallSellCarWheel(sellCarWheel, _rearWheelData, this);
				}
			}
		}
		CalculatePosition();
	}

	private void SetLightProbeAnchor()
	{
		if (!_lightProbeAnchor)
		{
			_lightProbeAnchor = Utils.SetLpAnchor(base.transform, base.gameObject, getCenter: true);
			_lightProbeAnchor.parent = base.transform;
		}
		else
		{
			Utils.SetLpAnchor(_lightProbeAnchor, base.gameObject);
		}
	}

	public void OnMainBodyLoaded()
	{
		if (!mainMat && body.TryGetComponent<BodyMarker>(out var component))
		{
			mainMat = component.mainMat;
			SetLightProbeAnchor();
		}
		if (!windowMat && (bool)body)
		{
			Transform transform = body.transform.Find("glass window");
			if ((bool)transform && transform.TryGetComponent<MeshRenderer>(out var component2))
			{
				windowMat = component2.sharedMaterial;
			}
		}
		if ((bool)mainMat)
		{
			PurchasedColor = _sellCarData.CarInfoDefault.Colors.CarColor;
			mainMat.mainTexture = null;
			mainMat.color = PurchasedColor.Color;
			ColorsService.SetCarPaintColor(mainMat, PurchasedColor);
			mainMat.SetInt(ShaderPropertyIDs.DirtAmount, 0);
		}
		if ((bool)windowMat)
		{
			windowMat.mainTexture = null;
		}
		if ((bool)V.CoreGameManager)
		{
			LoadPreset().Forget();
		}
		LoadPresetBodyKits(stockOnly: false, this.GetCancellationTokenOnDestroy()).Forget();
	}

	private async UniTaskVoid LoadPreset()
	{
		if ((bool)allVinylsPreset)
		{
			return;
		}
		await UniTask.WaitWhile(() => V.CoreGameManager.dealerWindow == null);
		Debug.Log("Request for load preset from resources");
		ResourceRequest rr = Resources.LoadAsync<AllVinylPreset>($"LiveryPresets/PresetLiveryCar{CarID}");
		rr.completed += (AsyncOperation _) =>
		{
			allVinylsPreset = rr.asset as AllVinylPreset;
			if ((bool)allVinylsPreset && allVinylsPreset.allPresetList.Count != 0)
			{
				V.CoreGameManager.dealerWindow.OnCarPreSetLoaded();
				if ((int)_sellCarData.DefaultStyle >= 0)
				{
					indexStyle = (int)_sellCarData.DefaultStyle + 1;
					UpdatePreset(indexStyle);
				}
			}
			OnChanged?.Invoke(this);
		};
	}

	private async void ReadEveryLivery()
	{
		await UniTask.Delay(500);
		int count = 1;
		int ver = PlayerPrefs.GetInt("verPreset", 0);
		if (allVinylsPreset != null)
		{
			foreach (OneVinylsPreset allPreset in allVinylsPreset.allPresetList)
			{
				_ = allPreset;
				UpdatePreset(count);
				await UniTask.Delay(100);
				string path = $"liveryPresetPack{ver}/{ver}_{CarID}_{count}.png";
				SaveRTToPNG(renderTexture, path);
				count++;
			}
		}
		if (!pauseSaving)
		{
			V.OnCarLiveryEnd?.Invoke();
		}
	}

	private void SaveRTToPNG(RenderTexture rt, string path)
	{
		if (rt == null)
		{
			Debug.LogError("No RenderTexture selected.");
			return;
		}
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt;
		Texture2D texture2D = new Texture2D(rt.width, rt.height);
		texture2D.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		byte[] bytes = texture2D.EncodeToPNG();
		if (!Directory.Exists(Path.GetDirectoryName(path)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
		}
		File.WriteAllBytes(path, bytes);
		Debug.Log("Saved RenderTexture to " + path);
		UnityEngine.Object.DestroyImmediate(texture2D);
	}

	public void ResetCustomization()
	{
		indexStyle = 0;
		OnStyleChanged(0);
		if ((bool)BuyCarBodyKits)
		{
			BuyCarBodyKits.ResetBodyKits();
		}
	}

	public int OnStyleChanged(int step)
	{
		if ((bool)allVinylsPreset)
		{
			maxStyleCount = allVinylsPreset.allPresetList.Count;
		}
		indexStyle = (int)Mathf.Repeat(indexStyle + step, maxStyleCount + 1);
		UpdatePreset(indexStyle);
		OnChanged?.Invoke(this);
		return indexStyle;
	}

	private void OnDestroy()
	{
		ReleasePresetKits();
		_swapCts?.Cancel();
		_swapCts?.Dispose();
		if (_handle.HasValue && _handle.Value.IsValid())
		{
			Addressables.Release(_handle.Value);
		}
		if ((bool)_sellCarData)
		{
			CarInfoMain carInfoDefault = _sellCarData.CarInfoDefault;
			if (carInfoDefault != null && carInfoDefault.Colors != null)
			{
				PurchasedColor = _sellCarData.CarInfoDefault.Colors.CarColor;
			}
		}
		if ((bool)mainMat)
		{
			mainMat.mainTexture = null;
			mainMat.color = PurchasedColor.Color;
		}
		if ((bool)windowMat)
		{
			windowMat.mainTexture = null;
		}
		if ((bool)windowRT)
		{
			windowRT.Release();
			UnityEngine.Object.Destroy(windowRT);
			windowRT = null;
		}
		if ((bool)renderTexture)
		{
			MonoBehaviour.print("destroing rneder tx: " + renderTexture);
			renderTexture.Release();
			UnityEngine.Object.Destroy(renderTexture);
			renderTexture = null;
			if ((bool)_engineSound)
			{
				_engineSound.TurnOffEngineSound();
				UnityEngine.Object.Destroy(_engineSound.gameObject);
				_engineSound = null;
			}
		}
	}

	private void UpdatePreset(int indexToSet)
	{
		if (indexToSet == 0 && (int)_sellCarData.DefaultStyle >= 0)
		{
			indexToSet++;
		}
		if (indexToSet == 0)
		{
			if ((bool)mainMat)
			{
				mainMat.mainTexture = null;
				PurchasedColor = _sellCarData.CarInfoDefault.Colors.CarColor;
				mainMat.color = PurchasedColor.Color;
				ColorsService.SetCarPaintColor(mainMat, PurchasedColor);
				CarColorChanged?.Invoke(PurchasedColor.Color);
			}
		}
		else if (!mainMat)
		{
			MonoBehaviour.print("main mat is null");
		}
		else
		{
			_currentVinylPreset = allVinylsPreset.allPresetList[indexToSet - 1];
			int valueOrDefault = (_currentVinylPreset.vynilOneCarList?.allVynils?.Count).GetValueOrDefault();
			int valueOrDefault2 = (_currentVinylPreset.windowVynilOneCarList?.allVynils?.Count).GetValueOrDefault();
			if (valueOrDefault + valueOrDefault2 > 0)
			{
				_drawQueue.Enqueue(this);
			}
		}
	}

	public int GetVinylsCount(VinylsType type, bool isFrontPlate)
	{
		if (_currentVinylPreset == null)
		{
			return 0;
		}
		switch (type)
		{
		case VinylsType.CarVinyls:
			return (_currentVinylPreset.vynilOneCarList?.allVynils?.Count).GetValueOrDefault();
		case VinylsType.WindowVinyls:
			if ((bool)windowMat)
			{
				return (_currentVinylPreset.windowVynilOneCarList?.allVynils?.Count).GetValueOrDefault();
			}
			break;
		}
		return 0;
	}

	public void RenderVinyls(VinylsType type, bool isFrontPlate)
	{
		try
		{
			switch (type)
			{
			case VinylsType.CarVinyls:
				if ((bool)renderTexture)
				{
					renderTexture.Release();
					UnityEngine.Object.Destroy(renderTexture);
					renderTexture = null;
				}
				mainMat.mainTexture = (renderTexture = _vinylDrawer.GetRenderTexture(VinylsType.CarVinyls));
				_vinylsDataUtility.LoadAllVinyls(_currentVinylPreset.vynilOneCarList, VinylsType.CarVinyls);
				_vinylDrawer.FocusOnVinyls(VinylsType.CarVinyls);
				_vinylDrawer.TakeSnapshot();
				break;
			case VinylsType.WindowVinyls:
				if ((bool)windowMat)
				{
					if ((bool)windowRT)
					{
						windowRT.Release();
						UnityEngine.Object.Destroy(windowRT);
						windowRT = null;
					}
					windowMat.mainTexture = (windowRT = _vinylDrawer.GetRenderTexture(VinylsType.WindowVinyls));
					_vinylsDataUtility.LoadAllVinyls(_currentVinylPreset.windowVynilOneCarList, VinylsType.WindowVinyls);
					_vinylDrawer.FocusOnVinyls(VinylsType.WindowVinyls);
					_vinylDrawer.TakeSnapshot();
				}
				break;
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		finally
		{
			if (type == VinylsType.CarVinyls)
			{
				PurchasedColor.Color = _currentVinylPreset.carColor;
				mainMat.color = PurchasedColor.Color;
				ColorsService.SetCarPaintColor(mainMat, PurchasedColor);
				CarColorChanged?.Invoke(PurchasedColor.Color);
			}
			_vinylDrawer.DeleteVinylContext(type);
		}
	}

	private void CalculatePosition()
	{
		float wheelbase = GetWheelbase();
		float x = Mathf.Atan2(RearHeight - FrontHeight, wheelbase) * 57.29578f;
		float num = (FrontHeight + RearHeight) * 0.5f;
		Quaternion rotation = Quaternion.Euler(x, base.transform.eulerAngles.y, base.transform.eulerAngles.z);
		Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + num, base.transform.position.z);
		base.transform.SetPositionAndRotation(position, rotation);
	}

	private float GetWheelbase()
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		SellCarWheel[] wheels = _wheels;
		for (int i = 0; i < wheels.Length; i++)
		{
			float z = wheels[i].transform.localPosition.z;
			num = Mathf.Min(num, z);
			num2 = Mathf.Max(num2, z);
		}
		return Mathf.Abs(num2 - num);
	}
}
