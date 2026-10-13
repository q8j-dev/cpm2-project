using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

public class ShowcaseUIItem : UIElement
{
	private PartData _partData;

	[SerializeField]
	private TextMeshProUGUI DetailNameText = default;

	[SerializeField]
	private TextMeshProUGUI MileageText = default;

	[SerializeField]
	private TextMeshProUGUI MassText = default;

	[SerializeField]
	private TextMeshProUGUI PriceText = default;

	[SerializeField]
	private TextMeshProUGUI ReliabilityText = default;

	[SerializeField]
	private TextMeshProUGUI DescriptionText = default;

	[SerializeField]
	private Button closeButton = default;

	private AllParts _allParts;

	[Inject]
	public void Construct(AllParts allParts)
	{
		_allParts = allParts;
	}

	protected override void Awake()
	{
		if ((bool)closeButton)
		{
			closeButton.onClick.AddListener(() =>
			{
				base.gameObject.SetActive(value: false);
			});
		}
		if (TryGetComponent<Button>(out var component))
		{
			component.onClick.AddListener(() =>
			{
				base.gameObject.SetActive(value: false);
			});
		}
		base.Awake();
	}

	public override void Init(PartData part, IClickable clickable)
	{
		_partData = part;
		_ = ParseData();
	}

	private async UniTaskVoid ParseData()
	{
		PartItem part = _allParts.GetPart<PartItem>(_partData);
		if ((bool)DetailNameText)
		{
			DetailNameText.text = part.GetName;
		}
		if ((bool)MileageText)
		{
			MileageText.text = $"{Utils.GetConvertedMileage(part.MaxMileage):n0} {Utils.CurrentMetric}";
		}
		if ((bool)MassText)
		{
			MassText.text = Utils.GetMass(part.Mass);
		}
		if ((bool)PriceText)
		{
			PriceText.text = string.Format("{0}{1:d0} $", "<color=#49CC01>", part.GetPrice);
		}
		if ((bool)ReliabilityText)
		{
			ReliabilityText.text = "<color=#FFB900>" + part.GetMaxReliability;
		}
		if ((bool)DescriptionText)
		{
			DescriptionText.text = new LocalizedString(part.Description);
		}
		SetImage((int)_partData.PartType);
	}

	public override void OnClickItem()
	{
	}

	public void Clear()
	{
		if ((bool)DetailNameText)
		{
			DetailNameText.text = string.Empty;
		}
		if ((bool)MileageText)
		{
			MileageText.text = string.Empty;
		}
		if ((bool)MassText)
		{
			MassText.text = string.Empty;
		}
		if ((bool)PriceText)
		{
			PriceText.text = string.Empty;
		}
		if ((bool)DescriptionText)
		{
			DescriptionText.text = string.Empty;
		}
	}

	public void EnableAlphaCanvasGroup(bool enable)
	{
		if (_canvasGroup == null)
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}
		if (enable)
		{
			_canvasGroup.alpha = 1f;
			_canvasGroup.blocksRaycasts = true;
		}
		else
		{
			_canvasGroup.alpha = 0f;
			_canvasGroup.blocksRaycasts = false;
		}
	}
}
