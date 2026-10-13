using System;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace Car_Parking.Scripts.Racing
{
	public class ZoneTriggerHandler : MonoBehaviour
	{
		private const float ZoneExitOffset = 1f;

		private VehicleData _vehicleData;

		private float _zoneRadius;

		private bool _isInZone;

		private FreeDriveProvider _freeDriveProvider;

		public Action OnZoneEntered;

		public Action OnZoneExit;

		public bool Enabled { get; set; } = true;

		public bool CanWithoutCar { get; set; }

		public bool IsInZone => _isInZone;

		public void ForceExit()
		{
			if (_isInZone)
			{
				_isInZone = false;
				OnZoneExit?.Invoke();
			}
		}

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		private void Start()
		{
			Vector3 lossyScale = base.transform.lossyScale;
			_zoneRadius = Mathf.Max(lossyScale.x, lossyScale.z) * 0.5f;
		}

		public void CheckZone()
		{
			if (!Enabled || !base.gameObject.activeSelf || !_freeDriveProvider.FirstCarMultiPlayer)
			{
				return;
			}
			if ((bool)_freeDriveProvider.FirstCarMultiPlayer && !_vehicleData)
			{
				_vehicleData = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
			}
			Vector3 position;
			if (!CanWithoutCar && (bool)_vehicleData)
			{
				position = _freeDriveProvider.FirstCarMultiPlayer.transform.position;
			}
			else
			{
				if (!CanWithoutCar || !V.DriverPerson)
				{
					return;
				}
				position = V.DriverPerson.transform.position;
			}
			Vector3 a = new Vector3(position.x, 0f, position.z);
			Vector3 b = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			float num = Vector3.Distance(a, b);
			if (!_isInZone && num <= _zoneRadius)
			{
				_isInZone = true;
				OnZoneEntered?.Invoke();
			}
			else if (_isInZone && num >= _zoneRadius + 1f)
			{
				_isInZone = false;
				OnZoneExit?.Invoke();
			}
		}
	}
}
