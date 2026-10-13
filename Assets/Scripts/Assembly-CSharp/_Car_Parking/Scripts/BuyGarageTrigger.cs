using CPM.Scripts.Garage.GarageBuyer;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts
{
	public class BuyGarageTrigger : BuyTrigger
	{
		[SerializeField]
		private int GarageIndex = default;

		private IGarageBuyService _garageBuyService;

		[Inject]
		public void Construct(IGarageBuyService garageBuyService)
		{
			_garageBuyService = garageBuyService;
		}

		private void OnEnable()
		{
			if (_garageBuyService.GetGarageStatus(GarageIndex))
			{
				base.gameObject.SetActive(value: false);
			}
			else
			{
				_garageBuyService.GarageBought += OnGarageBuy;
			}
		}

		private void OnDisable()
		{
			_garageBuyService.GarageBought -= OnGarageBuy;
		}

		private void OnTriggerExit(Collider other)
		{
			V.CoreGameManager.ManagerUI.CloseConfirmGarageBuyWindow();
		}

		protected override void Execute()
		{
			_checkOperation?.Dispose();
			_checkOperation = null;
			V.CoreGameManager.ManagerUI.ShowConfirmGarageBuyWindow(new ConfirmGarageBuyWindow.AdditionalData
			{
				GarageIndex = GarageIndex
			});
		}

		private void OnGarageBuy(int garageIndex)
		{
			if (garageIndex == GarageIndex)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
