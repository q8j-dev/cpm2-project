using System;
using UnityEngine;

namespace Service
{
	[Serializable]
	public class Location
	{
		public GameObject pointerToGameObject;

		public Vector3 position;

		public Quaternion rotation;

		public string city;

		public GameObject gameObject
		{
			get
			{
				return pointerToGameObject;
			}
			set
			{
				pointerToGameObject = value;
			}
		}

		public Location(Vector3 pos, string city, GameObject pointerToGameObject = null)
		{
			position = pos;
			this.city = city;
			this.pointerToGameObject = pointerToGameObject;
			rotation = Quaternion.identity;
		}

		public Location(Vector3 pos, Quaternion rot, string city, GameObject pointerToGameObject = null)
		{
			position = pos;
			this.city = city;
			this.pointerToGameObject = pointerToGameObject;
			rotation = rot;
		}
	}
}
