using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Garage.GarageSellService;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.QuicklyGarageSelector
{
	[RequireComponent(typeof(Button))]
	public class QuicklyGarageItem : PlaceUIItem<GaragePositionsConfig, GarageData>
	{
		[SerializeField]
		private RawImageLoadAsync MainImage = default;

		[SerializeField]
		private TextMeshProUGUI GarageNameText = default;

		[SerializeField]
		private RealCurrencyView GaragePriceView = default;

		[SerializeField]
		private Image SelectFrame = default;

		[SerializeField]
		private GameObject BlockPanel = default;

		[SerializeField]
		private GameObject NamePanel = default;

		[SerializeField]
		private GameObject SelectedIcon = default;

		private Button _button;

		private IGarageBuyService _garageBuyService;

		private IGarageSelectorService _garageSelectorService;

		private IPurchaseSystem _purchaseSystem;

		private IGarageSellService _garageSellService;

		[Inject]
		public void Construct(IGarageBuyService garageBuyService, IGarageSellService garageSellService, IGarageSelectorService garageSelectorService, IPurchaseSystem purchaseSystem)
		{
			_garageBuyService = garageBuyService;
			_garageSelectorService = garageSelectorService;
			_purchaseSystem = purchaseSystem;
			_garageSellService = garageSellService;
		}

		private void Start()
		{
			_purchaseSystem.OnInitializedCompletedEvent += PurchaseSystemInitializedCallback;
		}

		private void OnDestroy()
		{
			_purchaseSystem.OnInitializedCompletedEvent -= PurchaseSystemInitializedCallback;
		}

		private void OnEnable()
		{
			Init();
		}

		private void OnDisable()
		{
			if ((bool)_button)
			{
				_button.onClick.RemoveListener(OnClick);
			}
		}

		public override void Init()
		{
			base.Init();
			_button = GetComponent<Button>();
			_button.onClick.AddListener(OnClick);
			bool flag = _garageSelectorService.CurrentGarageIndex == base.index;
			MainImage.LoadFromAddressables(base.DataItem.GarageIconAddressablePath);
			GarageNameText.text = new LocalizedString(base.DataItem.NameLocalizedKey);
			UpdatePriceText();
			Color color = SelectFrame.color;
			color.a = (flag ? 1f : 0f);
			SelectFrame.color = color;
			SelectedIcon.SetActive(flag);
			_garageBuyService.GetGarageStatus(base.index).Get(out var value);
			BlockPanel.SetActive(!value);
			NamePanel.SetActive(value);
		}

		private void OnClick()
		{
			_garageBuyService.GetGarageStatus(base.index).Get(out var value);
			if (value)
			{
				if (_garageSelectorService.CurrentGarageIndex != base.index)
				{
					_garageSelectorService.ChangeMenuGarage(base.DataItem.Index, withScreen: true);
					V.CoreGameManager.ManagerUI.CloseWindow("MainMenuWindow");
				}
			}
			else
			{
				V.CoreGameManager.ManagerUI.ShowConfirmGarageBuyWindow(new ConfirmGarageBuyWindow.AdditionalData
				{
					GarageIndex = base.DataItem.Index,
					PreviousWindow = "MainMenuWindow"
				});
			}
		}

		private void UpdatePriceText()
		{
			PurchaseItemType purchaseItemType = PurchaseItemType.Garage1;
			_garageBuyService.GetGarageStatus(base.index).Get(out var value);
			if (!value)
			{
				PurchaseProduct purchaseProduct = _purchaseSystem.GetPurchaseProduct(purchaseItemType);
				GaragePriceView.UpdateText(purchaseProduct?.LocalizedPriceText ?? "");
			}
		}

		protected override GarageData GetData(int ind)
		{
			return config.Get(ind);
		}

		private void PurchaseSystemInitializedCallback()
		{
			UpdatePriceText();
		}
	}
}
