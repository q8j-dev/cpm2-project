using System;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.FirstGameSetup.Presets
{
	public class PersonPresetToggle : MonoBehaviour
	{
		private CharacterEquipmentPreset _preset;

		[field: SerializeField]
		public Image TypeIcon { get; private set; }

		[field: SerializeField]
		public Image Icon { get; private set; }

		[field: SerializeField]
		public int Gender { get; private set; }

		[field: SerializeField]
		public Image Back { get; private set; }

		[field: SerializeField]
		public Sprite BackSelectedSprite { get; private set; }

		[field: SerializeField]
		public Sprite BackDeselectedSprite { get; private set; }

		[field: SerializeField]
		public Toggle Toggle { get; private set; }

		[field: SerializeField]
		public ClickDetector ClickDetector { get; private set; }

		public event Action<CharacterEquipmentPreset> OnSelected;

		public void Init(CharacterEquipmentPreset preset, ToggleGroup toggleGroup)
		{
			_preset = preset;
			TypeIcon.sprite = preset.TypeIcon;
			Icon.sprite = preset.Icon;
			Toggle.group = toggleGroup;
			Toggle.onValueChanged.AddListener(OnValueChanged);
			Gender = preset.Gender;
		}

		private void OnValueChanged(bool value)
		{
			if (value)
			{
				UpdateVisible(selected: true);
				OnSelected?.Invoke(_preset);
			}
			else
			{
				UpdateVisible(selected: false);
			}
		}

		public void UpdateVisible(bool selected)
		{
			if (selected)
			{
				Back.sprite = BackSelectedSprite;
				TypeIcon.color = Color.black;
			}
			else
			{
				Back.sprite = BackDeselectedSprite;
				TypeIcon.color = Color.white;
			}
		}

		private void OnDestroy()
		{
			Toggle.onValueChanged.RemoveAllListeners();
		}
	}
}
