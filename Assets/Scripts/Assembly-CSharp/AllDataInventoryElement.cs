using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

public class AllDataInventoryElement : MonoBehaviour, ICell
{
	[SerializeField]
	private TextMeshProUGUI TitleText = default;

	[SerializeField]
	private TextMeshProUGUI PartInstalledText = default;

	[SerializeField]
	private Image PartImage = default;

	[SerializeField]
	private Image PartRarityImage = default;

	[SerializeField]
	private Image PartStatus = default;

	[SerializeField]
	private Image PartInstalledPlate = default;

	[SerializeField]
	private Image VendorImage = default;

	[SerializeField]
	private Image BorderImage = default;

	[SerializeField]
	private GameObject Checkbox = default;

	public Toggle Toggle;

	private ManagerUI _managerUI;

	private AllPartsInventoryUIWindow _allPartsInventoryUIWindow;

	private CarTuningDetailRaritySpritesConfig _carTuningDetailRaritySpritesConfig;

	private DetailVendorsConfigs _detailVendorsConfigs;

	private PartData _partData;

	private PartItem _partItem;

	private Vendor _vendor;

	private bool _isInstalled;

	[Inject]
	public void Construct(ManagerUI managerUI, CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig, DetailVendorsConfigs detailVendorsConfigs)
	{
		_managerUI = managerUI;
		_carTuningDetailRaritySpritesConfig = carTuningDetailRaritySpritesConfig;
		_detailVendorsConfigs = detailVendorsConfigs;
	}

	private void Start()
	{
		Toggle.onValueChanged.AddListener(OnClick);
	}

	private void OnClick(bool state)
	{
		if (!_isInstalled)
		{
			Checkbox.SetActive(state);
			BorderImage.SetActive(state);
		}
		if (!state && !_isInstalled)
		{
			_allPartsInventoryUIWindow.RemoveSelectedItem(_partData, _partItem, _vendor, this, _isInstalled);
		}
		else
		{
			_allPartsInventoryUIWindow.SelectItem(_partData, _partItem, _vendor, this, _isInstalled);
		}
	}

	public void Init(PartData partData, PartItem part, bool isCompatible, bool isInstalled, AllPartsInventoryUIWindow allPartsInventoryUIWindow)
	{
		_isInstalled = isInstalled;
		PartInstalledText.text = (isInstalled ? "Installed" : "");
		if (isInstalled)
		{
			PartInstalledText.color = Color.black;
			PartInstalledPlate.color = allPartsInventoryUIWindow.InstalledColor;
		}
		PartInstalledPlate.gameObject.SetActive(isInstalled);
		_partData = partData;
		_partItem = part;
		TitleText.text = part.GetName;
		TitleText.color = allPartsInventoryUIWindow.RarityColots[part.Rarity];
		BorderImage.color = allPartsInventoryUIWindow.RarityColots[part.Rarity];
		_allPartsInventoryUIWindow = allPartsInventoryUIWindow;
		ParseImages(part);
	}

	private void ParseImages(PartItem part)
	{
		PartImage.sprite = _managerUI.GetEngineIcon((int)part.TypeId);
		_vendor = _detailVendorsConfigs.GetVendorById(part.VendorId);
		VendorImage.sprite = _vendor.CarTuningWindowSprite;
		PartRarityImage.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[part.Rarity].AllInventoryRaritySprite;
		float durability = _partData.GetDurability();
		if (durability < 0.5f)
		{
			PartStatus.gameObject.SetActive(value: true);
			PartStatus.color = ((durability < 0.2f) ? _allPartsInventoryUIWindow.ErrorColor : _allPartsInventoryUIWindow.WarningColor);
		}
		else
		{
			PartStatus.gameObject.SetActive(value: false);
		}
	}
}
