using System.Collections.Generic;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.Physics
{
	public static class CarColliders
	{
		private static readonly List<Collider> BufferA = new List<Collider>(8);

		private static readonly List<Collider> BufferB = new List<Collider>(8);

		private static bool _buffersInUse;

		public static void Collect(VehicleData car, List<Collider> result)
		{
			if (!car)
			{
				return;
			}
			Collider collider = car.Collider;
			if (IsUsable(collider))
			{
				result.Add(collider);
			}
			Powertrain getPowertrain = car.GetPowertrain;
			Axles axles = (getPowertrain ? getPowertrain.GetAxles : null);
			List<WheelProvider> list = (axles ? axles.AllWheels : null);
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				WheelProvider wheelProvider = list[i];
				if ((bool)wheelProvider && wheelProvider.OpenWheels)
				{
					Collider rimCollider = wheelProvider.RimCollider;
					if (IsUsable(rimCollider))
					{
						result.Add(rimCollider);
					}
				}
			}
		}

		public static void SetIgnore(VehicleData car, Collider other, bool ignore)
		{
			if (!car || !IsUsable(other))
			{
				return;
			}
			bool shared = TakeBuffers(out var a, out var _);
			try
			{
				Collect(car, a);
				for (int i = 0; i < a.Count; i++)
				{
					UnityEngine.Physics.IgnoreCollision(a[i], other, ignore);
				}
			}
			finally
			{
				ReleaseBuffers(shared);
			}
		}

		public static void SetIgnore(VehicleData carA, VehicleData carB, bool ignore)
		{
			if (!carA || !carB)
			{
				return;
			}
			bool shared = TakeBuffers(out var a, out var b);
			try
			{
				Collect(carA, a);
				Collect(carB, b);
				for (int i = 0; i < a.Count; i++)
				{
					for (int j = 0; j < b.Count; j++)
					{
						UnityEngine.Physics.IgnoreCollision(a[i], b[j], ignore);
					}
				}
			}
			finally
			{
				ReleaseBuffers(shared);
			}
		}

		private static bool TakeBuffers(out List<Collider> a, out List<Collider> b)
		{
			bool flag = !_buffersInUse;
			_buffersInUse = true;
			a = (flag ? BufferA : new List<Collider>(8));
			b = (flag ? BufferB : new List<Collider>(8));
			a.Clear();
			b.Clear();
			return flag;
		}

		private static void ReleaseBuffers(bool shared)
		{
			if (shared)
			{
				BufferA.Clear();
				BufferB.Clear();
				_buffersInUse = false;
			}
		}

		private static bool IsUsable(Collider collider)
		{
			if ((bool)collider && collider.enabled)
			{
				return collider.gameObject.activeInHierarchy;
			}
			return false;
		}
	}
}
