using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.PlayerProperty.Trigger;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	public class BusinessRoot : MonoBehaviour
	{
		[SerializeField]
		private int Id;

		[SerializeField]
		[Space(20f)]
		private BusinessBuyTrigger BuyTrigger;

		[SerializeField]
		private ProfitCollectTrigger ProfitTrigger;

		[SerializeField]
		private ProfitControl ProfitControl;

		private FreeDriveControl _freeDriveControl;

		private BusinessModel _businessModel;

		[Inject]
		public void Construct(IBusinessService businessService, FreeDriveControl freeDriveControl)
		{
			_businessModel = businessService.GetBusinessData(Id);
			_freeDriveControl = freeDriveControl;
		}

		private void Start()
		{
			if (!(_freeDriveControl == null))
			{
				BuyTrigger.Initialize(_businessModel);
				ProfitTrigger.Initialize(_businessModel);
				ProfitControl.idOfBuilding = _businessModel.BusinessId;
			}
		}

		private void OnEnable()
		{
			if (_freeDriveControl != null)
			{
				_freeDriveControl.OnFastFoodBuildBought += OnStatusChanged;
			}
		}

		private void OnStatusChanged(int id, bool on)
		{
			if (id == Id && on)
			{
				BuyTrigger.HideTriggerVisual();
				BuyTrigger.SetEnabled(value: false);
				ProfitTrigger.gameObject.SetActive(value: true);
				ProfitTrigger.SetEnabled(value: true);
			}
		}

		private void OnDisable()
		{
			if (_freeDriveControl != null)
			{
				_freeDriveControl.OnFastFoodBuildBought -= OnStatusChanged;
			}
		}
	}
}
