using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public sealed class RainConeRendererFeature : ScriptableRendererFeature
{
	[Serializable]
	public sealed class Stage
	{
		public Material Material;

		public Vector3 LocalScale = Vector3.one;

		public Vector3 ExtraWorldOffset = Vector3.zero;

		public bool Enabled = true;

		[NonSerialized]
		public Material _cachedMat;

		[NonSerialized]
		public int _passIndex = -2;
	}

	[Serializable]
	public sealed class Settings
	{
		[Header("Mesh")]
		public Mesh ConeMesh;

		public int SubMeshIndex;

		[Header("Stages")]
		public Stage[] Stages = new Stage[3]
		{
			new Stage
			{
				LocalScale = new Vector3(0.2f, 0.2f, 0.2f),
				Enabled = true
			},
			new Stage
			{
				LocalScale = new Vector3(0.3f, 0.3f, 0.3f),
				Enabled = true
			},
			new Stage
			{
				LocalScale = new Vector3(0.7f, 0.7f, 0.7f),
				Enabled = true
			}
		};

		[Header("Transform")]
		public float GlobalScale = 1f;

		public Vector3 WorldOffset = Vector3.zero;

		[Range(-180f, 180f)]
		public float WindYawDeg;

		[Header("Velocity Tilt")]
		public bool VelocityTiltEnabled = true;

		[Range(0f, 60f)]
		public float VelocityTiltMaxAngle = 25f;

		[Range(1f, 100f)]
		public float VelocityTiltMaxSpeed = 30f;

		[Range(0.01f, 0.5f)]
		public float VelocityTiltSmoothing = 0.12f;

		[Range(0f, 360f)]
		[Tooltip("Camera angular velocity (deg/s) above which tilt is suppressed. Prevents rain tilting during orbit-camera rotations. 0 = disabled.")]
		public float TiltSuppressAboveDegSec = 60f;

		[Header("Render")]
		public RenderPassEvent InjectionPoint = RenderPassEvent.BeforeRenderingTransparents;

		public bool DrawInSceneView = true;

		public bool SkipPreviewCameras = true;

		public int ForcedRenderQueue = -1;

		[Tooltip("Request _CameraDepthTexture from URP (needed for cone soft-particle depth fade). Disable on mobile TBDR GPUs (Mali/Adreno/PowerVR) to avoid tile memory resolve.")]
		public bool RequestDepthTexture = true;

		[Header("Fade (Global Shader Float)")]
		public string FadeGlobalName = "_RainConeFade";

		[Range(0f, 1f)]
		public float DefaultFade = 1f;

		[Tooltip("Fade the cone out inside RainDryZones (tunnels, covered areas) — the same mask that already stops the wipers, the puddle splashes and the screen droplets there. Replaces the per-tunnel URP Volumes, which cost a physics query each, per camera, every frame.")]
		public bool FadeInDryZones = true;

		[Range(0.01f, 100f)]
		[Tooltip("Fade-in rate (exp). The dry-zone mask is already soft in world space, so this can stay fast.")]
		public float FadeInSpeed = 100f;

		[Tooltip("Fade-out rate (exp).")]
		[Range(0.01f, 100f)]
		public float FadeOutSpeed = 100f;

		public bool FadeUnscaledTime = true;
	}

	private sealed class RainConePass : ScriptableRenderPass
	{
		private readonly Settings s;

		private readonly string tag;

		private readonly ProfilingSampler _profSampler;

		private Quaternion rot;

		private float _cachedYaw = float.NaN;

		private Vector3 _prevCamWorldPos;

		private bool _prevCamPosValid;

		private int _trackedCameraId;

		private Vector3 _smoothedVelXZ;

		private Vector3 _velXZDamp;

		private Quaternion _prevCamRot;

		private bool _prevCamRotValid;

		private float _tiltSuppressMul = 1f;

		private int fadeId;

		private float fade01;

		private bool fadeInit;

		private int _fadeFrame = -1;

		private float _lastPushedFade = float.NaN;

		private static readonly int ID_RainIntensity = Shader.PropertyToID("_RainIntensity");

		public RainConePass(string tag, Settings s)
		{
			this.tag = tag;
			this.s = s;
			fadeId = Shader.PropertyToID(string.IsNullOrEmpty(s.FadeGlobalName) ? "_RainConeFade" : s.FadeGlobalName);
			_profSampler = new ProfilingSampler(tag);
		}

		public void UpdateRotationIfNeeded()
		{
			if (_cachedYaw != s.WindYawDeg)
			{
				_cachedYaw = s.WindYawDeg;
				rot = Quaternion.Euler(0f, s.WindYawDeg, 0f);
			}
		}

		private Quaternion GetFrameRot(Camera cam)
		{
			if (!s.VelocityTiltEnabled)
			{
				return rot;
			}
			float deltaTime = Time.deltaTime;
			Vector3 position = cam.transform.position;
			int instanceID = cam.GetInstanceID();
			bool flag = _trackedCameraId != instanceID;
			_trackedCameraId = instanceID;
			if (flag)
			{
				_prevCamPosValid = false;
				_prevCamRotValid = false;
				_smoothedVelXZ = Vector3.zero;
				_velXZDamp = Vector3.zero;
				_tiltSuppressMul = 1f;
			}
			else if (_prevCamPosValid && deltaTime > 0.0001f)
			{
				Vector3 target = (position - _prevCamWorldPos) / deltaTime;
				target.y = 0f;
				float magnitude = target.magnitude;
				float num = Mathf.Max(1f, s.VelocityTiltMaxSpeed) * 4f;
				if (magnitude > num)
				{
					target *= num / magnitude;
				}
				_smoothedVelXZ = Vector3.SmoothDamp(_smoothedVelXZ, target, ref _velXZDamp, Mathf.Max(0.01f, s.VelocityTiltSmoothing), float.MaxValue, deltaTime);
			}
			_prevCamWorldPos = position;
			_prevCamPosValid = true;
			Quaternion rotation = cam.transform.rotation;
			_ = _smoothedVelXZ.magnitude;
			if (s.TiltSuppressAboveDegSec > 0.01f && CarVelocityXZ.magnitude < 5f)
			{
				float num2 = 1f;
				if (!flag && _prevCamRotValid && deltaTime > 0.0001f)
				{
					float num3 = Mathf.Clamp01(Mathf.Abs(Quaternion.Dot(_prevCamRot, rotation)));
					float num4 = 2f * Mathf.Sqrt(Mathf.Max(0f, 1f - num3 * num3)) / deltaTime;
					float num5 = Mathf.Max(1f, s.TiltSuppressAboveDegSec) * ((float)Math.PI / 180f);
					num2 = 1f - Mathf.Clamp01((num4 - num5) / num5);
				}
				float num6 = ((num2 < _tiltSuppressMul) ? 10f : 3f);
				_tiltSuppressMul = Mathf.MoveTowards(_tiltSuppressMul, num2, num6 * deltaTime);
			}
			else
			{
				_tiltSuppressMul = 1f;
			}
			_prevCamRot = rotation;
			_prevCamRotValid = true;
			float magnitude2 = _smoothedVelXZ.magnitude;
			if (magnitude2 < 0.05f)
			{
				return rot;
			}
			float num7 = Mathf.Clamp01(magnitude2 / Mathf.Max(0.01f, s.VelocityTiltMaxSpeed)) * s.VelocityTiltMaxAngle * _tiltSuppressMul;
			if (num7 < 0.01f)
			{
				return rot;
			}
			Vector3 axis = Vector3.Cross(Vector3.up, _smoothedVelXZ / magnitude2);
			return Quaternion.AngleAxis(num7, axis) * rot;
		}

		private static int ResolvePassIndex(Stage st)
		{
			Material material = st.Material;
			if (material == null)
			{
				st._cachedMat = null;
				st._passIndex = -1;
				return -1;
			}
			if (st._cachedMat == material && st._passIndex != -2)
			{
				return st._passIndex;
			}
			st._cachedMat = material;
			int num = material.FindPass("UniversalForward");
			st._passIndex = ((num >= 0) ? num : 0);
			return st._passIndex;
		}

		private RainConeFadeVolume GetFadeVolume()
		{
			return VolumeManager.instance.stack.GetComponent<RainConeFadeVolume>();
		}

		private void UpdateFadeForCamera(Camera cam)
		{
			if (!fadeInit)
			{
				fadeInit = true;
				fade01 = Mathf.Clamp01(s.DefaultFade);
			}
			if (Application.isPlaying)
			{
				int frameCount = Time.frameCount;
				if (_fadeFrame == frameCount)
				{
					return;
				}
				_fadeFrame = frameCount;
			}
			float num = 1f;
			float num2 = Mathf.Max(0.0001f, s.FadeInSpeed);
			float num3 = Mathf.Max(0.0001f, s.FadeOutSpeed);
			bool flag = s.FadeUnscaledTime;
			if (s.FadeInDryZones && cam != null)
			{
				num = RainDryZoneManager.WetMaskAt(cam.transform.position);
			}
			RainConeFadeVolume fadeVolume = GetFadeVolume();
			if (fadeVolume != null && fadeVolume.active && fadeVolume.EnableFade.value)
			{
				num = Mathf.Min(num, Mathf.Clamp01(fadeVolume.Target.value));
				num2 = Mathf.Max(0.0001f, fadeVolume.InSpeed.value);
				num3 = Mathf.Max(0.0001f, fadeVolume.OutSpeed.value);
				flag = fadeVolume.UseUnscaledTime.value;
			}
			float num4 = (flag ? Time.unscaledDeltaTime : Time.deltaTime);
			if (!(num4 <= 0f))
			{
				float num5 = ((num > fade01) ? num2 : num3);
				float num6 = 1f - Mathf.Exp((0f - num5) * num4);
				fade01 += (num - fade01) * num6;
				if (fade01 < 0.0001f)
				{
					fade01 = 0f;
				}
				else if (fade01 > 0.9999f)
				{
					fade01 = 1f;
				}
			}
		}

		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData rd)
		{
			if (s.RequestDepthTexture)
			{
				ConfigureInput(ScriptableRenderPassInput.Depth);
			}
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData rd)
		{
			Camera camera = rd.cameraData.camera;
			if ((s.SkipPreviewCameras && camera.cameraType == CameraType.Preview) || s.ConeMesh == null || s.Stages == null || s.Stages.Length == 0)
			{
				return;
			}
			if (Shader.GetGlobalFloat(ID_RainIntensity) < 0.0001f)
			{
				_prevCamPosValid = false;
				_smoothedVelXZ = Vector3.zero;
				_velXZDamp = Vector3.zero;
				return;
			}
			UpdateFadeForCamera(camera);
			Quaternion frameRot = GetFrameRot(camera);
			if (fade01 <= 0f && _lastPushedFade == 0f)
			{
				return;
			}
			int submeshIndex = Mathf.Clamp(s.SubMeshIndex, 0, Mathf.Max(0, s.ConeMesh.subMeshCount - 1));
			Vector3 vector = camera.transform.position + s.WorldOffset;
			CommandBuffer commandBuffer = CommandBufferPool.Get(tag);
			if (fade01 != _lastPushedFade)
			{
				commandBuffer.SetGlobalFloat(fadeId, fade01);
				_lastPushedFade = fade01;
			}
			if (fade01 <= 0f)
			{
				context.ExecuteCommandBuffer(commandBuffer);
				CommandBufferPool.Release(commandBuffer);
				return;
			}
			for (int i = 0; i < s.Stages.Length; i++)
			{
				Stage stage = s.Stages[i];
				if (stage == null || !stage.Enabled)
				{
					continue;
				}
				Material material = stage.Material;
				if (!(material == null))
				{
					Vector3 vector2 = stage.LocalScale * Mathf.Max(0.0001f, s.GlobalScale);
					Matrix4x4 matrix = Matrix4x4.TRS(vector + stage.ExtraWorldOffset, frameRot, vector2);
					int num = ResolvePassIndex(stage);
					if (num >= 0)
					{
						commandBuffer.DrawMesh(s.ConeMesh, matrix, material, submeshIndex, num);
					}
				}
			}
			context.ExecuteCommandBuffer(commandBuffer);
			CommandBufferPool.Release(commandBuffer);
		}
	}

	public static Vector3 CarVelocityXZ;

	public Settings settings = new Settings();

	private RainConePass pass;

	private void ApplyForcedRenderQueues()
	{
		if (settings.ForcedRenderQueue < 0 || settings.Stages == null)
		{
			return;
		}
		Stage[] stages = settings.Stages;
		foreach (Stage stage in stages)
		{
			if (stage?.Material != null && stage.Material.renderQueue != settings.ForcedRenderQueue)
			{
				stage.Material.renderQueue = settings.ForcedRenderQueue;
			}
		}
	}

	public override void Create()
	{
		pass = new RainConePass("Rain Cone Draw", settings)
		{
			renderPassEvent = settings.InjectionPoint
		};
		pass.UpdateRotationIfNeeded();
		ApplyForcedRenderQueues();
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (RainWetnessDriver.HasActiveDriver && renderingData.cameraData.cameraType != CameraType.Reflection)
		{
			if (pass.renderPassEvent != settings.InjectionPoint)
			{
				pass.renderPassEvent = settings.InjectionPoint;
			}
			pass.UpdateRotationIfNeeded();
			renderer.EnqueuePass(pass);
		}
	}
}
