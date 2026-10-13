using System;
using Steamworks;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Offers
{
	[Preserve]
	public class OfferPriceUpdater : IInitializable, IDisposable
	{
		private readonly IPurchaseSystem _purchaseSystem;

		private readonly IOfferSystem _offerSystem;

		public OfferPriceUpdater(IPurchaseSystem purchaseSystem, IOfferSystem offerSystem)
		{
			_purchaseSystem = purchaseSystem;
			_offerSystem = offerSystem;
		}

		public void Initialize()
		{
			_purchaseSystem.OnInitializedPreCompletedEvent += OnOfferPricesUpdated;
			_offerSystem.OnDataUpdatedEvent += OnOfferPricesUpdated;
		}

		public void Dispose()
		{
			_purchaseSystem.OnInitializedPreCompletedEvent -= OnOfferPricesUpdated;
			_offerSystem.OnDataUpdatedEvent -= OnOfferPricesUpdated;
		}

		private void OnOfferPricesUpdated()
		{
			OfferModel[] offers = _offerSystem.Offers;
			if (offers == null)
			{
				return;
			}
			foreach (OfferModel offerModel in offers)
			{
				if (offerModel.Inizilizated)
				{
					continue;
				}
				InventoryDef inventoryDef = null;
				if (offerModel.Payload.FirstSteamItemDefId > 0)
				{
					inventoryDef = _purchaseSystem.GetSteamInventoryDef(offerModel.Payload.FirstSteamItemDefId);
				}
				offerModel.Enabled = inventoryDef != null;
				if (!offerModel.Enabled && offerModel.Payload != null && offerModel.Payload.HavePurchaseOptionCoins)
				{
					offerModel.Enabled = true;
					offerModel.Inizilizated = offerModel.Enabled;
					continue;
				}
				offerModel.Inizilizated = offerModel.Enabled;
				if (!offerModel.Enabled)
				{
					continue;
				}
				offerModel.Payload.OldPrice = (float)inventoryDef.LocalPrice * offerModel.Payload.OldPriceModifier;
				if (!offerModel.Payload.HaveOldBundleID)
				{
					continue;
				}
				int num = (offerModel.Payload.RewardIsCar ? offerModel.Payload.RewardItems[0].ID : 0);
				InventoryDef inventoryDef2 = ((num > 0) ? _purchaseSystem.GetSteamInventoryDefByCarId(num) : null);
				if (!(inventoryDef2 == null))
				{
					offerModel.Payload.OldPrice = inventoryDef2.LocalPrice;
					offerModel.Payload.OldPriceText = inventoryDef2.LocalPriceFormatted;
					float num2 = 10f;
					if (inventoryDef2.LocalPrice > 0)
					{
						num2 = (1f - (float)inventoryDef.LocalPrice / (float)inventoryDef2.LocalPrice) * 100f;
					}
					int value = (int)(MathF.Round(num2 / 5f) * 5f);
					offerModel.Payload.Discount = Math.Clamp(value, 0, 100);
				}
			}
		}
	}
}
