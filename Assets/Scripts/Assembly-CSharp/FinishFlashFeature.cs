using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class FinishFlashFeature : ScriptableRendererFeature
{
	[Serializable]
	public sealed class Settings
	{
		public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;

		[ColorUsage(true, true)]
		public Color flashTint = new Color(1f, 0.96f, 0.88f, 1f);

		[Range(0f, 2f)]
		public float flashGain = 2f;

		[Range(0f, 1f)]
		public float desaturate = 0.3f;

		[Range(0f, 20f)]
		public float bloomAtPeak = 2f;

		public Shader shader;
	}

	private sealed class FinishFlashPass : ScriptableRenderPass
	{
		private static readonly ProfilingSampler kSampler = new ProfilingSampler("FinishFlash");

		private static readonly int SID_Temp = Shader.PropertyToID("_FinishFlash_Temp");

		private readonly Material _mat;

		private RTHandle _temp;

		public FinishFlashPass(Material mat)
		{
			_mat = mat;
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (Shader.IsKeywordEnabled("_FINISH_FLASH_ON") && !(_mat == null))
			{
				CommandBuffer commandBuffer = CommandBufferPool.Get();
				using (new ProfilingScope(commandBuffer, kSampler))
				{
					ref CameraData cameraData = ref renderingData.cameraData;
					RTHandle cameraColorTargetHandle = cameraData.renderer.cameraColorTargetHandle;
					RenderTextureDescriptor descriptor = cameraData.cameraTargetDescriptor;
					descriptor.depthBufferBits = 0;
					RenderingUtils.ReAllocateIfNeeded(ref _temp, in descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, isShadowMap: false, 1, 0f, "_FinishFlash_Temp");
					Blitter.BlitCameraTexture(commandBuffer, cameraColorTargetHandle, _temp, _mat, 0);
					Blitter.BlitCameraTexture(commandBuffer, _temp, cameraColorTargetHandle);
				}
				context.ExecuteCommandBuffer(commandBuffer);
				CommandBufferPool.Release(commandBuffer);
			}
		}

		public void Dispose()
		{
			_temp?.Release();
			_temp = null;
		}
	}

	public const string KW_ON = "_FINISH_FLASH_ON";

	private static readonly int SID_Gain = Shader.PropertyToID("_FinishFlash_Gain");

	private static readonly int SID_Tint = Shader.PropertyToID("_FinishFlash_Tint");

	private static readonly int SID_Desat = Shader.PropertyToID("_FinishFlash_Desat");

	[SerializeField]
	private Settings settings = new Settings();

	private FinishFlashPass _pass;

	private Material _mat;

	public float BloomAtPeak => settings.bloomAtPeak;

	public override void Create()
	{
		if (_mat != null)
		{
			CoreUtils.Destroy(_mat);
		}
		Shader shader = ((settings.shader != null) ? settings.shader : Shader.Find("Hidden/Screen/FinishFlash"));
		_mat = ((shader != null) ? CoreUtils.CreateEngineMaterial(shader) : null);
		_pass = new FinishFlashPass(_mat)
		{
			renderPassEvent = settings.passEvent
		};
	}

	public void ApplyStaticUniforms()
	{
		Shader.SetGlobalFloat(SID_Gain, settings.flashGain);
		Shader.SetGlobalColor(SID_Tint, settings.flashTint);
		Shader.SetGlobalFloat(SID_Desat, settings.desaturate);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (!renderingData.cameraData.isPreviewCamera && renderingData.cameraData.cameraType == CameraType.Game && !(_mat == null))
		{
			ApplyStaticUniforms();
			renderer.EnqueuePass(_pass);
		}
	}
}
