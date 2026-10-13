using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class ItemContentDraggableExample : ItemContentDraggable
	{
		[SerializeField]
		private GameObject SelectGraphics;

		[SerializeField]
		private GameObject EmptyGO;

		[SerializeField]
		private GameObject SelectContentGO;

		[SerializeField]
		private Image Icon;

		public override bool CanDrag => !IsEmpty;

		public override bool IsEmpty => EmptyGO.activeSelf;

		public override bool CanSelect => !IsEmpty;

		public override bool IsSelected => SelectGraphics.activeSelf;

		public Sprite IconSprite => Icon.sprite;

		public override void TryRevertSelect()
		{
			SelectVisibleUpdate(!IsSelected);
			base.TryRevertSelect();
		}

		public override void StartDrag()
		{
			base.StartDrag();
			EmptyGO.SetActive(value: true);
			SelectContentGO.SetActive(value: true);
		}

		public override void StopDragEnd()
		{
			base.StopDragEnd();
			SelectContentGO.SetActive(value: false);
			base.ContentRectTransform.anchoredPosition = Vector2.zero;
		}

		public override void CopyContent(ItemContentDraggable itemContentDraggable)
		{
			Icon.sprite = ((ItemContentDraggableExample)itemContentDraggable).IconSprite;
		}

		public override void SelectVisibleUpdate(bool visible)
		{
			base.SelectVisibleUpdate(visible);
			SelectGraphics.SetActive(visible);
		}

		public override void ContentVisibleUpdate(bool visible)
		{
			base.ContentVisibleUpdate(visible);
			base.ContentRectTransform.gameObject.SetActive(visible);
		}

		public override void EmptyVisibleUpdate(bool visible)
		{
			base.EmptyVisibleUpdate(visible);
			EmptyGO.SetActive(visible);
		}
	}
}
