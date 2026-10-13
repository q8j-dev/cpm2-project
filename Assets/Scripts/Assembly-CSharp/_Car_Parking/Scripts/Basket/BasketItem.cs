using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Basket
{
	public class BasketItem : MonoBehaviour
	{
		[SerializeField]
		private Image IconType;

		[SerializeField]
		private RawImage KitRawImage;

		[SerializeField]
		private TMP_Text NameText;

		[SerializeField]
		private TMP_Text PriceText;

		[field: SerializeField]
		public Button RemoveItemButton { get; private set; }

		public BasketItemInfoBase Info { get; private set; }

		public event Action<BasketItemInfoBase> OnRemoveItem;

		private void Awake()
		{
			RemoveItemButton.onClick.AddListener(() =>
			{
				OnRemoveItem?.Invoke(Info);
				base.gameObject.SetActive(value: false);
			});
		}

		public void Init(BasketItemInfoBase info)
		{
			Info = info;
		}

		public void SetPrice()
		{
			bool flag = Info is BasketItemPersonKitInfo basketItemPersonKitInfo && basketItemPersonKitInfo.PackId >= 1;
			PriceText.gameObject.SetActive(!flag);
			if (!flag)
			{
				PriceText.text = Info.Price.ToString();
			}
		}

		public void SetIcon(Sprite icon)
		{
			IconType.sprite = icon;
		}

		public void SetName(string nameKey)
		{
			NameText.text = new LocalizedString(nameKey);
		}

		public void SetTexture(Texture texture)
		{
			KitRawImage.texture = texture;
		}

		private void OnDestroy()
		{
			RemoveItemButton.onClick.RemoveAllListeners();
			OnRemoveItem = null;
		}
	}
}
