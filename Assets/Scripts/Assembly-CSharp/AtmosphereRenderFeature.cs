using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AtmosphereRenderFeature : ScriptableRendererFeature
{
	private class CustomRenderPass : ScriptableRenderPass
	{
		public Material skyViewLutMaterial;

		public Texture2D m_transmittanceLut;

		public Texture2D m_multiScatteringLut;

		private RTHandle m_TransmittanceRTHandle;

		private RTHandle m_MultiScatteringRTHandle;

		private RTHandle m_SkyViewLutHandle;

		private static readonly int SkyViewLutID = Shader.PropertyToID("_skyViewLut");

		private static readonly int TransmittanceLutID = Shader.PropertyToID("_transmittanceLut");

		private static readonly int MultiScatteringLutID = Shader.PropertyToID("_multiScatteringLut");

		private static readonly int AerialPerspectiveVoxelSizeID = Shader.PropertyToID("_AerialPerspectiveVoxelSize");

		private void UpdateRTHandles()
		{
			if (m_transmittanceLut != null && (m_TransmittanceRTHandle == null || m_TransmittanceRTHandle.rt != m_transmittanceLut))
			{
				m_TransmittanceRTHandle?.Release();
				m_TransmittanceRTHandle = RTHandles.Alloc(m_transmittanceLut);
			}
			if (m_multiScatteringLut != null && (m_MultiScatteringRTHandle == null || m_MultiScatteringRTHandle.rt != m_multiScatteringLut))
			{
				m_MultiScatteringRTHandle?.Release();
				m_MultiScatteringRTHandle = RTHandles.Alloc(m_multiScatteringLut);
			}
			if (m_SkyViewLutHandle == null)
			{
				m_SkyViewLutHandle = RTHandles.Alloc(192, 100, 1, DepthBits.None, GraphicsFormat.R8G8B8A8_UNorm, FilterMode.Bilinear, TextureWrapMode.Clamp, TextureDimension.Tex2D, enableRandomWrite: false, useMipMap: false, autoGenerateMips: true, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: false, RenderTextureMemoryless.None, VRTextureUsage.None, "_skyViewLut_Persistent");
			}
		}

		public void Dispose()
		{
			m_TransmittanceRTHandle?.Release();
			m_TransmittanceRTHandle = null;
			m_MultiScatteringRTHandle?.Release();
			m_MultiScatteringRTHandle = null;
			m_SkyViewLutHandle?.Release();
			m_SkyViewLutHandle = null;
		}

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			UpdateRTHandles();
			if (m_SkyViewLutHandle != null)
			{
				cmd.SetGlobalTexture(SkyViewLutID, m_SkyViewLutHandle.rt);
			}
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (m_SkyViewLutHandle != null)
			{
				CommandBuffer commandBuffer = CommandBufferPool.Get("Atmosphere Pass (Legacy)");
				if (m_transmittanceLut != null)
				{
					commandBuffer.SetGlobalTexture(TransmittanceLutID, m_transmittanceLut);
				}
				if (m_multiScatteringLut != null)
				{
					commandBuffer.SetGlobalTexture(MultiScatteringLutID, m_multiScatteringLut);
				}
				commandBuffer.SetGlobalVector(AerialPerspectiveVoxelSizeID, new Vector4(32f, 32f, 32f, 0f));
				commandBuffer.Blit(null, m_SkyViewLutHandle.rt, skyViewLutMaterial);
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
				CommandBufferPool.Release(commandBuffer);
			}
		}

		public override void OnCameraCleanup(CommandBuffer cmd)
		{
		}
	}

	private CustomRenderPass m_ScriptablePass;

	public Material skyViewLutMaterial;

	public Texture2D m_transmittanceLut;

	public Texture2D m_multiScatteringLut;

	public override void Create()
	{
		m_ScriptablePass = new CustomRenderPass();
		m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRendering;
		m_ScriptablePass.skyViewLutMaterial = skyViewLutMaterial;
		m_ScriptablePass.m_transmittanceLut = m_transmittanceLut;
		m_ScriptablePass.m_multiScatteringLut = m_multiScatteringLut;
	}

	protected override void Dispose(bool disposing)
	{
		m_ScriptablePass?.Dispose();
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		renderer.EnqueuePass(m_ScriptablePass);
	}
}
