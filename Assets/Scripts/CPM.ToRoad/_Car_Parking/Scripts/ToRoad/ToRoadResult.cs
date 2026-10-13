using UnityEngine;

namespace _Car_Parking.Scripts.ToRoad
{
	public readonly struct ToRoadResult
	{
		public enum Kind
		{
			NotHandled = 0,
			Handled = 1,
			Place = 2
		}

		public readonly Kind Type;

		public readonly Vector3 Position;

		public readonly Quaternion Rotation;

		public readonly bool LoadEnvironment;

		private ToRoadResult(Kind type, Vector3 position, Quaternion rotation, bool loadEnvironment)
		{
			Type = type;
			Position = position;
			Rotation = rotation;
			LoadEnvironment = loadEnvironment;
		}

		public static ToRoadResult NotHandled()
		{
			return new ToRoadResult(Kind.NotHandled, Vector3.zero, Quaternion.identity, loadEnvironment: false);
		}

		public static ToRoadResult Handled()
		{
			return new ToRoadResult(Kind.Handled, Vector3.zero, Quaternion.identity, loadEnvironment: false);
		}

		public static ToRoadResult Place(Vector3 position, Quaternion rotation, bool loadEnvironment = true)
		{
			return new ToRoadResult(Kind.Place, position, rotation, loadEnvironment);
		}
	}
}
