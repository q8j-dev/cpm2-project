using System.Collections.Generic;
using System.Linq;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using CodeStage.AntiCheat.Storage;
using DG.Tweening;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class GarageSelectWindow : PropertyWindowBase<GaragePositionsConfig, GarageData>
	{
		[SerializeField]
		private List<GaragePlaceUIItem> GaragePlaceUIItems = default;

		private List<PlaceUIItem<GaragePositionsConfig, GarageData>> _cachedPlaceItems;

		private IGarageSelectorService _garageSelectorService;

		private IGarageBuyService _garageBuyService;

		private ISlotService _slotService;

		protected override List<PlaceUIItem<GaragePositionsConfig, GarageData>> PlaceUIItems
		{
			get
			{
				if (_cachedPlaceItems == null)
				{
					_cachedPlaceItems = GaragePlaceUIItems.Cast<PlaceUIItem<GaragePositionsConfig, GarageData>>().ToList();
				}
				return _cachedPlaceItems;
			}
		}

		private GaragePlaceUIItem CurrentGarage => GaragePlaceUIItems[base.SelectedItem];

		[Inject]
		public void Construct(IGarageSelectorService garageSelectorService, ProgressContainer progressContainer, IGarageBuyService garageBuyService, ISlotService selectedSlotService)
		{
			_garageSelectorService = garageSelectorService;
			_garageBuyService = garageBuyService;
			_slotService = selectedSlotService;
		}

		protected override void Init()
		{
			base.Init();
			int num = _garageBuyService.LastBuyingGarageId;
			if (ObscuredPrefs.GetInt("GarageButtonImportantClicks") == 0)
			{
				ObscuredPrefs.SetInt("GarageButtonImportantClicks", 1);
			}
			else if (ObscuredPrefs.GetInt("GarageButtonImportantClicks") == 1 && _slotService.OccupiedSlotCount == _slotService.MaxSlotCount)
			{
				ObscuredPrefs.SetInt("GarageButtonImportantClicks", 2);
			}
			if (num == -1)
			{
				num = _garageSelectorService.CurrentGarageIndex;
			}
			GaragePlaceUIItems[num].IsChosen = true;
			ChooseItem(num, withAnimation: false);
		}

		protected override void UpdateButton(bool animate = true)
		{
			if (CurrentGarage.IsBought)
			{
				SetButtonsVisibility(state: true, animate);
				SelectButton.interactable = _garageSelectorService.CurrentGarageIndex != CurrentGarage.DataItem.Index;
				SelectCanvasGroup.DOFade((_garageSelectorService.CurrentGarageIndex != CurrentGarage.DataItem.Index) ? 1f : 0.2f, 0.3f * (float)(animate ? 1 : 0));
				if (SelectButton.interactable)
				{
					CurrentGarage.SetNavigationDown(SelectButton);
				}
			}
			else
			{
				SetButtonsVisibility(state: false, animate);
				CurrentGarage.SetNavigationDown(BuyButton);
			}
		}

		protected override void OnSelectItem()
		{
			if (CurrentGarage.IsBought)
			{
				_garageSelectorService.ChangeMenuGarage(CurrentGarage.DataItem.Index, withScreen: true);
				V.CoreGameManager.ManagerUI.CloseWindow("PropertyWindow");
			}
		}

		protected override void OnBuyItem()
		{
			V.CoreGameManager.ManagerUI.ShowConfirmGarageBuyWindow(new ConfirmGarageBuyWindow.AdditionalData
			{
				GarageIndex = CurrentGarage.DataItem.Index
			});
		}
	}
}
