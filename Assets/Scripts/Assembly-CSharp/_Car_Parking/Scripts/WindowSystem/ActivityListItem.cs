using System;
using MVPFree.MapIcons.ScrObjs.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.GameActivity;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ActivityListItem : MonoBehaviour
	{
		[SerializeField]
		private Button Button = default;

		[SerializeField]
		private Image Icon = default;

		[SerializeField]
		private TMP_Text Title = default;

		[SerializeField]
		private TMP_Text Description = default;

		private MapIconsDataSo _icons;

		private GameActivityEntry _entry;

		public event Action<GameActivityEntry> OnButtonClick;

		[Inject]
		public void Construct(MapIconsDataSo icons)
		{
			_icons = icons;
		}

		private void OnEnable()
		{
			Button.onClick.AddListener(OnButtonClicked);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(OnButtonClicked);
		}

		private void OnDestroy()
		{
			OnButtonClick = null;
		}

		public void Render(GameActivityEntry entry)
		{
			_entry = entry;
			if ((bool)Title)
			{
				Title.text = entry.Name;
			}
			if ((bool)Description)
			{
				Description.text = entry.Description;
			}
			if ((bool)Icon)
			{
				if ((bool)_icons)
				{
					_icons.Init();
				}
				IconData obj = (_icons ? _icons.GetIconData(entry.IconType) : null);
				Icon.sprite = obj?.SpriteIcon;
			}
		}

		private void OnButtonClicked()
		{
			OnButtonClick?.Invoke(_entry);
		}
	}
}
