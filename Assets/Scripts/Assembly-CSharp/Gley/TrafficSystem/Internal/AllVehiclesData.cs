using System;
using System.Collections.Generic;
using Gley.UrbanSystem.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class AllVehiclesData
	{
		private readonly VehicleComponent[] _allVehicles;

		private readonly Func<GameObject, Transform, GameObject> _instantiateVehicle;

		public VehicleComponent[] AllVehicles => _allVehicles;

		public AllVehiclesData(Transform parent, VehiclePool vehiclePool, int nrOfVehicles, LayerMask buildingLayers, LayerMask obstacleLayers, LayerMask playerLayers, LayerMask roadLayers, bool lightsOn, ModifyTriggerSize modifyTriggerSize, TrafficWaypointsData trafficWaypointsData, float minOffset, float maxOffset, List<Color> colorPool, Func<GameObject, Transform, GameObject> instantiateVehicle = null)
		{
			_instantiateVehicle = instantiateVehicle;
			Transform transform = MonoBehaviourUtilities.CreateGameObject("TrafficHolder", parent, parent.position, addEditorTag: false).transform;
			transform.SetParent(GameObject.Find("Global point").transform);
			transform.localPosition = Vector3.zero;
			_allVehicles = new VehicleComponent[nrOfVehicles];
			int num = 0;
			int num2 = vehiclePool.trafficCars.Length;
			if (num2 > nrOfVehicles)
			{
				num2 = nrOfVehicles;
			}
			for (int i = 0; i < num2; i++)
			{
				_allVehicles[num] = LoadVehicle(num, vehiclePool.trafficCars[i].VehiclePrefab, transform, buildingLayers, obstacleLayers, playerLayers, roadLayers, vehiclePool.trafficCars[i].Ignore, lightsOn, modifyTriggerSize, trafficWaypointsData, minOffset, maxOffset, colorPool);
				num++;
			}
			nrOfVehicles -= num2;
			float num3 = 0f;
			List<float> list = new List<float>();
			for (int j = 0; j < vehiclePool.trafficCars.Length; j++)
			{
				num3 += (float)vehiclePool.trafficCars[j].Percent;
				list.Add(num3);
			}
			float num4 = num3 / (float)nrOfVehicles;
			int num5 = 0;
			for (int k = 0; k < nrOfVehicles; k++)
			{
				while ((float)(k + 1) * num4 > list[num5])
				{
					num5++;
					if (num5 >= vehiclePool.trafficCars.Length)
					{
						num5 = vehiclePool.trafficCars.Length - 1;
						break;
					}
				}
				_allVehicles[num] = LoadVehicle(num, vehiclePool.trafficCars[num5].VehiclePrefab, transform, buildingLayers, obstacleLayers, playerLayers, roadLayers, vehiclePool.trafficCars[num5].Ignore, lightsOn, modifyTriggerSize, trafficWaypointsData, minOffset, maxOffset, colorPool);
				num++;
			}
		}

		private VehicleComponent LoadVehicle(int vehicleIndex, GameObject carPrefab, Transform parent, LayerMask buildingLayers, LayerMask obstacleLayers, LayerMask playerLayers, LayerMask roadLayers, bool ignored, bool lightsOn, ModifyTriggerSize modifyTriggerSize, TrafficWaypointsData trafficWaypointsData, float minOffset, float maxOffset, List<Color> colorPool)
		{
			GameObject gameObject = _instantiateVehicle?.Invoke(carPrefab, parent);
			if (!gameObject)
			{
				gameObject = UnityEngine.Object.Instantiate(carPrefab, Vector3.zero, Quaternion.identity, parent);
			}
			VehicleComponent vehicleComponent = gameObject.GetComponent<VehicleComponent>().Initialize(buildingLayers, obstacleLayers, playerLayers, roadLayers, lightsOn, modifyTriggerSize, trafficWaypointsData, vehicleIndex, ignored, minOffset, maxOffset);
			vehicleComponent.name += vehicleIndex;
			if (colorPool != null && colorPool.Count > 0 && vehicleComponent.meshRenderer != null)
			{
				vehicleComponent.meshRenderer.material.color = colorPool[UnityEngine.Random.Range(0, colorPool.Count)];
			}
			if (vehicleComponent.meshRenderer != null && vehicleComponent.meshRenderer.material.HasFloat("_MaskValue"))
			{
				float num = UnityEngine.Random.Range(0f, 0.5f);
				if (num > 0.3f)
				{
					vehicleComponent.meshRenderer.material.SetFloat("_MaskValue", num);
				}
			}
			vehicleComponent.DeactivateVehicle();
			return vehicleComponent;
		}

		public int GetTotalWheels()
		{
			int num = 0;
			for (int i = 0; i < _allVehicles.Length; i++)
			{
				num += _allVehicles[i].allWheels.Length;
				if (_allVehicles[i].trailer != null)
				{
					num += _allVehicles[i].trailer.allWheels.Length;
				}
			}
			return num;
		}

		public void UpdateVehicleLights(bool on)
		{
			for (int i = 0; i < _allVehicles.Length; i++)
			{
				_allVehicles[i].SetMainLights(on);
			}
		}

		public int GetVehicleIndex(GameObject vehicle)
		{
			for (int i = 0; i < _allVehicles.Length; i++)
			{
				if (_allVehicles[i].gameObject == vehicle)
				{
					return i;
				}
			}
			return -1;
		}

		public VehicleComponent GetVehicle(int vehicleIndex)
		{
			if (IsVehicleIndexValid(vehicleIndex))
			{
				return _allVehicles[vehicleIndex];
			}
			return null;
		}

		public bool IsVehicleIndexValid(int vehicleIndex)
		{
			if (vehicleIndex < 0)
			{
				Debug.LogError($"Vehicle index {vehicleIndex} should be >= 0");
				return false;
			}
			if (vehicleIndex >= _allVehicles.Length)
			{
				Debug.LogError($"Vehicle index {vehicleIndex} should be < {_allVehicles.Length}");
				return false;
			}
			if (_allVehicles[vehicleIndex] == null)
			{
				Debug.LogError($"Vehicle at {vehicleIndex} is null, Verify the setup");
				return false;
			}
			return true;
		}

		public List<VehicleComponent> GetIgnoredVehicleList()
		{
			List<VehicleComponent> list = new List<VehicleComponent>();
			for (int i = 0; i < _allVehicles.Length; i++)
			{
				if (_allVehicles[i].Ignored)
				{
					list.Add(_allVehicles[i]);
				}
			}
			return list;
		}

		public int GetIgnoredVehicleIndex(GameObject vehicle)
		{
			for (int i = 0; i < _allVehicles.Length; i++)
			{
				if (_allVehicles[i].Ignored && _allVehicles[i].gameObject == vehicle)
				{
					return i;
				}
			}
			return -1;
		}

		public void TriggerColliderRemovedEvent(Collider[] colliders)
		{
			for (int i = 0; i < _allVehicles.Length; i++)
			{
				_allVehicles[i].ColliderRemoved(colliders);
			}
		}

		public void RemoveVehicle(int vehicleIndex)
		{
			_allVehicles[vehicleIndex].DeactivateVehicle();
			for (int i = 0; i < _allVehicles.Length; i++)
			{
				_allVehicles[i].ColliderRemoved(_allVehicles[vehicleIndex].AllColliders);
			}
		}

		public void ModifyTriggerSize(int vehicleIndex, ModifyTriggerSize modifyTriggerSizeDelegate)
		{
			if (vehicleIndex < 0)
			{
				for (int i = 0; i < _allVehicles.Length; i++)
				{
					_allVehicles[i].SetTriggerSizeModifierDelegate(modifyTriggerSizeDelegate);
				}
			}
			else
			{
				_allVehicles[vehicleIndex].SetTriggerSizeModifierDelegate(modifyTriggerSizeDelegate);
			}
		}
	}
}
