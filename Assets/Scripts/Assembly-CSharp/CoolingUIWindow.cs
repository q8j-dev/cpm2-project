using System;
using System.Collections.Generic;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

public class CoolingUIWindow : WindowScript, IClickable, IBackableWindow
{
	[SerializeField]
	private UIPartsController UIPartsController = default;

	[SerializeField]
	private PartType DefaultPartTypeInNullElements = default;

	[SerializeField]
	private EngineImageRotator DetailImageRotator = default;

	private bool _isInCar;

	private ICarInfoService _carInfoService;

	[Inject]
	public void Construct(ManagerUI managerUI, ICarInfoService carInfoService)
	{
		_managerUI = managerUI;
		_carInfoService = carInfoService;
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		_managerUI.ShowEngineUIWindow();
		Close();
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			ManagerUI managerUI = _managerUI;
			managerUI.ShowUI = (Action<bool>)Delegate.Remove(managerUI.ShowUI, new Action<bool>(ShowUI));
			base.Close();
		}
	}

	protected override void Start()
	{
		base.Start();
		ManagerUI managerUI = _managerUI;
		managerUI.ShowUI = (Action<bool>)Delegate.Combine(managerUI.ShowUI, new Action<bool>(ShowUI));
		Init();
	}

	private void Init()
	{
		CarInfo fromSlot = _carInfoService.GetFromSlot(V.SlotID);
		if (_managerUI.EditingEngineKitData == null || _managerUI.EditingEngineKitData.EngineId.InstanceID == -1)
		{
			_isInCar = false;
		}
		else
		{
			_isInCar = _managerUI.EditingEngineKitData.EngineId.ModelId == fromSlot.TuningData.EngineKitData.EngineId.ModelId;
		}
		UIPartsController.DestroyElements();
		InitBackButton();
		ParseDetails(_managerUI.EditingEngineKitData).Forget();
		if ((bool)DetailImageRotator)
		{
			_ = DetailImageRotator.Init("Intercooler_0");
		}
	}

	public override void ReloadWindow()
	{
		Init();
	}

	private async UniTaskVoid ParseDetails(EngineKitData EngineKitData)
	{
		PartData item = ((EngineKitData.Intercooler != null) ? EngineKitData.Intercooler : new PartData
		{
			ModelId = 0,
			PartType = PartType.Intercooler,
			InstanceID = -1
		});
		List<PartData> list = new List<PartData> { EngineKitData.Radiator };
		list.Add(item);
		UIPartsController.CreateElements(list, this, DefaultPartTypeInNullElements, _isInCar);
	}

	public void OnItemClick(PartData parData, int index, UIElement uiElement = null)
	{
		_managerUI.ShowMarketInventoryWindow(parData, index);
		Close();
	}

	public void CloseClickable()
	{
		Close();
	}
}
