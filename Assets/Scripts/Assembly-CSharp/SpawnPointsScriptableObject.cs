using System.Collections.Generic;
using Service;
using Traffic;
using Treasures;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnPoints", menuName = "Scriptable/SpawnPoints", order = 1)]
public class SpawnPointsScriptableObject : ScriptableObject
{
	private enum TYPE
	{
		Traffic = 0,
		Treasures = 1,
		Service = 2
	}

	[SerializeField]
	private TYPE type;

	public List<Location> locations;

	public List<ScenePoints> peopleTrafficPoints;

	public TreasurePoints treasurePoints;
}
