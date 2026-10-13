using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using _Car_Parking.Scripts.DataEntities;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Offers
{
	public class PurchaseCheckerOfferVersion : IPurchaseCheckProductToBuy
	{
		private class CheckOfferVersionRequest
		{
			public int OfferID;

			public int Version;
		}

		public const string GetOfferFn = "CheckOfferVersion24_1";

		private readonly IOfferSystem _offerSystem;

		public PurchaseCheckerOfferVersion(IOfferSystem offerSystem)
		{
			_offerSystem = offerSystem;
		}

		public async UniTask<PreBuyActionResult> CheckProductToBuyAsync(string productID)
		{
			OfferModel offer = _offerSystem.GetOffer(productID);
			if (offer == null)
			{
				return PreBuyActionResult.Success;
			}
			if (!offer.IsValid)
			{
				return PreBuyActionResult.OfferIsOld;
			}
			FirebaseWrappedFunction firebaseWrappedFunction = new FirebaseWrappedFunctionFactory().Create("CheckOfferVersion24_1");
			CheckOfferVersionRequest value = new CheckOfferVersionRequest
			{
				OfferID = offer.ID,
				Version = offer.Version
			};
			return (await firebaseWrappedFunction.CallAsyncResult<EmptyClass>(JsonConvert.SerializeObject(value, JsonConvertCustomSettingsForDatabase.JsonSettingsForDataBase))).IsSuccess ? PreBuyActionResult.Success : PreBuyActionResult.OfferIsOld;
		}
	}
}
