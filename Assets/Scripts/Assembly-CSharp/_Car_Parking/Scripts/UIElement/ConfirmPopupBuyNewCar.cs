using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.UIElement
{
	public class ConfirmPopupBuyNewCar : ConfirmPopup, IBackableWindow
	{
		[SerializeField]
		private CombinedPriceView CombinedPriceView;

		[SerializeField]
		private RawImageLoadAsync CarIcon;

		[SerializeField]
		private TextMeshProUGUI Description;

		private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		private ManagerUI _managerUI;

		private bool _mustBuySlot;

		private CombinedPrice _combinedPrice;

		[Inject]
		public void Construct(VirtualPurchaseAnalytic virtualPurchaseAnalytic, ManagerUI managerUI)
		{
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
			_managerUI = managerUI;
		}

		public void Init(CombinedPrice combinedPrice, AdvancedInt cardID, bool mustBuySlot)
		{
			_mustBuySlot = mustBuySlot;
			_combinedPrice = combinedPrice;
			CombinedPriceView.Init(combinedPrice, mustBuySlot);
			cardID.Get(out int value);
			CarIcon.Load(PathGetterHelper.GetCarPreviewPath(value));
			Description.text = new LocalizedString("Newcarswindow.areyousureyouwanttobuythiscar?");
			if (mustBuySlot)
			{
				string text = new LocalizedString("DealerWindow.YouAreOutOfSlotsDoYouWantToBuy");
				Description.text = text;
			}
			RebuildLayout();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		protected override void OnEnable()
		{
			_managerUI.SuspendUiNavigationCurrentWindow();
			InitBackButton();
			base.OnEnable();
			base.Canceled += TurnOnBackButton;
			base.Confirmed += TurnOnBackButton;
			base.Confirmed += OnConfirm;
			base.Canceled += OnCancel;
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.SellCarValues && (bool)V.CoreGameManager.SellCarValues.BuyCarBodyKits && (bool)V.CoreGameManager.SellCarValues.BuyCarBodyKits.ButtonPhysicsClickerController)
			{
				V.CoreGameManager.SellCarValues.BuyCarBodyKits.ButtonPhysicsClickerController.enabled = false;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			base.Canceled -= TurnOnBackButton;
			base.Confirmed -= TurnOnBackButton;
			base.Confirmed -= OnConfirm;
			base.Canceled -= OnCancel;
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.SellCarValues && (bool)V.CoreGameManager.SellCarValues.BuyCarBodyKits && (bool)V.CoreGameManager.SellCarValues.BuyCarBodyKits.ButtonPhysicsClickerController)
			{
				V.CoreGameManager.SellCarValues.BuyCarBodyKits.ButtonPhysicsClickerController.enabled = true;
			}
			_managerUI.RestoreUiNavigationCurrentWindow();
		}

		private void OnConfirm(ConfirmPopup popup)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_NewCars(_mustBuySlot, _combinedPrice, PurchaseStatus.PURCHASE_CONFIRMED);
		}

		private void OnCancel(ConfirmPopup popup)
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_NewCars(_mustBuySlot, _combinedPrice, PurchaseStatus.PURCHASE_CANCELED_BY_USER);
		}

		private void TurnOnBackButton(ConfirmPopup popup)
		{
			if (V.CoreGameManager.HasFirstCar())
			{
				V.CoreGameManager.ManagerUI.OnBackButtonActivated?.Invoke(obj: true);
			}
		}
	}
}
