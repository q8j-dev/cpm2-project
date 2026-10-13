using UnityEngine;

namespace _Car_Parking.Scripts.Drag
{
	public class DynamicSpacers : MonoBehaviour
	{
		private const float OrigWidth = 0.009f;

		private const float OrigDiam = 0.133f;

		private void Awake()
		{
			base.transform.localScale = Vector3.one;
			Resize(0.24f, 0.003f, 1);
		}

		public void Resize(float diameter, float width, int side)
		{
			float num = diameter / 0.133f;
			float num2 = width / 0.009f;
			base.transform.localScale = new Vector3((float)side * num2, num, num);
		}
	}
}
