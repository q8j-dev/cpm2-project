using System;
using System.IO;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace NewUI.Scripts
{
	[Serializable]
	public class MarketPartItem
	{
		[SerializeField]
		public string classPath = "Assets/_Car Parking/Scripts/EngineTuning/Test/B58/Stock/";

		public string type;

		public PartItem part;

		public string[] GetDirectoriesByType()
		{
			return Directory.GetDirectories(classPath);
		}
	}
}
