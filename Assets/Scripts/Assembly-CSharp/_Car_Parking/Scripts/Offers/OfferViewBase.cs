using CPMEngine;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Offers
{
	public abstract class OfferViewBase : MonoBehaviour
	{
		protected OfferModel currentOfferModel;

		[SerializeField]
		protected RawImageLoadAsync RawImageLoadAsync;

		[SerializeField]
		private Image BG = default;

		[SerializeField]
		private Sprite[] BGSprites = default;

		[SerializeField]
		private TMP_Text Title = default;

		[SerializeField]
		private TMP_Text Discount = default;

		[SerializeField]
		private GameObject DiscountRoot = default;

		[SerializeField]
		private TMP_Text Time = default;

		[SerializeField]
		private GameObject TimeRoot = default;

		[SerializeField]
		private Text OldPrice = default;

		[SerializeField]
		private CrossedLine OldPriceLine = default;

		[SerializeField]
		private GameObject OldPriceRoot = default;

		[SerializeField]
		private Text CurrentPrice = default;

		private IPurchaseSystem _purchaseSystem;

		public OfferModel CurrentOfferModel => currentOfferModel;

		public int OfferId => currentOfferModel?.ID ?? (-1);

		public int CarId
		{
			get
			{
				if (currentOfferModel == null || currentOfferModel.Payload == null || currentOfferModel.Payload.RewardItems == null)
				{
					return -1;
				}
				for (int i = 0; i < currentOfferModel.Payload.RewardItems.Length; i++)
				{
					if (currentOfferModel.Payload.RewardItems[i].Type == RewardType.Car)
					{
						return currentOfferModel.Payload.RewardItems[i].ID;
					}
				}
				return -1;
			}
		}

		protected abstract string ImagePath { get; }

		[Inject]
		public void ConstructBase(IPurchaseSystem purchaseSystem)
		{
			_purchaseSystem = purchaseSystem;
		}

		public virtual void Init(OfferModel offerModel)
		{
			currentOfferModel = offerModel;
			if (currentOfferModel == null)
			{
				return;
			}
			_purchaseSystem.GetPurchaseProduct(currentOfferModel.Payload.FirstBundleID);
			if ((bool)BG && BGSprites.Length != 0)
			{
				BG.sprite = BGSprites[currentOfferModel.Skin.BGImgIndex % BGSprites.Length];
			}
			if ((bool)Title)
			{
				string text = "";
				text = currentOfferModel.Skin.TitleText;
				if (string.IsNullOrEmpty(text))
				{
					switch (currentOfferModel.Payload.OfferType)
					{
					case OfferType.Conversion:
						text = "Offer.Title.Conversion";
						break;
					case OfferType.Starter:
						text = "Offer.Title.Starter";
						break;
					}
				}
				Title.text = new LocalizedString(text);
			}
			if ((bool)Discount)
			{
				Discount.text = $"-{currentOfferModel.Payload.Discount}%";
			}
			if ((bool)DiscountRoot)
			{
				DiscountRoot.SetActive(currentOfferModel.Payload.Discount > 0);
			}
			if ((bool)OldPrice && currentOfferModel.Payload.HaveOldPrice)
			{
				OldPrice.text = currentOfferModel.Payload.OldPriceText;
			}
			if ((bool)CurrentPrice && currentOfferModel.Payload.FirstSteamItemDefId > 0)
			{
				InventoryDef steamInventoryDef = _purchaseSystem.GetSteamInventoryDef(currentOfferModel.Payload.FirstSteamItemDefId);
				if (steamInventoryDef != null)
				{
					CurrentPrice.text = steamInventoryDef.LocalPriceFormatted;
				}
			}
			bool flag = currentOfferModel.Payload.OfferType == OfferType.OTO;
			GameObject gameObject = (TimeRoot ? TimeRoot : (Time ? Time.gameObject : null));
			if ((bool)gameObject)
			{
				gameObject.SetActive(!flag);
			}
			if (flag)
			{
				if ((bool)DiscountRoot)
				{
					DiscountRoot.SetActive(value: false);
				}
				if ((bool)Discount)
				{
					Discount.gameObject.SetActive(value: false);
				}
				if ((bool)OldPrice)
				{
					OldPrice.gameObject.SetActive(value: false);
				}
				if ((bool)OldPriceLine)
				{
					OldPriceLine.gameObject.SetActive(value: false);
				}
			}
			if ((bool)OldPriceRoot)
			{
				OldPriceRoot.SetActive(currentOfferModel.Payload.HaveOldPrice);
			}
			if ((bool)OldPriceLine)
			{
				OldPriceLine.UpdateLine();
			}
			if ((bool)RawImageLoadAsync && !string.IsNullOrEmpty(ImagePath))
			{
				RawImageLoadAsync.LoadFromSDN(ImagePath);
			}
		}

		private void Update()
		{
			if ((bool)Time && currentOfferModel != null && currentOfferModel.Payload.OfferType != OfferType.OTO)
			{
				Time.text = Utils.SecondsToText(currentOfferModel.SecondsToComplete - 5);
			}
		}
	}
}
