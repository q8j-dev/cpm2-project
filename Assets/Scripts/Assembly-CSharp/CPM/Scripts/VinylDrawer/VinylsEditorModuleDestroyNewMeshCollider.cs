using System.Collections.Generic;
using UnityEngine;

namespace CPM.Scripts.VinylDrawer
{
	public class VinylsEditorModuleDestroyNewMeshCollider : VinylsEditorModule
	{
		private List<MeshCollider> _oldColliders = new List<MeshCollider>();

		private List<MeshCollider> _newColliders = new List<MeshCollider>();

		public VinylsEditorModuleDestroyNewMeshCollider(VinylsEditor vinylsEditor)
			: base(vinylsEditor)
		{
		}

		public override void PrepareBodyStarted(VehicleData vehicleData)
		{
			base.PrepareBodyStarted(vehicleData);
			_oldColliders.Clear();
			if ((bool)vehicleData)
			{
				_oldColliders = new List<MeshCollider>(vehicleData.gameObject.GetComponentsInChildren<MeshCollider>(includeInactive: true));
			}
		}

		public override void PrepareBodyCompleted(VehicleData vehicleData)
		{
			base.PrepareBodyCompleted(vehicleData);
			_newColliders.Clear();
			if (!vehicleData)
			{
				return;
			}
			MeshCollider[] componentsInChildren = vehicleData.gameObject.GetComponentsInChildren<MeshCollider>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!_oldColliders.Contains(componentsInChildren[i]))
				{
					_newColliders.Add(componentsInChildren[i]);
				}
			}
		}

		public override void CloseVinylsWindowStarted()
		{
			base.CloseVinylsWindowStarted();
			for (int i = 0; i < _newColliders.Count; i++)
			{
				if ((bool)_newColliders[i])
				{
					Object.Destroy(_newColliders[i]);
				}
			}
			_oldColliders.Clear();
			_newColliders.Clear();
		}
	}
}
