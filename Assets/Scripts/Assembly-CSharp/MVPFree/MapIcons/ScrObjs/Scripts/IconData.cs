using System;
using MVPFree.MapIcons.Enum;
using UnityEngine;
using _Car_Parking.Scripts.EventSystem;

namespace MVPFree.MapIcons.ScrObjs.Scripts
{
	[Serializable]
	public class IconData
	{
		[field: SerializeField]
		public Sprite SpriteIcon { get; private set; }

		[field: SerializeField]
		public IconType IconType { get; private set; }

		[field: SerializeField]
		public _Car_Parking.Scripts.EventSystem.EventType EventType { get; private set; } = _Car_Parking.Scripts.EventSystem.EventType.None;

		[field: SerializeField]
		public string LocalizeKey { get; private set; }

		[field: SerializeField]
		public Vector3 ScaleIcon { get; private set; } = new Vector3(1f, 1f, 1f);

		[field: SerializeField]
		public int SortingOrder { get; private set; }
	}
}
