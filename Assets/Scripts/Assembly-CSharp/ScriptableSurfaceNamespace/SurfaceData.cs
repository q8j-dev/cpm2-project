using System;
using UnityEngine;

namespace ScriptableSurfaceNamespace
{
	[Serializable]
	public class SurfaceData
	{
		public int hash;

		public Surface surface;

		public Material material;

		public GameObject gameObject;

		public SurfaceData(int hash, Material material, GameObject gameObject)
		{
			this.hash = hash;
			this.material = material;
			this.gameObject = gameObject;
		}

		public SurfaceData(SurfaceData data)
		{
			surface = data.surface;
			hash = data.hash;
			material = null;
			gameObject = null;
		}
	}
}
