using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

public class CarTuningUIElement : MonoBehaviour
{
	[SerializeField]
	private Button ItemButton = default;

	[SerializeField]
	private TextMeshProUGUI CountText = default;

	[SerializeField]
	private TextMeshProUGUI NameText = default;

	[SerializeField]
	private TextMeshProUGUI TypeText = default;

	[SerializeField]
	private Image ItemImage = default;

	[SerializeField]
	private Image VendorImage = default;

	[SerializeField]
	private Image DetailRarityImage = default;

	private PartData _partData;

	private CarTuningPartsUIWindow _carTuningPartsUIWindow;

	private AllParts _allParts;

	private ManagerUI _managerUI;

	private DetailVendorsConfigs _detailVendorsConfigs;

	private CarTuningDetailRaritySpritesConfig _carTuningDetailRaritySpritesConfig;

	[Inject]
	public void Construct(AllParts allParts, ManagerUI managerUI, DetailVendorsConfigs detailVendorsConfigs, CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig)
	{
		_allParts = allParts;
		_managerUI = managerUI;
		_detailVendorsConfigs = detailVendorsConfigs;
		_carTuningDetailRaritySpritesConfig = carTuningDetailRaritySpritesConfig;
	}

	private void Start()
	{
		ItemButton.onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		_carTuningPartsUIWindow.OnItemClick(_partData);
	}

	public void Init(PartData partData, CarTuningPartsUIWindow carTuningPartsUIWindow)
	{
		_carTuningPartsUIWindow = carTuningPartsUIWindow;
		_partData = partData;
		TypeText.text = partData.PartType.ToString();
		NameText.text = partData.GetName();
		CountText.text = _allParts.GetCompatiblePartsCount(carTuningPartsUIWindow.EngineModel, partData, carTuningPartsUIWindow.CarId).ToString();
		PartItem part = partData.GetPart(_allParts);
		ItemImage.sprite = _managerUI.GetEngineIcon((int)part.TypeId);
		VendorImage.sprite = _detailVendorsConfigs.Vendors[part.VendorId].CarTuningWindowSprite;
		DetailRarityImage.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[part.Rarity].SquareRaritySpriteForInventory;
	}
}
