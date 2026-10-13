using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	[Preserve]
	public class SummaryOfEventsView : MonoBehaviour
	{
		public ScrollRect ScrollRect;

		[SerializeField]
		private GameObject Root = default;

		[SerializeField]
		private Transform Content = default;

		[SerializeField]
		private TabIconsController TabIcons = default;

		[SerializeField]
		private UI_InfiniteScroll UIInfiniteScroll = default;

		[SerializeField]
		private HorizontalScrollSnap HorizontalScrollSnap = default;

		public Transform ContentTransform => Content;

		public TabIconsController TabIconsController => TabIcons;

		public UI_InfiniteScroll InfiniteScroll => UIInfiniteScroll;

		public HorizontalScrollSnap ScrollSnap => HorizontalScrollSnap;

		public void UpdateRootVisible(bool isVisible)
		{
			Root.SetActive(isVisible);
		}
	}
}
