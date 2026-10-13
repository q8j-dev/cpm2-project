using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Other
{
	public class CopySpriteImage : MonoBehaviour
	{
		[SerializeField]
		private Image Source;

		[SerializeField]
		private Image Target;

		private void OnEnable()
		{
			if ((bool)Source && (bool)Target)
			{
				Target.sprite = Source.sprite;
			}
		}
	}
}
