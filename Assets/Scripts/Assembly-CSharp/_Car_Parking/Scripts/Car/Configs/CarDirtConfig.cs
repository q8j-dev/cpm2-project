using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Car.Configs
{
	[CreateAssetMenu(fileName = "CarDirtConfig", menuName = "Scriptable/CarDirtConfig")]
	[Preserve]
	public class CarDirtConfig : ScriptableObject
	{
		[Serializable]
		private class CarIdPath
		{
			public int carId;

			public string path;
		}

		[SerializeField]
		private string dirtTexturesPath = "DirtTexture";

		[SerializeField]
		private List<CarIdPath> carIdToPath = new List<CarIdPath>();

		[Preserve]
		public CarDirtConfig()
		{
		}

		public Texture GetDirtTexture(int carId)
		{
			Texture result = null;
			foreach (CarIdPath item in carIdToPath)
			{
				if (item.carId == carId)
				{
					result = Resources.Load<Texture>(dirtTexturesPath + "/" + item.path);
					return result;
				}
			}
			Texture[] array = Resources.LoadAll<Texture>(dirtTexturesPath + "/Other");
			if (array.Length != 0)
			{
				result = array[UnityEngine.Random.Range(0, array.Length)];
			}
			return result;
		}
	}
}
