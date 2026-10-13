using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Animations.Emotes.Locker;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Offers.Reward;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public abstract class BigOfferEmoteWindowBase : BigOfferWindowBase
	{
		[SerializeField]
		private TextMeshProUGUI TitleText = default;

		[SerializeField]
		private TextMeshProUGUI DescriptionText = default;

		[SerializeField]
		private TextMeshProUGUI ShopButtonCaptionText = default;

		[SerializeField]
		private Image PreviewImage = default;

		private AnimationShopOpenRequest _animationShopOpenRequest;

		private EmoteLockerOpenRequest _emoteLockerOpenRequest;

		private IEmoteOfferBannersConfig _bannersConfig;

		protected abstract AnimationShopTab TargetTab { get; }

		protected abstract RewardType BannerRewardType { get; }

		[Inject]
		public void ConstructEmote(AnimationShopOpenRequest animationShopOpenRequest, EmoteLockerOpenRequest emoteLockerOpenRequest, IEmoteOfferBannersConfig bannersConfig)
		{
			_animationShopOpenRequest = animationShopOpenRequest;
			_emoteLockerOpenRequest = emoteLockerOpenRequest;
			_bannersConfig = bannersConfig;
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
			if (FlagsStorage.IsMainMenu)
			{
				_animationShopOpenRequest.Set(TargetTab, FirstRewardBundleId());
				_coreGameManager.menuControl.ShowSelectDriver(JsonConvert.SerializeObject(new PersonWindowAdditionalData
				{
					ParentWindow = PersonWindowAdditionalData.ParentWindowType.MainMenu,
					OpenCategory = AnimationShopRouting.CategoryFor(TargetTab)
				}));
				_managerUI.ShowUserBalanceWindow();
			}
			else
			{
				_emoteLockerOpenRequest.Set(new EmoteLockerContext
				{
					OpenPickerTab = TargetTab,
					FocusBundleId = FirstRewardBundleId()
				});
				_managerUI.ShowEmoteLockerWindow();
			}
		}

		protected override void OnOfferLoaded(OfferModel offerModel)
		{
			bool flag = TryResolveEntry(offerModel, out var data);
			ApplyTitle(flag ? data.Title : null);
			ApplyTextField(DescriptionText, flag ? data.Description : null);
			ApplyTextField(ShopButtonCaptionText, flag ? data.ShopButtonCaption : null);
			ApplyPreview(flag ? data.PreviewSprite : null);
		}

		private void ApplyTitle(string locKey)
		{
			if ((bool)TitleText && !string.IsNullOrEmpty(locKey))
			{
				TitleText.text = new LocalizedString(locKey);
			}
		}

		private int FirstRewardBundleId()
		{
			RewardItemData[] array = OfferBigBannerView.CurrentOfferModel?.Payload?.RewardItems;
			if (array == null)
			{
				return -1;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].Type == BannerRewardType)
				{
					return array[i].ID;
				}
			}
			return -1;
		}

		private bool TryResolveEntry(OfferModel offerModel, out EmoteOfferBannerData data)
		{
			data = default;
			RewardItemData[] array = offerModel?.Payload?.RewardItems;
			if (array == null || _bannersConfig == null)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].Type == BannerRewardType && _bannersConfig.TryGetEntry(array[i].Type, array[i].ID, out data))
				{
					return true;
				}
			}
			return false;
		}

		private static void ApplyTextField(TextMeshProUGUI target, string locKey)
		{
			if ((bool)target)
			{
				if (string.IsNullOrEmpty(locKey))
				{
					target.gameObject.SetActive(value: false);
					return;
				}
				target.gameObject.SetActive(value: true);
				target.text = new LocalizedString(locKey);
			}
		}

		private void ApplyPreview(Sprite sprite)
		{
			if ((bool)PreviewImage)
			{
				PreviewImage.gameObject.SetActive(sprite);
				if ((bool)sprite)
				{
					PreviewImage.sprite = sprite;
				}
			}
		}
	}
}
