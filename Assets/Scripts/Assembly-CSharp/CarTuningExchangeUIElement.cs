using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

public class CarTuningExchangeUIElement : MonoBehaviour
{
	private const float NormalColorPercent = 0.5f;

	private const float ErrorColorPercent = 0.2f;

	private const string BadStateLocalizedKey = "Buythiscarwindow.Bad";

	private const string NormalStateLocalizedKey = "Buythiscarwindow.Normal";

	private const string GoodStateLocalizedKey = "Buythiscarwindow.Good";

	private const string PerfectStateLocalizedKey = "Buythiscarwindow.Perfect";

	[SerializeField]
	private TextMeshProUGUI PartName;

	[SerializeField]
	private TextMeshProUGUI PartType;

	[SerializeField]
	private Image PartRarityImage;

	[SerializeField]
	private Image PartVendorImage;

	[SerializeField]
	private Color WarningColor;

	[SerializeField]
	private Color ErrorColor;

	[SerializeField]
	private Color NormalColor;

	[SerializeField]
	private Color ExcelentColor;

	[SerializeField]
	private CarTuningUISlider CarTuningUISlider;

	private AllParts _allParts;

	private DetailVendorsConfigs _detailVendorsConfigs;

	private CarTuningDetailRaritySpritesConfig _carTuningDetailRaritySpritesConfig;

	[Inject]
	public void Construct(AllParts allParts, DetailVendorsConfigs detailVendorsConfigs, CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig)
	{
		_allParts = allParts;
		_detailVendorsConfigs = detailVendorsConfigs;
		_carTuningDetailRaritySpritesConfig = carTuningDetailRaritySpritesConfig;
	}

	private void OnValidate()
	{
		CarTuningUISlider = GetComponentInChildren<CarTuningUISlider>();
	}

	public void Init(PartData partData)
	{
		base.gameObject.transform.parent.gameObject.SetActive(value: true);
		PartName.text = partData.GetPartTypeName();
		PartItem part = partData.GetPart(_allParts);
		PartType.text = partData.GetName();
		PartRarityImage.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[part.Rarity].SquareRaritySprite;
		PartVendorImage.sprite = _detailVendorsConfigs.Vendors[part.VendorId].CarTuningWindowSprite;
		SetSliderColor(partData.GetDurability());
	}

	public void Init(int state)
	{
		Color excelentColor = ExcelentColor;
		float num = 0f;
		string text;
		switch (state)
		{
		case 0:
			num = 0.1f;
			excelentColor = ErrorColor;
			text = new LocalizedString("Buythiscarwindow.Bad");
			break;
		case 1:
			num = 0.5f;
			excelentColor = WarningColor;
			text = new LocalizedString("Buythiscarwindow.Normal");
			break;
		case 2:
			num = 0.75f;
			excelentColor = NormalColor;
			text = new LocalizedString("Buythiscarwindow.Good");
			break;
		default:
			num = 1f;
			excelentColor = ExcelentColor;
			text = new LocalizedString("Buythiscarwindow.Perfect");
			break;
		}
		CarTuningUISlider.Init(excelentColor, text, num);
	}

	private void SetSliderColor(float percent)
	{
		if (percent < 0.5f)
		{
			bool flag = percent > 0.2f;
			CarTuningUISlider.Init(flag ? WarningColor : ErrorColor, percent);
		}
		else
		{
			CarTuningUISlider.Init(NormalColor, percent);
		}
	}
}
