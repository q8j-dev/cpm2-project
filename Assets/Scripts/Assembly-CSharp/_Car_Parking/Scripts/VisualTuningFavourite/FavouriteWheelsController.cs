using System;
using VContainer;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	public class FavouriteWheelsController : VisualTuningFavouriteControllerBase
	{
		private AllWheelsCounterConfig _allWheelsCounterConfig;

		public event Action<bool> OnFavouriteWheelsShow;

		public event Action OnFavouriteWheelsUpdated;

		[Inject]
		public void Construct(AllWheelsCounterConfig allWheelsCounterConfig)
		{
			_allWheelsCounterConfig = allWheelsCounterConfig;
		}

		private void Start()
		{
			base.CurrentFavouriteCount = _allWheelsCounterConfig.GetFavouriteWheels().Count;
		}

		protected override string GetFavouriteKey(int id)
		{
			return "FavouriteWheel" + id;
		}

		protected override void UpdateFavourite()
		{
			OnFavouriteWheelsUpdated?.Invoke();
		}

		public override void SwitchViewContainers(bool showAllItems)
		{
			OnFavouriteWheelsShow?.Invoke(!showAllItems);
		}
	}
}
