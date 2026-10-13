using UnityEngine;
using UnityEngine.Pool;
using _Car_Parking.Scripts.Car.Visual;

namespace _Car_Parking.Scripts.Scopes
{
	public class WindshieldBrakeVfxPool
	{
		private readonly ObjectPool<WindshieldBrakeVfx> _pool;

		public WindshieldBrakeVfxPool(WindshieldBrakeVfx prefab)
		{
			Transform root = new GameObject("WindshieldBrakeVfxRoot").transform;
			_pool = new ObjectPool<WindshieldBrakeVfx>(() => Object.Instantiate(prefab, root), (WindshieldBrakeVfx vfx) =>
			{
				vfx.gameObject.SetActive(value: true);
			}, (WindshieldBrakeVfx vfx) =>
			{
				vfx.ReturnToPool();
				vfx.gameObject.SetActive(value: false);
			}, (WindshieldBrakeVfx vfx) =>
			{
				Object.Destroy(vfx.gameObject);
			}, collectionCheck: false, 2, 8);
		}

		public WindshieldBrakeVfx Spawn(Transform parent)
		{
			WindshieldBrakeVfx windshieldBrakeVfx = _pool.Get();
			windshieldBrakeVfx.ActivateAt(parent);
			return windshieldBrakeVfx;
		}

		public void Despawn(WindshieldBrakeVfx vfx)
		{
			_pool.Release(vfx);
		}
	}
}
