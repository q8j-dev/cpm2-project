using RacingScripts.RaceControllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

public class CircuitDisqualifiedWindow : WindowScript
{
	[SerializeField]
	private Button ExitButton = default;

	[SerializeField]
	private Button MenuButton = default;

	[SerializeField]
	private TextMeshProUGUI ReasonDisqualificationText = default;

	[SerializeField]
	private TextMeshProUGUI DisqualificationText = default;

	private CircuitRaceControlller _circuitRaceControlller;

	private RaceModeController _raceModeController;

	private LapRaceCar _ourCar;

	[Inject]
	public void Construct(RaceModeController raceModeController)
	{
		_raceModeController = raceModeController;
	}

	protected override void Start()
	{
		CacheCircuitRaceController();
		MenuButton.onClick.AddListener(OnGarage);
		ExitButton.onClick.AddListener(GoMainMenu);
		if (V.Car.TryGetComponent<LapRaceCar>(out var component))
		{
			_ourCar = component;
			DisqualificationText.text = new LocalizedString("CircuitPlayWindow.YouDisqualified");
			string text = new LocalizedString("DisqualificationType." + _ourCar.CurrentDisqualificationType);
			ReasonDisqualificationText.text = text;
		}
		base.Start();
	}

	private void CacheCircuitRaceController()
	{
		if (!_circuitRaceControlller && (bool)_raceModeController && (bool)_raceModeController.SpawnedObject)
		{
			_circuitRaceControlller = _raceModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
		}
	}

	private void OnGarage()
	{
		CacheCircuitRaceController();
		if ((bool)_circuitRaceControlller)
		{
			ResetDisqualificationType();
			if (_circuitRaceControlller != null)
			{
				_circuitRaceControlller.GoMainGarage();
			}
			Close();
		}
	}

	private void GoMainMenu()
	{
		CacheCircuitRaceController();
		if ((bool)_circuitRaceControlller)
		{
			ResetDisqualificationType();
			if (_circuitRaceControlller == null)
			{
				Close();
			}
			else
			{
				_circuitRaceControlller.GoMainMenu();
			}
		}
	}

	private void ResetDisqualificationType()
	{
		if ((bool)_ourCar)
		{
			_ourCar.CurrentDisqualificationType = DisqualificationType.None;
		}
	}
}
