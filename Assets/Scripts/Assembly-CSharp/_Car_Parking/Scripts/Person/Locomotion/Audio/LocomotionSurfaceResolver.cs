using UnityEngine;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.Configs;

namespace _Car_Parking.Scripts.Person.Locomotion.Audio
{
	public class LocomotionSurfaceResolver
	{
		private static readonly uint SurfaceSwitchGroup = AudioWrapper.GetId("Locomotion_Surface_Switch");

		private static readonly uint Concrete = AudioWrapper.GetId("Concrete");

		private static readonly uint WetConcrete = AudioWrapper.GetId("Wet_Concrete");

		private static readonly uint Grass = AudioWrapper.GetId("Grass");

		private static readonly uint HouseFloor = AudioWrapper.GetId("HouseFloor");

		private static readonly uint Sand = AudioWrapper.GetId("Sand");

		private static readonly uint Offroad = AudioWrapper.GetId("Offroad");

		private static readonly uint Snow = AudioWrapper.GetId("Snow");

		private static readonly uint Wood = AudioWrapper.GetId("Wood");

		private static readonly uint Gravel = AudioWrapper.GetId("Gravel");

		private static readonly uint Dirt = AudioWrapper.GetId("Dirt");

		private const float ProbeUp = 0.35f;

		private const float ProbeDown = 0.9f;

		private readonly int _layerMask;

		private Surface _lastSurface = Surface.Dryasphalt;

		public uint SwitchGroup => SurfaceSwitchGroup;

		public Surface LastSurface => _lastSurface;

		public LocomotionSurfaceResolver()
		{
			_layerMask = ~(1 << LayerMask.NameToLayer("car"));
		}

		public uint Resolve(Vector3 footPosition)
		{
			_lastSurface = Probe(footPosition);
			return ToSwitch(_lastSurface, IsWet(footPosition));
		}

		private Surface Probe(Vector3 footPosition)
		{
			RideableMeshesGraph rideableMeshesGraph = WheelSurfaceConfigContainer.rideableMeshesGraph;
			if (rideableMeshesGraph == null)
			{
				return _lastSurface;
			}
			if (!Physics.Raycast(footPosition + Vector3.up * 0.35f, Vector3.down, out var hitInfo, 1.25f, _layerMask, QueryTriggerInteraction.Ignore))
			{
				return _lastSurface;
			}
			if (!(hitInfo.collider is MeshCollider meshCollider) || meshCollider.sharedMesh == null)
			{
				return Surface.Dryasphalt;
			}
			Surface surface = rideableMeshesGraph.GetSurface(hitInfo.collider, hitInfo.triangleIndex, hitInfo.barycentricCoordinate);
			if (surface != Surface.None)
			{
				return surface;
			}
			return Surface.Dryasphalt;
		}

		private static bool IsWet(Vector3 footPosition)
		{
			if (!RainWetnessDriver.HasActiveDriver)
			{
				return false;
			}
			if (RainWetnessDriver.CurrentIntensity <= 0.001f)
			{
				return false;
			}
			return RainDryZoneManager.WetMaskAt(footPosition) > 0.5f;
		}

		public static uint ToSwitch(Surface surface, bool wet)
		{
			switch (surface)
			{
			case Surface.Indoor:
				return HouseFloor;
			case Surface.Wood:
				return Wood;
			case Surface.Snow:
			case Surface.Winterroad:
			case Surface.Ice:
				return Snow;
			case Surface.Grass:
				if (!wet)
				{
					return Grass;
				}
				return Dirt;
			case Surface.Sand:
				if (!wet)
				{
					return Sand;
				}
				return Dirt;
			case Surface.Gravel:
				if (!wet)
				{
					return Gravel;
				}
				return Dirt;
			case Surface.OutOfRoad:
			case Surface.Damp:
			case Surface.DirtRoad:
			case Surface.Offroad:
				if (!wet)
				{
					return Offroad;
				}
				return Dirt;
			default:
				if (!wet)
				{
					return Concrete;
				}
				return WetConcrete;
			}
		}
	}
}
