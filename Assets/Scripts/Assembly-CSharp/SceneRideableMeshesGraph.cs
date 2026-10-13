using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Car_Parking.Scripts.Car.Configs;

public class SceneRideableMeshesGraph : MonoBehaviour
{
	public List<RideableMeshPair> rideableMeshPairs;

	public bool refillEachTime;

	private HashSet<Collider> _colliders;

	private RideableMeshesGraph _rideableMeshesGraph;

	private const string WheelSurfaceListPath = "WheelSurfaceDataConfig";

	public void Start()
	{
		if (rideableMeshPairs != null && rideableMeshPairs.Count != 0)
		{
			if (_rideableMeshesGraph == null)
			{
				_rideableMeshesGraph = WheelSurfaceConfigContainer.rideableMeshesGraph;
			}
			Dictionary<Collider, RideableMeshData> sceneGraph = GetSceneGraph();
			PopulateVertexBlendArrays(sceneGraph);
			_rideableMeshesGraph.AddSceneDict(sceneGraph);
		}
	}

	private static void PopulateVertexBlendArrays(Dictionary<Collider, RideableMeshData> dict)
	{
		foreach (RideableMeshData value in dict.Values)
		{
			if (value.mesh == null)
			{
				continue;
			}
			if (!value.mesh.isReadable)
			{
				Debug.LogWarning("[SceneRideableMeshesGraph] Mesh '" + value.mesh.name + "' is not readable — enable Read/Write in import settings for VertexBlend surfaces to work.");
				continue;
			}
			bool flag = false;
			RideableMeshData.SubmeshData[] submeshDatas = value.submeshDatas;
			foreach (RideableMeshData.SubmeshData submeshData in submeshDatas)
			{
				if (submeshData.isVertexBlend)
				{
					flag = true;
					submeshData.triangleIndices = value.mesh.GetTriangles(submeshData.submeshIndex);
				}
			}
			if (flag)
			{
				value.meshVertexColors = value.mesh.colors32;
			}
		}
	}

	private void OnDestroy()
	{
		_rideableMeshesGraph?.RemoveSceneDict(_colliders);
	}

	private Dictionary<Collider, RideableMeshData> GetSceneGraph()
	{
		Dictionary<Collider, RideableMeshData> dictionary = new Dictionary<Collider, RideableMeshData>();
		if (rideableMeshPairs == null)
		{
			_colliders = new HashSet<Collider>();
			return dictionary;
		}
		foreach (RideableMeshPair rideableMeshPair in rideableMeshPairs)
		{
			if ((bool)rideableMeshPair.collider && rideableMeshPair.rideableMeshData != null)
			{
				dictionary.TryAdd(rideableMeshPair.collider, rideableMeshPair.rideableMeshData);
			}
		}
		_colliders = dictionary.Keys.ToHashSet();
		rideableMeshPairs.Clear();
		return dictionary;
	}
}
