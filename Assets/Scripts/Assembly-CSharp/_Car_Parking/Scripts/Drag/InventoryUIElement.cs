using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag
{
	public class InventoryUIElement : UIElement
	{
		[Header("Texts")]
		[SerializeField]
		private TextMeshProUGUI ItemTypeText;

		[SerializeField]
		private TextMeshProUGUI detailNameText;

		[SerializeField]
		private TextMeshProUGUI massText;

		[SerializeField]
		private TextMeshProUGUI priceText;

		[SerializeField]
		private TextMeshProUGUI milliageText;

		[SerializeField]
		private TextMeshProUGUI reliabilityText;

		[SerializeField]
		[Header("Hints")]
		private Sprite[] hintStateSprites;

		[SerializeField]
		private Image hintImage;

		[SerializeField]
		private TextMeshProUGUI hintText;

		[SerializeField]
		private Image PlusImage;

		[SerializeField]
		private CanvasGroup[] CanvasGroups;

		private Toggle _toggle;

		private Button itemButton;

		private AllParts _allParts;

		private PartData _partData;

		public int IndexForMarket;

		private IClickable _clickable;

		[Inject]
		public void Construct(AllParts allParts)
		{
			_allParts = allParts;
		}

		public override void Init(PartData part, IClickable clickable)
		{
			_partData = part;
			itemButton = GetComponent<Button>();
			_toggle = GetComponent<Toggle>();
			if ((bool)itemButton)
			{
				itemButton.onClick.RemoveAllListeners();
				itemButton.onClick.AddListener(() =>
				{
					OnButtonClicked(clickable);
				});
			}
			if ((bool)_toggle)
			{
				_clickable = clickable;
				_toggle.onValueChanged.RemoveAllListeners();
				_toggle.onValueChanged.AddListener(OnToggleClicked);
				ToggleGroup component = base.transform.parent.GetComponent<ToggleGroup>();
				if ((bool)component)
				{
					_toggle.group = component;
				}
			}
			ParseData().Forget();
			detailImage.gameObject.SetActive(part.ModelId != 0);
			if (part.ModelId == 0)
			{
				PlusImage.gameObject.SetActive(part.ModelId == 0);
				for (int num = 0; num < CanvasGroups.Length; num++)
				{
					CanvasGroups[num].alpha = 0f;
				}
			}
		}

		private void OnToggleClicked(bool state)
		{
			if (state)
			{
				_clickable.OnItemClick(_partData, IndexForMarket);
			}
		}

		private void OnButtonClicked(IClickable clickable)
		{
			clickable.OnItemClick(_partData, IndexForMarket);
		}

		private async UniTaskVoid ParseData()
		{
			if (_partData == null)
			{
				return;
			}
			PartItem part = _allParts.GetPart<PartItem>(_partData);
			if (_partData.ModelId != 0 && !part)
			{
				if ((bool)detailNameText)
				{
					detailNameText.text = "null:" + _partData.ModelId;
				}
				return;
			}
			if ((bool)ItemTypeText)
			{
				ItemTypeText.text = _partData.GetPartTypeName();
			}
			if (_partData.ModelId != 0)
			{
				if ((bool)massText)
				{
					massText.text = Utils.GetMass(part.Mass) ?? "";
				}
				if ((bool)detailNameText)
				{
					detailNameText.text = part.GetName;
				}
				if ((bool)priceText)
				{
					priceText.text = string.Format("{0}{1:d0} $", "<color=#49CC01>", part.GetPrice);
				}
				if ((bool)milliageText)
				{
					milliageText.text = $"{Utils.GetConvertedMileage(part.MaxMileage):n0} {Utils.CurrentMetric}";
				}
				if ((bool)reliabilityText)
				{
					reliabilityText.text = "<color=#FFB900>" + part.GetMaxReliability;
				}
				SetImage((int)part.TypeId);
			}
			else
			{
				if ((bool)detailNameText)
				{
					detailNameText.text = string.Empty;
				}
				if ((bool)massText)
				{
					massText.text = string.Empty;
				}
				if ((bool)priceText)
				{
					priceText.text = string.Empty;
				}
				if ((bool)milliageText)
				{
					milliageText.text = string.Empty;
				}
				if ((bool)reliabilityText)
				{
					reliabilityText.text = string.Empty;
				}
				SetImage(0);
			}
		}
	}
}
