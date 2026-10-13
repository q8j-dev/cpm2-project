using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts
{
	public class BuyBusinessTrigger : BuyTrigger
	{
		[SerializeField]
		private int businessNumber = default;

		protected FreeDriveProvider FreeDriveProvider;

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider)
		{
			FreeDriveProvider = freeDriveProvider;
		}

		protected override void Execute()
		{
			_checkOperation?.Dispose();
			_checkOperation = null;
			FreeDriveProvider.FreeDriveControl.ShowActionButton(businessNumber, string.Concat(new LocalizedString("BuyBusinessTrigger.BuyGetProfitN"), 200000.ToString()));
		}
	}
}
