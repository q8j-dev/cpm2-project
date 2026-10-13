using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public static class UIHelper
	{
		public static async UniTask DelayedRebuildLayout(RectTransform toRebuild, CancellationToken cancellationToken = default(CancellationToken))
		{
			if ((bool)toRebuild)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(toRebuild);
			}
			await UniTask.Yield();
			if (!cancellationToken.IsCancellationRequested && (bool)toRebuild)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(toRebuild);
			}
		}

		public static async UniTask DoubleDelayedRebuildLayout(RectTransform firstLayout, RectTransform secondLayout)
		{
			await DelayedRebuildLayout(firstLayout);
			await DelayedRebuildLayout(secondLayout);
		}

		public static Vector2 CalculateGridElementWidth(RectTransform rectTransform, GridLayoutGroup gridLayoutGroup, int cellCountInOneRow, int space = 0)
		{
			return new Vector2((rectTransform.rect.width - ((float)(gridLayoutGroup.padding.left + gridLayoutGroup.padding.right) + gridLayoutGroup.spacing.x)) / (float)cellCountInOneRow - (float)space, gridLayoutGroup.cellSize.y);
		}

		public static async UniTask SizeFitterMaxWidth(ContentSizeFitter contentSizeFitter, float maxSize)
		{
			contentSizeFitter.enabled = true;
			await UniTask.Yield();
			RectTransform component = contentSizeFitter.GetComponent<RectTransform>();
			Vector2 sizeDelta = component.sizeDelta;
			if (sizeDelta.x > maxSize)
			{
				contentSizeFitter.enabled = false;
				sizeDelta.x = maxSize;
				component.sizeDelta = sizeDelta;
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(component.parent.GetComponent<RectTransform>());
		}

		public static async UniTask SizeFitterMaxHeight(ContentSizeFitter contentSizeFitter, float maxSize)
		{
			await UniTask.Yield();
			RectTransform component = contentSizeFitter.GetComponent<RectTransform>();
			Vector2 sizeDelta = component.sizeDelta;
			if (sizeDelta.y > maxSize)
			{
				contentSizeFitter.enabled = false;
				sizeDelta.y = maxSize;
				component.sizeDelta = sizeDelta;
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(component.parent.GetComponent<RectTransform>());
		}

		public static async UniTask SetScrollHorizontalPosDelayed(this ScrollRect scrollRect, float position)
		{
			await UniTask.Yield();
			scrollRect.horizontalNormalizedPosition = position;
		}

		public static async UniTask SetScrollToRectTransform(this ScrollRect scrollRect, RectTransform target)
		{
			await UniTask.Yield();
			RectTransform content = scrollRect.content;
			RectTransform rectTransform = ((scrollRect.viewport != null) ? scrollRect.viewport : ((RectTransform)scrollRect.transform));
			Vector2 vector = content.InverseTransformPoint(target.position);
			if (scrollRect.vertical)
			{
				float num = content.rect.height - rectTransform.rect.height;
				if (num > 0f)
				{
					scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1f + (vector.y + rectTransform.rect.height * 0.5f) / num);
				}
			}
			if (scrollRect.horizontal)
			{
				float num2 = content.rect.width - rectTransform.rect.width;
				if (num2 > 0f)
				{
					scrollRect.horizontalNormalizedPosition = Mathf.Clamp01((vector.x - rectTransform.rect.width * 0.5f) / num2);
				}
			}
		}

		public static async UniTask SetScrollVerticalPosDelayed(this ScrollRect scrollRect, float position)
		{
			await UniTask.Yield();
			scrollRect.verticalNormalizedPosition = position;
		}

		public static void VerticalNormalizedPosByElementsCount(this ScrollRect scrollRect, int item, int itemsCount, bool reverse = false)
		{
			float scrollNormalizedPos = GetScrollNormalizedPos(item, itemsCount, out var reversedPos);
			scrollRect.verticalNormalizedPosition = (reverse ? reversedPos : scrollNormalizedPos);
		}

		public static void HorizontalNormalizedPosByElementsCount(this ScrollRect scrollRect, int item, int itemsCount, bool reverse = false)
		{
			float scrollNormalizedPos = GetScrollNormalizedPos(item, itemsCount, out var reversedPos);
			scrollRect.horizontalNormalizedPosition = (reverse ? reversedPos : scrollNormalizedPos);
		}

		private static float GetScrollNormalizedPos(int item, int itemsCount, out float reversedPos)
		{
			float num = (float)item / (float)(itemsCount - 1);
			reversedPos = 1f - num;
			return num;
		}

		public static Tween AnimatedChangeTimeText(TextMeshProUGUI tmp, float targetValue, float duration = 0.3f)
		{
			float startValue = (float.TryParse(tmp.text, out var result) ? result : 0f);
			return DOTween.To(() => startValue, (float x) =>
			{
				startValue = x;
				tmp.text = "+" + Utils.ConvertToTime(startValue);
			}, targetValue, duration);
		}

		public static Tween AnimatedChangeNumbersText(TextMeshProUGUI tmp, int targetValue, float duration = 0.3f)
		{
			int startValue = int.Parse(tmp.text);
			return DOTween.To(() => startValue, (int x) =>
			{
				startValue = x;
				tmp.text = $"{startValue}";
			}, targetValue, duration);
		}
	}
}
