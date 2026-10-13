using UnityEngine.UI;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class ProgressionImageFill : ProgressionBase
	{
		public Image Image;

		public override float Progression
		{
			get
			{
				return Image.fillAmount;
			}
			set
			{
				Image.fillAmount = value;
			}
		}
	}
}
