using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Offers.Reward;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class BigOfferBundleWindow : BigOfferWindowBase
	{
		[SerializeField]
		private Button ButtonViewSuitsMale;

		[SerializeField]
		private Button ButtonViewSuitsFemale;

		[SerializeField]
		private Button ButtonViewHeadwear;

		[SerializeField]
		private Image ButtonViewSuitsMaleIcon;

		[SerializeField]
		private Image ButtonViewSuitsFemaleIcon;

		[SerializeField]
		private Image ButtonViewHeadwearIcon;

		[SerializeField]
		private Image ButtonViewInShopIcon;

		[SerializeField]
		private TextMeshProUGUI MaleDescription;

		[SerializeField]
		private TextMeshProUGUI FemaleDescription;

		[SerializeField]
		private TextMeshProUGUI CapDescription;

		[SerializeField]
		private TextMeshProUGUI CarDescription;

		[SerializeField]
		private TextMeshProUGUI PackTitle;

		[SerializeField]
		private TextMeshProUGUI PackDescription;

		[SerializeField]
		private Image ShadowImage;

		private BundleOfferCatalog _bundleCatalog;

		private int _malePackId = -1;

		private int _femalePackId = -1;

		private int _hatPackId = -1;

		[Inject]
		public void ConstructBundle(BundleOfferCatalog bundleCatalog)
		{
			_bundleCatalog = bundleCatalog;
		}

		protected override void ButtonBuyClickCallback()
		{
			base.ButtonBuyClickCallback();
			ButtonCloseClickCallback();
			_offerSystem.SetCurrentOfferID(OfferBigBannerView.CurrentOfferModel.ID);
			_purchaseSystem.BuyProductID(OfferBigBannerView.CurrentOfferModel.Payload.FirstBundleID);
		}

		protected override void ButtonViewInShopClickCallback()
		{
			base.ButtonViewInShopClickCallback();
			PrepareToClose();
			_managerUI.CloseWindow("EventViewWindow");
			if (!(_garage == null))
			{
				_garage.LastSelectedCar = OfferBigBannerView.CurrentOfferModel.Payload.FirstRewardCarId;
			}
			_managerUI.ShowNewCarsWindow(JsonConvert.SerializeObject(new DealerWindowAdditionalData
			{
				UseFilter = true,
				ShowOnlySpecial = true,
				TargetCarId = OfferBigBannerView.CurrentOfferModel.Payload.FirstRewardCarId
			}));
			_coreGameManager.OpenSellCars();
			_managerUI.ShowUserBalanceWindow();
		}

		protected override void AddCallbacks()
		{
			base.AddCallbacks();
			ButtonViewSuitsMale.onClick.AddListener(OpenSuitsMale);
			ButtonViewSuitsFemale.onClick.AddListener(OpenSuitsFemale);
			if ((bool)ButtonViewHeadwear)
			{
				ButtonViewHeadwear.onClick.AddListener(OpenHeadwear);
			}
		}

		protected override void RemoveCallbacks()
		{
			base.RemoveCallbacks();
			ButtonViewSuitsMale.onClick.RemoveListener(OpenSuitsMale);
			ButtonViewSuitsFemale.onClick.RemoveListener(OpenSuitsFemale);
			if ((bool)ButtonViewHeadwear)
			{
				ButtonViewHeadwear.onClick.RemoveListener(OpenHeadwear);
			}
		}

		private void OpenSuitsMale()
		{
			OpenPacks(1, _malePackId);
		}

		private void OpenSuitsFemale()
		{
			OpenPacks(0, _femalePackId);
		}

		private void OpenHeadwear()
		{
			OpenPacks(-1, _hatPackId);
		}

		private void OpenPacks(int gender, int packId)
		{
			PrepareToClose();
			_managerUI.CloseWindow("EventViewWindow");
			_coreGameManager.menuControl.ShowSelectDriver(JsonConvert.SerializeObject(new PersonWindowAdditionalData
			{
				ParentWindow = PersonWindowAdditionalData.ParentWindowType.MainMenu,
				Gender = gender,
				OpenCategory = PersonWindowCategoryType.Packs,
				OpenPackId = ((packId >= 0) ? new int?(packId) : ((int?)null))
			}));
			_managerUI.ShowUserBalanceWindow();
		}

		protected override void OnOfferLoaded(OfferModel offerModel)
		{
			if (offerModel?.Payload != null && _bundleCatalog != null)
			{
				ApplyPackButton(ButtonViewInShop, ButtonViewInShopIcon, CarDescription, offerModel, PackIconKind.Car, out var _);
				ApplyPackButton(ButtonViewSuitsMale, ButtonViewSuitsMaleIcon, MaleDescription, offerModel, PackIconKind.Male, out _malePackId);
				ApplyPackButton(ButtonViewSuitsFemale, ButtonViewSuitsFemaleIcon, FemaleDescription, offerModel, PackIconKind.Female, out _femalePackId);
				ApplyPackButton(ButtonViewHeadwear, ButtonViewHeadwearIcon, CapDescription, offerModel, PackIconKind.Cap, out _hatPackId);
				ApplyPackHeader(offerModel);
			}
		}

		private void ApplyPackHeader(OfferModel offer)
		{
			bool flag = _bundleCatalog.TryGetFirstPackId(offer, out var packId);
			ApplyTextField(PackTitle, (flag && _bundleCatalog.TryGetPackTitle(packId, out var title)) ? title : null);
			ApplyTextField(PackDescription, (flag && _bundleCatalog.TryGetPackDescription(packId, out var description)) ? description : null);
			if ((bool)ShadowImage)
			{
				ShadowImage.gameObject.SetActive(flag && _bundleCatalog.IsShadowEnabled(packId));
			}
		}

		private static void ApplyTextField(TextMeshProUGUI target, string text)
		{
			if ((bool)target)
			{
				if (string.IsNullOrEmpty(text))
				{
					target.gameObject.SetActive(value: false);
					return;
				}
				target.gameObject.SetActive(value: true);
				target.text = new LocalizedString(text);
			}
		}

		private void ApplyPackButton(Button button, Image icon, TextMeshProUGUI description, OfferModel offer, PackIconKind kind, out int packId)
		{
			bool flag = _bundleCatalog.TryGetFirstPackForIcon(offer, kind, out packId, out var icon2);
			if ((bool)button)
			{
				button.gameObject.SetActive(flag);
			}
			if (flag && (bool)icon)
			{
				icon.sprite = icon2;
			}
			if ((bool)description)
			{
				if (flag && _bundleCatalog.TryGetPackDescription(packId, kind, out var description2))
				{
					description.gameObject.SetActive(value: true);
					description.text = new LocalizedString(description2);
				}
				else
				{
					description.gameObject.SetActive(value: false);
				}
			}
		}
	}
}
