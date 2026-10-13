using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

public class InventroyPopupElement : MonoBehaviour
{
	[SerializeField]
	private Image PartImage;

	[SerializeField]
	private Image PartRarityColor;

	[SerializeField]
	private TextMeshProUGUI PartName;

	private AllParts _allParts;

	private ManagerUI _managerUI;

	private CarTuningDetailRaritySpritesConfig _carTuningDetailRaritySpritesConfig;

	[Inject]
	public void Construct(AllParts allParts, ManagerUI managerUI, CarTuningDetailRaritySpritesConfig carTuningDetailRaritySpritesConfig)
	{
		_allParts = allParts;
		_managerUI = managerUI;
		_carTuningDetailRaritySpritesConfig = carTuningDetailRaritySpritesConfig;
	}

	public void Init(PartData partData)
	{
		PartItem part = _allParts.GetPart(partData);
		PartImage.sprite = _managerUI.GetEngineIcon((int)part.TypeId);
		PartRarityColor.sprite = _carTuningDetailRaritySpritesConfig.CarTuningDetailRarityDatas[part.Rarity].SquareRaritySpriteForInventory;
		PartName.text = part.GetName;
	}
}
