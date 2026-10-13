using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class SpeedometerTireViewParent : MonoBehaviour
	{
		private void OnEnable()
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(FlagsStorage.IsFreeDriveMode);
			}
		}
	}
}
