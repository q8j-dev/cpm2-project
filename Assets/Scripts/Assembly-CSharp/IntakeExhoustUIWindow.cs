using System.Collections.Generic;
using CPM.Database;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

public class IntakeExhoustUIWindow : WindowScript, IClickable, IBackableWindow
{
	public InventoryUIElement ButtonPrefab;

	private EngineKitData _engineKitData;

	[SerializeField]
	private UIInventory _uiInventory = default;

	public TextMeshProUGUI allWeightText;

	[SerializeField]
	private Button backButton = default;

	public List<ButtonsSlots> ButtonsSlotsList = new List<ButtonsSlots>();

	private List<InventoryUIElement> _spawnedButtons = new List<InventoryUIElement>();

	private ICarInfoService _carInfoService;

	[Inject]
	public void Construct(ManagerUI managerUI, ICarInfoService carInfoService)
	{
		_carInfoService = carInfoService;
		_managerUI = managerUI;
	}

	protected override void Start()
	{
		base.Start();
		InitBackButton();
		_engineKitData = _managerUI.EditingEngineKitData;
		if (_carInfoService.GetFromSlot(V.SlotID) != null)
		{
			Init(_engineKitData);
		}
		backButton.onClick.AddListener(Back);
	}

	public override void ReloadWindow()
	{
		_uiInventory.ClearItemsList();
		if (_carInfoService.GetFromSlot(V.SlotID) != null)
		{
			Init(_engineKitData);
		}
	}

	public void OnItemClick(PartData parData, int index, UIElement uiElement = null)
	{
		_managerUI.ShowMarketInventoryWindow(parData, index);
		Close();
	}

	public void CloseClickable()
	{
		Close();
	}

	public void Init(EngineKitData engineKitData)
	{
		allWeightText.text = $"{9999:N0} KG";
		_ = ParseDetails(_engineKitData);
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		_managerUI.ShowEngineUIWindow();
		Close();
	}

	private async UniTaskVoid ParseDetails(EngineKitData EngineKitData)
	{
		List<PartData> partDatas = new List<PartData> { EngineKitData.IntakeManifold, EngineKitData.ExhaustHeader, EngineKitData.ExhaustPipe, EngineKitData.AirFilter };
		_uiInventory.CreateButtons(partDatas, ButtonsSlotsList, ButtonPrefab, this);
	}
}
