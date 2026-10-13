using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag
{
	public class InventoryPartUIElement : UIElement
	{
		private PartData _partData;

		[SerializeField]
		private Image HintImage;

		[SerializeField]
		private TextMeshProUGUI DetailNameText;

		[SerializeField]
		private TextMeshProUGUI MassText;

		[SerializeField]
		private TextMeshProUGUI PriceText;

		[SerializeField]
		private TextMeshProUGUI MileageText;

		[SerializeField]
		private TextMeshProUGUI ReliabilityText;

		[SerializeField]
		private Sprite[] HintStateSprites;

		[SerializeField]
		private TextMeshProUGUI HintText;

		[SerializeField]
		private TextMeshProUGUI CapabilityText;

		[SerializeField]
		private Color CompatibleColor = Color.green;

		[SerializeField]
		private Color NotCompatibleColor = Color.red;

		[SerializeField]
		private Toggle ItemToggle;

		public PartType PartType;

		private AllParts _allParts;

		private ObscuredInt _currentCarId;

		private ObscuredInt _carId;

		private ObscuredInt _engineId;

		[Inject]
		public void Construct(AllParts allParts, ManagerUI managerUI)
		{
			_allParts = allParts;
			_managerUI = managerUI;
		}

		public void Init(PartData part, IClickable clickable, int carId, int engineId)
		{
			_engineId = engineId;
			_carId = carId;
			_partData = part;
			if (!ItemToggle)
			{
				ItemToggle = GetComponent<Toggle>();
			}
			ItemToggle.onValueChanged.RemoveAllListeners();
			ItemToggle.onValueChanged.AddListener((bool isOn) =>
			{
				if (isOn)
				{
					clickable.OnItemClick(_partData, 0, this);
				}
			});
			ToggleGroup component = base.transform.parent.GetComponent<ToggleGroup>();
			if ((bool)component)
			{
				ItemToggle.group = component;
			}
			ParseData().Forget();
		}

		public override void SetInstalledState(bool state)
		{
			CapabilityText.color = (state ? Color.white : CompatibleColor);
			CapabilityText.text = (state ? "Installed" : "Compatible");
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
				if ((bool)DetailNameText)
				{
					DetailNameText.text = "null";
				}
				return;
			}
			if ((bool)DetailNameText)
			{
				string getName = part.GetName;
				DetailNameText.text = getName;
			}
			if ((bool)MassText)
			{
				MassText.text = Utils.GetMass(part.Mass) ?? "";
			}
			if ((bool)PriceText)
			{
				PriceText.text = $"{part.GetPrice:d0} $";
			}
			if ((bool)MileageText)
			{
				float convertedMileage = Utils.GetConvertedMileage(_partData.GetMileage(part));
				float convertedMileage2 = Utils.GetConvertedMileage(part.MaxMileage);
				string arg = string.Format("{0:n0} " + Utils.CurrentMetric.ToUpper(), convertedMileage);
				string arg2 = string.Format("{0:n0} " + Utils.CurrentMetric.ToUpper(), convertedMileage2);
				float condition = _partData.GetCondition();
				MileageText.text = $"{arg}/{arg2}({math.round(condition * 100f):p0}";
			}
			if ((bool)ReliabilityText)
			{
				ReliabilityText.text = part.GetMaxReliability + " " + Utils.GetTorquePostfix;
			}
			if ((bool)CapabilityText)
			{
				bool num = part.CheckNotCompatibleCar(_carId);
				bool flag = part.CheckNotCompatibleEngine(_engineId);
				if (num)
				{
					CapabilityText.color = NotCompatibleColor;
					CapabilityText.text = "Not for this car.";
				}
				else if (flag)
				{
					CapabilityText.color = NotCompatibleColor;
					string text = "";
					foreach (EngineId model in _allParts.EngineIds.Models)
					{
						if (model != null && part.IsEngineLinked(model.ModelId))
						{
							text = text + model.GetName + " ";
						}
					}
					CapabilityText.text = "For engine:" + text;
				}
				else
				{
					CapabilityText.color = CompatibleColor;
					CapabilityText.text = "Compatible";
				}
			}
			SetImage((int)part.TypeId);
		}

		public override void SetImage(int partId)
		{
			detailImage.sprite = _managerUI.GetEngineIcon(partId);
			detailImage.transform.localScale = Vector3.one * 0.95f;
		}
	}
}
