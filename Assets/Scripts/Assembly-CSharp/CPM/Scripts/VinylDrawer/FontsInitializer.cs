using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CPM.Scripts.VinylDrawer
{
	public class FontsInitializer : Dropdown
	{
		[SerializeField]
		private List<FontSelect> fonts = new List<FontSelect>();

		private int index;

		protected override void Awake()
		{
			base.Awake();
			PopulateDropdown();
			base.onValueChanged.AddListener(UpdateDropdownItemFonts);
		}

		public Font GetFont(int i)
		{
			return fonts[i].font;
		}

		private void PopulateDropdown()
		{
			ClearOptions();
			List<string> list = fonts.ConvertAll((FontSelect x) => x.fontName);
			AddOptions(list);
		}

		protected override DropdownItem CreateItem(DropdownItem itemTemplate)
		{
			DropdownItem dropdownItem = base.CreateItem(itemTemplate);
			dropdownItem.text.font = fonts[index++].font;
			return dropdownItem;
		}

		protected override void DestroyDropdownList(GameObject dropdownList)
		{
			base.DestroyDropdownList(dropdownList);
			index = 0;
		}

		public void UpdateDropdownItemFonts(int i)
		{
			base.captionText.font = fonts[i].font;
		}
	}
}
