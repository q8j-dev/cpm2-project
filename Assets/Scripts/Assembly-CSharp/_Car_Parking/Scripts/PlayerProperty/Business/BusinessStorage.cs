using UnityEngine;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	[CreateAssetMenu(fileName = "BusinessStorage", menuName = "Scriptable/BusinessStorage")]
	public class BusinessStorage : ScriptableObject
	{
		public BusinessConfig[] Businesses;

		private void RefreshBusinessIds()
		{
			OnBusinessesCollectionChanged();
		}

		public BusinessConfig GetBusinessById(int id)
		{
			BusinessConfig[] businesses = Businesses;
			foreach (BusinessConfig businessConfig in businesses)
			{
				if (businessConfig.BusinessId == id)
				{
					return businessConfig;
				}
			}
			return null;
		}

		private void OnBusinessesCollectionChanged()
		{
			if (Businesses == null)
			{
				return;
			}
			for (int i = 0; i < Businesses.Length; i++)
			{
				if (Businesses[i] != null)
				{
					Businesses[i].BusinessId = i + 1;
				}
			}
		}

		private void OnValidate()
		{
			OnBusinessesCollectionChanged();
		}
	}
}
