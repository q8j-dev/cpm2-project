using System;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public class ScrollViewConfig : ScriptableObject
	{
		[Serializable]
		public class ItemSelectData
		{
			public bool IsWorking;

			public float TimeToSelect;
		}

		[Serializable]
		public class DragData
		{
			public bool Enabled;

			public float DelayToStartDragItem;

			public float MoveToStartDragItem;
		}

		[Serializable]
		public class InertiaData
		{
			public bool Enabled;

			public float DecelerationRate = 0.135f;
		}

		public const string MenuPath = "Configs/UI/ScrollView/";

		public float MoveToStartScroll;

		public float WheelScrollSensitivity = 1f;

		public float WheelSmoothSpeed = 16f;

		public DragData Drag;

		public ItemSelectData ItemSelect;

		public InertiaData Inertia;
	}
}
