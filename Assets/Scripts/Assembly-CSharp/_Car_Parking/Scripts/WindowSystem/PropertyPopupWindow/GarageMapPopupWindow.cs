using System.Threading;
using CPM.Scripts.Garage.GarageBuyer;
using Cysharp.Threading.Tasks;
using MVPFree.MapIcons.Enum;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.WindowSystem.PropertyPopupWindow
{
	public class GarageMapPopupWindow : PropertyPopupWindow
	{
		[SerializeField]
		private TextMeshProUGUI CapacityTexts = default;

		[SerializeField]
		private TextMeshProUGUI[] MaxCapacityText = default;

		[SerializeField]
		private Transform ForSaleContainer = default;

		[SerializeField]
		private Transform BoughtContainer = default;

		private IGarageBuyService _garageBuyService;

		private GaragePositionsConfig _garageConfig;

		private ISlotService _slotService;

		private IPurchaseSystem _purchaseSystem;

		[Inject]
		public void Construct(IGarageBuyService garageBuyService, GaragePositionsConfig garageConfig, ISlotService slotService, IPurchaseSystem purchaseSystem)
		{
			_garageBuyService = garageBuyService;
			_garageConfig = garageConfig;
			_slotService = slotService;
			_purchaseSystem = purchaseSystem;
		}

		public void Initialize(int index, IconType iconType)
		{
			CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
			GarageData garageData = _garageConfig.Get(index);
			TextMeshProUGUI[] maxCapacityText = MaxCapacityText;
			for (int i = 0; i < maxCapacityText.Length; i++)
			{
				maxCapacityText[i].text = garageData.MaxCarSpotCount.ToString();
			}
			CapacityTexts.text = _slotService.GetSlotCollection(index).OccupiedSlotCount.ToString();
			bool flag = IsBuy(index);
			UpdateOwnershipState(flag);
			PurchaseItemType purchaseItemType = PurchaseItemType.Garage1;
			if (!cancellationTokenOnDestroy.IsCancellationRequested)
			{
				PurchaseProduct purchaseProduct = _purchaseSystem.GetPurchaseProduct(purchaseItemType);
				InitRealCost(new LocalizedString(garageData.NameLocalizedKey), new LocalizedString(garageData.LocationKey), iconType, purchaseProduct?.LocalizedPriceText ?? "");
			}
		}

		public bool IsBuy(int index)
		{
			_garageBuyService.GetGarageStatus(index).Get(out var value);
			return value;
		}

		private void UpdateOwnershipState(bool isBought)
		{
			BoughtContainer.gameObject.SetActive(isBought);
			ForSaleContainer.gameObject.SetActive(!isBought);
		}
	}
}
