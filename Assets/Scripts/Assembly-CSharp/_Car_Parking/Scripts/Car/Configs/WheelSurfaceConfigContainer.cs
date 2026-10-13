using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.Configs
{
	public static class WheelSurfaceConfigContainer
	{
		public static RideableMeshesGraph rideableMeshesGraph;

		private static Dictionary<int, Surface> SurfaceDataDictionary { get; set; }

		private static Dictionary<Surface, Color> DirtColorDictionary { get; set; }

		public static void Construct(WheelSurfaceDataListSO config)
		{
			if (SurfaceDataDictionary != null)
			{
				return;
			}
			SurfaceDataDictionary = new Dictionary<int, Surface>();
			foreach (WheelSurfaceData wheelSurfaceData in config.wheelSurfaceDataList)
			{
				if (!string.IsNullOrEmpty(wheelSurfaceData.textureName))
				{
					int hashCode = wheelSurfaceData.textureName.GetHashCode();
					if (!SurfaceDataDictionary.ContainsKey(hashCode))
					{
						SurfaceDataDictionary.Add(hashCode, wheelSurfaceData.surfaceType);
					}
				}
			}
			DirtColorDictionary = new Dictionary<Surface, Color>();
			foreach (SurfaceDirtColor surfaceDirtColor in config.surfaceDirtColors)
			{
				if (!DirtColorDictionary.ContainsKey(surfaceDirtColor.surface))
				{
					DirtColorDictionary.Add(surfaceDirtColor.surface, surfaceDirtColor.color);
				}
			}
		}

		public static bool TryGetDirtColor(Surface surface, out Color colorOut)
		{
			if (DirtColorDictionary == null)
			{
				colorOut = Color.white;
				return false;
			}
			if (DirtColorDictionary.TryGetValue(surface, out var value))
			{
				colorOut = value;
				return true;
			}
			colorOut = Color.white;
			return false;
		}

		public static bool TryGetSurface(int textureNameHash, out Surface surfaceOut)
		{
			if (SurfaceDataDictionary == null)
			{
				surfaceOut = Surface.Dryasphalt;
				return false;
			}
			if (SurfaceDataDictionary.TryGetValue(textureNameHash, out var value))
			{
				surfaceOut = value;
				return true;
			}
			surfaceOut = Surface.Dryasphalt;
			return false;
		}
	}
}
