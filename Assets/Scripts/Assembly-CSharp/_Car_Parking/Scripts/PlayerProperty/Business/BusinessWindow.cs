using System.Globalization;
using Newtonsoft.Json;
using Services.PriceStyleService;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	public class BusinessWindow : ConfirmBuyWindowBase
	{
		[SerializeField]
		private TextMeshProUGUI IncomeText;

		[SerializeField]
		private TextMeshProUGUI LocationText;

		private FreeDriveControl _freeDriveControl;

		private PropertyTriggerService _propertyTriggerService;

		private PriceStyleConfig _priceStyleConfig;

		private BusinessStorage _businessStorage;

		private BusinessModel _businessModel;

		[Inject]
		public void Construct(FreeDriveControl freeDriveControl, PropertyTriggerService propertyTriggerService, PriceStyleConfig priceStyleConfig, BusinessStorage businessStorage)
		{
			_freeDriveControl = freeDriveControl;
			_propertyTriggerService = propertyTriggerService;
			_priceStyleConfig = priceStyleConfig;
			_businessStorage = businessStorage;
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			_businessModel = JsonConvert.DeserializeObject<BusinessModel>(data);
			InitUI();
		}

		protected override void OnBuy()
		{
			if (!_freeDriveControl.BuyFastFoodBuilding(_businessModel.BusinessId))
			{
				_propertyTriggerService.CreateButton().Forget();
			}
			Close();
		}

		protected override void OnEnable()
		{
			_managerUI.ShowUserBalanceOnlyWindow();
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			_managerUI.CloseBalanceOnlyWindow();
		}

		private void InitUI()
		{
			BusinessConfig businessById = _businessStorage.GetBusinessById(_businessModel.BusinessId);
			SetMainView(businessById.MainIconAddressablePath, new LocalizedString(_businessModel.Name.GetEnumMemberValue()));
			LocationText.text = new LocalizedString(businessById.Location.GetEnumMemberValue());
			PriceStyleData byType = _priceStyleConfig.GetByType(_businessModel.PriceModel.PriceType);
			SetPrice(byType.PriceSprite, _businessModel.PriceModel.GetPrice.ToString());
			IncomeText.text = _businessModel.ProfitPerSecond.ToString(CultureInfo.InvariantCulture);
		}

		protected override void OnCancel()
		{
			base.OnCancel();
			_propertyTriggerService.CreateButton().Forget();
			Close();
		}
	}
}
