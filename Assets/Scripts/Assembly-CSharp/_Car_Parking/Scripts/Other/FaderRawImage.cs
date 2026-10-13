using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Other
{
	public class FaderRawImage : Fader
	{
		[SerializeField]
		private RawImage RawImage = default;

		public override float Alpha
		{
			get
			{
				if (!RawImage)
				{
					return 0f;
				}
				return RawImage.color.a;
			}
			set
			{
				if ((bool)RawImage)
				{
					Color color = RawImage.color;
					color.a = value;
					RawImage.color = color;
				}
			}
		}
	}
}
