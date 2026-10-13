namespace _Car_Parking.Scripts.Offers
{
	public class OfferSmallBannerView : OfferViewBase
	{
		protected override string ImagePath => currentOfferModel?.Skin.ImgPath;
	}
}
