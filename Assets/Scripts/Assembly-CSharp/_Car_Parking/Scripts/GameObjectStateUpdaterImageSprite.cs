using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts
{
	public class GameObjectStateUpdaterImageSprite : GameObjectStateUpdater
	{
		[SerializeField]
		private Image Image = default;

		[SerializeField]
		private Sprite On = default;

		[SerializeField]
		private Sprite Off = default;

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
