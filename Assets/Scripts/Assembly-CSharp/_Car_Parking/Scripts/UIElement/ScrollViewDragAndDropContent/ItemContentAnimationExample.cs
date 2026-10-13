using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class ItemContentAnimationExample : ItemContentDraggableContent
	{
		[SerializeField]
		private Image Image = default;

		public override void CopyContent(ItemContentDraggableContent content)
		{
			ItemContentAnimationExample itemContentAnimationExample = (ItemContentAnimationExample)content;
			if ((bool)Image && (bool)itemContentAnimationExample && (bool)itemContentAnimationExample.Image)
			{
				Image.sprite = itemContentAnimationExample.Image.sprite;
			}
		}

		public override object GetContentData()
		{
			return Image.sprite;
		}

		public override void SetContentData(object content)
		{
			Image.sprite = (Sprite)content;
		}
	}
}
