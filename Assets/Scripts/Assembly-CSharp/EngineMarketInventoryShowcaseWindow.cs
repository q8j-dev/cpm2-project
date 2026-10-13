using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

public class EngineMarketInventoryShowcaseWindow : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI MaxMeshTitleText;

	[SerializeField]
	private TextMeshProUGUI DurabilitTitleText;

	[SerializeField]
	private Image DurabilityImage;

	[SerializeField]
	private Sprite MaxMilliageSprite;

	[SerializeField]
	private Sprite DurabilitySprite;

	[SerializeField]
	private TextMeshProUGUI EngineTitleText;

	[SerializeField]
	private TextMeshProUGUI EnginePriceText;

	[SerializeField]
	private TextMeshProUGUI EngineMilliageText;

	[SerializeField]
	private TextMeshProUGUI EngineMassText;

	[SerializeField]
	private TextMeshProUGUI EngineMaxTorqueText;

	[SerializeField]
	private TextMeshProUGUI DescriptionText;

	[SerializeField]
	private Button BuyAndSetButton;

	[SerializeField]
	private Button BuyButton;

	[SerializeField]
	private Button SellButton;

	[SerializeField]
	private Button SetButton;

	[SerializeField]
	private Button ConfigurateEngineButton;

	[SerializeField]
	private Image InstalledButton;

	[SerializeField]
	[FormerlySerializedAs("DefaultEngineMarketWindow")]
	private DefaultEngineMarketWindow _marketWindow;

	public GameObject Panel;

	private AllParts _allParts;

	[Inject]
	public void Construct(AllParts allParts)
	{
		_allParts = allParts;
	}

	private void OnEnable()
	{
		SellButton.onClick.AddListener(_marketWindow.SellEngine);
		BuyAndSetButton.onClick.AddListener(_marketWindow.OnBuyAndSetClick);
		BuyButton.onClick.AddListener(_marketWindow.OnBuyClick);
		SetButton.onClick.AddListener(_marketWindow.OnSetClick);
		ConfigurateEngineButton.onClick.AddListener(_marketWindow.OnConfigurateEngine);
	}

	private void OnDisable()
	{
		SellButton.onClick.RemoveListener(_marketWindow.SellEngine);
		BuyAndSetButton.onClick.RemoveListener(_marketWindow.OnBuyAndSetClick);
		BuyButton.onClick.RemoveListener(_marketWindow.OnBuyClick);
		SetButton.onClick.RemoveListener(_marketWindow.OnSetClick);
		ConfigurateEngineButton.onClick.RemoveListener(_marketWindow.OnConfigurateEngine);
	}

	private void ResetButtonsVisibleState()
	{
		InstalledButton.gameObject.SetActive(value: false);
		BuyAndSetButton.gameObject.SetActive(value: false);
		SellButton.gameObject.SetActive(value: false);
		SetButton.gameObject.SetActive(value: false);
		ConfigurateEngineButton.SetActive(active: false);
		BuyButton.gameObject.SetActive(value: false);
	}

	public void Init(EngineKitOffer offer, bool isCompatible)
	{
		MaxMeshTitleText.text = new LocalizedString("Defaultenginesmarketwindow.maxMileage");
		DurabilitTitleText.text = new LocalizedString("Defaultenginesmarketwindow.Endurance");
		DurabilityImage.sprite = MaxMilliageSprite;
		ResetButtonsVisibleState();
		BuyAndSetButton.GetComponentInChildren<TextMeshProUGUI>().text = new LocalizedString("Defaultenginesmarketwindow.buy&set");
		EngineTitleText.text = offer.GetDisplayName();
		if ((bool)EngineMilliageText)
		{
			EngineMilliageText.text = $"{Utils.GetConvertedMileage(offer.GetTotalMileage())} {Utils.CurrentMetric}";
		}
		if ((bool)EngineMassText)
		{
			EngineMassText.text = $"{offer.GetMass():N0} {Utils.CurrentWeight}";
		}
		if ((bool)EngineMaxTorqueText)
		{
			EngineMaxTorqueText.text = "<color=#FFB900>" + offer.Base.GetMaxReliability;
		}
		if ((bool)DescriptionText)
		{
			string displayDescription = offer.GetDisplayDescription();
			DescriptionText.text = (string.IsNullOrEmpty(displayDescription) ? string.Empty : ((string)new LocalizedString(displayDescription)));
		}
		CarInfo currentCarInfo = _marketWindow.CurrentCarInfo;
		if (currentCarInfo == null)
		{
			Debug.LogError("[EngineMarketShowcase] Init called but DefaultEngineMarketWindow has no current car — InitMarket must run successfully first. Aborting.", this);
			return;
		}
		int carID = currentCarInfo.CarID;
		offer.GetFullPrice(_allParts, carID, out var total);
		total.Get(out int value);
		string text = string.Format("{0}{1}", "<color=#49CC01>", value);
		_marketWindow.SetPriceText(text);
		if ((bool)EnginePriceText)
		{
			EnginePriceText.text = text;
		}
		BuyAndSetButton.gameObject.SetActive(isCompatible);
		BuyButton.gameObject.SetActive(value: true);
		Rebuild().Forget();
	}

	public void Init(EngineKitData engineKitData, EngineItem engine, bool isCompatible, bool isInstalled = false)
	{
		MaxMeshTitleText.text = new LocalizedString("Allpartsinventorywindow.Durability");
		DurabilitTitleText.text = new LocalizedString("Defaultenginesmarketwindow.Endurance");
		DurabilityImage.sprite = DurabilitySprite;
		ResetButtonsVisibleState();
		if (isInstalled)
		{
			InstalledButton.gameObject.SetActive(value: true);
		}
		EngineTitleText.text = engineKitData.EngineId.GetName();
		float durability = engineKitData.EngineId.GetDurability();
		if ((bool)EngineMilliageText)
		{
			EngineMilliageText.text = $"{Mathf.RoundToInt(Utils.GetConvertedMileage(engineKitData.EngineId.GetMileage())):d0} {Utils.CurrentMetric}";
		}
		if ((bool)EngineMassText)
		{
			EngineMassText.text = Utils.GetMass(engineKitData.GetMass(_allParts)) ?? "";
		}
		engineKitData.GetPrice(_allParts, out var total);
		if ((bool)EnginePriceText)
		{
			EnginePriceText.text = $"{total:d0}";
		}
		_marketWindow.SetPriceText(EnginePriceText.text);
		if ((bool)EngineMaxTorqueText)
		{
			EngineMaxTorqueText.text = $"{durability:p0}";
		}
		if ((bool)DescriptionText)
		{
			DescriptionText.text = (string.IsNullOrEmpty(engine.Description) ? string.Empty : ((string)new LocalizedString(engine.Description)));
		}
		if (isInstalled)
		{
			InstalledButton.gameObject.SetActive(value: true);
		}
		else
		{
			SetButton.gameObject.SetActive(isCompatible);
			SellButton.gameObject.SetActive(value: true);
			ConfigurateEngineButton.gameObject.SetActive(value: true);
		}
		Rebuild().Forget();
	}

	private async UniTaskVoid Rebuild()
	{
		await UniTask.Yield();
		LayoutRebuilder.ForceRebuildLayoutImmediate(EnginePriceText.transform.parent.GetComponent<RectTransform>());
	}
}
