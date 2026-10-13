using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuCategoryItem : MonoBehaviour
{
	[SerializeField]
	private ClothItemCategory[] ClothItemCategories;

	[SerializeField]
	[Space(15f)]
	private Image BackImage;

	[SerializeField]
	private Sprite DeselectedSprite;

	[SerializeField]
	private Sprite SelectedSprite;

	public Toggle Toggle;

	[field: SerializeField]
	public PersonWindowCategoryType PersonWindowCategory { get; private set; }

	public event Action<PersonWindowCategoryType> OnSelected;

	public void Init()
	{
		Toggle.onValueChanged.AddListener(OnValueChanged);
	}

	public void OnValueChanged(bool state)
	{
		if (state)
		{
			OnSelected?.Invoke(PersonWindowCategory);
			if (ClothItemCategories.Length != 0)
			{
				if (ClothItemCategories[0].Toggle.isOn)
				{
					ClothItemCategories[0].OnSelected(state: true);
				}
				else
				{
					ClothItemCategories[0].Toggle.isOn = true;
				}
			}
			BackImage.sprite = SelectedSprite;
		}
		else
		{
			BackImage.sprite = DeselectedSprite;
		}
	}

	public void HideItemCategory(int index, bool state = false)
	{
		ClothItemCategories[index].gameObject.SetActive(state);
	}

	private void OnDestroy()
	{
		OnSelected = null;
		Toggle.onValueChanged.RemoveAllListeners();
	}
}
