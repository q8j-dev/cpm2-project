using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ScrollRectItemsCenterController : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IPointerDownHandler
	{
		[SerializeField]
		private ScrollRect ScrollRect = default;

		[SerializeField]
		private RectTransform Center = default;

		[SerializeField]
		private float MoveVelocity = default;

		private int _oldItemCenterIndex = -1;

		private Tween _moveToItemCenterTween;

		public int ItemCenterIndex
		{
			get
			{
				int result = -1;
				if ((bool)ScrollRect && (bool)ScrollRect.content && (bool)Center)
				{
					float num = float.MaxValue;
					for (int i = 0; i < ScrollRect.content.childCount; i++)
					{
						float num2 = Extensions.DistanceSquare(ScrollRect.content.GetChild(i).position, Center.position);
						if (num2 < num)
						{
							num = num2;
							result = i;
						}
					}
				}
				return result;
			}
			set
			{
				if ((bool)ScrollRect && (bool)ScrollRect.content && (bool)Center && ScrollRect.content.childCount > 0)
				{
					_moveToItemCenterTween?.Kill();
					value = Mathf.Clamp(value, 0, ScrollRect.content.childCount - 1);
					ScrollRect.content.transform.position += Center.position - ScrollRect.content.GetChild(value).position;
				}
			}
		}

		public GameObject ItemCenter
		{
			get
			{
				if ((bool)ScrollRect && (bool)ScrollRect.content && ItemCenterIndex >= 0 && ItemCenterIndex < ScrollRect.content.childCount)
				{
					return ScrollRect.content.GetChild(ItemCenterIndex).gameObject;
				}
				return null;
			}
		}

		public event Action<GameObject> OnItemCenterChanged;

		public void MoveToItemCenter(int index)
		{
			if ((bool)ScrollRect && (bool)ScrollRect.content && (bool)Center && ScrollRect.content.childCount > 0)
			{
				ScrollRect.velocity = Vector2.zero;
				_moveToItemCenterTween?.Kill();
				index = Mathf.Clamp(index, 0, ScrollRect.content.childCount - 1);
				Vector3 vector = ScrollRect.content.transform.position + (Center.position - ScrollRect.content.GetChild(index).position);
				_moveToItemCenterTween = ScrollRect.content.transform.DOMove(vector, Vector3.Distance(vector, ScrollRect.content.transform.position) / MoveVelocity).SetEase(Ease.OutSine);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_moveToItemCenterTween?.Kill();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_moveToItemCenterTween?.Kill();
		}

		private void OnDestroy()
		{
			_moveToItemCenterTween?.Kill();
		}

		private void Update()
		{
			if (_oldItemCenterIndex != ItemCenterIndex)
			{
				_oldItemCenterIndex = ItemCenterIndex;
				OnItemCenterChanged?.Invoke(ItemCenter);
			}
		}
	}
}
