using UnityEngine;
using _Car_Parking.Scripts.ArtistTools.Enums;
using _Car_Parking.Scripts.ArtistTools.Trackables;

namespace _Car_Parking.Scripts.ArtistTools
{
	public class ProceduralAnimationRotator : TrackableWithAspects
	{
		[SerializeField]
		private CoordinateAxis RotationAxis;

		[SerializeField]
		private float RotationSpeed;

		[SerializeField]
		public bool IsRandomStart;

		private void OnEnable()
		{
			InitRandomRotating();
		}

		public void InitRandomRotating()
		{
			if (IsRandomStart)
			{
				float angle = Random.Range(0f, 360f);
				Vector3 axis = Vector3.zero;
				switch (RotationAxis)
				{
				case CoordinateAxis.X:
					axis = Vector3.right;
					break;
				case CoordinateAxis.Y:
					axis = Vector3.up;
					break;
				case CoordinateAxis.Z:
					axis = Vector3.forward;
					break;
				}
				base.transform.Rotate(axis, angle);
			}
		}

		private void Update()
		{
			Vector3 axis = Vector3.zero;
			float deltaTime = Time.deltaTime;
			switch (RotationAxis)
			{
			case CoordinateAxis.X:
				axis = Vector3.right;
				break;
			case CoordinateAxis.Y:
				axis = Vector3.up;
				break;
			case CoordinateAxis.Z:
				axis = Vector3.forward;
				break;
			}
			base.transform.Rotate(axis, RotationSpeed * deltaTime);
		}
	}
}
