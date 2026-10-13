using MVPFree.MapIcons.Enum;
using UnityEngine;

namespace _Car_Parking.Scripts.PlayerProperty
{
	[CreateAssetMenu(menuName = "Scriptable/Property Config", fileName = "PropertyConfig")]
	public class PropertyConfig : ScriptableObject
	{
		public PropertyMarkerData[] DataProperties;

		public Texture2D GetMarkerSprite(IconType icon)
		{
			PropertyMarkerData[] dataProperties = DataProperties;
			foreach (PropertyMarkerData propertyMarkerData in dataProperties)
			{
				if (propertyMarkerData.IconMapView == icon)
				{
					return propertyMarkerData.MainTexture;
				}
			}
			return null;
		}

		public Material GetDecalMarker(IconType icon)
		{
			PropertyMarkerData[] dataProperties = DataProperties;
			foreach (PropertyMarkerData propertyMarkerData in dataProperties)
			{
				if (propertyMarkerData.IconMapView == icon)
				{
					return propertyMarkerData.DecalMaterial;
				}
			}
			return null;
		}

		public Sprite GetRieltorIcon(IconType icon)
		{
			PropertyMarkerData[] dataProperties = DataProperties;
			foreach (PropertyMarkerData propertyMarkerData in dataProperties)
			{
				if (propertyMarkerData.IconMapView == icon)
				{
					return propertyMarkerData.RieltorIcon;
				}
			}
			return null;
		}
	}
}
