using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class SpawnPlaceItemUI : MonoBehaviour
	{
		public const string NewPlaceIconSinglePrefKey = "NewPlaceIconSingle";

		public const string NewPlaceIconMultiPrefKey = "NewPlaceIconMulti";

		public V.placeToSpawn SpawnPlace;

		[SerializeField]
		private Toggle PlaceToggle;

		[SerializeField]
		private Image NewPlaceImage;

		[SerializeField]
		private bool IsMultiplayerUIItem;

		public bool IsChosen
		{
			get
			{
				if (!PlaceToggle)
				{
					PlaceToggle = GetComponent<Toggle>();
				}
				if (!PlaceToggle)
				{
					return false;
				}
				return PlaceToggle.isOn;
			}
		}

		private void Start()
		{
			string text = (IsMultiplayerUIItem ? "NewPlaceIconMulti" : "NewPlaceIconSingle");
			if ((bool)NewPlaceImage && PlayerPrefs.GetInt(text + SpawnPlace, 0) == 1)
			{
				NewPlaceImage.gameObject.SetActive(value: false);
			}
		}

		private void OnValidate()
		{
			PlaceToggle = GetComponent<Toggle>();
		}
	}
}
