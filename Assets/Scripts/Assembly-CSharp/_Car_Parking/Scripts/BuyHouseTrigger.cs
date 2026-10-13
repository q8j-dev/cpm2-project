using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts
{
	public class BuyHouseTrigger : BuyBusinessTrigger
	{
		private HomesConfig _homesConfig;

		private OptionalDependency<HomesConfig> _homesConfigDep;

		[Inject]
		public void Construct(OptionalDependency<HomesConfig> homesConfig)
		{
			_homesConfigDep = homesConfig;
		}

		private void Awake()
		{
			_homesConfig = _homesConfigDep?.Value;
		}

		protected override void Execute()
		{
			_checkOperation?.Dispose();
			_checkOperation = null;
		}
	}
}
