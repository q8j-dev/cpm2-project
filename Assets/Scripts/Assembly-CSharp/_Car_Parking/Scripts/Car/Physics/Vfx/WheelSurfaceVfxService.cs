using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Car.Physics.Vfx
{
	public class WheelSurfaceVfxService : IInitializable, IDisposable
	{
		private readonly IObjectResolver _resolver;

		private GameObject _root;

		public WheelSurfaceVfxRuntime Runtime { get; private set; }

		[Inject]
		public WheelSurfaceVfxService(IObjectResolver resolver)
		{
			_resolver = resolver;
		}

		public void Initialize()
		{
			V.WheelSurfaceVfxService = this;
			if (_resolver.TryResolve<WheelSurfaceVfxLibrary>(out var resolved) && (bool)resolved && resolved.entries != null && resolved.entries.Count != 0)
			{
				_root = new GameObject("[WheelSurfaceVfxRoot]");
				UnityEngine.Object.DontDestroyOnLoad(_root);
				Runtime = WheelSurfaceVfxRuntime.Build(resolved, _root.transform);
			}
		}

		public void Dispose()
		{
			Runtime?.Dispose();
			Runtime = null;
			if ((bool)_root)
			{
				UnityEngine.Object.Destroy(_root);
			}
			_root = null;
			if (V.WheelSurfaceVfxService == this)
			{
				V.WheelSurfaceVfxService = null;
			}
		}
	}
}
