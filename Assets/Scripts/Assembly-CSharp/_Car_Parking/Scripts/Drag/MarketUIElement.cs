using CPMEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag
{
	public class MarketUIElement : UIElement
	{
		private PartData _partData;

		[SerializeField]
		private TextMeshProUGUI detailNameText = default;

		[SerializeField]
		private TextMeshProUGUI massText = default;

		[SerializeField]
		private TextMeshProUGUI priceText = default;

		[SerializeField]
		private TextMeshProUGUI milliageText = default;

		public PartType PartType;

		private Button itemButton;

		private AllParts _allParts;

		[Inject]
		public void Construct(AllParts allParts)
		{
			_allParts = allParts;
		}

		public override void Init(PartData part, IClickable clickable)
		{
			_partData = part;
			itemButton = GetComponent<Button>();
			itemButton.onClick.RemoveAllListeners();
			itemButton.onClick.AddListener(() =>
			{
				clickable.OnItemClick(_partData);
			});
			ParseData().Forget();
		}

		private async UniTaskVoid ParseData()
		{
			if (_partData == null)
			{
				return;
			}
			PartItem part = _allParts.GetPart<PartItem>(_partData);
			if (part == null)
			{
				if ((bool)detailNameText)
				{
					detailNameText.text = "null";
				}
				return;
			}
			if ((bool)detailNameText)
			{
				string getName = part.GetName;
				detailNameText.text = getName;
			}
			if ((bool)massText)
			{
				massText.text = Utils.GetMass(part.Mass);
			}
			if ((bool)priceText)
			{
				priceText.text = $"{part.GetPrice:d0}";
			}
			if ((bool)milliageText)
			{
				milliageText.text = $"{Utils.GetConvertedMileage(part.MaxMileage):n0} {Utils.CurrentMetric}";
			}
			SetImage((int)part.TypeId);
		}
	}
}
