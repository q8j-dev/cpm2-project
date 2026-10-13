using System;
using UnityEngine;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	[CreateAssetMenu(fileName = "DragAndDropItemContent", menuName = "Configs/UI/ScrollView/ItemContent")]
	public class DragAndDropItemContentConfig : ScrollViewConfig
	{
		[Serializable]
		public class ScrollByItemDragData
		{
			public float ZoneSize;

			public float Velocity;

			public float DelayToStart;
		}

		[Serializable]
		public class SwapData
		{
			public bool IsWork;

			[Range(0f, 1f)]
			public float SizeSwap;
		}

		public ItemContentDraggable ItemContentDraggablePrefab;

		public ScrollByItemDragData ScrollByItemDrag;

		public SwapData Swap;

		public float DragLerpPower = 15f;
	}
}
