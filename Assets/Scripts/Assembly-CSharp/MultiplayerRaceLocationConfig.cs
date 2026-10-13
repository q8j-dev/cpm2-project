using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Racing_Scripts;

public class MultiplayerRaceLocationConfig : MonoBehaviour
{
	public DragRacingMultiplayerTree Tree;

	public V.placeToSpawn PlaceToSpawn;

	public List<DragRacingMultiplayerTree.RaceEntry> Races = new List<DragRacingMultiplayerTree.RaceEntry>();
}
