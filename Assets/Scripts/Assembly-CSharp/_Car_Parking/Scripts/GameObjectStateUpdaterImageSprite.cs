using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts
{
	public class GameObjectStateUpdaterImageSprite : GameObjectStateUpdater
	{
		[SerializeField]
		private Image Image;

		[SerializeField]
		private Sprite On;

		[SerializeField]
		private Sprite Off;

		public override bool IsOn
		{
			get
			{
				if (!Image)
				{
					return false;
				}
				return Image.sprite == On;
			}
		}

		public override void UpdateState(bool isOn)
		{
			if ((bool)Image)
			{
				Image.sprite = (isOn ? On : Off);
			}
		}
	}
}
