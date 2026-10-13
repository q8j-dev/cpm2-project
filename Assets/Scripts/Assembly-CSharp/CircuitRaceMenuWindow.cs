using RacingScripts.RaceControllers;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.SOAP_Registry;

public class CircuitRaceMenuWindow : WindowScript
{
	[SerializeField]
	private Button StartRaceButton;

	[SerializeField]
	private Button SettingsButton;

	private CircuitRaceControlller _circuitRaceControlller;

	private RaceModeController _raceModeController;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

	protected override bool ChangeRootGameInputGroup => true;

	[Inject]
	public void Construct(ManagerUI managerUI, RaceModeController raceModeController)
	{
		_managerUI = managerUI;
		_raceModeController = raceModeController;
	}

	protected override void Start()
	{
		base.Start();
		_circuitRaceControlller = _raceModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
		StartRaceButton.onClick.AddListener(StartRace);
		SettingsButton.onClick.AddListener(ShowSettingsWindow);
		FlagsStorage.IsInputEnabledInThisWindow = false;
	}

	private void StartRace()
	{
		_circuitRaceControlller.StartRaceOrQualification();
		Close();
	}

	private void ShowSettingsWindow()
	{
		_managerUI.ShowSettingsWindow();
	}
}
