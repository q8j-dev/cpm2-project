using System;
using System.Collections.Generic;
using MVPFree.MapIcons.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;

namespace MVPFree.MapIcons.Views
{
	public class IconsDescriptionView : MonoBehaviour
	{
		[SerializeField]
		private Image HideShowAllImage;

		[SerializeField]
		private TMP_Text HideShowAllText;

		[SerializeField]
		private ForceRebuildLayoutImmediate HideShowAllForceRebuildLayoutImmediate;

		[field: SerializeField]
		public Button HideShowAllButton { get; private set; }

		[field: SerializeField]
		public ClickDetector ClickDetector { get; private set; }

		[field: SerializeField]
		public ScrollRect ScrollView { get; private set; }

		[field: SerializeField]
		public GameObject ContentParent { get; private set; }

		[field: SerializeField]
		public Button ExpandButton { get; private set; }

		[field: SerializeField]
		public RectTransform ImageArrowRectTransform { get; private set; }

		public RectTransform ExpandButtonRect => ExpandButton.GetComponent<RectTransform>();

		public RectTransform ScrollViewRect => ScrollView.GetComponent<RectTransform>();

		public Dictionary<IconType, IconDescriptionItemView> IconDescriptionItems { get; private set; } = new Dictionary<IconType, IconDescriptionItemView>();

		public event Action<bool> OnEnabled;

		private void OnEnable()
		{
			OnEnabled?.Invoke(obj: true);
		}

		private void OnDisable()
		{
			OnEnabled?.Invoke(obj: false);
		}

		public void RebuildIconDescriptionItems()
		{
			foreach (IconDescriptionItemView value in IconDescriptionItems.Values)
			{
				if (value.TryGetComponent<RectTransform>(out var component))
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(component);
				}
			}
		}

		public void UpdateShowHideView(Sprite icon, string text)
		{
			HideShowAllImage.sprite = icon;
			HideShowAllText.text = text;
			HideShowAllForceRebuildLayoutImmediate.RebuildLayout();
		}

		public IconType GetIconType(IconDescriptionItemView iconDescriptionItemView)
		{
			foreach (KeyValuePair<IconType, IconDescriptionItemView> iconDescriptionItem in IconDescriptionItems)
			{
				if (iconDescriptionItem.Value == iconDescriptionItemView)
				{
					return iconDescriptionItem.Key;
				}
			}
			return IconType.None;
		}

		public bool HaveDescription(IconType iconType)
		{
			return IconDescriptionItems.ContainsKey(iconType);
		}
	}
}
