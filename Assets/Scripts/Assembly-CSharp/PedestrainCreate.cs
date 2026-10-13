using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class PedestrainCreate : MonoBehaviour
{
	public WalkPath[] walkPath;

	public StandingPeopleStreet[] talkingPeople;

	public StandingPeopleConcert standingPeopleConcert;

	private readonly List<GameObject> _createdPeople = new List<GameObject>();

	private GlobalCollectionsProvider _collectionsProvider;

	private LevelsProvider _levelsProvider;

	[Inject]
	public void Construct(GlobalCollectionsProvider collectionsProvider, LevelsProvider levelsProvider)
	{
		_levelsProvider = levelsProvider;
		_collectionsProvider = collectionsProvider;
	}

	private void Start()
	{
		WalkPath[] array = this.walkPath;
		foreach (WalkPath walkPath in array)
		{
			if ((bool)walkPath.par)
			{
				Object.DestroyImmediate(walkPath.par);
			}
			if (CPMGraphicsSettingsManager.Instance.Quality != -1)
			{
				walkPath.SpawnPeople();
			}
		}
		StandingPeopleStreet[] array2 = talkingPeople;
		foreach (StandingPeopleStreet obj in array2)
		{
			obj.RemoveButton();
			obj.PopulateButton();
			foreach (Transform item in obj.par.transform)
			{
				if ((bool)V.Car && (bool)_levelsProvider.LevelScript)
				{
					item.LookAt(_levelsProvider.CurrentLevelData.ParkingTriggerParentPos);
					Vector3 eulerAngles = item.eulerAngles;
					eulerAngles.z = 0f;
					eulerAngles.x = 0f;
					item.eulerAngles = eulerAngles;
					if (item.TryGetComponent<BipedIK>(out var component))
					{
						component.enabled = false;
						component.solvers.lookAt.IKPositionWeight = 0f;
					}
					VehicleData component2 = V.Car.transform.GetComponent<VehicleData>();
					if ((bool)component2.CarBodyReferences && (bool)component2.CarBodyReferences.Body && (bool)component)
					{
						component.solvers.lookAt.target = component2.CarBodyReferences.Body.transform;
					}
					_createdPeople.Add(item.gameObject);
					_collectionsProvider.RegisterNpc(item.gameObject);
				}
			}
		}
		if ((bool)standingPeopleConcert)
		{
			standingPeopleConcert.PopulateButton();
		}
	}

	private void OnDestroy()
	{
		foreach (GameObject createdPerson in _createdPeople)
		{
			_collectionsProvider.UnregisterNpc(createdPerson);
		}
		CommonExtensions.ClearGOCollection(_createdPeople);
	}
}
