using UnityEngine;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.SummaryOfEvents.ItemViews
{
	[RequireComponent(typeof(RawImageLoadAsync))]
	public class ItemEventImageView : MonoBehaviour, IItemEventView
	{
		[SerializeField]
		private EventImageKind ImageKind = default;

		private RawImageLoadAsync _rawImageLoad;

		private void Awake()
		{
			_rawImageLoad = GetComponent<RawImageLoadAsync>();
		}

		public void UpdateView(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			if (_rawImageLoad == null)
			{
				_rawImageLoad = GetComponent<RawImageLoadAsync>();
			}
			string url = ImageKind switch
			{
				EventImageKind.Summary => @event?.Images?.SummaryImageUrl, 
				EventImageKind.Big => @event?.Images?.BigImageUrl, 
				_ => null, 
			};
			_rawImageLoad.LoadFromSDN(url);
		}
	}
}
