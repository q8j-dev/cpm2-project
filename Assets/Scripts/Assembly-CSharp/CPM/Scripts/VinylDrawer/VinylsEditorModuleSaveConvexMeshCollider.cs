using System.Collections.Generic;
using UnityEngine;

namespace CPM.Scripts.VinylDrawer
{
	public class VinylsEditorModuleSaveConvexMeshCollider : VinylsEditorModule
	{
		private List<MeshCollider> _meshColliders = new List<MeshCollider>();

		public VinylsEditorModuleSaveConvexMeshCollider(VinylsEditor vinylsEditor)
			: base(vinylsEditor)
		{
		}

		public override void PrepareBodyCompleted(VehicleData vehicleData)
		{
			base.PrepareBodyCompleted(vehicleData);
			_meshColliders.Clear();
			if (!vehicleData.CarBodyReferences || !vehicleData.CarBodyReferences.Body)
			{
				return;
			}
			MeshCollider[] componentsInChildren = vehicleData.CarBodyReferences.Body.GetComponentsInChildren<MeshCollider>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].convex)
				{
					componentsInChildren[i].convex = false;
					_meshColliders.Add(componentsInChildren[i]);
				}
			}
		}

		public override void CloseVinylsWindowStarted()
		{
			base.CloseVinylsWindowStarted();
			for (int i = 0; i < _meshColliders.Count; i++)
			{
				if ((bool)_meshColliders[i])
				{
					_meshColliders[i].convex = true;
				}
			}
			_meshColliders.Clear();
		}
	}
}
