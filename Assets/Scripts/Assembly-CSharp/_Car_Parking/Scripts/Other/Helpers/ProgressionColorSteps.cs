using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class ProgressionColorSteps : ProgressionBase
	{
		[Serializable]
		private class ColorStepData
		{
			public float Progression;

			public Color Color;
		}

		public Image Image;

		public SpriteRenderer SpriteRenderer;

		[SerializeField]
		private ColorStepData[] ColorStepDatas;

		private float _progression;

		public override float Progression
		{
			get
			{
				return _progression;
			}
			set
			{
				_progression = value;
				Color = GetColorProgression(value);
			}
		}

		private Color Color
		{
			get
			{
				if ((bool)Image)
				{
					return Image.color;
				}
				if ((bool)SpriteRenderer)
				{
					return SpriteRenderer.color;
				}
				return Color.white;
			}
			set
			{
				if ((bool)Image)
				{
					Image.color = value;
				}
				if ((bool)SpriteRenderer)
				{
					SpriteRenderer.color = value;
				}
			}
		}

		private Color GetColorProgression(float progression)
		{
			for (int i = 0; i < ColorStepDatas.Length; i++)
			{
				if (progression < ColorStepDatas[i].Progression)
				{
					return ColorStepDatas[i].Color;
				}
			}
			if (ColorStepDatas.Length == 0)
			{
				return Color.white;
			}
			ColorStepData[] colorStepDatas = ColorStepDatas;
			return colorStepDatas[colorStepDatas.Length - 1].Color;
		}
	}
}
