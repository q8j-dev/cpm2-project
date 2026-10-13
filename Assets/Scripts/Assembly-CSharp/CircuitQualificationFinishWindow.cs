using System;
using RacingScripts.RaceControllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

public class CircuitQualificationFinishWindow : WindowScript
{
	[SerializeField]
	private TextMeshProUGUI resultText;

	[SerializeField]
	private Button ExitButton;

	[SerializeField]
	private Button ChangeButton;

	[SerializeField]
	private Button RestartButton;

	private CircuitRaceControlller _circuitRaceControlller;

	private RaceModeController _racingModeController;

	[Inject]
	public void Construct(RaceModeController raceModeController)
	{
		_racingModeController = raceModeController;
	}

	protected override void Start()
	{
		base.Start();
		_circuitRaceControlller = _racingModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
		ExitButton.onClick.AddListener(Exit);
		RestartButton.onClick.AddListener(Restart);
		ChangeButton.onClick.AddListener(ChangeMap);
		Init();
	}

	private void Init()
	{
		LapRaceCar userLapRaceCar = _circuitRaceControlller.circuitCarsControl.GetUserLapRaceCar();
		resultText.text = string.Format(new LocalizedString("QualificationWindow.Result"), MathF.Round(userLapRaceCar.GetClearTime(), 1), MathF.Round(userLapRaceCar.GetPenaltyTime(), 1), MathF.Round(userLapRaceCar.GetTotalTime(), 1));
	}

	private void Exit()
	{
		_circuitRaceControlller.GoMainMenu();
		Close();
	}

	private void Restart()
	{
		_circuitRaceControlller.RestartRace();
		_managerUI.ShowCircuitPlayWindow();
		Close();
	}

	private void ChangeMap()
	{
		_managerUI.ShowCircuitMapWindow();
		Close();
	}
}
