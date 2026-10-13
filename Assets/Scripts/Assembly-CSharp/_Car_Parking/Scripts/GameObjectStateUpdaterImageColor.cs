using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts
{
	public class GameObjectStateUpdaterImageColor : GameObjectStateUpdater
	{
		[SerializeField]
		private Image Image;

		[SerializeField]
		private Color ColorOn;

		[SerializeField]
		private Color ColorOff;

		public override bool IsOn
		{
			get
			{
				if ((bool)Image)
				{
					return GetLen(Image.color, ColorOn) < GetLen(Image.color, ColorOff);
				}
				return false;
			}
		}

		public override void UpdateState(bool isOn)
		{
			if ((bool)Image)
			{
				Image.color = (isOn ? ColorOn : ColorOff);
			}
		}

		private float GetLen(Color color01, Color color02)
		{
			Color color3 = color01 - color02;
			return Mathf.Abs(color3.r) + Mathf.Abs(color3.g) + Mathf.Abs(color3.b) + Mathf.Abs(color3.a);
		}
	}
}
