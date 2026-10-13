using System;
using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	[Serializable]
	public class ColorTMPTextSetup
	{
		[field: SerializeField]
		public TMP_Text TMPText { get; private set; }

		[field: SerializeField]
		public Color EnabledColor { get; private set; }

		[field: SerializeField]
		public Color DisabledColor { get; private set; }

		[field: SerializeField]
		public string EnabledTextKey { get; private set; }

		[field: SerializeField]
		public string DisabledTextKey { get; private set; }
	}
}
