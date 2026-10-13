using System.Collections.Generic;
using UnityEngine;

namespace CPM.Checkpoints
{
	public sealed class CheckpointRoute : MonoBehaviour
	{
		[SerializeField]
		private Collider[] _checkpoints = default;

		public IReadOnlyList<Collider> Checkpoints => _checkpoints;

		private void OnValidate()
		{
			if (_checkpoints == null)
			{
				return;
			}
			for (int i = 0; i < _checkpoints.Length; i++)
			{
				Collider collider = _checkpoints[i];
				if ((bool)collider)
				{
					_ = collider.isTrigger;
					for (int j = 0; j < i; j++)
					{
						_ = _checkpoints[j] == collider;
					}
				}
			}
		}
	}
}
