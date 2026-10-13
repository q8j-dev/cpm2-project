using System;
using CPM.Scripts.VinylDrawer;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CPM.CarVisualModel
{
	public sealed class CarModelVinylPerformer : MonoBehaviour, IVinylDrawPerformer
	{
		private int _carId;

		private OneVinylsPreset _preset;

		private Material _bodyMaterial;

		private Material _windowMaterial;

		private VinylDrawer _drawer;

		private PlayerDatas.VinylsDataUtility _util;

		private VinylsType _lastType;

		private bool _cancelled;

		private RenderTexture _vinylRT;

		private RenderTexture _windowRT;

		private UniTaskCompletionSource _done;

		public MonoBehaviour ComponentToCheck => this;

		public UniTask Completion => _done?.Task ?? UniTask.CompletedTask;

		public void Init(int carId, OneVinylsPreset preset, Material bodyMaterial, Material windowMaterial, VinylDrawer drawer, PlayerDatas.VinylsDataUtility util)
		{
			_carId = carId;
			_preset = preset;
			_bodyMaterial = bodyMaterial;
			_windowMaterial = windowMaterial;
			_drawer = drawer;
			_util = util;
			_lastType = ((!windowMaterial || CarVinylRenderer.WindowVinylCount(preset) <= 0) ? VinylsType.CarVinyls : VinylsType.WindowVinyls);
			_done = new UniTaskCompletionSource();
		}

		public void Cancel()
		{
			_cancelled = true;
			_done?.TrySetResult();
		}

		public void TakeTextures(out RenderTexture vinylRT, out RenderTexture windowRT)
		{
			vinylRT = _vinylRT;
			windowRT = _windowRT;
			_vinylRT = null;
			_windowRT = null;
		}

		public int GetVinylsCount(VinylsType type, bool isFrontPlate)
		{
			if (_cancelled || _preset == null)
			{
				return 0;
			}
			if (type == VinylsType.CarVinyls && (bool)_bodyMaterial)
			{
				return CarVinylRenderer.BodyVinylCount(_preset);
			}
			if (type == VinylsType.WindowVinyls && (bool)_windowMaterial)
			{
				return CarVinylRenderer.WindowVinylCount(_preset);
			}
			return 0;
		}

		public void RenderVinyls(VinylsType type, bool isFrontPlate)
		{
			try
			{
				switch (type)
				{
				case VinylsType.CarVinyls:
					_vinylRT = _drawer.GetRenderTexture(VinylsType.CarVinyls);
					_bodyMaterial.mainTexture = _vinylRT;
					_util.LoadAllVinyls(_preset.vynilOneCarList, VinylsType.CarVinyls);
					_drawer.FocusOnVinyls(VinylsType.CarVinyls);
					_drawer.TakeSnapshot();
					break;
				case VinylsType.WindowVinyls:
					_windowRT = _drawer.GetRenderTexture(VinylsType.WindowVinyls);
					_windowMaterial.mainTexture = _windowRT;
					_util.LoadAllVinyls(_preset.windowVynilOneCarList, VinylsType.WindowVinyls);
					_drawer.FocusOnVinyls(VinylsType.WindowVinyls);
					_drawer.TakeSnapshot();
					break;
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				if (type == VinylsType.CarVinyls && (bool)_bodyMaterial && _preset != null)
				{
					_bodyMaterial.color = _preset.carColor;
				}
				if ((bool)_drawer)
				{
					_drawer.DeleteVinylContext(type);
				}
				if (type == _lastType)
				{
					_done?.TrySetResult();
				}
			}
		}

		private void OnDestroy()
		{
			if ((bool)_vinylRT)
			{
				CommonExtensions.DeleteRenderTex(_vinylRT);
				_vinylRT = null;
			}
			if ((bool)_windowRT)
			{
				CommonExtensions.DeleteRenderTex(_windowRT);
				_windowRT = null;
			}
			_cancelled = true;
			_done?.TrySetResult();
		}
	}
}
