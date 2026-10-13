using Cysharp.Threading.Tasks;
using MVPFree;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;

public class RaceIconsPresenter : IPresenter
{
	private MapIconsModel _mapIconsModel;

	private ManagerUI _managerUI;

	private IconDescriptionItemView _iconDescriptionItemView;

	private TimeAttackMapRacePopupWindow _popupWindow;

	private bool _isPopupCreating;

	[Inject]
	public RaceIconsPresenter(MapIconsModel mapIconsModel, ManagerUI managerUI)
	{
		_mapIconsModel = mapIconsModel;
		_managerUI = managerUI;
	}

	public void Subscribe()
	{
		_mapIconsModel.OnMapIconTypeSelectedAction += OnMapIconSelect;
	}

	private void OnMapIconSelect(int index, IconMapView iconMapView)
	{
		if (iconMapView.IconType == IconType.RaceZone || iconMapView.IconType == IconType.DriftZone || iconMapView.IconType == IconType.Rally)
		{
			ShowPopupAsync(index).Forget();
			PlayerPrefs.SetInt($"newRaceIcon_{index}", 1);
			iconMapView.ShowNewIcon(state: false);
		}
	}

	private async UniTaskVoid ShowPopupAsync(int index)
	{
		if (!_isPopupCreating)
		{
			_isPopupCreating = true;
			if ((bool)_popupWindow)
			{
				_popupWindow.Close();
			}
			TimeAttackMapRacePopupWindow timeAttackMapRacePopupWindow = (_popupWindow = await _managerUI.CreateWindowGeneric<TimeAttackMapRacePopupWindow>("TimeAttackMapRacePopupWindow", onTop: true));
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ShowUIMapPopupSwtch, timeAttackMapRacePopupWindow.gameObject);
			timeAttackMapRacePopupWindow.Init(index);
			_isPopupCreating = false;
		}
	}

	public void Unsubscribe()
	{
		_mapIconsModel.OnMapIconTypeSelectedAction -= OnMapIconSelect;
	}
}
