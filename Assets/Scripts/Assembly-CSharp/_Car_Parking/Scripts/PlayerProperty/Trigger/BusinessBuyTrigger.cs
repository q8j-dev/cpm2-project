using VContainer;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.PlayerProperty.Processor;

namespace _Car_Parking.Scripts.PlayerProperty.Trigger
{
	public class BusinessBuyTrigger : PropertyTrigger
	{
		private IBusinessService _businessService;

		private BusinessModel _model;

		[Inject]
		public void Construct(IBusinessService businessService)
		{
			_businessService = businessService;
		}

		public void Initialize(BusinessModel model)
		{
			_model = model;
			bool flag = !_businessService.HasBought(_model.BusinessId);
			base.gameObject.SetActive(flag);
			InitializeProcessor(new BusinessTriggerProcessor(_model));
			SetEnabled(flag);
			if (flag)
			{
				SetMainImage(_model.IconType);
			}
		}
	}
}
