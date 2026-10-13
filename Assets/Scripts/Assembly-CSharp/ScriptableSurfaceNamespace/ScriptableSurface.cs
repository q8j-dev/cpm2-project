using System.Collections.Generic;
using UnityEngine;

namespace ScriptableSurfaceNamespace
{
	[CreateAssetMenu(fileName = "Data", menuName = "Scriptable/Surface", order = 1)]
	public class ScriptableSurface : ScriptableObject
	{
		public bool edit = true;

		public List<SurfaceData> surfaces;

		public BinaryTree bt = new BinaryTree();

		public bool Contains(int hash)
		{
			foreach (SurfaceData surface in surfaces)
			{
				if (surface.hash == hash)
				{
					return true;
				}
			}
			return false;
		}

		public Surface GetSurfaceByHash(int hash)
		{
			return bt.Find(hash);
		}

		public void Add(ref SurfaceData data)
		{
			surfaces.Add(data);
		}

		public void CreateBinaryTree()
		{
			bt = new BinaryTree();
			foreach (SurfaceData surface in surfaces)
			{
				bt.Add(surface);
			}
		}

		private void CopyBinaryTree(List<SurfaceData> surfaceDatas)
		{
			bt = new BinaryTree();
			foreach (SurfaceData surfaceData in surfaceDatas)
			{
				SurfaceData newSurfaceData = new SurfaceData(surfaceData);
				bt.Add(newSurfaceData);
			}
		}

		public void Copy(ScriptableSurface other, MODE mode)
		{
			if (mode == MODE.BUILD)
			{
				surfaces = null;
				edit = false;
				CopyBinaryTree(other.surfaces);
			}
		}

		private void OnValidate()
		{
			if (edit)
			{
				return;
			}
			foreach (SurfaceData surface in surfaces)
			{
				surface.gameObject = null;
			}
		}
	}
}
