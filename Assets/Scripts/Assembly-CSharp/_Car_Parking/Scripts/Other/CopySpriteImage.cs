using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Other
{
	public class CopySpriteImage : MonoBehaviour
	{
		[SerializeField]
		private Image Source = default;

		[SerializeField]
		private Image Target = default;

		private void OnEnable()
		{
			if ((bool)Source && (bool)Target)
			{
				Target.sprite = Source.sprite;
			}
		}
	}
}
