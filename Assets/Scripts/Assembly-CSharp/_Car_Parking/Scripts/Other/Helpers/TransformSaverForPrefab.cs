using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public abstract class TransformSaverForPrefab : MonoBehaviour
	{
		protected abstract string PrefabPath { get; }
	}
}
