using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Free_drive
{
	public class SpawnPointsContainer : MonoBehaviour
	{
		public Transform[] spawnPointsCity;

		public Transform[] spawnPointsCity2;

		public Transform[] spawnPointsHighway;

		public Transform[] spawnPointsDesert;

		public Transform[] spawnPointsMountain;

		public Transform[] spawnPointsRacing;

		public Transform[] spawnPointsOffrad;

		public Transform[] spawnPointsJapan;

		public Transform[] spawnPointsJapanSuburban;

		public Transform[] spawnPointsIsland;

		public Transform[] spawnPointsTropicalIsland;

		public Transform[] spawnPointsNurburgring;

		public Transform[] spawnPointsItaly;

		private Dictionary<V.placeToSpawn, Transform[]> _spawnPoints = new Dictionary<V.placeToSpawn, Transform[]>();

		private Dictionary<V.placeToSpawn, int> _spawnPointsIndexArrayPosition = new Dictionary<V.placeToSpawn, int>();

		public (Transform[] points, int IndexArrayPostionCar) GetSpawnPoints(V.placeToSpawn placeToSpawn)
		{
			if (_spawnPoints.TryGetValue(placeToSpawn, out var value))
			{
				if (_spawnPointsIndexArrayPosition.TryGetValue(placeToSpawn, out var value2))
				{
					return (points: value, IndexArrayPostionCar: value2);
				}
				Debug.LogError("Can't find index array position for " + placeToSpawn);
			}
			else
			{
				Debug.LogError("Can't find spawn points for " + placeToSpawn);
			}
			return (points: spawnPointsCity, IndexArrayPostionCar: 860);
		}

		private void Awake()
		{
			_spawnPointsIndexArrayPosition = new Dictionary<V.placeToSpawn, int>
			{
				{
					V.placeToSpawn.MainCity,
					860
				},
				{
					V.placeToSpawn.Highway,
					358
				},
				{
					V.placeToSpawn.Desert,
					133
				},
				{
					V.placeToSpawn.Mountain,
					1102
				},
				{
					V.placeToSpawn.Racing,
					200
				},
				{
					V.placeToSpawn.City2,
					860
				},
				{
					V.placeToSpawn.Offroad,
					133
				},
				{
					V.placeToSpawn.JapanSerpentine,
					133
				},
				{
					V.placeToSpawn.JapanSuburban,
					133
				},
				{
					V.placeToSpawn.GarageEntrance,
					860
				},
				{
					V.placeToSpawn.Island,
					860
				},
				{
					V.placeToSpawn.TropicalIsland,
					860
				},
				{
					V.placeToSpawn.Nurburgring,
					860
				},
				{
					V.placeToSpawn.Italy,
					860
				}
			};
			_spawnPoints = new Dictionary<V.placeToSpawn, Transform[]>
			{
				{
					V.placeToSpawn.MainCity,
					spawnPointsCity
				},
				{
					V.placeToSpawn.Highway,
					spawnPointsHighway
				},
				{
					V.placeToSpawn.Desert,
					spawnPointsDesert
				},
				{
					V.placeToSpawn.Mountain,
					spawnPointsMountain
				},
				{
					V.placeToSpawn.Racing,
					spawnPointsRacing
				},
				{
					V.placeToSpawn.City2,
					spawnPointsCity2
				},
				{
					V.placeToSpawn.Offroad,
					spawnPointsOffrad
				},
				{
					V.placeToSpawn.JapanSerpentine,
					spawnPointsJapan
				},
				{
					V.placeToSpawn.JapanSuburban,
					spawnPointsJapanSuburban
				},
				{
					V.placeToSpawn.GarageEntrance,
					spawnPointsCity
				},
				{
					V.placeToSpawn.Island,
					spawnPointsIsland
				},
				{
					V.placeToSpawn.TropicalIsland,
					spawnPointsTropicalIsland
				},
				{
					V.placeToSpawn.Nurburgring,
					spawnPointsNurburgring
				},
				{
					V.placeToSpawn.Italy,
					spawnPointsItaly
				}
			};
		}
	}
}
