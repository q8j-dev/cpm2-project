using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public static class ScrollViewContainer
	{
		private static List<ScrollViewBase> _scrollViews = new List<ScrollViewBase>();

		public static List<ScrollViewBase> GetScrollViews()
		{
			return _scrollViews;
		}

		public static void AddScrollView(ScrollViewBase scrollView)
		{
			_scrollViews.Add(scrollView);
		}

		public static void RemoveScrollView(ScrollViewBase scrollView)
		{
			_scrollViews.Remove(scrollView);
		}

		public static ScrollViewBase GetScrollView(Vector2 mousePosition)
		{
			for (int i = 0; i < _scrollViews.Count; i++)
			{
				if ((bool)_scrollViews[i] && _scrollViews[i].CanvasRect.Contains(mousePosition))
				{
					return _scrollViews[i];
				}
			}
			return null;
		}
	}
}
