using System;
using UnityEngine;

namespace _Car_Parking.Scripts.EditableHUD
{
	[CreateAssetMenu(fileName = "EditableHUDConfig", menuName = "Configs/EditableHUDConfig")]
	public class EditableHUDConfig : ScriptableObject
	{
		[Serializable]
		public class EditorData
		{
			[Range(0f, 1f)]
			public float InvisibleAlpha;

			public Color SelectedColor;

			public float TapZoneSize = 1f;
		}

		public float ScaleMin;

		public float ScaleMax;

		public EditorData EditorSettings;

		public FakeElement FakeElementPrefab;

		public string[] StaticElements;

		public bool IsStaticElement(string elementName)
		{
			return StaticElements.Contains(elementName);
		}
	}
}
