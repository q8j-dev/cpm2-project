using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ScrollViewFocusFunctions
{
	public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint)
	{
		Vector2 size = scrollView.content.rect.size;
		Vector2 size2 = ((RectTransform)scrollView.content.parent).rect.size;
		Vector2 scale = scrollView.content.localScale;
		size.Scale(scale);
		focusPoint.Scale(scale);
		Vector2 normalizedPosition = scrollView.normalizedPosition;
		if (scrollView.horizontal && size.x > size2.x)
		{
			normalizedPosition.x = Mathf.Clamp01((focusPoint.x - size2.x * 0.5f) / (size.x - size2.x));
		}
		if (scrollView.vertical && size.y > size2.y)
		{
			normalizedPosition.y = Mathf.Clamp01((focusPoint.y - size2.y * 0.5f) / (size.y - size2.y));
		}
		return normalizedPosition;
	}

	public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, RectTransform item)
	{
		Vector2 vector = scrollView.content.InverseTransformPoint(item.transform.TransformPoint(item.rect.center));
		Vector2 size = scrollView.content.rect.size;
		size.Scale(scrollView.content.pivot);
		return scrollView.CalculateFocusedScrollPosition(vector + size);
	}

	public static void FocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint)
	{
		scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(focusPoint);
	}

	public static void FocusOnItem(this ScrollRect scrollView, RectTransform item)
	{
		scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(item);
	}

	private static IEnumerator LerpToScrollPositionCoroutine(this ScrollRect scrollView, Vector2 targetNormalizedPos, float speed)
	{
		Vector2 initialNormalizedPos = scrollView.normalizedPosition;
		for (float t = 0f; t < 1f; t += speed * Time.unscaledDeltaTime)
		{
			scrollView.normalizedPosition = Vector2.LerpUnclamped(initialNormalizedPos, targetNormalizedPos, 1f - (1f - t) * (1f - t));
			yield return null;
		}
		scrollView.normalizedPosition = targetNormalizedPos;
	}

	public static IEnumerator FocusAtPointCoroutine(this ScrollRect scrollView, Vector2 focusPoint, float speed)
	{
		yield return scrollView.LerpToScrollPositionCoroutine(scrollView.CalculateFocusedScrollPosition(focusPoint), speed);
	}

	public static IEnumerator FocusOnItemCoroutine(this ScrollRect scrollView, RectTransform item, float speed)
	{
		yield return scrollView.LerpToScrollPositionCoroutine(scrollView.CalculateFocusedScrollPosition(item), speed);
	}
}
