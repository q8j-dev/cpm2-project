using CPMEngine;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

public class CarExchangeLongInfoPanel : MonoBehaviour
{
	[SerializeField]
	private CarTuningExchangeUIElement Block = default;

	[SerializeField]
	private CarTuningExchangeUIElement ConnectionRod = default;

	[SerializeField]
	private CarTuningExchangeUIElement Piston = default;

	[SerializeField]
	private CarTuningExchangeUIElement Crankshaft = default;

	[SerializeField]
	private CarTuningExchangeUIElement Flywheel = default;

	[SerializeField]
	private CarTuningExchangeUIElement RotorHousing = default;

	[SerializeField]
	private CarTuningExchangeUIElement Rotor = default;

	[SerializeField]
	private CarTuningExchangeUIElement RotorTrackingGear = default;

	[SerializeField]
	private CarTuningExchangeUIElement EccentricShaft = default;

	[SerializeField]
	private CarTuningExchangeUIElement CylinderHead = default;

	[SerializeField]
	private CarTuningExchangeUIElement IntakeCamshaft = default;

	[SerializeField]
	private CarTuningExchangeUIElement Exhausteamshaft = default;

	[SerializeField]
	private CarTuningExchangeUIElement IntakeManifold = default;

	[SerializeField]
	private CarTuningExchangeUIElement AirFilter = default;

	[SerializeField]
	private CarTuningExchangeUIElement ExhaustHeader = default;

	[SerializeField]
	private CarTuningExchangeUIElement ExhaustPipe = default;

	[SerializeField]
	private CarTuningExchangeUIElement FuelRail = default;

	[SerializeField]
	private CarTuningExchangeUIElement ThrottleValve = default;

	[SerializeField]
	private CarTuningExchangeUIElement FuelPump = default;

	[SerializeField]
	private CarTuningExchangeUIElement FuelInjector = default;

	[SerializeField]
	private CarTuningExchangeUIElement Radiator = default;

	[SerializeField]
	private CarTuningExchangeUIElement Intercooler = default;

	[SerializeField]
	private CarTuningExchangeUIElement Turbo = default;

	[SerializeField]
	private CarTuningExchangeUIElement Ecu = default;

	[SerializeField]
	private CarTuningExchangeUIElement Gearbox = default;

	[SerializeField]
	private CarTuningExchangeUIElement Clutch = default;

	[SerializeField]
	private CarTuningExchangeUIElement FrontLsd = default;

	[SerializeField]
	private CarTuningExchangeUIElement RearLsd = default;

	[SerializeField]
	private GameObject TurboPanel = default;

	[SerializeField]
	private GameObject CylinderHeaderPanel = default;

	[SerializeField]
	private TextMeshProUGUI BlockTitle = default;

	private AllParts _allParts;

	[Inject]
	public void Construct(AllParts allParts)
	{
		_allParts = allParts;
	}

