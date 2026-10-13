using CPM.Scripts.Garage.GarageBuyer;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Garage.GarageSellService;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement
{
	public class GaragePlaceUIItem : PlaceUIItem<GaragePositionsConfig, GarageData>
	{
		[SerializeField]
		private TextMeshProUGUI Name;

		[SerializeField]
		private TextMeshProUGUI Capacity;

		[SerializeField]
		private TextMeshProUGUI Location;

		[SerializeField]
		private RawImageLoadAsync Icon;

		[SerializeField]
		private Button ManageGarage;

		[SerializeField]
		private GameObject BlockObject;

		[SerializeField]
		private Text PriceText;

		[SerializeField]
		private RealCurrencyView GaragePriceView;

		[SerializeField]
		private TextMeshProUGUI FreeSpotsText;

		[SerializeField]
		private GameObject FreeSpotsPlace;

		[SerializeField]
		private Image SelectedItemPlace;

		[SerializeField]
		private Button Button;

		[SerializeField]
		private OccupiedCarSlotsUI OccupiedCarSlotsUI;

		private IGarageBuyService _garageBuyService;

		private IPurchaseSystem _purchaseSystem;

		private IGarageSellService _garageSellService;

		private bool _isChosen;

		private bool _isSell;

		public bool IsBought
		{
			get
			{
				_garageBuyService.GetGarageStatus(base.index).Get(out var value);
				return value;
			}
		}

		[Inject]
		public void Construct(IGarageBuyService garageBuyService, IPurchaseSystem purchaseSystem, IGarageSellService garageSellService)
		{
			_garageBuyService = garageBuyService;
			_purchaseSystem = purchaseSystem;
			_garageSellService = garageSellService;
		}

		public void SetNavigationDown(Selectable go)
		{
			Navigation navigation = Button.navigation;
			navigation.selectOnDown = go;
			Button.navigation = navigation;
		}

		public override void Init()
		{
			base.Init();
			Name.text = new LocalizedString(base.DataItem.NameLocalizedKey);
			Capacity.text = base.DataItem.MaxCarSpotCount.ToString();
			Location.text = new LocalizedString(base.DataItem.LocationKey);
			Icon.LoadFromAddressables(base.DataItem.GarageIconAddressablePath);
			SelectedItemPlace.SetActive(active: false);
			FreeSpotsPlace.SetActive(value: false);
			_garageBuyService.GetGarageStatus(base.index).Get(out var value);
			if (base.index > 0 && !value)
			{
				FreeSpotsText.text = base.DataItem.DefaultCarSpotCount.ToString();
				FreeSpotsPlace.SetActive(value: true);
				BlockObject.SetActive(value: true);
				UpdatePrice();
			}
			OccupiedCarSlotsUI.Init(base.index);
			ManageGarage.SetActive(base.DataItem.ForSale && IsBought);
		}

		protected void OnEnable()
		{
			Button.onClick.AddListener(OnClick);
			ManageGarage?.onClick.AddListener(OnManageGarage);
			_purchaseSystem.OnInitializedCompletedEvent += PurchaseSystemInitializedCallback;
		}

		protected void OnDisable()
		{
			Button.onClick.RemoveListener(OnClick);
			ManageGarage?.onClick.RemoveListener(OnManageGarage);
			_purchaseSystem.OnInitializedCompletedEvent -= PurchaseSystemInitializedCallback;
		}

		protected override GarageData GetData(int index)
		{
			return config.Get(index);
		}

		protected override void OnChosenChanged()
		{
			SetSelectedItemPlaceActive(base.IsChosen);
		}

		private void OnManageGarage()
		{
			V.CoreGameManager.ManagerUI.ShowConfirmGarageSellWindow(base.DataItem.Index);
		}

		private void PurchaseSystemInitializedCallback()
		{
			UpdatePrice();
		}

		private void OnClick()
		{
			if (!base.IsChosen)
			{
				base.IsChosen = true;
			}
		}

		private void SetSelectedItemPlaceActive(bool active)
		{
			SelectedItemPlace.DOKill();
			if (active)
			{
				SelectedItemPlace.gameObject.SetActive(value: true);
				SelectedItemPlace.DOFade(1f, 0.3f);
			}
			else
			{
				SelectedItemPlace.DOFade(0f, 0.3f).OnComplete(() =>
				{
					SelectedItemPlace.gameObject.SetActive(value: false);
				});
			}
		}

		private void UpdatePrice()
		{
			PurchaseItemType purchaseItemType = PurchaseItemType.Garage1;
			string text = base.DataItem.GetPrice.ToString();
			PurchaseProduct purchaseProduct = _purchaseSystem.GetPurchaseProduct(purchaseItemType);
			if (FlagsStorage.IsPurchaserInitialized && purchaseProduct != null)
			{
				text = purchaseProduct.LocalizedPriceText;
			}
			PriceText.text = text;
			GaragePriceView.UpdateText(text);
		}
	}
}
