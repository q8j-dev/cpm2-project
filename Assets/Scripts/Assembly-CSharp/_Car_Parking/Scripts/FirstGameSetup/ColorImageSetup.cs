using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	[Serializable]
	public class ColorImageSetup
	{
		[field: SerializeField]
		public Image Image { get; private set; }

		[field: SerializeField]
		public Color EnabledColor { get; private set; }

		[field: SerializeField]
		public Color DisabledColor { get; private set; }
	}
}
