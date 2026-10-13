using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class LightForChangeRoom : MonoBehaviour
	{
		public Light Light { get; private set; }

		private void Awake()
		{
			Light = GetComponent<Light>();
		}
	}
}
