using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public abstract class ItemContentDraggableContent : MonoBehaviour
	{
		public abstract void CopyContent(ItemContentDraggableContent content);

		public abstract object GetContentData();

		public abstract void SetContentData(object content);

		public virtual void DestroyAnimation()
		{
		}
	}
}
