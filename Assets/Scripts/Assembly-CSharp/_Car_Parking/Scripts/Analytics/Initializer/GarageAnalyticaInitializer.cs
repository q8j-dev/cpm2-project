using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.Analytics.Initializer
{
	public class GarageAnalyticaInitializer : MonoBehaviour
	{
		private ICharacterEquipmentDataService _characterEquipmentDataService;

		[Inject]
		public void Construct(ICharacterEquipmentDataService characterEquipmentDataService)
		{
			_characterEquipmentDataService = characterEquipmentDataService;
		}

		public void Start()
		{
			AnalyticsDependencies.RegisterDependence(_characterEquipmentDataService);
		}

		private void OnDestroy()
		{
			AnalyticsDependencies.Unregister(_characterEquipmentDataService);
		}
	}
}
