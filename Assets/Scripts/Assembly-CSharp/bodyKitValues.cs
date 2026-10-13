using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;

public class bodyKitValues : MonoBehaviour
{
	private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	public CarDoorType CarDoorType = CarDoorType.None;

	public Transform MoveableObjectsParent;

	public Transform PlateTrunkParent;

	public Transform PlatePoint;

	public Renderer render;

	public Material MainMaterial;

	public int Type;

	[NonSerialized]
	public int Index;

	private IBodyKitValuesParent _bodyKitValuesParent;

	public Vector3 localPosition;

	public Vector3 localEuler;

	public Material fadeMat;

	public Animator animator;

	public GameObject diod;

	public GameObject diodL;

	public GameObject diodR;

	public GameObject lights;

	public GameObject frontLight;

	public GameObject rearLight;

	public GameObject rearBrake;

	public GameObject turnLeft;

	public GameObject turnRight;

	public GameObject reverseLight;

	public SpoilerSystem SpoilerSystem;

	public GameObject[] AdditionalElements;

	public GameObject[] AdditionalElementsWithoutVinyls;

	public GameObject Trunk;

	public GameObject[] GlassWindows;

	private List<Renderer> _allRenderers = new List<Renderer>();

	private List<Material> _startMaterials = new List<Material>();

	private List<Material> _tempMaterials = new List<Material>();

	private const float _fadeDeltaTime = 0.01f;

	private bool _toDestroy;

	private bool _isMaterialRecreated;

	private CancellationTokenSource _fadeCancellationToken;

	private bool _isSellCar;

	private VehicleData _vehicleData;

	private bool _wasStart;

	public VinylDrawer VinylDrawer { private get; set; }

	[field: SerializeField]
	public bool IsMirrorsThisBodyKit { get; private set; }

	[field: SerializeField]
	public GameObject LMirror { get; private set; }

	[field: SerializeField]
	public GameObject RMirror { get; private set; }

	[field: SerializeField]
	public GameObject MMirror { get; private set; }

	[field: SerializeField]
	public bool DisableDefaultSpoilerSystem { get; private set; }

	public BodyKitType BodyKitType
	{
		get
		{
			return (BodyKitType)Type;
		}
		set
		{
			if (value != BodyKitType)
			{
				Type = (int)value;
				if (!FlagsStorage.OnVisualTuning)
				{
					UpdateMaterialInStart();
				}
			}
		}
	}

	private bool NoVinylMaterial
	{
		get
		{
			BodyKitType bodyKitType = BodyKitType;
			return bodyKitType == BodyKitType.Spoiler || bodyKitType == BodyKitType.AirIntakeHood || bodyKitType == BodyKitType.AirIntakeRoof;
		}
	}

	[field: SerializeField]
	public GameObject[] LowPolyLODs { get; private set; }

	[field: SerializeField]
	public GameObject[] NormalLODs { get; private set; }

	public bool IsFading { get; private set; }

	public event Action OnBodyKitDestroed;

	private void Awake()
	{
		TryGetValuesParentIfNeeds();
		_isSellCar = GetComponentInParent<SellCarValues>() != null;
		if (!render)
		{
			render = GetComponentInChildren<MeshRenderer>(includeInactive: true);
		}
		if ((bool)render && NoVinylMaterial && (bool)render.sharedMaterial)
		{
			MainMaterial = new Material(render.sharedMaterial.shader);
			MainMaterial.CopyPropertiesFromMaterial(render.sharedMaterial);
			MainMaterial.name = base.gameObject.name;
			render.sharedMaterial = MainMaterial;
			_isMaterialRecreated = true;
		}
		SetLayerRecursively(base.gameObject, LayerViaName.CarBody);
		ResetMainMaterial();
	}

	public void SetLayerRecursively(GameObject carBodyObj, int layer)
	{
		carBodyObj.layer = layer;
		foreach (Transform item in carBodyObj.transform)
		{
			if ((bool)item)
			{
				SetLayerRecursively(item.gameObject, layer);
			}
		}
	}

	private void OnDestroy()
	{
		if (_isMaterialRecreated)
		{
			CommonExtensions.DestroyMat(ref MainMaterial);
		}
		CommonExtensions.DestroyMat(ref fadeMat);
		CommonExtensions.ClearObjectCollection(_tempMaterials);
		_allRenderers.Clear();
		_startMaterials.Clear();
		_toDestroy = true;
		CommonExtensions.CancelAndDisposeToken(ref _fadeCancellationToken);
		OnBodyKitDestroed?.Invoke();
		OnBodyKitDestroed = null;
		if (DisableDefaultSpoilerSystem && (bool)_vehicleData && _vehicleData.BodyLoadingComposition != null)
		{
			_vehicleData.BodyLoadingComposition.RemoveDisableSpoilerKit(this);
		}
	}

