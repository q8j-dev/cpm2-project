using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using MVPFree.MapIcons.Enum;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem.PropertyPopupWindow
{
	public class BusinessMapPopupWindow : PropertyPopupWindow
	{
		[SerializeField]
		private TextMeshProUGUI CurrentIncomeStoredText;

		[SerializeField]
		private TextMeshProUGUI MaxIncomeStoredText;

		[SerializeField]
		private Transform StoredContainer;

		private BusinessStorage _businessStorage;

		private IBusinessService _businessService;

		private IEarnMoneyService _buyMoneyService;

		private GlobalCollectionsProvider _collectionsProvider;

		private IBusinessProfitControl _businessProfitControl;

		private int _profitControlId;

		[Inject]
		public void Construct(BusinessStorage businessStorage, IBusinessService businessService, IEarnMoneyService buyMoneyService, GlobalCollectionsProvider collectionsProvider, IBusinessProfitControl businessProfitControl)
		{
			_businessStorage = businessStorage;
			_businessService = businessService;
			_buyMoneyService = buyMoneyService;
			_collectionsProvider = collectionsProvider;
			_businessProfitControl = businessProfitControl;
		}

		protected override void Awake()
		{
			base.Awake();
			Observable.Interval(TimeSpan.FromSeconds(1.5)).Subscribe((long _) =>
			{
				IntervalUpdate();
			}).AddTo(this);
		}

		public void Initialize(int index, IconType iconType)
		{
			_profitControlId = index;
			BusinessConfig businessById = _businessStorage.GetBusinessById(index);
			MaxIncomeStoredText.text = _buyMoneyService.BusinessProfitMaxPerSession.ToString(CultureInfo.InvariantCulture);
			Init(new LocalizedString(businessById.Name.GetEnumMemberValue()), new LocalizedString(businessById.Location.GetEnumMemberValue()), iconType, businessById.Price);
			bool flag = _businessService.HasBought(index);
			UpdateOwnershipState(flag);
			UpdateIncome();
		}

		private void IntervalUpdate()
		{
			UpdateIncome();
		}

		private void UpdateIncome()
		{
			CurrentIncomeStoredText.text = _businessProfitControl.GetProfit(_profitControlId).ToString();
			if (StoredContainer is RectTransform toRebuild)
			{
				UIHelper.DelayedRebuildLayout(toRebuild).Forget();
			}
		}

		private void UpdateOwnershipState(bool isBought)
		{
			StoredContainer.gameObject.SetActive(isBought);
			base.CostContainer.gameObject.SetActive(!isBought);
		}
	}
}
