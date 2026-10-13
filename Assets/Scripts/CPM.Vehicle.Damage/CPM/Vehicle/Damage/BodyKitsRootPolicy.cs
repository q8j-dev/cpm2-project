using System.Collections.Generic;
using UnityEngine;

namespace CPM.Vehicle.Damage
{
	public static class BodyKitsRootPolicy
	{
		public static bool ShouldScanKits(Transform mainRoot, Transform kitsRoot)
		{
			if (!kitsRoot)
			{
				return false;
			}
			if ((bool)mainRoot)
			{
				return !kitsRoot.IsChildOf(mainRoot);
			}
			return true;
		}

		public static void Collect(Transform mainRoot, Transform kitsRoot, List<MeshFilter> results, List<MeshFilter> scratch)
		{
			if (results != null && scratch != null && results != scratch)
			{
				if ((bool)mainRoot)
				{
					mainRoot.GetComponentsInChildren(results);
				}
				else
				{
					results.Clear();
				}
				if (ShouldScanKits(mainRoot, kitsRoot))
				{
					kitsRoot.GetComponentsInChildren(scratch);
					results.AddRange(scratch);
				}
				scratch.Clear();
			}
		}
	}
}
