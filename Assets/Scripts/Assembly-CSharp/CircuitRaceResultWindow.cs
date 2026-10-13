using System.Collections.Generic;
using System.Linq;
using RacingScripts.RaceControllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.UIElement;

public class CircuitRaceResultWindow : WindowScript
{
	[Header("Buttons")]
	[SerializeField]
	private Button ExitButton;

	[SerializeField]
	private Transform MembersParent = default;

	[SerializeField]
	private LapRaceLeaderbordUiElement LeaderbordUiElementPrefab = default;

	[Header("Status")]
	[SerializeField]
	private TextMeshProUGUI StatusText;

	[SerializeField]
	[Header("Reward")]
	private Sprite MoneySprite;

	[SerializeField]
	private CircuitRewardUiElement[] RewardUiElements = default;

	[SerializeField]
	private TextMeshProUGUI TitleText = default;

	private List<LapRaceLeaderbordUiElement> LeaderbordUiElements = new List<LapRaceLeaderbordUiElement>();

	private IPrefabFactory _prefabFactory;

	private RaceModeController _raceModeController;

	private CircuitRaceControlller _circuitRaceControlller;

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, RaceModeController raceModeController)
	{
		_prefabFactory = prefabFactory;
		_raceModeController = raceModeController;
	}

	protected override void Start()
	{
		_circuitRaceControlller = _raceModeController.SpawnedObject.GetComponent<CircuitRaceControlller>();
	}

	public void Init(List<LapRaceCar> lapRaceCars, int[] rewards)
	{
		TitleText.text = _circuitRaceControlller.GetRaceMapModel().RaceConfig.TrackName + "/Results";
		InitRewards(rewards);
		CreateLeaderbordUiElements(lapRaceCars);
	}

	public void InitRewards(int[] rewards)
	{
		rewards = rewards.Reverse().ToArray();
		for (int i = 0; i < rewards.Length; i++)
		{
			RewardUiElements[i].Init(rewards[i]);
		}
	}

	private void CreateLeaderbordUiElements(List<LapRaceCar> lapRaceCars)
	{
		for (int i = 0; i < lapRaceCars.Count; i++)
		{
			LapRaceLeaderbordUiElement item = _prefabFactory.Instantiate(LeaderbordUiElementPrefab, MembersParent);
			LeaderbordUiElements.Add(item);
			if (lapRaceCars[i].IsUser())
			{
				ParseUserProgress(lapRaceCars[i]);
			}
		}
		_ = UIHelper.DelayedRebuildLayout(MembersParent.GetComponent<RectTransform>());
	}

	private void ParseUserProgress(LapRaceCar lapRaceCar)
	{
	}
}
