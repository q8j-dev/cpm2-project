using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class ScrollViewDragAndDropItemContentHelper
	{
		public static void SwapItems(ItemContentDraggable item1, ItemContentDraggable item2)
		{
			Vector3 position = item1.ContentRectTransform.position;
			Vector3 position2 = item2.ContentRectTransform.position;
			object contentData = item1.Content.GetContentData();
			object contentData2 = item2.Content.GetContentData();
			item1.Content.SetContentData(contentData2);
			item2.Content.SetContentData(contentData);
			item1.ContentRectTransform.position = position2;
			item2.ContentRectTransform.position = position;
			item1.StopDragEnd();
			item2.StopDragEnd();
		}
	}
}
