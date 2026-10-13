using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class FaderCanvas : Fader
	{
		[SerializeField]
		private CanvasGroup CanvasGroup;

		public override float Alpha
		{
			get
			{
				if (!(CanvasGroup != null))
				{
					return 0f;
				}
				return CanvasGroup.alpha;
			}
			set
			{
				if ((bool)CanvasGroup)
				{
					CanvasGroup.alpha = value;
				}
			}
		}
	}
}
