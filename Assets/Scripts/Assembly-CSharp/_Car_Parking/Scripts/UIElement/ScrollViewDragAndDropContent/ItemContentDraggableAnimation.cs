using DG.Tweening;
using UnityEngine;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class ItemContentDraggableAnimation : ItemContentDraggable
	{
		[SerializeField]
		private GameObjectStateUpdater SelectGraphics;

		[SerializeField]
		private GameObjectStateUpdater EmptyGO;

		[SerializeField]
		private GameObjectStateUpdater SelectContentGO;

		[SerializeField]
		private MoveToTransformByTween MoveToTransformByTween;

		[SerializeField]
		private RectTransformSizeSetter RectTransformSizeSetter;

		[SerializeField]
		private ItemContentDraggableAnimationConfig AnimationConfig;

		private bool _isBackAnimation;

		public override bool CanDrag
		{
			get
			{
				if (!IsEmpty)
				{
					return !_isBackAnimation;
				}
				return false;
			}
		}

		public override bool IsEmpty => EmptyGO.IsOn;

		public override bool CanSelect => !IsEmpty;

		public override bool IsSelected => SelectGraphics.IsOn;

		public override void TryRevertSelect()
		{
			SelectVisibleUpdate(!IsSelected);
			base.TryRevertSelect();
		}

		public override void CopyContent(ItemContentDraggable itemContentDraggable)
		{
			base.CopyContent(itemContentDraggable);
			base.ContentRectTransform.position = itemContentDraggable.ContentRectTransform.position;
			StopDragEnd();
		}

		public override void DestroyAnimation()
		{
			if ((bool)ItemContentDraggableContent)
			{
				ItemContentDraggableContent.DestroyAnimation();
			}
			Vector2 size = base.SelfRectTransform.rect.size;
			if ((bool)_scrollView && _scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Horizontal)
			{
				size.x = 0f - _scrollView.ContentSpaceBetweenItems;
			}
			else if ((bool)_scrollView && _scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Vertical)
			{
				size.y = 0f - _scrollView.ContentSpaceBetweenItems;
			}
			RectTransformSizeSetter.SetSize(size, AnimationConfig.DestroyAnimationTime, (RectTransformSizeSetter rectTransformSizeSetter) =>
			{
				Object.Destroy(base.gameObject);
			});
		}

		public override void NewItemAnimation()
		{
			base.NewItemAnimation();
			Vector2 size = base.SelfRectTransform.rect.size;
			Vector2 size2 = size;
			if ((bool)_scrollView && _scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Horizontal)
			{
				size2.x = 0f - _scrollView.ContentSpaceBetweenItems;
			}
			else if ((bool)_scrollView && _scrollView.TypeMoveAxis == ScrollViewBase.MoveAxisType.Vertical)
			{
				size2.y = 0f - _scrollView.ContentSpaceBetweenItems;
			}
			RectTransformSizeSetter.SetSize(size2, -1f);
			RectTransformSizeSetter.SetSize(size, AnimationConfig.DestroyAnimationTime);
		}

		public override void StartDrag()
		{
			base.StartDrag();
			EmptyGO.UpdateState(isOn: true);
			SelectContentVisibleUpdate(visible: true);
		}

		public override void StopDragEnd()
		{
			base.StopDragEnd();
			if ((bool)_scrollView)
			{
				base.ContentRectTransform.SetParent(_scrollView.TransformTop);
			}
			SelectContentVisibleUpdate(visible: false);
			BackAnimation();
		}

		public override void SelectContentVisibleUpdate(bool visible)
		{
			SelectContentGO.UpdateState(visible);
		}

		public override void SelectVisibleUpdate(bool visible)
		{
			base.SelectVisibleUpdate(visible);
			SelectGraphics.UpdateState(visible);
		}

		public override void ContentVisibleUpdate(bool visible)
		{
			base.ContentVisibleUpdate(visible);
			base.ContentRectTransform.gameObject.SetActive(visible);
		}

		public override void EmptyVisibleUpdate(bool visible)
		{
			base.EmptyVisibleUpdate(visible);
			EmptyGO.UpdateState(visible);
		}

		private void BackAnimation()
		{
			_isBackAnimation = true;
			float a = Vector3.Distance(base.transform.position, base.ContentRectTransform.transform.position) / AnimationConfig.ItemBackVelocity;
			a = Mathf.Max(a, AnimationConfig.DestroyAnimationTime);
			MoveToTransformByTween.Move(base.transform, a, Ease.OutSine, () =>
			{
				RevertContentParent();
				_isBackAnimation = false;
			});
		}
	}
}
