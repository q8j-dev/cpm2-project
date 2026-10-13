using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using _Car_Parking.Scripts.Steam.Perf;

namespace _Car_Parking.Scripts.Steam.Graphics
{
	public static class SteamMirrorTuning
	{
		public const float FarClip = 150f;

		public const float FastRate = 60f;

		public const float MidRate = 30f;

		public const float SlowRate = 15f;

		public const float InteriorOutsideRate = 5f;

		public const float SideVisibleOutsideRate = 5f;

		public const float SideHiddenOutsideRate = 0.1f;

		public const float InteriorViewDistance = 2.5f;

		private const float SideLodBias = 1f;

		private const float InteriorLodBias = 1.5f;

		private static readonly Dictionary<Camera, float> LodBiasByCamera = new Dictionary<Camera, float>();

		private static float _savedLodBias = -1f;

		private static bool _hooked;

		public static bool ClipEnabled => SteamPerfFlags.IsOn("mirrorClip");

		public static bool LodEnabled => SteamPerfFlags.IsOn("mirrorLod");

		public static bool RateEnabled => SteamPerfFlags.IsOn("mirrorRate");

		public static bool InteriorEnabled => SteamPerfFlags.IsOn("mirrorInterior");

		public static bool ExteriorEnabled => SteamPerfFlags.IsOn("mirrorExterior");

		public static void RegisterCamera(Camera camera, bool interior)
		{
			if ((bool)camera)
			{
				LodBiasByCamera[camera] = (interior ? 1.5f : 1f);
				if (!_hooked)
				{
					_hooked = true;
					RenderPipelineManager.beginCameraRendering += OnBeginCamera;
					RenderPipelineManager.endCameraRendering += OnEndCamera;
				}
			}
		}

		public static void UnregisterCamera(Camera camera)
		{
			if ((bool)camera)
			{
				LodBiasByCamera.Remove(camera);
			}
		}

		private static void OnBeginCamera(ScriptableRenderContext context, Camera camera)
		{
			if (LodEnabled && LodBiasByCamera.TryGetValue(camera, out var value))
			{
				_savedLodBias = QualitySettings.lodBias;
				QualitySettings.lodBias = Mathf.Min(_savedLodBias, value);
			}
		}

		private static void OnEndCamera(ScriptableRenderContext context, Camera camera)
		{
			if (!(_savedLodBias < 0f) && LodBiasByCamera.ContainsKey(camera))
			{
				QualitySettings.lodBias = _savedLodBias;
				_savedLodBias = -1f;
			}
		}
	}
}
