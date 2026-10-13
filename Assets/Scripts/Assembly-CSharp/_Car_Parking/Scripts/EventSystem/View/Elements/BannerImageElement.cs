using UnityEngine;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.EventSystem.View.Elements
{
	[RequireComponent(typeof(RawImageLoadAsync))]
	public class BannerImageElement : MonoBehaviour, IBannerElement
	{
		private RawImageLoadAsync _rawImageLoad;

		public void Bind(BannerContext context)
		{
			if (!_rawImageLoad)
			{
				_rawImageLoad = GetComponent<RawImageLoadAsync>();
			}
			_rawImageLoad.LoadFromSDN(context?.Event?.Images?.BigImageUrl);
		}
	}
}
