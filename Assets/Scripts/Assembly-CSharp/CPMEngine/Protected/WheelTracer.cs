using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Car.Configs;

namespace CPMEngine.Protected
{
	public class WheelTracer : IRay
	{
		private LayerMask _mask;

		private readonly RideableMeshesGraph _rideableMeshesGraph;

		private const bool LoadingSurfaceTypesFromScriptable = true;

		private static List<Material> _tmpMaterialsList = new List<Material>(30);

		private static readonly int Albedo = Shader.PropertyToID("_Albedo");

		private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

		private readonly Dictionary<Collider, MeshRenderer> _meshRendererCache = new Dictionary<Collider, MeshRenderer>(32);

		private readonly List<Collider> _staleColliders = new List<Collider>(32);

		private readonly Queue<Collider> _insertionOrder = new Queue<Collider>(32);

		private const int MaxCacheSize = 128;

		private readonly Collider _collider;

		private bool _colliderStatus;

		public WheelTracer(RideableMeshesGraph rideableMeshesGraph, Collider carCollider)
		{
			_mask = LayerMask.GetMask("Default", "Road", "Trailer", "Terrain", "Garage", "pedestrain", "traffic car", "CarBody");
			_rideableMeshesGraph = rideableMeshesGraph;
			_collider = carCollider;
			if ((bool)_collider)
			{
				_colliderStatus = _collider.isTrigger;
			}
		}

		public void Destroy()
		{
			_meshRendererCache.Clear();
			_insertionOrder.Clear();
			List<Material> tmpMaterialsList = _tmpMaterialsList;
			if (tmpMaterialsList == null || tmpMaterialsList.Count <= 0)
			{
				return;
			}
			foreach (Material tmpMaterials in _tmpMaterialsList)
			{
				Object.Destroy(tmpMaterials);
			}
			_tmpMaterialsList.Clear();
		}

		private void PruneDeadEntries()
		{
			_staleColliders.Clear();
			foreach (Collider key in _meshRendererCache.Keys)
			{
				if (!key)
				{
					_staleColliders.Add(key);
				}
			}
			for (int i = 0; i < _staleColliders.Count; i++)
			{
				_meshRendererCache.Remove(_staleColliders[i]);
			}
			_staleColliders.Clear();
			int count = _insertionOrder.Count;
			for (int j = 0; j < count; j++)
			{
				Collider collider = _insertionOrder.Dequeue();
				if (_meshRendererCache.ContainsKey(collider))
				{
					_insertionOrder.Enqueue(collider);
				}
			}
		}

		public virtual bool GetRay(Vector3 p1, Vector3 p2, out RayInfo rayInfo)
		{
			rayInfo = new RayInfo
			{
				Surface = Surface.None
			};
			if (!Physics.Linecast(p1, p2, out var hitInfo, _mask.value, QueryTriggerInteraction.Ignore))
			{
				return false;
			}
			rayInfo.Target = hitInfo.point;
			rayInfo.Normal = hitInfo.normal;
			if ((bool)hitInfo.collider)
			{
				if (_rideableMeshesGraph != null)
				{
					rayInfo.Surface = _rideableMeshesGraph.GetSurface(hitInfo.collider, hitInfo.triangleIndex, hitInfo.barycentricCoordinate);
				}
				else
				{
					MeshCollider meshCollider = hitInfo.collider as MeshCollider;
					if ((bool)meshCollider && (bool)meshCollider.sharedMesh)
					{
						rayInfo.Surface = GetSurfaceLegacy(meshCollider, hitInfo);
					}
				}
			}
			return true;
		}

		private static Surface GetSurfaceByHash(int hash)
		{
			if (!WheelSurfaceConfigContainer.TryGetSurface(hash, out var surfaceOut))
			{
				return Surface.Dryasphalt;
			}
			return surfaceOut;
		}

		private int GetSubMeshIndexByRaycastHit(RaycastHit hit, MeshCollider meshCollider)
		{
			Mesh sharedMesh = meshCollider.sharedMesh;
			int triangleIndex = hit.triangleIndex;
			int result = -1;
			int num = 0;
			for (int i = 0; i < sharedMesh.subMeshCount; i++)
			{
				num += sharedMesh.GetSubMesh(i).indexCount / 3;
				if (triangleIndex < num)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		private Surface GetSurfaceLegacy(MeshCollider meshCollider, RaycastHit hitDown)
		{
			Surface surface = Surface.Dryasphalt;
			Collider collider = hitDown.collider;
			if (!_meshRendererCache.TryGetValue(collider, out var value))
			{
				if (_meshRendererCache.Count >= 128)
				{
					PruneDeadEntries();
					while (_meshRendererCache.Count >= 128 && _insertionOrder.Count > 0)
					{
						Collider key = _insertionOrder.Dequeue();
						_meshRendererCache.Remove(key);
					}
				}
				value = collider.GetComponent<MeshRenderer>();
				_meshRendererCache[collider] = value;
				_insertionOrder.Enqueue(collider);
			}
			if ((bool)value)
			{
				int subMeshIndexByRaycastHit = GetSubMeshIndexByRaycastHit(hitDown, meshCollider);
				if (subMeshIndexByRaycastHit >= 0)
				{
					_tmpMaterialsList.Clear();
					value.GetSharedMaterials(_tmpMaterialsList);
					if (subMeshIndexByRaycastHit < _tmpMaterialsList.Count)
					{
						Material material = _tmpMaterialsList[subMeshIndexByRaycastHit];
						Texture texture = null;
						texture = (material.HasTexture(Albedo) ? material.GetTexture(Albedo) : ((!material.HasTexture(BaseMap)) ? material.mainTexture : material.GetTexture(BaseMap)));
						int hash = 0;
						if ((bool)texture)
						{
							hash = texture.name.GetHashCode();
						}
						surface = GetSurfaceByHash(hash);
						if (surface == Surface.None)
						{
							surface = Surface.Dryasphalt;
						}
					}
					_tmpMaterialsList.Clear();
				}
			}
			return surface;
		}
	}
}
