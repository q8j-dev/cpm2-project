using System.Threading;
using CPM.Ghost.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CPM.Ghost
{
	public interface IGhostCarFactory
	{
		UniTask<GameObject> CreateAsync(int carId, GhostMaterialPreset preset, int[] lodWheelPool, LayerMask groundLayers, Transform observer, CancellationToken ct);

		void Release(GameObject ghost);
	}
}
