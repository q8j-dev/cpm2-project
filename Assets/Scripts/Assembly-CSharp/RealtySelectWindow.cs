using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.UIElement;

public class RealtySelectWindow : WindowScript
{
	[SerializeField]
	private List<RealtyPlaceUIItem> RealtyPlaceUIItems = new List<RealtyPlaceUIItem>();

	[SerializeField]
	private GameObject GarageBacklight = default;

	private ISlotService _slotService;

	[Inject]
	public void Construct(ISlotService slotService)
	{
		_slotService = slotService;
	}

	protected override void Awake()
	{
		base.Awake();
		V.CoreGameManager.ManagerUI.OnBackButtonActivated?.Invoke(obj: true);
		foreach (RealtyPlaceUIItem realtyPlaceUIItem in RealtyPlaceUIItems)
		{
			realtyPlaceUIItem.Init();
		}
		int num = ObscuredPrefs.GetInt("GarageButtonImportantClicks");
		GarageBacklight.SetActive(num == 1 && _slotService.OccupiedSlotCount == _slotService.MaxSlotCount);
	}
}
