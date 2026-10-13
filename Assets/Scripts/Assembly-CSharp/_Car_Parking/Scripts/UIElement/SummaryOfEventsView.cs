using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.UIElement
{
	public class SummaryOfEventsView : MonoBehaviour
	{
		private IOfferSystem _offerSystem;

		[Inject]
		public void Construct(IOfferSystem offerSystem)
		{
			_offerSystem = offerSystem;
		}

		private void OnEnable()
		{
			_offerSystem.OnDataUpdatedEvent += OffersDataUpdatedCallback;
		}

		private void OnDisable()
		{
			_offerSystem.OnDataUpdatedEvent -= OffersDataUpdatedCallback;
		}

		private void OffersDataUpdatedCallback()
		{
		}
	}
}
