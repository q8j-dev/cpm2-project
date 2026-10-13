using UnityEngine;

namespace _Car_Parking.Scripts.Other.Accessories
{
	public class CarAccessory : MonoBehaviour
	{
		public Transform trinketSpawnPoint;

		public Transform toySpawnPoint;

		private GameObject _activeAccessory;

		public void SpawnAccessory(AccessoryType accessoryType, GameObject accessoryPrefab)
		{
			switch (accessoryType)
			{
			case AccessoryType.Trinket:
				SpawnAccessory(accessoryPrefab, trinketSpawnPoint);
				break;
			case AccessoryType.Toy:
				SpawnAccessory(accessoryPrefab, toySpawnPoint);
				break;
			default:
				Debug.LogError($"Unknown type of accessory in car: {accessoryType}. prefab: {accessoryPrefab}");
				break;
			}
		}

		public void RemoveAccessory()
		{
			if (_activeAccessory != null)
			{
				Object.Destroy(_activeAccessory);
			}
		}

		private void SpawnAccessory(GameObject accessoryPrefab, Transform spawnPoint)
		{
			RemoveAccessory();
			_activeAccessory = Object.Instantiate(accessoryPrefab, spawnPoint);
			_activeAccessory.transform.localPosition = Vector3.zero;
			_activeAccessory.transform.localRotation = Quaternion.identity;
		}
	}
}
