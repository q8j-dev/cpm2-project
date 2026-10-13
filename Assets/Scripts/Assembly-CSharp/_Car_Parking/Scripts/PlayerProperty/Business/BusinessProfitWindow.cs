using System;
using Cysharp.Threading.Tasks;
using MVPFree.MapIcons.ScrObjs.Scripts;
using Newtonsoft.Json;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	public class BusinessProfitWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Image TypeMiniIcon = default;

		[SerializeField]
		private TextMeshProUGUI NameText = default;

		[SerializeField]
		private TextMeshProUGUI LocationText = default;

		[SerializeField]
		private TextMeshProUGUI[] CurrentIncomeTexts = default;

		[SerializeField]
		private TextMeshProUGUI MaxIncomeText = default;

		[SerializeField]
		private RectTransform MainInformationContainer = default;

		[SerializeField]
		private Button CollectButton = default;

		[SerializeField]
		private Button CancelButton = default;

		private GlobalCollectionsProvider _collectionsProvider;

		private PropertyTriggerService _propertyTriggerService;

		private MapIconsDataSo _mapIconsDataSo;

		private IBusinessProfitControl _businessProfitControl;

		private BusinessModel _businessModel;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(GlobalCollectionsProvider collectionsProvider, PropertyTriggerService propertyTriggerService, MapIconsDataSo mapIconsDataSo, IBusinessProfitControl businessProfitControl)
		{
			_collectionsProvider = collectionsProvider;
			_propertyTriggerService = propertyTriggerService;
			_mapIconsDataSo = mapIconsDataSo;
			_businessProfitControl = businessProfitControl;
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			OnCancelClicked();
		}

		protected override void Awake()
		{
			base.Awake();
			RebuildContainer();
			Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				IntervalUpdate();
			}).AddTo(this);
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			_businessModel = JsonConvert.DeserializeObject<BusinessModel>(data);
			UpdateUI();
		}

		protected override void OnEnable()
		{
			CollectButton.onClick.AddListener(OnCollectClicked);
			CancelButton.onClick.AddListener(OnCancelClicked);
			base.OnEnable();
			InitBackButton();
		}

		private void UpdateUI()
		{
			NameText.text = new LocalizedString(_businessModel.Name.GetEnumMemberValue());
			LocationText.text = new LocalizedString(_businessModel.Location.GetEnumMemberValue());
			MaxIncomeText.text = _businessModel.MaxProfitStorage.ToString();
			TypeMiniIcon.sprite = _mapIconsDataSo.GetIconData(_businessModel.IconType).SpriteIcon;
			UpdateCurrentIncome();
			RebuildContainer();
		}

		private void OnCollectClicked()
		{
			_businessProfitControl.OnMoneyCollect(_businessModel.BusinessId);
			Close();
		}

		private void IntervalUpdate()
		{
			UpdateCurrentIncome();
		}

		private void RebuildContainer()
		{
			MainInformationContainer.Rebuild().Forget();
		}

		private void UpdateCurrentIncome()
		{
			TextMeshProUGUI[] currentIncomeTexts = CurrentIncomeTexts;
			for (int i = 0; i < currentIncomeTexts.Length; i++)
			{
				currentIncomeTexts[i].text = _businessProfitControl.GetProfit(_businessModel.BusinessId).ToString();
			}
		}

		private void OnCancelClicked()
		{
			_propertyTriggerService.CreateButton().Forget();
			Close();
		}

		protected void OnDisable()
		{
			CollectButton.onClick.RemoveListener(OnCollectClicked);
			CancelButton.onClick.RemoveListener(OnCancelClicked);
		}
	}
}
