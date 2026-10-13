using System.Collections.Generic;
using CPM.Database;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag
{
	public class WheelsUIWindow : WindowScript, IClickable, IBackableWindow
	{
		[Header("Button prefab")]
		public InventoryUIElement ButtonPrefab;

		private CarTuningData _car;

		[SerializeField]
		private UIInventory _uiInventory = default;

		public TextMeshProUGUI allWeightText;

		[SerializeField]
		private Button backButton = default;

		public List<ButtonsSlots> ButtonsSlotsFrontList = new List<ButtonsSlots>();

		public List<ButtonsSlots> ButtonsSlotsRearList = new List<ButtonsSlots>();

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
			if (_carInfoService.GetFromSlot(V.SlotID) != null)
			{
				Init(_carInfoService.GetFromSlot(V.SlotID).TuningData);
			}
			backButton.onClick.AddListener(Back);
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

		public void Init(CarTuningData carTuningData)
		{
			_car = carTuningData;
			allWeightText.text = $"{9999:N0} KG";
			_ = ParseDetails(_car);
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_managerUI.ShowCarTuningWindow();
			Close();
		}

		private async UniTaskVoid ParseDetails(CarTuningData carTuningData)
		{
			List<PartData> partDatas = new List<PartData>();
			foreach (UIElement item in _uiInventory.CreateButtons(partDatas, ButtonsSlotsFrontList, ButtonPrefab, this))
			{
				if (item is InventoryUIElement inventoryUIElement)
				{
					inventoryUIElement.IndexForMarket = 0;
				}
			}
			List<PartData> partDatas2 = new List<PartData>();
			foreach (UIElement item2 in _uiInventory.CreateButtons(partDatas2, ButtonsSlotsRearList, ButtonPrefab, this))
			{
				if (item2 is InventoryUIElement inventoryUIElement2)
				{
					inventoryUIElement2.IndexForMarket = 1;
				}
			}
		}
	}
}
