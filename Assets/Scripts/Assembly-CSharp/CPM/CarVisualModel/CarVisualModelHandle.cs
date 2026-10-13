using System;
using System.Collections.Generic;
using CPM.VisualModel;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CPM.CarVisualModel
{
	public sealed class CarVisualModelHandle : IVisualModelHandle, IDisposable
	{
		private AsyncOperationHandle _bodyHandle;

		private bool _bodyHandleValid;

		private Material _paintMaterial;

		private IReadOnlyList<AsyncOperationHandle> _extraHandles;

		private RenderTexture _vinylRT;

		private Material _windowMaterial;

		private RenderTexture _windowRT;

		private IReadOnlyList<Material> _wheelMaterials;

		private bool _disposed;

		public GameObject Root { get; }

		public Bounds Bounds { get; }

		public float FrontZSign => 1f;

		public int CarId { get; }

		public Material BodyMaterial => _paintMaterial;

		public CarVisualModelHandle(GameObject root, Bounds bounds, int carId, AsyncOperationHandle bodyHandle, bool bodyHandleValid, Material paintMaterial = null, IReadOnlyList<AsyncOperationHandle> extraHandles = null, RenderTexture vinylRT = null, Material windowMaterial = null, RenderTexture windowRT = null, IReadOnlyList<Material> wheelMaterials = null)
		{
			Root = root;
			Bounds = bounds;
			CarId = carId;
			_bodyHandle = bodyHandle;
			_bodyHandleValid = bodyHandleValid;
			_paintMaterial = paintMaterial;
			_extraHandles = extraHandles;
			_vinylRT = vinylRT;
			_windowMaterial = windowMaterial;
			_windowRT = windowRT;
			_wheelMaterials = wheelMaterials;
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}
			_disposed = true;
			if ((bool)Root)
			{
				UnityEngine.Object.Destroy(Root);
			}
			if (_bodyHandleValid && _bodyHandle.IsValid())
			{
				Addressables.Release(_bodyHandle);
			}
			_bodyHandleValid = false;
			if (_extraHandles != null)
			{
				for (int i = 0; i < _extraHandles.Count; i++)
				{
					if (_extraHandles[i].IsValid())
					{
						Addressables.Release(_extraHandles[i]);
					}
				}
				_extraHandles = null;
			}
			if ((bool)_paintMaterial)
			{
				UnityEngine.Object.Destroy(_paintMaterial);
			}
			_paintMaterial = null;
			if ((bool)_vinylRT)
			{
				CommonExtensions.DeleteRenderTex(_vinylRT);
				_vinylRT = null;
			}
			if ((bool)_windowMaterial)
			{
				UnityEngine.Object.Destroy(_windowMaterial);
			}
			_windowMaterial = null;
			if ((bool)_windowRT)
			{
				CommonExtensions.DeleteRenderTex(_windowRT);
				_windowRT = null;
			}
			if (_wheelMaterials == null)
			{
				return;
			}
			for (int j = 0; j < _wheelMaterials.Count; j++)
			{
				if ((bool)_wheelMaterials[j])
				{
					UnityEngine.Object.Destroy(_wheelMaterials[j]);
				}
			}
			_wheelMaterials = null;
		}
	}
}
