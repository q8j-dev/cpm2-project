using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ButtonSpiteUpdater : MonoBehaviour
	{
		[Serializable]
		public class SpriteData
		{
			public Sprite Main;

			public Sprite Pressed;
		}

		[SerializeField]
		private Image Image;

		[SerializeField]
		private Button Button;

		public void UpdateSprite(SpriteData spriteData)
		{
			if (spriteData != null)
			{
				if ((bool)Image)
				{
					Image.sprite = spriteData.Main;
				}
				if ((bool)Button)
				{
					SpriteState spriteState = Button.spriteState;
					spriteState.pressedSprite = spriteData.Pressed;
					Button.spriteState = spriteState;
				}
			}
		}
	}
}