	private void Start()
	{
		if (_wasStart)
		{
			return;
		}
		_wasStart = true;
		if (_bodyKitValuesParent == null)
		{
			return;
		}
		_vehicleData = _bodyKitValuesParent.transform.GetComponent<VehicleData>();
		if ((bool)MainMaterial)
		{
			ResetMainMaterial();
		}
		if ((bool)render)
		{
			if (FlagsStorage.OnVisualTuning)
			{
				StartShowing();
			}
			else
			{
				UpdateMaterialInStart();
			}
		}
		bool num = (bool)_vehicleData && (bool)_vehicleData.photonView && _vehicleData.photonView.IsMine;
		bool flag = (bool)_vehicleData && _vehicleData.gameObject == V.Car;
		bool flag2 = (bool)_vehicleData && _vehicleData.FreeDriveProvider != null && _vehicleData.FreeDriveProvider.FirstCarMultiPlayer == _vehicleData.gameObject;
		bool flag3 = FlagsStorage.InGarageMenu || FlagsStorage.InGarageFree;
		bool flag4 = num | flag | flag2 | flag3;
		SetLODsLevel(!flag4);
		if (_bodyKitValuesParent != null)
		{
			SetMaterials();
			Utils.SetLpAnchor(_bodyKitValuesParent.transform, base.gameObject);
			InitializeKits();
		}
	}

	private void SetMaterials()
	{
		if (_bodyKitValuesParent.LightsComposition != null)
		{
			_bodyKitValuesParent.LightsComposition.SetHeadLight(frontLight);
			_bodyKitValuesParent.LightsComposition.SetDiod(diod, diodL, diodR);
			_bodyKitValuesParent.LightsComposition.SetRearBrake(rearBrake);
			_bodyKitValuesParent.LightsComposition.SetRearLight(rearLight);
			_bodyKitValuesParent.LightsComposition.SetLeftTurn(turnLeft);
			_bodyKitValuesParent.LightsComposition.SetRightTurn(turnRight);
			_bodyKitValuesParent.LightsComposition.SetReverseLight(reverseLight);
			_bodyKitValuesParent.LightsComposition.SetLights(lights);
			_bodyKitValuesParent.LightsComposition.SetupHDLights();
			_bodyKitValuesParent.LightsComposition.ApplyCurrentLightState();
		}
	}

