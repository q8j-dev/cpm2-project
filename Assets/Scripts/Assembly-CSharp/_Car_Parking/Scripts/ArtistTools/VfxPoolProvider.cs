using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.ArtistTools.SO;

namespace _Car_Parking.Scripts.ArtistTools
{
	[Preserve]
	public class VfxPoolProvider : IDisposable
	{
		private const string DestroyablesSoPath = "DestroyablesSO/VfxModel";

		private const int PoolSize = 10;

		private VfxHandlerSO _handlerSO;

		private Dictionary<string, List<ParticleSystem>> _pool = new Dictionary<string, List<ParticleSystem>>();

		private Transform _vfxParent;

		public void Initialize()
		{
			_vfxParent = new GameObject("VfxPoolParent").transform;
			_vfxParent.position = Vector3.zero;
			_vfxParent.parent = V.GlobalPoint;
			_handlerSO = Resources.Load<VfxHandlerSO>("DestroyablesSO/VfxModel");
			_pool = new Dictionary<string, List<ParticleSystem>>();
			foreach (VfxModel model in _handlerSO.Models)
			{
				_pool.Add(model.ID, new List<ParticleSystem>());
				for (int i = 0; i < 10; i++)
				{
					InstantiatePoolObj(model.ID, model.ReferenceVFX);
				}
			}
		}

		private ParticleSystem InstantiatePoolObj(string id, ParticleSystem referenceVfx)
		{
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate(referenceVfx, _vfxParent);
			particleSystem.gameObject.SetActive(value: false);
			_pool[id].Add(particleSystem);
			return particleSystem;
		}

		public void Dispose()
		{
			if ((bool)_vfxParent)
			{
				UnityEngine.Object.Destroy(_vfxParent.gameObject);
			}
			_vfxParent = null;
			Resources.UnloadAsset(_handlerSO);
			if (_pool == null)
			{
				return;
			}
			foreach (KeyValuePair<string, List<ParticleSystem>> item in _pool)
			{
				if (item.Value != null)
				{
					CommonExtensions.ClearGOCollection(item.Value);
				}
			}
			_pool.Clear();
			_pool = null;
		}

		public ParticleSystem GetFromPool(string id)
		{
			if (_pool == null || !_pool.TryGetValue(id, out var value))
			{
				return null;
			}
			if (value.IsNullOrEmpty())
			{
				return null;
			}
			ParticleSystem particleSystem = ((!value.Any((ParticleSystem x) => (bool)x && !x.gameObject.activeInHierarchy)) ? InstantiatePoolObj(id, _handlerSO.Models.First((VfxModel x) => x.ID == id).ReferenceVFX) : value.First((ParticleSystem x) => (bool)x && !x.gameObject.activeInHierarchy));
			particleSystem.gameObject.SetActive(value: true);
			return particleSystem;
		}

		public void ReturnToPool(ParticleSystem vfxObj)
		{
			if ((bool)vfxObj)
			{
				vfxObj.transform.parent = _vfxParent;
				vfxObj.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				vfxObj.gameObject.SetActive(value: false);
			}
		}
	}
}
