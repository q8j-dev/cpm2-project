using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class NewMarkIcon : MonoBehaviour
	{
		[SerializeField]
		private string Key;

		private const int HiddenValue = 1;

		private void Awake()
		{
			if (PlayerPrefs.GetInt(Key, 0) == 1)
			{
				base.gameObject.SetActive(value: false);
			}
		}

		public void HideAndSave()
		{
			if (base.gameObject.activeSelf)
			{
				PlayerPrefs.SetInt(Key, 1);
				PlayerPrefs.Save();
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
