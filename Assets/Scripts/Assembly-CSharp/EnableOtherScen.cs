using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class EnableOtherScen : MonoBehaviour
{
	private LevelsProvider _levelsProvider;

	[Inject]
	public void Construct(LevelsProvider levelsProvider)
	{
		_levelsProvider = levelsProvider;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.isTrigger)
		{
			_levelsProvider.LevelScript.OtherSceneBuildings.SetActive(value: true);
		}
	}
}
