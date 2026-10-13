using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GraphicsRND.Rain.PuddleRipples
{
	public sealed class RainDropsCRTFeature : ScriptableRendererFeature
	{
		[Serializable]
		public class Settings
		{
			[Header("CRT")]
			public CustomRenderTexture crt;

			public string globalName = "_RainDropsXY";

			public RenderPassEvent passEvent;

			[Range(0.05f, 5f)]
			[Header("Ripples")]
			public float rippleTimeSpeed = 1f;

			[Min(0.0001f)]
			public float rippleTimeSmoothTime = 0.35f;

			[Min(0.0001f)]
			public float emissionRisePerSec = 2f;

			[Min(0.0001f)]
			public float emissionFallPerSec = 0.9f;

			[Range(0f, 1f)]
			public float baseWeight = 0.2f;

			[Range(0f, 8f)]
			public int steadyUpdatesPerFrame = 1;

			[Range(1f, 32f)]
			public int steadyUpdatesPerFrameWet = 1;

			[Range(0f, 1f)]
			public float emissionUpdateBoostThreshold = 0.25f;

			[Range(0f, 10f)]
			public float stopAfterDrySeconds = 2f;
		}

		private sealed class Pass : ScriptableRenderPass
		{
			private readonly Settings s;

			private readonly int _globalTexId;

			private static readonly int ID_RainIntensity = Shader.PropertyToID("_RainIntensity");

			private static readonly int ID_TimeSpeed = Shader.PropertyToID("_TimeSpeed");

			private static readonly int ID_Weight = Shader.PropertyToID("_Weight");

			private float _emission;

			private float _timeSpeed;

			private float _dryTailTimer;

			private int _lastTickFrame = -1;

			private readonly ProfilingSampler _profSampler;

			public Pass(Settings settings)
			{
				s = settings;
				_globalTexId = Shader.PropertyToID(settings.globalName);
				_profSampler = new ProfilingSampler("RainDrops CRT Update");
			}

			public void RegisterGlobalTexture()
			{
				if (s.crt != null)
				{
					Shader.SetGlobalTexture(_globalTexId, s.crt);
				}
			}

			private bool ShouldTickThisFrame()
			{
				int frameCount = Time.frameCount;
				if (frameCount == _lastTickFrame)
				{
					return false;
				}
				_lastTickFrame = frameCount;
				return true;
			}

			private void TickSimulation()
			{
				if (!ShouldTickThisFrame())
				{
					return;
				}
				CustomRenderTexture crt = s.crt;
				if (!(crt == null))
				{
					Shader.SetGlobalTexture(_globalTexId, crt);
					float num = Mathf.Clamp01(Shader.GetGlobalFloat(ID_RainIntensity));
					float num2 = DeltaTimeSafe();
					float maxDelta = ((num > _emission) ? s.emissionRisePerSec : s.emissionFallPerSec) * num2;
					_emission = Mathf.Clamp01(Mathf.MoveTowards(_emission, num, maxDelta));
					float num3 = 1f - Mathf.Exp((0f - num2) / s.rippleTimeSmoothTime);
					_timeSpeed += (s.rippleTimeSpeed - _timeSpeed) * num3;
					Shader.SetGlobalFloat(ID_TimeSpeed, _timeSpeed);
					Shader.SetGlobalFloat(ID_Weight, Mathf.Lerp(s.baseWeight, 1f, _emission));
					int num4 = ((_emission >= s.emissionUpdateBoostThreshold) ? s.steadyUpdatesPerFrameWet : s.steadyUpdatesPerFrame);
					if (num <= 0.0001f && _emission <= 0.0001f)
					{
						_dryTailTimer = Mathf.Max(0f, _dryTailTimer - num2);
					}
					else
					{
						_dryTailTimer = s.stopAfterDrySeconds;
					}
					if ((s.steadyUpdatesPerFrame != 0 || !(num <= 0.0001f) || !(_emission <= 0.0001f) || !(_dryTailTimer <= 0f)) && (num4 > 0 || _dryTailTimer > 0f))
					{
						crt.Update((num4 <= 0) ? 1 : num4);
					}
				}
			}

			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				if (!(s.crt == null))
				{
					TickSimulation();
				}
			}

			private float DeltaTimeSafe()
			{
				float deltaTime = Time.deltaTime;
				if (!(deltaTime > 0f))
				{
					return 0.0001f;
				}
				return deltaTime;
			}
		}

		public Settings settings = new Settings();

		private Pass _pass;

		public override void Create()
		{
			_pass = new Pass(settings)
			{
				renderPassEvent = settings.passEvent
			};
			if (settings.crt != null)
			{
				settings.crt.updateMode = CustomRenderTextureUpdateMode.OnDemand;
				_pass.RegisterGlobalTexture();
			}
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (RainWetnessDriver.HasActiveDriver && _pass != null && !(settings.crt == null))
			{
				CameraType cameraType = renderingData.cameraData.cameraType;
				if (cameraType != CameraType.Reflection && cameraType != CameraType.Preview)
				{
					renderer.EnqueuePass(_pass);
				}
			}
		}
	}
}
