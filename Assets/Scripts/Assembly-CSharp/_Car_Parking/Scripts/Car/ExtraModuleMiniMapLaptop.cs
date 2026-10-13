using System;
using UnityEngine;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Car
{
	public class ExtraModuleMiniMapLaptop : VehicleDataExtraModule
	{
		private readonly Renderer _renderer;

		private readonly Material _defaultMaterial;

		private readonly FreeDriveProvider _freeDriveProvider;

		private Material _miniMapMaterial;

		public ExtraModuleMiniMapLaptop(CarInteriorReferences interior, VehicleData vehicleData, FreeDriveProvider freeDriveProvider)
			: base(vehicleData)
		{
			_freeDriveProvider = freeDriveProvider;
			if ((bool)interior && (bool)interior.MiniMap && interior.MiniMap.TryGetComponent<Renderer>(out _renderer))
			{
				_defaultMaterial = _renderer.sharedMaterial;
				_miniMapMaterial = new Material(Shader.Find("Custom/FlipTexture"));
				UpdateVisible();
				V.OnCarEntered += OnCarEnteredCallback;
				V.OnCarExit += OnCarExitCallback;
				V.OnInGarageFreeChanged = (Action<bool>)Delegate.Combine(V.OnInGarageFreeChanged, new Action<bool>(GarageFreeChangedCallback));
				if (_freeDriveProvider != null)
				{
					_freeDriveProvider.OnSetMapCameraEvent += MapCameraSetCallback;
					MapCameraSetCallback(new FreeDriveProvider.MapCameraSetEventData
					{
						OldMapCamera = _freeDriveProvider.MapCamera,
						NewMapCamera = _freeDriveProvider.MapCamera
					});
				}
			}
		}

		public override void Dispose()
		{
			CommonExtensions.DestroyMat(ref _miniMapMaterial);
			V.OnCarEntered -= OnCarEnteredCallback;
			V.OnCarExit -= OnCarExitCallback;
			V.OnInGarageFreeChanged = (Action<bool>)Delegate.Remove(V.OnInGarageFreeChanged, new Action<bool>(GarageFreeChangedCallback));
			if (_freeDriveProvider != null)
			{
				_freeDriveProvider.OnSetMapCameraEvent -= MapCameraSetCallback;
				if ((bool)_freeDriveProvider.MapCamera)
				{
					_freeDriveProvider.MapCamera.OnSmallRenderTextureCreatedEvent -= MapCameraSmallTextureCreatedCallback;
				}
			}
		}

		private void GarageFreeChangedCallback(bool inGarage)
		{
			UpdateVisible();
		}

		private void OnCarEnteredCallback(GameObject gameObject, VehicleData vehicleData)
		{
			UpdateVisible();
		}

		private void OnCarExitCallback(GameObject gameObject, VehicleData vehicleData)
		{
			UpdateVisible();
		}

		private void MapCameraSetCallback(FreeDriveProvider.MapCameraSetEventData eventData)
		{
			if ((bool)eventData.OldMapCamera)
			{
				eventData.OldMapCamera.OnSmallRenderTextureCreatedEvent -= MapCameraSmallTextureCreatedCallback;
			}
			if ((bool)eventData.NewMapCamera)
			{
				eventData.NewMapCamera.OnSmallRenderTextureCreatedEvent += MapCameraSmallTextureCreatedCallback;
				MapCameraSmallTextureCreatedCallback(eventData.NewMapCamera.rtSmall);
			}
		}

		private void MapCameraSmallTextureCreatedCallback(RenderTexture renderTexture)
		{
			if ((bool)_miniMapMaterial)
			{
				_miniMapMaterial.mainTexture = renderTexture;
			}
		}

		private void UpdateVisible()
		{
			if ((bool)_renderer)
			{
				bool flag = (bool)V.Car && V.Car == vehicleData.gameObject;
				if (flag)
				{
					flag = !FlagsStorage.InGarageFree && !FlagsStorage.InGarageMenu;
				}
				_renderer.sharedMaterial = (flag ? _miniMapMaterial : _defaultMaterial);
			}
		}
	}
}
