using System.Collections.Generic;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace MVPFree.MapIcons.Presenters
{
	public class CreateIconsDescriptionPresenter : IPresenter
	{
		private MapIconsDataSo _mapIconsDataSo;

		private MapIconsStarter _mapIconsStarter;

		private MapIconsModel _mapIconsModel;

		private IEventService _eventService;

		[Inject]
		public CreateIconsDescriptionPresenter(MapIconsDataSo mapIconsDataSo, MapIconsStarter mapIconsStarter, MapIconsModel mapIconsModel, IEventService eventService)
		{
			_mapIconsDataSo = mapIconsDataSo;
			_mapIconsStarter = mapIconsStarter;
			_mapIconsModel = mapIconsModel;
			_eventService = eventService;
		}

		public void Subscribe()
		{
			_mapIconsModel.Initialized += OnCreateIconsDescription;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.Initialized -= OnCreateIconsDescription;
		}

		private void OnCreateIconsDescription()
		{
			List<IconData> list = new List<IconData>();
			SortIconsToCreateDescription(_mapIconsDataSo.GetCurrentEventIcons(_eventService), list);
			SortIconsToCreateDescription(_mapIconsDataSo.MapIconsData, list);
			foreach (IconData item in list)
			{
				if (item != null && !_mapIconsDataSo.IconsToIgnoreToAddDescriptionList.Contains(item.IconType))
				{
					IconDescriptionItemView iconDescriptionItemView = Object.Instantiate(_mapIconsDataSo.UIMapDescriptionItemPrefab, _mapIconsStarter.IconsDescriptionView.ContentParent.transform);
					iconDescriptionItemView.Image.sprite = item.SpriteIcon;
					iconDescriptionItemView.LocalizedTMPText.LocalizationKey = item.LocalizeKey;
					_mapIconsStarter.IconsDescriptionView.IconDescriptionItems.Add(item.IconType, iconDescriptionItemView);
				}
			}
			WireExplicitNavigation();
		}

		private void WireExplicitNavigation()
		{
			List<Button> list = new List<Button>();
			foreach (KeyValuePair<IconType, IconDescriptionItemView> iconDescriptionItem in _mapIconsStarter.IconsDescriptionView.IconDescriptionItems)
			{
				if (!(iconDescriptionItem.Value == null))
				{
					Button mainButtonRef = iconDescriptionItem.Value.MainButtonRef;
					if (mainButtonRef != null)
					{
						list.Add(mainButtonRef);
					}
				}
			}
			int count = list.Count;
			if (count > 1)
			{
				for (int i = 0; i < count; i++)
				{
					Button button = list[i];
					Button selectOnUp = list[(i - 1 + count) % count];
					Button selectOnDown = list[(i + 1) % count];
					Navigation navigation = button.navigation;
					navigation.mode = Navigation.Mode.Explicit;
					navigation.selectOnUp = selectOnUp;
					navigation.selectOnDown = selectOnDown;
					button.navigation = navigation;
				}
			}
		}

		private void SortIconsToCreateDescription(List<IconData> icons, List<IconData> sortedIcons)
		{
			foreach (IconData icon in icons)
			{
				if (!FlagsStorage.IsMultiFree)
				{
					bool flag = false;
					foreach (IconType item in _mapIconsDataSo.IconsDescriptionsToIgnoreCreateInSinglePlayer)
					{
						if (icon.IconType == item)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
				}
				sortedIcons.Add(icon);
			}
		}
	}
}