	private void SetLODsLevel(bool isLowPoly)
	{
		GameObject[] lowPolyLODs;
		if (!LowPolyLODs.IsNullOrEmpty())
		{
			lowPolyLODs = LowPolyLODs;
			foreach (GameObject gameObject in lowPolyLODs)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(isLowPoly);
				}
			}
		}
		if (NormalLODs.IsNullOrEmpty())
		{
			return;
		}
		lowPolyLODs = NormalLODs;
		foreach (GameObject gameObject2 in lowPolyLODs)
		{
			if ((bool)gameObject2)
			{
				gameObject2.SetActive(!isLowPoly);
			}
		}
	}

	private void InitializeKits()
	{
		SetElementsWithoutVinyls();
		SetAdditionalElements().Forget();
		SetCarWindowElements();
	}

	private void OnEnable()
	{
		if (_bodyKitValuesParent != null)
		{
			_bodyKitValuesParent.CarColorChanged += CarColorChangedCallback;
		}
	}

	private void OnDisable()
	{
		if (_bodyKitValuesParent != null)
		{
			_bodyKitValuesParent.CarColorChanged -= CarColorChangedCallback;
		}
	}

	private void UpdateMaterialInStart()
	{
		if ((bool)render && !fadeMat && fadeMat != render.sharedMaterial)
		{
			render.sharedMaterial = ((NoVinylMaterial && (bool)MainMaterial) ? MainMaterial : _bodyKitValuesParent.CarMaterial);
		}
	}

	public void ResetMainMaterial()
	{
		TryGetValuesParentIfNeeds();
		if (_bodyKitValuesParent != null && (bool)MainMaterial && NoVinylMaterial)
		{
			VehicleData component = _bodyKitValuesParent.transform.GetComponent<VehicleData>();
			if ((bool)component && component.CarInfoProvider.CarInfo.Colors.BodyKits.TryGet(BodyKitType, out var color))
			{
				ColorsService.SetCarPaintColor(MainMaterial, color);
			}
			else if ((bool)_bodyKitValuesParent.CarMaterial)
			{
				MainMaterial.CopyPropertiesFromMaterial(_bodyKitValuesParent.CarMaterial);
				MainMaterial.color = _bodyKitValuesParent.CarMaterial.color;
				MainMaterial.mainTexture = null;
			}
		}
	}

	public void StartFade()
	{
		PrepareAndFade(fade: true);
	}

	public void StartShowing()
	{
		PrepareAndFade(fade: false);
	}

	public void ShowInstant()
	{
		CommonExtensions.CancelAndDisposeToken(ref _fadeCancellationToken);
		IsFading = false;
		Start();
		if (!render)
		{
			render = GetComponentInChildren<Renderer>(includeInactive: true);
		}
		if ((bool)render)
		{
			TryGetValuesParentIfNeeds();
			if (_bodyKitValuesParent != null)
			{
				CollectChildRenderers();
				RevertAllRenderers();
				SetMaterials();
			}
		}
	}

	private void PrepareAndFade(bool fade)
	{
		Start();
		if (!render)
		{
			render = GetComponentInChildren<Renderer>(includeInactive: true);
		}
		if (!render)
		{
			return;
		}
		if (!fadeMat)
		{
			TryGetValuesParentIfNeeds();
			if (_bodyKitValuesParent?.DefaultMaterialsConfig == null)
			{
				return;
			}
			fadeMat = new Material(_bodyKitValuesParent.DefaultMaterialsConfig.TransparentLit);
		}
		Fade(fade);
	}

	private void TryGetValuesParentIfNeeds()
	{
		if (_bodyKitValuesParent == null)
		{
			_bodyKitValuesParent = GetComponentInParent<IBodyKitValuesParent>(includeInactive: true);
		}
	}

	private void CarColorChangedCallback(Color color)
	{
		ResetMainMaterial();
	}

	private void Fade(bool fade)
	{
		_fadeCancellationToken?.Cancel();
		_fadeCancellationToken?.Dispose();
		_fadeCancellationToken = null;
		_fadeCancellationToken = new CancellationTokenSource();
		IsFading = true;
		TrackFade(FadeAsync(fade, _fadeCancellationToken.Token)).Forget();
	}

	private async UniTaskVoid TrackFade(UniTask fadeTask)
	{
		try
		{
			await fadeTask;
		}
		finally
		{
			IsFading = false;
		}
	}

	private async UniTask FadeAsync(bool fade, CancellationToken cancellationToken)
	{
		while (_bodyKitValuesParent == null && !cancellationToken.IsCancellationRequested && !_toDestroy)
		{
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, cancellationToken);
		}
		if (this == null || cancellationToken.IsCancellationRequested || _bodyKitValuesParent == null || !_bodyKitValuesParent.CarMaterial || _toDestroy)
		{
			return;
		}
		RevertAllRenderers();
		fadeMat.color = _bodyKitValuesParent.CarMaterial.color;
		if ((bool)render)
		{
			render.sharedMaterial = fadeMat;
		}
		CollectChildRenderers();
		float num;
		float target;
		float step;
		if (fade)
		{
			num = 1f;
			target = 0f;
			step = -0.05f;
		}
		else
		{
			num = 0f;
			target = 1f;
			step = 0.05f;
		}
		for (float f = num; !f.AlmostEquals(target, 0.06f); f += step)
		{
			SetAlphaAll(f);
			await UniTask.Delay(TimeSpan.FromSeconds(0.009999999776482582), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (this == null || _toDestroy || cancellationToken.IsCancellationRequested)
			{
				if (this != null && !fade && !_toDestroy)
				{
					RevertAllRenderers();
				}
				return;
			}
		}
		if (!(this == null))
		{
			if (fade)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			RevertAllRenderers();
			SetMaterials();
		}
	}

	private void SetAlphaAll(float alpha)
	{
		if (!fadeMat)
		{
			return;
		}
		Color color = fadeMat.color;
		color.a = alpha;
		fadeMat.color = color;
		foreach (Material tempMaterial in _tempMaterials)
		{
			if ((bool)tempMaterial)
			{
				Color color2 = tempMaterial.color;
				color2.a = alpha;
				tempMaterial.color = color2;
			}
		}
	}

	private void RevertAllRenderers()
	{
		if ((bool)render && _bodyKitValuesParent != null && (bool)_bodyKitValuesParent.CarMaterial)
		{
			render.sharedMaterial = ((NoVinylMaterial && (bool)MainMaterial) ? MainMaterial : _bodyKitValuesParent.CarMaterial);
		}
		for (int i = 0; i < _allRenderers.Count; i++)
		{
			_allRenderers[i].sharedMaterial = _startMaterials[i];
		}
		CommonExtensions.ClearObjectCollection(_tempMaterials);
		_allRenderers.Clear();
		_startMaterials.Clear();
	}

	public void CopyFromMainMat(Material carMat, Color? mainColor)
	{
		if ((bool)carMat && (bool)MainMaterial)
		{
			Color color = MainMaterial.color;
			MainMaterial.CopyPropertiesFromMaterial(carMat);
			if (!mainColor.HasValue)
			{
				MainMaterial.color = color;
			}
			else
			{
				MainMaterial.color = mainColor.Value;
			}
			MainMaterial.mainTexture = null;
		}
	}

	private void CollectChildRenderers()
	{
		CommonExtensions.ClearObjectCollection(_tempMaterials);
		_allRenderers.Clear();
		_startMaterials.Clear();
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (!(renderer == render))
			{
				_allRenderers.Add(renderer);
				Material sharedMaterial = renderer.sharedMaterial;
				_startMaterials.Add(sharedMaterial);
				if (!(sharedMaterial == null))
				{
					Material material = new Material(_bodyKitValuesParent.DefaultMaterialsConfig.TransparentLit);
					CommonExtensions.CopyColor(sharedMaterial, material);
					CopyTextureIfExists(sharedMaterial, material, BaseMap, MainTex);
					CopyTextureIfExists(sharedMaterial, material, MainTex, BaseMap);
					CopyTextureIfExists(sharedMaterial, material, BaseMap, BaseMap);
					CopyTextureIfExists(sharedMaterial, material, MainTex, MainTex);
					renderer.sharedMaterial = material;
					_tempMaterials.Add(material);
				}
			}
		}
	}

	private void CopyTextureIfExists(Material from, Material to, int fromProperty, int toProperty)
	{
		if (from.HasProperty(fromProperty) && to.HasProperty(toProperty))
		{
			to.SetTexture(toProperty, from.GetTexture(fromProperty));
		}
	}

	private async UniTaskVoid SetAdditionalElements()
	{
		if (AdditionalElements.IsNullOrEmpty())
		{
			return;
		}
		GameObject[] additionalElements = AdditionalElements;
		foreach (GameObject element in additionalElements)
		{
			if (!element || !element.TryGetComponent<Renderer>(out var component))
			{
				continue;
			}
			component.sharedMaterial = _bodyKitValuesParent.CarMaterial;
			SaveFadeMaterial(component);
			if (ScenesHelper.IsMenuSceneActive())
			{
				if ((bool)_vehicleData && (!_vehicleData.CarBodyReferences || !_vehicleData.CarBodyReferences.Body))
				{
					await UniTask.DelayFrame(10);
				}
				if (!element.TryGetComponent<MeshCollider>(out var _))
				{
					element.AddComponent<MeshCollider>().convex = true;
				}
			}
		}
	}

	private void SetElementsWithoutVinyls()
	{
		if (AdditionalElementsWithoutVinyls.IsNullOrEmpty())
		{
			return;
		}
		GameObject[] additionalElementsWithoutVinyls = AdditionalElementsWithoutVinyls;
		foreach (GameObject gameObject in additionalElementsWithoutVinyls)
		{
			if ((bool)gameObject && gameObject.TryGetComponent<Renderer>(out var component))
			{
				component.sharedMaterial = (_isSellCar ? component.sharedMaterial : _bodyKitValuesParent.CarMaterial);
				SaveFadeMaterial(component);
				if (ScenesHelper.IsMenuSceneActive())
				{
					gameObject.layer = 27;
				}
			}
		}
	}

	private void SetCarWindowElements()
	{
		if (GlassWindows.IsNullOrEmpty() || !_vehicleData || !_vehicleData.MainModel)
		{
			return;
		}
		BodyMarker component = _vehicleData.MainModel.GetComponent<BodyMarker>();
		List<GameObject> list = component.AdditionalWindowElements.ToList();
		list.RemoveAll((GameObject item) => !item);
		GameObject[] glassWindows = GlassWindows;
		foreach (GameObject gameObject in glassWindows)
		{
			if (!gameObject || !gameObject.TryGetComponent<Renderer>(out var component2))
			{
				continue;
			}
			component2.sharedMaterial = (_isSellCar ? component2.sharedMaterial : _vehicleData.WindowComposition.WindowMat);
			if (!ScenesHelper.IsMenuSceneActive())
			{
				continue;
			}
			if (_allRenderers.Contains(component2))
			{
				int num2 = _allRenderers.IndexOf(component2);
				if (num2 >= 0 && num2 < _startMaterials.Count)
				{
					_startMaterials[num2] = component2.sharedMaterial;
				}
			}
			gameObject.layer = 3;
			if (!list.Contains(gameObject))
			{
				list.Add(gameObject);
			}
		}
		component.AdditionalWindowElements = list.ToArray();
	}

	private void SaveFadeMaterial(Renderer rend)
	{
		if (_startMaterials == null || _allRenderers == null)
		{
			return;
		}
		for (int i = 0; i < _allRenderers.Count; i++)
		{
			if ((bool)_allRenderers[i] && _allRenderers[i] == rend && i < _startMaterials.Count && (bool)_startMaterials[i])
			{
				_startMaterials[i] = rend.sharedMaterial;
				break;
			}
		}
	}
}
