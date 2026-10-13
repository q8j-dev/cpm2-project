using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Steam.Pedestrians
{
	public class SteamPedestrianAssets : MonoBehaviour
	{
		private readonly List<Object> _owned = new List<Object>();

		public void Own(Object asset)
		{
			_owned.Add(asset);
		}

		private void OnDestroy()
		{
			foreach (Object item in _owned)
			{
				if ((bool)item)
				{
					Object.Destroy(item);
				}
			}
			_owned.Clear();
		}
	}
}
