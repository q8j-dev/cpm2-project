using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;

public class ParkingBuildingControl : MonoBehaviour
{
	public bool Entering;

	private LevelsProvider _levelsProvider;

	[Inject]
	public void Construct(LevelsProvider levelsProvider)
	{
		_levelsProvider = levelsProvider;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (FlagsStorage.LevelStarted && !other.isTrigger && (other.gameObject == V.Car || other.gameObject == V.DriverPerson))
		{
			_levelsProvider.LevelScript.ParkingBuilding.SetActive(Entering);
		}
	}
}
