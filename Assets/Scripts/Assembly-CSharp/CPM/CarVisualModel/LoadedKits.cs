using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CPM.CarVisualModel
{
	public readonly struct LoadedKits
	{
		public readonly List<LoadedKitInstance> Kits;

		public readonly List<AsyncOperationHandle> Handles;

		public LoadedKits(List<LoadedKitInstance> kits, List<AsyncOperationHandle> handles)
		{
			Kits = kits;
			Handles = handles;
		}
	}
}
