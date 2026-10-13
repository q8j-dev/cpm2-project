using MVPFree.MapIcons.Enum;
using VContainer;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.PlayerProperty.Processor;

namespace _Car_Parking.Scripts.PlayerProperty.Trigger
{
	public class ProfitCollectTrigger : PropertyTrigger
	{
		private IBusinessService _businessService;

		private BusinessModel _businessModel;

		[Inject]
		public void Construct(IBusinessService businessService)
		{
			_businessService = businessService;
		}

		public void Initialize(BusinessModel businessModel)
		{
			_businessModel = businessModel;
			bool flag = _businessService.HasBought(businessModel.BusinessId);
			base.gameObject.SetActive(flag);
			InitializeProcessor(new ProfitCollectProcessor(_businessModel));
			SetEnabled(flag);
			if (flag)
			{
				SetMainImage(IconType.ProfitBusiness);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (_businessService != null && _businessService.HasBought(_businessModel.BusinessId))
			{
				SetMainImage(IconType.ProfitBusiness);
			}
		}
	}
}
