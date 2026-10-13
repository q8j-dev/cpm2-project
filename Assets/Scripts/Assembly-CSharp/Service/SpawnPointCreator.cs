using System;
using System.Collections.Generic;
using UnityEngine;

namespace Service
{
	public class SpawnPointCreator : MonoBehaviour
	{
		[NonSerialized]
		public bool edit;

		public TYPE type;

		public GameObject prefab;

		public List<Location> positions = new List<Location>();
	}
}