	public void Init(CarTuningData carTuningData)
	{
		if (carTuningData.EngineKitData.RotorHousing != null)
		{
			RotorHousing.Init(carTuningData.EngineKitData.RotorHousing);
			if (carTuningData.EngineKitData.Rotors != null)
			{
				Rotor.Init(carTuningData.EngineKitData.Rotors);
			}
			if (carTuningData.EngineKitData.RotorTrackingGear != null)
			{
				RotorTrackingGear.Init(carTuningData.EngineKitData.RotorTrackingGear);
			}
			if (carTuningData.EngineKitData.EccentricShaft != null)
			{
				EccentricShaft.Init(carTuningData.EngineKitData.EccentricShaft);
			}
			if (carTuningData.EngineKitData.Flywheel != null)
			{
				Flywheel.Init(carTuningData.EngineKitData.Flywheel);
			}
			Block.transform.parent.gameObject.SetActive(value: false);
			ConnectionRod.transform.parent.gameObject.SetActive(value: false);
			Piston.transform.parent.gameObject.SetActive(value: false);
			Crankshaft.transform.parent.gameObject.SetActive(value: false);
			CylinderHeaderPanel.SetActive(value: false);
			BlockTitle.text = new LocalizedString("PartType.RotorHousing");
		}
		else
		{
			if (carTuningData.EngineKitData.Block != null)
			{
				Block.Init(carTuningData.EngineKitData.Block);
			}
			if (carTuningData.EngineKitData.ConnectionRod != null)
			{
				ConnectionRod.Init(carTuningData.EngineKitData.ConnectionRod);
			}
			if (carTuningData.EngineKitData.Pistons != null)
			{
				Piston.Init(carTuningData.EngineKitData.Pistons);
			}
			if (carTuningData.EngineKitData.Crankshaft != null)
			{
				Crankshaft.Init(carTuningData.EngineKitData.Crankshaft);
			}
			if (carTuningData.EngineKitData.Flywheel != null)
			{
				Flywheel.Init(carTuningData.EngineKitData.Flywheel);
			}
			if (carTuningData.EngineKitData.CylinderHead != null)
			{
				CylinderHead.Init(carTuningData.EngineKitData.CylinderHead);
			}
			if (carTuningData.EngineKitData.Camshaft1 != null)
			{
				IntakeCamshaft.Init(carTuningData.EngineKitData.Camshaft1);
			}
			if (carTuningData.EngineKitData.Camshaft2 != null)
			{
				Exhausteamshaft.Init(carTuningData.EngineKitData.Camshaft2);
			}
			BlockTitle.text = new LocalizedString("Buythiscarwindow.BlockInline");
			RotorHousing.transform.parent.gameObject.SetActive(value: false);
			Rotor.transform.parent.gameObject.SetActive(value: false);
			RotorTrackingGear.transform.parent.gameObject.SetActive(value: false);
			EccentricShaft.transform.parent.gameObject.SetActive(value: false);
		}
		if (carTuningData.EngineKitData.IntakeManifold != null)
		{
			IntakeManifold.Init(carTuningData.EngineKitData.IntakeManifold);
		}
		if (carTuningData.EngineKitData.AirFilter != null)
		{
			AirFilter.Init(carTuningData.EngineKitData.AirFilter);
		}
		if (carTuningData.EngineKitData.ExhaustHeader != null)
		{
			ExhaustHeader.Init(carTuningData.EngineKitData.ExhaustHeader);
		}
		if (carTuningData.EngineKitData.ExhaustPipe != null)
		{
			ExhaustPipe.Init(carTuningData.EngineKitData.ExhaustPipe);
		}
		if (carTuningData.EngineKitData.FuelRail != null)
		{
			FuelRail.Init(carTuningData.EngineKitData.FuelRail);
		}
		if (carTuningData.EngineKitData.ThrottleValve != null)
		{
			ThrottleValve.Init(carTuningData.EngineKitData.ThrottleValve);
		}
		if (carTuningData.EngineKitData.FuelPump != null)
		{
			FuelPump.Init(carTuningData.EngineKitData.FuelPump);
		}
		if (carTuningData.EngineKitData.FuelInjectors != null)
		{
			FuelInjector.Init(carTuningData.EngineKitData.FuelInjectors);
		}
		if (carTuningData.EngineKitData.Radiator != null)
		{
			Radiator.Init(carTuningData.EngineKitData.Radiator);
		}
		if (carTuningData.EngineKitData.Intercooler != null)
		{
			Intercooler.Init(carTuningData.EngineKitData.Intercooler);
		}
		if (carTuningData.EngineKitData.Charger != null)
		{
			Turbo.Init(carTuningData.EngineKitData.Charger);
		}
		else
		{
			TurboPanel.SetActive(value: false);
		}
		if (carTuningData.ECU != null)
		{
			Ecu.Init(carTuningData.ECU);
		}
		if (carTuningData.EngineKitData.Clutch != null)
		{
			Clutch.Init(carTuningData.EngineKitData.Clutch);
		}
		GearboxType gearboxType = GearboxType.Gear0Wd;
		if (carTuningData.GearBox != null)
		{
			Gearbox.Init(carTuningData.GearBox);
			GearBoxPart part = _allParts.GetPart<GearBoxPart>(carTuningData.GearBox.CheckPartData());
			gearboxType = (GearboxType)new GearBoxInstance((GearBoxPart)carTuningData.GearBox.GetPart(_allParts))
			{
				Part = part,
				Mileage = carTuningData.GearBox.GetMileage(part)
			}.Part.GetParams.Type;
		}
		if (carTuningData.FrontLSD != null)
		{
			FrontLsd.Init(carTuningData.FrontLSD);
		}
		if (carTuningData.RearLSD != null)
		{
			RearLsd.Init(carTuningData.RearLSD);
		}
		FrontLsd.transform.parent.gameObject.SetActive(value: false);
		RearLsd.transform.parent.gameObject.SetActive(value: false);
		switch (gearboxType)
		{
		case GearboxType.GearFwd:
			FrontLsd.transform.parent.gameObject.SetActive(value: true);
			break;
		case GearboxType.GearRwd:
		case GearboxType.Gear4Wdr2:
			RearLsd.transform.parent.gameObject.SetActive(value: true);
			break;
		case GearboxType.GearAwd:
		case GearboxType.Gear4Wd:
		case GearboxType.Gear4Wdr1:
		case GearboxType.Gear6Wd:
		case GearboxType.Gear6Wdr2:
		case GearboxType.Gear8Wd:
			FrontLsd.transform.parent.gameObject.SetActive(value: true);
			RearLsd.transform.parent.gameObject.SetActive(value: true);
			break;
		}
	}
}
