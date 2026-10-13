using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class PositionValidator
	{
		private readonly float _minDistanceToAdd;

		private readonly bool _debugDensity;

		private Collider[] _results;

		private Transform[] _activeCameras;

		private LayerMask _trafficLayer;

		private LayerMask _playerLayer;

		private LayerMask _buildingsLayers;

		private CustomPositionValidation _customValidation;

		public List<Matrix4x4> MatrixToCheck { get; private set; }

		public List<Vector3> LastSize { get; private set; }

		public PositionValidator(Transform[] activeCameras, LayerMask trafficLayer, LayerMask playerLayer, LayerMask buildingsLayers, float minDistanceToAdd, bool debugDensity)
		{
			UpdateCamera(activeCameras);
			_trafficLayer = trafficLayer;
			_playerLayer = playerLayer;
			_minDistanceToAdd = minDistanceToAdd * minDistanceToAdd;
			_buildingsLayers = buildingsLayers;
			_debugDensity = debugDensity;
			_results = new Collider[1];
			if (debugDensity)
			{
				MatrixToCheck = new List<Matrix4x4>();
				LastSize = new List<Vector3>();
			}
		}

		public void SetCustomPositionValidation(CustomPositionValidation customPositionValidation)
		{
			_customValidation = customPositionValidation;
		}

		public bool IsValid(Vector3 position, float vehicleLength, float vehicleHeight, float vehicleWidth, bool ignoreLineOfSight, float frontWheelOffset, Quaternion rotation)
		{
			if (_customValidation != null)
			{
				return _customValidation(position, vehicleLength, vehicleHeight, vehicleWidth, rotation);
			}
			position -= rotation * new Vector3(0f, 0f, frontWheelOffset);
			for (int i = 0; i < _activeCameras.Length; i++)
			{
				if (!ignoreLineOfSight && Vector3.SqrMagnitude(_activeCameras[i].position - position) < _minDistanceToAdd && !Physics.Linecast(position, _activeCameras[i].position, _buildingsLayers))
				{
					return false;
				}
			}
			return IsPositionFree(position, vehicleLength, vehicleHeight, vehicleWidth, rotation);
		}

		public bool IsPositionFree(Vector3 position, float length, float height, float width, Quaternion rotation)
		{
			if (_debugDensity)
			{
				MatrixToCheck.Add(Matrix4x4.TRS(position, rotation, Vector3.one));
				LastSize.Add(new Vector3(width, height * 2f, length));
				if (MatrixToCheck.Count > 10)
				{
					MatrixToCheck.RemoveAt(0);
					LastSize.RemoveAt(0);
				}
			}
			if (Physics.OverlapBoxNonAlloc(position, new Vector3(width / 2f, height, length / 2f), _results, rotation, (int)_trafficLayer | (int)_playerLayer) > 0)
			{
				return false;
			}
			return true;
		}

		public bool CheckTrailerPosition(Vector3 position, Quaternion vehicleRotation, Quaternion trailerRotation, VehicleComponent vehicle)
		{
			Vector3 position2 = position - vehicleRotation * Vector3.forward * (vehicle.frontTrigger.transform.localPosition.z + vehicle.carHolder.transform.localPosition.z);
			position2 -= trailerRotation * Vector3.forward * vehicle.trailer.length / 2f;
			return IsPositionFree(position2, vehicle.trailer.length, vehicle.trailer.height, vehicle.trailer.width, trailerRotation);
		}

		public void UpdateCamera(Transform[] activeCameras)
		{
			_activeCameras = activeCameras;
		}
	}
}
