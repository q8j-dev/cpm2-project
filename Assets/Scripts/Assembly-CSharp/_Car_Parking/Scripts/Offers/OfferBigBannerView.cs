using UnityEngine;

namespace _Car_Parking.Scripts.Offers
{
	public class OfferBigBannerView : OfferViewBase
	{
		[SerializeField]
		private bool NeetToUpdateScale = true;

		protected override string ImagePath => currentOfferModel?.Skin.BigImgPath;

		public override void Init(OfferModel offerModel)
		{
			base.Init(offerModel);
			if ((bool)RawImageLoadAsync && (bool)RawImageLoadAsync.ImageRaw && NeetToUpdateScale)
			{
				RawImageLoadAsync.ImageRaw.transform.localScale = new Vector3((!string.IsNullOrEmpty(ImagePath)) ? 1 : (-1), 1f, 1f);
			}
		}
	}
}
