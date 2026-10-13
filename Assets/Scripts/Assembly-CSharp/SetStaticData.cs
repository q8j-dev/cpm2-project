using System.Collections.Generic;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Economics;

public class SetStaticData : MonoBehaviour
{
	public GameObject spark;

	public GameObject sandSmoke;

	public GameObject skidSmoke;

	public GameObject damagedExhaustParticle;

	public GameObject shootExhaustParticle;

	public GameObject shootWeakExhaustParticle;

	public GameObject burbleExhaustParticle;

	public Material mainWindowMat;

	public AnimationCurve temperatureCooling;

	public AnimationCurve temperatureHeating;

	public AnimationCurve temperatureGrip;

	public AnimationCurve healtGrip;

	public Font[] fonts;

	[Header("Vinyls")]
	[SerializeField]
	private VinylsTuningConfig AllVinylsConfig;

	private IPricesService _pricesService;

	private VinylsTuningConfig _allVinylsConfigCopy;

	private bool _vinylPricesApplied;

	[Inject]
	public void Construct(IPricesService pricesService)
	{
		_pricesService = pricesService;
		TryApplyVinylPrices();
	}

	private void Awake()
	{
		InitVinyls();
		V.SparkPrefab = spark;
		V.SandSmoke = sandSmoke.gameObject;
		if ((bool)skidSmoke)
		{
			V.SkidSmoke = skidSmoke;
		}
		V.DamagedExhaustParticle = damagedExhaustParticle;
		V.ShootStrongExhaustParticle = shootExhaustParticle;
		V.ShootWeakExhaustParticle = shootWeakExhaustParticle;
		V.BurbleExhaustParticle = burbleExhaustParticle;
		V.TemperatureCooling = temperatureCooling;
		V.TemperatureGrip = temperatureGrip;
		V.TemperatureHeating = temperatureHeating;
		V.HealtGrip = healtGrip;
		V.Fonts = fonts;
		if ((bool)mainWindowMat)
		{
			mainWindowMat.mainTexture = null;
		}
		if (V.FontMaterials == null)
		{
			V.FontMaterials = new List<Material>();
			Shader shader = Shader.Find("GUI/CPM Text Shader");
			Font[] array = fonts;
			for (int i = 0; i < array.Length; i++)
			{
				Material material = new Material(array[i].material);
				material.shader = shader;
				V.FontMaterials.Add(material);
			}
		}
	}

	private void InitVinyls()
	{
		_allVinylsConfigCopy = Object.Instantiate(AllVinylsConfig);
		AllVinylsConfig = null;
		V.AllVinyls = _allVinylsConfigCopy.AllVinyls;
		TryApplyVinylPrices();
	}

	private void TryApplyVinylPrices()
	{
		if (_vinylPricesApplied || _pricesService == null || _allVinylsConfigCopy == null)
		{
			return;
		}
		int num = 0;
		OneVinylData[] allVinyls = _allVinylsConfigCopy.AllVinyls;
		foreach (OneVinylData oneVinylData in allVinyls)
		{
			float value = (float)((int)oneVinylData.price * num) * 0.1f + (float)(int)_pricesService.VinylsStep;
			if (num == 1)
			{
				value = (int)_pricesService.VinylsText;
			}
			value = Mathf.Clamp(value, (int)_pricesService.VinylsMin, (int)_pricesService.VinylsMax);
			int num2 = Mathf.RoundToInt(value);
			if (Mathf.Abs(num2 - (int)value) == 0)
			{
				oneVinylData.price = num2;
			}
			num++;
		}
		_vinylPricesApplied = true;
	}

	private void OnDestroy()
	{
		Object.Destroy(_allVinylsConfigCopy);
	}
}
