using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace HighlightPlus
{
	[ExecuteInEditMode]
	public class HighlightEffect : MonoBehaviour
	{
		private struct ModelMaterials
		{
			public bool render;

			public Transform transform;

			public bool bakedTransform;

			public Vector3 currentPosition;

			public Vector3 currentRotation;

			public Vector3 currentScale;

			public bool renderWasVisibleDuringSetup;

			public Mesh mesh;

			public Mesh originalMesh;

			public Renderer renderer;

			public bool preserveOriginalMesh;

			public Material[] fxMatMask;

			public Material[] fxMatSolidColor;

			public Material[] fxMatSeeThroughInner;

			public Material[] fxMatSeeThroughBorder;

			public Material[] fxMatOverlay;

			public Material[] fxMatInnerGlow;

			public Matrix4x4 renderingMatrix;
		}

		private enum FadingState
		{
			FadingOut = -1,
			NoFading = 0,
			FadingIn = 1
		}

		public HighlightProfile profile;

		[Tooltip("If enabled, settings will be synced with profile.")]
		public bool profileSync;

		public bool previewInEditor = true;

		public TargetOptions effectGroup;

		public LayerMask effectGroupLayer = -1;

		[Range(0f, 1f)]
		public float alphaCutOff;

		public bool cullBackFaces = true;

		[Tooltip("Show highlight effects even if the object is not visible. If this object or its children use GPU Instancing tools, the MeshRenderer can be disabled although the object is visible. In this case, this option is useful to enable highlighting.")]
		public bool ignoreObjectVisibility;

		[Tooltip("Support reflection probes. Enable only if you want the effects to be visible in reflections.")]
		public bool reflectionProbes;

		[Tooltip("Enables GPU instancing. Reduces draw calls in outline and outer glow effects on platforms that support GPU instancing. Should be enabled by default.")]
		public bool GPUInstancing = true;

		[Tooltip("Enables depth buffer clipping. Only applies to outline or outer glow in High Quality mode.")]
		public bool depthClip;

		[Tooltip("Do not smooth normals and use the original mesh to draw the effects.")]
		public bool preserveOriginalMesh;

		[Tooltip("Ignore highlighting on this object.")]
		public bool ignore;

		[SerializeField]
		private bool _highlighted;

		public float fadeInDuration;

		public float fadeOutDuration;

		public bool flipY;

		public bool constantWidth = true;

		[Range(0f, 1f)]
		public float overlay = 0.5f;

		public Color overlayColor = Color.yellow;

		public float overlayAnimationSpeed = 1f;

		[Range(0f, 1f)]
		public float overlayMinIntensity = 0.5f;

		[Range(0f, 1f)]
		public float overlayBlending = 1f;

		[Range(0f, 1f)]
		public float outline = 1f;

		public Color outlineColor = Color.black;

		public float outlineWidth = 0.45f;

		public QualityLevel outlineQuality = QualityLevel.High;

		[Range(1f, 8f)]
		public int outlineDownsampling = 2;

		public Visibility outlineVisibility;

		public bool outlineOptimalBlit = true;

		public bool outlineBlitDebug;

		public bool outlineIndependent;

		[Range(0f, 5f)]
		public float glow = 1f;

		public float glowWidth = 0.4f;

		public QualityLevel glowQuality = QualityLevel.High;

		[Range(1f, 8f)]
		public int glowDownsampling = 2;

		public Color glowHQColor = new Color(0.64f, 1f, 0f, 1f);

		public bool glowDithering = true;

		public GlowBlendMode glowBlendMode;

		public float glowMagicNumber1 = 0.75f;

		public float glowMagicNumber2 = 0.5f;

		public float glowAnimationSpeed = 1f;

		public Visibility glowVisibility;

		public bool glowOptimalBlit = true;

		public bool glowBlitDebug;

		public GlowPassData[] glowPasses;

		[Range(0f, 5f)]
		public float innerGlow;

		[Range(0f, 2f)]
		public float innerGlowWidth = 1f;

		public Color innerGlowColor = Color.white;

		public Visibility innerGlowVisibility;

		public bool targetFX;

		public Texture2D targetFXTexture;

		public Color targetFXColor = Color.white;

		public Transform targetFXCenter;

		public float targetFXRotationSpeed = 50f;

		public float targetFXInitialScale = 4f;

		public float targetFXEndScale = 1.5f;

		public float targetFXTransitionDuration = 0.5f;

		public float targetFXStayDuration = 1.5f;

		public SeeThroughMode seeThrough;

		public LayerMask seeThroughOccluderMask = -1;

		[Range(0.01f, 0.6f)]
		public float seeThroughOccluderThreshold = 0.3f;

		public float seeThroughOccluderCheckInterval = 1f;

		[Range(0f, 5f)]
		public float seeThroughIntensity = 0.8f;

		[Range(0f, 1f)]
		public float seeThroughTintAlpha = 0.5f;

		public Color seeThroughTintColor = Color.red;

		[Range(0f, 1f)]
		public float seeThroughNoise = 1f;

		[Range(0f, 1f)]
		public float seeThroughBorder;

		public Color seeThroughBorderColor = Color.black;

		public float seeThroughBorderWidth = 0.45f;

		[HideInInspector]
		[SerializeField]
		private ModelMaterials[] rms;

		[SerializeField]
		[HideInInspector]
		private int rmsCount;

		[NonSerialized]
		public Transform target;

		[NonSerialized]
		public float highlightStartTime;

		private const string SKW_ALPHACLIP = "HP_ALPHACLIP";

		private const string SKW_DEPTHCLIP = "HP_DEPTHCLIP";

		private const string UNIFORM_CUTOFF = "_CutOff";

		private const float TAU = 0.70711f;

		private static Material fxMatMask;

		private static Material fxMatSolidColor;

		private static Material fxMatSeeThroughInner;

		private static Material fxMatSeeThroughBorder;

		private static Material fxMatOverlay;

		private static Material fxMatClearStencil;

		private static Material fxMatGlowRef;

		private static Material fxMatInnerGlow;

		private static Material fxMatOutlineRef;

		private static Material fxMatTargetRef;

		private static Material fxMatComposeGlowRef;

		private static Material fxMatComposeOutlineRef;

		private static Material fxMatBlurGlowRef;

		private static Material fxMatBlurOutlineRef;

		private Material _fxMatOutline;

		private Material _fxMatGlow;

		private Material _fxMatTarget;

		private Material _fxMatComposeGlow;

		private Material _fxMatComposeOutline;

		private Material _fxMatBlurGlow;

		private Material _fxMatBlurOutline;

		private static Vector3[] offsets;

		private float fadeStartTime;

		private FadingState fading;

		private CommandBuffer cbMask;

		private CommandBuffer cbSeeThrough;

		private CommandBuffer cbGlow;

		private CommandBuffer cbOutline;

		private CommandBuffer cbOverlay;

		private CommandBuffer cbInnerGlow;

		private CommandBuffer cbSmoothBlend;

		private int[] mipGlowBuffers;

		private int[] mipOutlineBuffers;

		private int glowRT;

		private int outlineRT;

		private static Mesh quadMesh;

		private int sourceRT;

		private Matrix4x4 quadGlowMatrix;

		private Matrix4x4 quadOutlineMatrix;

		private Vector3[] corners;

		private RenderTextureDescriptor sourceDesc;

		private Color debugColor;

		private Color blackColor;

		private Visibility lastOutlineVisibility;

		private bool requireUpdateMaterial;

		private bool usingPipeline;

		private float occlusionCheckLastTime;

		private bool lastOcclusionTestResult;

		private bool useGPUInstancing;

		private MaterialPropertyBlock glowPropertyBlock;

		private MaterialPropertyBlock outlinePropertyBlock;

		private static List<Vector4> matDataDirection = new List<Vector4>();

		private static List<Vector4> matDataGlow = new List<Vector4>();

		private static List<Vector4> matDataColor = new List<Vector4>();

		private static Matrix4x4[] matrices;

		public static List<HighlightEffect> effects = new List<HighlightEffect>();

		private bool skipThisFrame;

		private List<Renderer> tempRR;

		private static List<Vector3> vertices;

		private static List<Vector3> normals;

		private static Vector3[] newNormals;

		private static int[] matches;

		private static Dictionary<Vector3, int> vv = new Dictionary<Vector3, int>();

		private static Dictionary<Mesh, Mesh> smoothMeshes = new Dictionary<Mesh, Mesh>();

		private float hitInitialIntensity;

		private float hitStartTime;

		private float hitFadeOutDuration;

		private Color hitColor;

		private bool hitActive;

		private static List<HighlightSeeThroughOccluder> occluders = new List<HighlightSeeThroughOccluder>();

		private static Dictionary<Camera, int> occludersFrameCount = new Dictionary<Camera, int>();

		private static CommandBuffer cbOccluder;

		private static Material fxMatOccluder;

		private static RaycastHit[] hits;

		private bool cancelSeeThroughThisFrame;

		public bool highlighted
		{
			get
			{
				return _highlighted;
			}
			set
			{
				SetHighlighted(value);
			}
		}

		private Material fxMatOutline
		{
			get
			{
				if (_fxMatOutline == null && fxMatOutlineRef != null)
				{
					_fxMatOutline = UnityEngine.Object.Instantiate(fxMatOutlineRef);
					if (useGPUInstancing)
					{
						_fxMatOutline.enableInstancing = true;
					}
					else
					{
						_fxMatOutline.enableInstancing = false;
					}
				}
				return _fxMatOutline;
			}
		}

		private Material fxMatGlow
		{
			get
			{
				if (_fxMatGlow == null && fxMatGlowRef != null)
				{
					_fxMatGlow = UnityEngine.Object.Instantiate(fxMatGlowRef);
					if (useGPUInstancing)
					{
						_fxMatGlow.enableInstancing = true;
					}
					else
					{
						_fxMatGlow.enableInstancing = false;
					}
				}
				return _fxMatGlow;
			}
		}

		private Material fxMatTarget
		{
			get
			{
				if (_fxMatTarget == null && fxMatTargetRef != null)
				{
					_fxMatTarget = UnityEngine.Object.Instantiate(fxMatTargetRef);
				}
				return _fxMatTarget;
			}
		}

		private Material fxMatComposeGlow
		{
			get
			{
				if (_fxMatComposeGlow == null && fxMatComposeGlowRef != null)
				{
					_fxMatComposeGlow = UnityEngine.Object.Instantiate(fxMatComposeGlowRef);
				}
				return _fxMatComposeGlow;
			}
		}

		private Material fxMatComposeOutline
		{
			get
			{
				if (_fxMatComposeOutline == null && fxMatComposeOutlineRef != null)
				{
					_fxMatComposeOutline = UnityEngine.Object.Instantiate(fxMatComposeOutlineRef);
				}
				return _fxMatComposeOutline;
			}
		}

		private Material fxMatBlurGlow
		{
			get
			{
				if (_fxMatBlurGlow == null && fxMatBlurGlowRef != null)
				{
					_fxMatBlurGlow = UnityEngine.Object.Instantiate(fxMatBlurGlowRef);
				}
				return _fxMatBlurGlow;
			}
		}

		private Material fxMatBlurOutline
		{
			get
			{
				if (_fxMatBlurOutline == null && fxMatBlurOutlineRef != null)
				{
					_fxMatBlurOutline = UnityEngine.Object.Instantiate(fxMatBlurOutlineRef);
				}
				return _fxMatBlurOutline;
			}
		}

		public event OnObjectHighlightStartEvent OnObjectHighlightStart;

		public event OnObjectHighlightEndEvent OnObjectHighlightEnd;

		public event OnRendererHighlightEvent OnRendererHighlightStart;

		private void OnEnable()
		{
			lastOutlineVisibility = outlineVisibility;
			debugColor = new Color(1f, 0f, 0f, 0.5f);
			blackColor = new Color(0f, 0f, 0f, 0f);
			if (offsets == null || offsets.Length != 8)
			{
				offsets = new Vector3[8]
				{
					Vector3.up,
					Vector3.right,
					Vector3.down,
					Vector3.left,
					new Vector3(-0.70711f, 0.70711f, 0f),
					new Vector3(0.70711f, 0.70711f, 0f),
					new Vector3(0.70711f, -0.70711f, 0f),
					new Vector3(-0.70711f, -0.70711f, 0f)
				};
			}
			if (corners == null || corners.Length != 8)
			{
				corners = new Vector3[8];
			}
			if (quadMesh == null)
			{
				BuildQuad();
			}
			if (target == null)
			{
				target = base.transform;
			}
			if (profileSync && profile != null)
			{
				profile.Load(this);
			}
			if (glowPasses == null || glowPasses.Length == 0)
			{
				glowPasses = new GlowPassData[4];
				glowPasses[0] = new GlowPassData
				{
					offset = 4f,
					alpha = 0.1f,
					color = new Color(0.64f, 1f, 0f, 1f)
				};
				glowPasses[1] = new GlowPassData
				{
					offset = 3f,
					alpha = 0.2f,
					color = new Color(0.64f, 1f, 0f, 1f)
				};
				glowPasses[2] = new GlowPassData
				{
					offset = 2f,
					alpha = 0.3f,
					color = new Color(0.64f, 1f, 0f, 1f)
				};
				glowPasses[3] = new GlowPassData
				{
					offset = 1f,
					alpha = 0.4f,
					color = new Color(0.64f, 1f, 0f, 1f)
				};
			}
			sourceRT = Shader.PropertyToID("_HPSourceRT");
			useGPUInstancing = GPUInstancing && SystemInfo.supportsInstancing;
			if (useGPUInstancing)
			{
				if (glowPropertyBlock == null)
				{
					glowPropertyBlock = new MaterialPropertyBlock();
				}
				if (outlinePropertyBlock == null)
				{
					outlinePropertyBlock = new MaterialPropertyBlock();
				}
			}
			CheckGeometrySupportDependencies();
			SetupMaterial();
			RenderPipelineManager.endCameraRendering -= SRPAfterCameraRenders;
			RenderPipelineManager.endCameraRendering += SRPAfterCameraRenders;
			if (!effects.Contains(this))
			{
				effects.Add(this);
			}
		}

		private void OnDisable()
		{
			RenderPipelineManager.endCameraRendering -= SRPAfterCameraRenders;
			UpdateMaterialPropertiesNow();
		}

		private void SRPAfterCameraRenders(ScriptableRenderContext context, Camera cam)
		{
			if (!(this == null) && base.isActiveAndEnabled)
			{
				usingPipeline = true;
				DoOnRenderObject(cam);
			}
		}

		private void Reset()
		{
			SetupMaterial();
		}

		private void DestroyMaterial(Material mat)
		{
			if (mat != null)
			{
				UnityEngine.Object.DestroyImmediate(mat);
			}
		}

		private void DestroyMaterialArray(Material[] mm)
		{
			if (mm != null)
			{
				for (int i = 0; i < mm.Length; i++)
				{
					DestroyMaterial(mm[i]);
				}
			}
		}

		private void OnDestroy()
		{
			if (rms != null)
			{
				for (int i = 0; i < rms.Length; i++)
				{
					DestroyMaterialArray(rms[i].fxMatMask);
					DestroyMaterialArray(rms[i].fxMatSolidColor);
					DestroyMaterialArray(rms[i].fxMatSeeThroughInner);
					DestroyMaterialArray(rms[i].fxMatSeeThroughBorder);
					DestroyMaterialArray(rms[i].fxMatOverlay);
					DestroyMaterialArray(rms[i].fxMatInnerGlow);
				}
			}
			DestroyMaterial(fxMatGlow);
			DestroyMaterial(fxMatOutline);
			DestroyMaterial(fxMatTarget);
			DestroyMaterial(fxMatComposeGlow);
			DestroyMaterial(fxMatComposeOutline);
			DestroyMaterial(fxMatBlurGlow);
			DestroyMaterial(fxMatBlurOutline);
			if (effects.Contains(this))
			{
				effects.Remove(this);
			}
			UnityEngine.Object.Destroy(quadMesh);
			foreach (KeyValuePair<Mesh, Mesh> smoothMesh in smoothMeshes)
			{
				UnityEngine.Object.Destroy(smoothMesh.Value);
			}
			smoothMeshes.Clear();
		}

		public static void DrawEffectsNow(Camera cam = null)
		{
			if (cam == null)
			{
				cam = Camera.current;
				if (cam == null)
				{
					return;
				}
			}
			int count = effects.Count;
			for (int i = 0; i < count; i++)
			{
				HighlightEffect highlightEffect = effects[i];
				if (highlightEffect != null)
				{
					highlightEffect.DoOnRenderObject(cam);
					highlightEffect.skipThisFrame = true;
				}
			}
		}

		private void OnRenderObject()
		{
			if (!usingPipeline)
			{
				DoOnRenderObject(Camera.current);
			}
		}

		public void ProfileLoad(HighlightProfile profile)
		{
			if (profile != null)
			{
				profile.Load(this);
			}
		}

		public void ProfileReload()
		{
			if (profile != null)
			{
				profile.Load(this);
			}
		}

		public void ProfileSaveChanges(HighlightProfile profile)
		{
			if (profile != null)
			{
				profile.Save(this);
			}
		}

		public void ProfileSaveChanges()
		{
			if (profile != null)
			{
				profile.Save(this);
			}
		}

		public void Refresh()
		{
			if (base.enabled)
			{
				SetupMaterial();
			}
		}

		private void DoOnRenderObject(Camera cam)
		{
			if (cam == null || (!reflectionProbes && cam.cameraType == CameraType.Reflection))
			{
				return;
			}
			if (requireUpdateMaterial)
			{
				requireUpdateMaterial = false;
				UpdateMaterialPropertiesNow();
			}
			bool flag = seeThroughIntensity > 0f && (seeThrough == SeeThroughMode.AlwaysWhenOccluded || (seeThrough == SeeThroughMode.WhenHighlighted && _highlighted));
			if (flag)
			{
				RenderOccluders(cam);
			}
			if (cancelSeeThroughThisFrame)
			{
				cancelSeeThroughThisFrame = false;
				flag = false;
			}
			if (flag && (int)seeThroughOccluderMask != -1)
			{
				flag = CheckOcclusion(cam);
			}
			if (!_highlighted && !flag && !hitActive)
			{
				return;
			}
			if (skipThisFrame)
			{
				skipThisFrame = false;
			}
			else
			{
				if (rms == null)
				{
					return;
				}
				int cullingMask = cam.cullingMask;
				if (!ignoreObjectVisibility)
				{
					for (int i = 0; i < rmsCount; i++)
					{
						if (rms[i].renderer != null && rms[i].renderer.isVisible != rms[i].renderWasVisibleDuringSetup)
						{
							SetupMaterial();
							break;
						}
					}
				}
				float num = (_highlighted ? glow : 0f);
				_ = base.gameObject.layer;
				if (fxMatMask == null)
				{
					return;
				}
				bool flag2 = glow > 0f && glowQuality == QualityLevel.Highest;
				bool flag3 = outline > 0f && outlineQuality == QualityLevel.Highest;
				bool flag4 = flag2 | flag3;
				if (flag4 && (flag2 & flag3))
				{
					outlineVisibility = glowVisibility;
				}
				Visibility visibility = glowVisibility;
				Visibility visibility2 = outlineVisibility;
				if (flag4)
				{
					if (cam.allowMSAA && QualitySettings.antiAliasing > 1)
					{
						if (depthClip)
						{
							cam.depthTextureMode |= DepthTextureMode.Depth;
						}
						visibility = (visibility2 = Visibility.AlwaysOnTop);
					}
					else if (XRSettings.enabled && Application.isPlaying)
					{
						if (depthClip)
						{
							cam.depthTextureMode |= DepthTextureMode.Depth;
						}
						visibility = (visibility2 = Visibility.AlwaysOnTop);
					}
				}
				bool flag5 = true;
				bool flag6 = false;
				for (int j = 0; j < rmsCount; j++)
				{
					rms[j].render = false;
					Transform transform = rms[j].transform;
					if (transform == null)
					{
						continue;
					}
					Mesh mesh = rms[j].mesh;
					if (mesh == null || ((1 << transform.gameObject.layer) & cullingMask) == 0 || !rms[j].renderer.isVisible)
					{
						continue;
					}
					rms[j].render = true;
					flag6 = true;
					if (!rms[j].preserveOriginalMesh)
					{
						Vector3 lossyScale = transform.lossyScale;
						Vector3 position = transform.position;
						if (rms[j].bakedTransform)
						{
							if (rms[j].currentPosition != transform.position || rms[j].currentRotation != transform.eulerAngles || rms[j].currentScale != transform.lossyScale)
							{
								BakeTransform(j, duplicateMesh: true);
							}
							rms[j].renderingMatrix = Matrix4x4.identity;
						}
						else
						{
							rms[j].renderingMatrix = Matrix4x4.TRS(position, transform.rotation, lossyScale);
						}
					}
					if (outlineIndependent)
					{
						if (flag4 & flag5)
						{
							flag5 = false;
							fxMatClearStencil.SetPass(0);
							Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
						}
						else if (outline > 0f)
						{
							Material material = fxMatOutline;
							for (int k = 0; k < mesh.subMeshCount; k++)
							{
								if (outlineQuality == QualityLevel.High)
								{
									for (int l = 0; l < offsets.Length; l++)
									{
										Vector3 vector = offsets[l] * (outlineWidth / 100f);
										vector.y *= cam.aspect;
										material.SetVector("_OutlineDirection", vector);
										if (rms[j].preserveOriginalMesh)
										{
											cbOutline.Clear();
											cbOutline.DrawRenderer(rms[j].renderer, material, k, 1);
											Graphics.ExecuteCommandBuffer(cbOutline);
										}
										else
										{
											material.SetPass(1);
											Graphics.DrawMeshNow(mesh, rms[j].renderingMatrix, k);
										}
									}
								}
								else if (rms[j].preserveOriginalMesh)
								{
									cbOutline.Clear();
									cbOutline.DrawRenderer(rms[j].renderer, material, k, 1);
									Graphics.ExecuteCommandBuffer(cbOutline);
								}
								else
								{
									material.SetPass(1);
									Graphics.DrawMeshNow(mesh, rms[j].renderingMatrix, k);
								}
							}
						}
					}
					if (rms[j].preserveOriginalMesh)
					{
						cbMask.Clear();
						for (int m = 0; m < mesh.subMeshCount; m++)
						{
							if (_highlighted && ((outline > 0f && visibility2 != Visibility.Normal) || (glow > 0f && visibility != Visibility.Normal) || (innerGlow > 0f && innerGlowVisibility != Visibility.Normal)))
							{
								rms[j].fxMatMask[m].SetInt("_ZTest", 8);
							}
							else
							{
								rms[j].fxMatMask[m].SetInt("_ZTest", 4);
							}
							cbMask.DrawRenderer(rms[j].renderer, rms[j].fxMatMask[m], m);
						}
						Graphics.ExecuteCommandBuffer(cbMask);
						continue;
					}
					for (int n = 0; n < mesh.subMeshCount; n++)
					{
						if (_highlighted && ((outline > 0f && visibility2 != Visibility.Normal) || (glow > 0f && visibility != Visibility.Normal) || (innerGlow > 0f && innerGlowVisibility != Visibility.Normal)))
						{
							rms[j].fxMatMask[n].SetInt("_ZTest", 8);
						}
						else
						{
							rms[j].fxMatMask[n].SetInt("_ZTest", 4);
						}
						rms[j].fxMatMask[n].SetPass(0);
						Graphics.DrawMeshNow(mesh, rms[j].renderingMatrix, n);
					}
				}
				if (!flag6)
				{
					return;
				}
				float num2 = 1f;
				if (fading != FadingState.NoFading)
				{
					if (fading == FadingState.FadingIn)
					{
						if (fadeInDuration > 0f)
						{
							num2 = (Time.time - fadeStartTime) / fadeInDuration;
							if (num2 > 1f)
							{
								num2 = 1f;
								fading = FadingState.NoFading;
							}
						}
					}
					else if (fadeOutDuration > 0f)
					{
						num2 = 1f - (Time.time - fadeStartTime) / fadeOutDuration;
						if (num2 < 0f)
						{
							num2 = 0f;
							fading = FadingState.NoFading;
							_highlighted = false;
							if (OnObjectHighlightEnd != null)
							{
								OnObjectHighlightEnd(base.gameObject);
							}
							SendMessage("HighlightEnd", null, SendMessageOptions.DontRequireReceiver);
						}
					}
				}
				if (glowQuality == QualityLevel.High)
				{
					num *= 0.25f;
				}
				int num3 = 0;
				int num4 = 0;
				Bounds bounds = default;
				if (flag4)
				{
					if (cbSmoothBlend == null)
					{
						CheckBlurCommandBuffer();
					}
					cbSmoothBlend.Clear();
					num3 = cam.pixelWidth;
					num4 = cam.pixelHeight;
					if (num4 <= 0)
					{
						num4 = 1;
					}
					if (XRSettings.enabled && Application.isPlaying)
					{
						sourceDesc = XRSettings.eyeTextureDesc;
					}
					else
					{
						sourceDesc = new RenderTextureDescriptor(num3, num4, Application.isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR);
						sourceDesc.volumeDepth = 1;
					}
					sourceDesc.msaaSamples = 1;
					sourceDesc.useMipMap = false;
					sourceDesc.depthBufferBits = 0;
					cbSmoothBlend.GetTemporaryRT(sourceRT, sourceDesc, FilterMode.Bilinear);
					if ((glow > 0f && visibility == Visibility.AlwaysOnTop) || (outline > 0f && visibility2 == Visibility.AlwaysOnTop))
					{
						cbSmoothBlend.SetRenderTarget(sourceRT);
					}
					else
					{
						cbSmoothBlend.SetRenderTarget(sourceRT, BuiltinRenderTextureType.CameraTarget);
					}
					cbSmoothBlend.ClearRenderTarget(clearDepth: false, clearColor: true, new Color(0f, 0f, 0f, 0f));
				}
				for (int num5 = 0; num5 < rmsCount; num5++)
				{
					if (!rms[num5].render)
					{
						continue;
					}
					Mesh mesh2 = rms[num5].mesh;
					if (flag)
					{
						bool flag7 = seeThroughBorder * seeThroughBorderWidth > 0f;
						if (rms[num5].preserveOriginalMesh)
						{
							cbSeeThrough.Clear();
							for (int num6 = 0; num6 < mesh2.subMeshCount; num6++)
							{
								if (num6 < rms[num5].fxMatSeeThroughInner.Length && rms[num5].fxMatSeeThroughInner[num6] != null)
								{
									cbSeeThrough.DrawRenderer(rms[num5].renderer, rms[num5].fxMatSeeThroughInner[num6], num6);
									if (flag7)
									{
										cbSeeThrough.DrawRenderer(rms[num5].renderer, rms[num5].fxMatSeeThroughBorder[num6], num6);
									}
								}
							}
							Graphics.ExecuteCommandBuffer(cbSeeThrough);
						}
						else
						{
							for (int num7 = 0; num7 < mesh2.subMeshCount; num7++)
							{
								if (num7 < rms[num5].fxMatSeeThroughInner.Length && rms[num5].fxMatSeeThroughInner[num7] != null)
								{
									rms[num5].fxMatSeeThroughInner[num7].SetPass(0);
									Graphics.DrawMeshNow(mesh2, rms[num5].renderingMatrix, num7);
									if (flag7)
									{
										rms[num5].fxMatSeeThroughBorder[num7].SetPass(0);
										Graphics.DrawMeshNow(mesh2, rms[num5].renderingMatrix, num7);
									}
								}
							}
						}
					}
					if (_highlighted || hitActive)
					{
						Color color = overlayColor;
						float y = overlayMinIntensity;
						float z = overlayBlending;
						if (hitActive)
						{
							float num8 = ((hitFadeOutDuration > 0f) ? ((Time.time - hitStartTime) / hitFadeOutDuration) : 1f);
							if (num8 >= 1f)
							{
								hitActive = false;
								color.a = (_highlighted ? overlay : 0f);
							}
							else
							{
								bool flag8 = _highlighted && overlay > 0f;
								color = (flag8 ? Color.Lerp(hitColor, color, num8) : hitColor);
								color.a = (flag8 ? Mathf.Lerp(1f - num8, overlay, num8) : (1f - num8));
								color.a *= hitInitialIntensity;
								y = 1f;
								z = 0f;
							}
						}
						else
						{
							color.a = overlay * num2;
						}
						if (color.a > 0f)
						{
							for (int num9 = 0; num9 < mesh2.subMeshCount; num9++)
							{
								if (num9 < rms[num5].fxMatOverlay.Length && rms[num5].fxMatOverlay[num9] != null)
								{
									Material material2 = rms[num5].fxMatOverlay[num9];
									material2.color = color;
									material2.SetVector("_OverlayData", new Vector3(overlayAnimationSpeed, y, z));
									if (rms[num5].preserveOriginalMesh)
									{
										cbOverlay.Clear();
										cbOverlay.DrawRenderer(rms[num5].renderer, material2, num9);
										Graphics.ExecuteCommandBuffer(cbOverlay);
									}
									else
									{
										material2.SetPass(0);
										Graphics.DrawMeshNow(mesh2, rms[num5].renderingMatrix, num9);
									}
								}
							}
						}
					}
					if (!_highlighted)
					{
						continue;
					}
					if (flag4)
					{
						if (num5 == 0)
						{
							bounds = rms[num5].renderer.bounds;
						}
						else
						{
							bounds.Encapsulate(rms[num5].renderer.bounds);
						}
					}
					for (int num10 = 0; num10 < mesh2.subMeshCount; num10++)
					{
						if (flag4 && num10 < rms[num5].fxMatSolidColor.Length)
						{
							if (rms[num5].preserveOriginalMesh)
							{
								cbSmoothBlend.DrawRenderer(rms[num5].renderer, rms[num5].fxMatSolidColor[num10], num10);
							}
							else
							{
								cbSmoothBlend.DrawMesh(mesh2, rms[num5].renderingMatrix, rms[num5].fxMatSolidColor[num10], num10);
							}
						}
						if (glow > 0f && glowQuality != QualityLevel.Highest)
						{
							matDataGlow.Clear();
							matDataColor.Clear();
							matDataDirection.Clear();
							Material material3 = fxMatGlow;
							material3.SetVector("_GlowDirection", Vector3.zero);
							for (int num11 = 0; num11 < glowPasses.Length; num11++)
							{
								Color color2 = glowPasses[num11].color;
								material3.SetColor("_GlowColor", color2);
								Vector4 vector2 = new Vector4(num2 * num * glowPasses[num11].alpha, glowPasses[num11].offset * glowWidth / 100f, glowMagicNumber1, glowMagicNumber2);
								material3.SetVector("_Glow", vector2);
								if (glowQuality == QualityLevel.High)
								{
									for (int num12 = 0; num12 < offsets.Length; num12++)
									{
										Vector3 vector3 = offsets[num12];
										vector3.y *= cam.aspect;
										material3.SetVector("_GlowDirection", vector3);
										if (rms[num5].preserveOriginalMesh)
										{
											cbGlow.Clear();
											cbGlow.DrawRenderer(rms[num5].renderer, material3, num10);
											Graphics.ExecuteCommandBuffer(cbGlow);
										}
										else if (useGPUInstancing)
										{
											matDataDirection.Add(vector3);
											matDataGlow.Add(vector2);
											matDataColor.Add(new Color(color2.r, color2.g, color2.b, color2.a));
										}
										else
										{
											material3.SetPass(0);
											Graphics.DrawMeshNow(mesh2, rms[num5].renderingMatrix, num10);
										}
									}
								}
								else if (rms[num5].preserveOriginalMesh)
								{
									cbGlow.Clear();
									cbGlow.DrawRenderer(rms[num5].renderer, material3, num10);
									Graphics.ExecuteCommandBuffer(cbGlow);
								}
								else if (useGPUInstancing)
								{
									matDataDirection.Add(Vector4.zero);
									matDataGlow.Add(vector2);
									matDataColor.Add(new Color(color2.r, color2.g, color2.b, color2.a));
								}
								else
								{
									material3.SetPass(0);
									Graphics.DrawMeshNow(mesh2, rms[num5].renderingMatrix, num10);
								}
							}
							if (useGPUInstancing)
							{
								int count = matDataDirection.Count;
								if (count > 0)
								{
									glowPropertyBlock.SetVectorArray("_GlowDirection", matDataDirection);
									glowPropertyBlock.SetVectorArray("_GlowColor", matDataColor);
									glowPropertyBlock.SetVectorArray("_Glow", matDataGlow);
									if (matrices == null || matrices.Length < count)
									{
										matrices = new Matrix4x4[count];
									}
									for (int num13 = 0; num13 < count; num13++)
									{
										matrices[num13] = rms[num5].renderingMatrix;
									}
									cbGlow.Clear();
									cbGlow.DrawMeshInstanced(mesh2, num10, material3, 0, matrices, count, glowPropertyBlock);
									Graphics.ExecuteCommandBuffer(cbGlow);
								}
							}
						}
						if (outline > 0f && outlineQuality != QualityLevel.Highest)
						{
							Color value = outlineColor;
							value.a = outline * num2;
							Material material4 = fxMatOutline;
							material4.SetColor("_OutlineColor", value);
							if (outlineQuality == QualityLevel.High)
							{
								matDataDirection.Clear();
								for (int num14 = 0; num14 < offsets.Length; num14++)
								{
									Vector3 vector4 = offsets[num14] * (outlineWidth / 100f);
									vector4.y *= cam.aspect;
									material4.SetVector("_OutlineDirection", vector4);
									if (rms[num5].preserveOriginalMesh)
									{
										cbOutline.Clear();
										cbOutline.DrawRenderer(rms[num5].renderer, material4, num10, 0);
										Graphics.ExecuteCommandBuffer(cbOutline);
									}
									else if (useGPUInstancing)
									{
										matDataDirection.Add(vector4);
									}
									else
									{
										material4.SetPass(0);
										Graphics.DrawMeshNow(mesh2, rms[num5].renderingMatrix, num10);
									}
								}
								if (useGPUInstancing)
								{
									int count2 = matDataDirection.Count;
									if (count2 > 0)
									{
										outlinePropertyBlock.SetVectorArray("_OutlineDirection", matDataDirection);
										if (matrices == null || matrices.Length < count2)
										{
											matrices = new Matrix4x4[count2];
										}
										for (int num15 = 0; num15 < count2; num15++)
										{
											matrices[num15] = rms[num5].renderingMatrix;
										}
										cbGlow.Clear();
										cbGlow.DrawMeshInstanced(mesh2, num10, material4, 0, matrices, count2, outlinePropertyBlock);
										Graphics.ExecuteCommandBuffer(cbGlow);
									}
								}
							}
							else if (rms[num5].preserveOriginalMesh)
							{
								cbOutline.Clear();
								cbOutline.DrawRenderer(rms[num5].renderer, material4, num10, 0);
								Graphics.ExecuteCommandBuffer(cbOutline);
							}
							else
							{
								material4.SetPass(0);
								Graphics.DrawMeshNow(mesh2, rms[num5].renderingMatrix, num10);
							}
						}
						if (innerGlow > 0f && innerGlowWidth > 0f && num10 < rms[num5].fxMatInnerGlow.Length && rms[num5].fxMatInnerGlow[num10] != null)
						{
							Color value2 = innerGlowColor;
							value2.a = innerGlow * num2;
							rms[num5].fxMatInnerGlow[num10].SetColor("_Color", value2);
							if (rms[num5].preserveOriginalMesh)
							{
								cbInnerGlow.Clear();
								cbInnerGlow.DrawRenderer(rms[num5].renderer, rms[num5].fxMatInnerGlow[num10], num10);
								Graphics.ExecuteCommandBuffer(cbInnerGlow);
							}
							else
							{
								rms[num5].fxMatInnerGlow[num10].SetPass(0);
								Graphics.DrawMeshNow(mesh2, rms[num5].renderingMatrix, num10);
							}
						}
					}
					if (!targetFX)
					{
						continue;
					}
					float num16 = 1f;
					if (Application.isPlaying)
					{
						num16 = Time.time - highlightStartTime;
						if (num16 >= targetFXStayDuration)
						{
							num16 -= targetFXStayDuration;
							num16 = 1f - num16;
						}
						if (num16 > 1f)
						{
							num16 = 1f;
						}
					}
					if (!(num16 > 0f))
					{
						continue;
					}
					float t = 1f;
					float num17;
					if (Application.isPlaying)
					{
						t = (Time.time - highlightStartTime) / targetFXTransitionDuration;
						if (t > 1f)
						{
							t = 1f;
						}
						t = Mathf.Sin(t * (float)Math.PI * 0.5f);
						num17 = Time.time;
					}
					else
					{
						num17 = (float)DateTime.Now.Subtract(DateTime.Today).TotalSeconds;
					}
					Bounds bounds2 = rms[num5].renderer.bounds;
					Vector3 size = bounds2.size;
					float num18 = size.x;
					if (size.y < num18)
					{
						num18 = size.y;
					}
					if (size.z < num18)
					{
						num18 = size.z;
					}
					size.x = (size.y = (size.z = num18));
					size = Vector3.Lerp(size * targetFXInitialScale, size * targetFXEndScale, t);
					Quaternion q = Quaternion.LookRotation(cam.transform.position - rms[num5].transform.position);
					Quaternion quaternion = Quaternion.Euler(0f, 0f, num17 * targetFXRotationSpeed);
					q *= quaternion;
					Matrix4x4 matrix = Matrix4x4.TRS((targetFXCenter != null) ? targetFXCenter.transform.position : bounds2.center, q, size);
					Color color3 = targetFXColor;
					color3.a *= num2 * num16;
					Material material5 = fxMatTarget;
					material5.color = color3;
					material5.SetPass(0);
					Graphics.DrawMeshNow(quadMesh, matrix);
				}
				if (flag4 && ComputeSmoothQuadMatrix(cam, bounds))
				{
					if (flag2)
					{
						float num19 = glow * num2;
						fxMatComposeGlow.color = new Color(glowHQColor.r * num19, glowHQColor.g * num19, glowHQColor.b * num19, glowHQColor.a * num19);
						SmoothGlow(num3 / glowDownsampling, num4 / glowDownsampling);
					}
					if (flag3)
					{
						fxMatComposeOutline.color = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 5f * outlineColor.a * outline * num2);
						SmoothOutline(num3 / outlineDownsampling, num4 / outlineDownsampling);
					}
					ComposeSmoothBlend(visibility, visibility2);
				}
			}
		}

		private bool ComputeSmoothQuadMatrix(Camera cam, Bounds bounds)
		{
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			corners[0] = new Vector3(min.x, min.y, min.z);
			corners[1] = new Vector3(min.x, min.y, max.z);
			corners[2] = new Vector3(max.x, min.y, min.z);
			corners[3] = new Vector3(max.x, min.y, max.z);
			corners[4] = new Vector3(min.x, max.y, min.z);
			corners[5] = new Vector3(min.x, max.y, max.z);
			corners[6] = new Vector3(max.x, max.y, min.z);
			corners[7] = new Vector3(max.x, max.y, max.z);
			Vector3 scrMin = new Vector3(float.MaxValue, float.MaxValue, 0f);
			Vector3 scrMax = new Vector3(float.MinValue, float.MinValue, 0f);
			float num = float.MaxValue;
			for (int i = 0; i < corners.Length; i++)
			{
				corners[i] = cam.WorldToScreenPoint(corners[i]);
				if (corners[i].x < scrMin.x)
				{
					scrMin.x = corners[i].x;
				}
				if (corners[i].y < scrMin.y)
				{
					scrMin.y = corners[i].y;
				}
				if (corners[i].x > scrMax.x)
				{
					scrMax.x = corners[i].x;
				}
				if (corners[i].y > scrMax.y)
				{
					scrMax.y = corners[i].y;
				}
				if (corners[i].z < num)
				{
					num = corners[i].z;
					if (num < cam.nearClipPlane)
					{
						scrMin.x = (scrMin.y = 0f);
						scrMax.x = cam.pixelWidth;
						scrMax.y = cam.pixelHeight;
						break;
					}
				}
			}
			if (scrMax.y == scrMin.y)
			{
				return false;
			}
			if (num < cam.nearClipPlane)
			{
				num = cam.nearClipPlane + 0.01f;
			}
			scrMin.z = (scrMax.z = num);
			if (outline > 0f)
			{
				BuildMatrix(cam, scrMin, scrMax, (int)(10f + 20f * outlineWidth + (float)(5 * outlineDownsampling)), ref quadOutlineMatrix);
			}
			if (glow > 0f)
			{
				BuildMatrix(cam, scrMin, scrMax, (int)(20f + 30f * glowWidth + (float)(10 * glowDownsampling)), ref quadGlowMatrix);
			}
			return true;
		}

		private void BuildMatrix(Camera cam, Vector3 scrMin, Vector3 scrMax, int border, ref Matrix4x4 quadMatrix)
		{
			scrMin.x -= border;
			scrMin.y -= border;
			scrMax.x += border;
			scrMax.y += border;
			Vector3 position = new Vector3(scrMax.x, scrMin.y, scrMin.z);
			scrMin = cam.ScreenToWorldPoint(scrMin);
			scrMax = cam.ScreenToWorldPoint(scrMax);
			position = cam.ScreenToWorldPoint(position);
			float x = Vector3.Distance(scrMin, position);
			float y = Vector3.Distance(scrMax, position);
			quadMatrix = Matrix4x4.TRS((scrMin + scrMax) * 0.5f, cam.transform.rotation, new Vector3(x, y, 1f));
		}

		private void SmoothGlow(int rtWidth, int rtHeight)
		{
			int num = 8;
			if (mipGlowBuffers == null || mipGlowBuffers.Length != num)
			{
				mipGlowBuffers = new int[num];
				for (int i = 0; i < num; i++)
				{
					mipGlowBuffers[i] = Shader.PropertyToID("_HPSmoothGlowTemp" + i);
				}
				glowRT = Shader.PropertyToID("_HPComposeGlowFinal");
				mipGlowBuffers[num - 2] = glowRT;
			}
			RenderTextureDescriptor desc = sourceDesc;
			desc.depthBufferBits = 0;
			if (desc.vrUsage == VRTextureUsage.TwoEyes)
			{
				desc.vrUsage = VRTextureUsage.None;
				fxMatBlurGlow.SetFloat("_StereoRendering", 0.5f);
				fxMatComposeGlow.SetFloat("_StereoRendering", 0.5f);
			}
			else
			{
				fxMatBlurGlow.SetFloat("_StereoRendering", 1f);
				fxMatComposeGlow.SetFloat("_StereoRendering", 1f);
			}
			for (int j = 0; j < num; j++)
			{
				float num2 = j / 2 + 2;
				int num3 = (int)((float)rtWidth / num2);
				int num4 = (int)((float)rtHeight / num2);
				if (num3 <= 0)
				{
					num3 = 1;
				}
				if (num4 <= 0)
				{
					num4 = 1;
				}
				desc.width = num3;
				desc.height = num4;
				cbSmoothBlend.GetTemporaryRT(mipGlowBuffers[j], desc, FilterMode.Bilinear);
			}
			Material mat = fxMatBlurGlow;
			for (int k = 0; k < num - 1; k += 2)
			{
				if (k == 0)
				{
					cbSmoothBlend.Blit(sourceRT, mipGlowBuffers[k + 1], mat, 0);
				}
				else
				{
					cbSmoothBlend.Blit(mipGlowBuffers[k], mipGlowBuffers[k + 1], mat, 0);
				}
				cbSmoothBlend.Blit(mipGlowBuffers[k + 1], mipGlowBuffers[k], mat, 1);
				if (k < num - 2)
				{
					cbSmoothBlend.Blit(mipGlowBuffers[k], mipGlowBuffers[k + 2], mat, 2);
				}
			}
		}

		private void SmoothOutline(int rtWidth, int rtHeight)
		{
			int num = 4;
			if (mipOutlineBuffers == null || mipOutlineBuffers.Length != num)
			{
				mipOutlineBuffers = new int[num];
				for (int i = 0; i < num; i++)
				{
					mipOutlineBuffers[i] = Shader.PropertyToID("_HPSmoothOutlineTemp" + i);
				}
				outlineRT = Shader.PropertyToID("_HPComposeOutlineFinal");
				mipOutlineBuffers[num - 2] = outlineRT;
			}
			RenderTextureDescriptor desc = sourceDesc;
			desc.depthBufferBits = 0;
			if (desc.vrUsage == VRTextureUsage.TwoEyes)
			{
				desc.vrUsage = VRTextureUsage.None;
				fxMatBlurOutline.SetFloat("_StereoRendering", 0.5f);
				fxMatComposeOutline.SetFloat("_StereoRendering", 0.5f);
			}
			else
			{
				fxMatBlurOutline.SetFloat("_StereoRendering", 1f);
				fxMatComposeOutline.SetFloat("_StereoRendering", 1f);
			}
			for (int j = 0; j < num; j++)
			{
				float num2 = j / 2 + 2;
				int num3 = (int)((float)rtWidth / num2);
				int num4 = (int)((float)rtHeight / num2);
				if (num3 <= 0)
				{
					num3 = 1;
				}
				if (num4 <= 0)
				{
					num4 = 1;
				}
				desc.width = num3;
				desc.height = num4;
				cbSmoothBlend.GetTemporaryRT(mipOutlineBuffers[j], desc, FilterMode.Bilinear);
			}
			Material mat = fxMatBlurOutline;
			for (int k = 0; k < num - 1; k += 2)
			{
				if (k == 0)
				{
					cbSmoothBlend.Blit(sourceRT, mipOutlineBuffers[k + 1], mat, 0);
				}
				else
				{
					cbSmoothBlend.Blit(mipOutlineBuffers[k], mipOutlineBuffers[k + 1], mat, 0);
				}
				cbSmoothBlend.Blit(mipOutlineBuffers[k + 1], mipOutlineBuffers[k], mat, 1);
				if (k < num - 2)
				{
					cbSmoothBlend.Blit(mipOutlineBuffers[k], mipOutlineBuffers[k + 2], mat, 2);
				}
			}
		}

		private void ComposeSmoothBlend(Visibility smoothGlowVisibility, Visibility smoothOutlineVisibility)
		{
			int num;
			if (glow > 0f)
			{
				num = ((glowQuality == QualityLevel.Highest) ? 1 : 0);
				if (num != 0)
				{
					Material material = fxMatComposeGlow;
					material.SetVector("_Flip", (XRSettings.enabled && flipY) ? new Vector3(1f, -1f, 0f) : new Vector3(0f, 1f, 0f));
					if (glowOptimalBlit)
					{
						cbSmoothBlend.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
						material.SetInt("_ZTest", GetZTestValue(smoothGlowVisibility));
						material.SetColor("_Debug", glowBlitDebug ? debugColor : blackColor);
						cbSmoothBlend.DrawMesh(quadMesh, quadGlowMatrix, material, 0, 0);
					}
					else
					{
						cbSmoothBlend.Blit(glowRT, BuiltinRenderTextureType.CameraTarget, material, 1);
					}
				}
			}
			else
			{
				num = 0;
			}
			bool flag = outline > 0f && outlineQuality == QualityLevel.Highest;
			if (flag)
			{
				Material material2 = fxMatComposeOutline;
				material2.SetVector("_Flip", (XRSettings.enabled && flipY) ? new Vector3(1f, -1f, 0f) : new Vector3(0f, 1f, 0f));
				if (outlineOptimalBlit)
				{
					cbSmoothBlend.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
					material2.SetInt("_ZTest", GetZTestValue(smoothOutlineVisibility));
					material2.SetColor("_Debug", outlineBlitDebug ? debugColor : blackColor);
					cbSmoothBlend.DrawMesh(quadMesh, quadOutlineMatrix, material2, 0, 0);
				}
				else
				{
					cbSmoothBlend.Blit(outlineRT, BuiltinRenderTextureType.CameraTarget, material2, 1);
				}
			}
			if (num != 0)
			{
				for (int i = 0; i < mipGlowBuffers.Length; i++)
				{
					cbSmoothBlend.ReleaseTemporaryRT(mipGlowBuffers[i]);
				}
			}
			if (flag)
			{
				for (int j = 0; j < mipOutlineBuffers.Length; j++)
				{
					cbSmoothBlend.ReleaseTemporaryRT(mipOutlineBuffers[j]);
				}
			}
			cbSmoothBlend.ReleaseTemporaryRT(sourceRT);
			Graphics.ExecuteCommandBuffer(cbSmoothBlend);
		}

		private void InitMaterial(ref Material material, string shaderName)
		{
			if (material == null)
			{
				Shader shader = Shader.Find(shaderName);
				if (shader == null)
				{
					Debug.LogError("Shader " + shaderName + " not found.");
					base.enabled = false;
				}
				else
				{
					material = new Material(shader);
				}
			}
		}

		public void SetTarget(Transform transform)
		{
			if (!(transform == target) && !(transform == null))
			{
				if (_highlighted)
				{
					SetHighlighted(state: false);
				}
				target = transform;
				SetupMaterial();
			}
		}

		public void SetTargets(Transform transform, Renderer[] renderers)
		{
			if (!(transform == target) && !(transform == null))
			{
				if (_highlighted)
				{
					SetHighlighted(state: false);
				}
				target = transform;
				SetupMaterial(renderers);
			}
		}

		public void SetHighlighted(bool state)
		{
			if (!Application.isPlaying)
			{
				_highlighted = state;
				return;
			}
			if (fading == FadingState.NoFading)
			{
				fadeStartTime = Time.time;
			}
			if (state && !ignore)
			{
				if ((_highlighted && fading == FadingState.NoFading) || (OnObjectHighlightStart != null && !OnObjectHighlightStart(base.gameObject)))
				{
					return;
				}
				SendMessage("HighlightStart", null, SendMessageOptions.DontRequireReceiver);
				highlightStartTime = Time.time;
				if (fadeInDuration > 0f)
				{
					if (fading == FadingState.FadingOut)
					{
						float num = fadeOutDuration - (Time.time - fadeStartTime);
						fadeStartTime = Time.time - num;
					}
					fading = FadingState.FadingIn;
				}
				else
				{
					fading = FadingState.NoFading;
				}
				_highlighted = true;
				requireUpdateMaterial = true;
			}
			else
			{
				if (!_highlighted)
				{
					return;
				}
				if (fadeOutDuration > 0f)
				{
					if (fading == FadingState.FadingIn)
					{
						float num2 = Time.time - fadeStartTime;
						fadeStartTime = Time.time + num2 - fadeInDuration;
					}
					fading = FadingState.FadingOut;
					return;
				}
				fading = FadingState.NoFading;
				_highlighted = false;
				if (OnObjectHighlightEnd != null)
				{
					OnObjectHighlightEnd(base.gameObject);
				}
				SendMessage("HighlightEnd", null, SendMessageOptions.DontRequireReceiver);
				requireUpdateMaterial = true;
			}
		}

		private void SetupMaterial()
		{
			if (target == null || fxMatMask == null)
			{
				return;
			}
			Renderer[] rr = null;
			switch (effectGroup)
			{
			case TargetOptions.OnlyThisObject:
			{
				Renderer component2 = target.GetComponent<Renderer>();
				if (component2 != null)
				{
					rr = new Renderer[1] { component2 };
				}
				break;
			}
			case TargetOptions.RootToChildren:
			{
				Transform parent = target;
				while (parent.parent != null)
				{
					parent = parent.parent;
				}
				rr = parent.GetComponentsInChildren<Renderer>();
				break;
			}
			case TargetOptions.LayerInScene:
			{
				HighlightEffect highlightEffect2 = this;
				if (target != base.transform)
				{
					HighlightEffect component3 = target.GetComponent<HighlightEffect>();
					if (component3 != null)
					{
						highlightEffect2 = component3;
					}
				}
				rr = FindRenderersWithLayerInScene(highlightEffect2.effectGroupLayer);
				break;
			}
			case TargetOptions.LayerInChildren:
			{
				HighlightEffect highlightEffect = this;
				if (target != base.transform)
				{
					HighlightEffect component = target.GetComponent<HighlightEffect>();
					if (component != null)
					{
						highlightEffect = component;
					}
				}
				rr = FindRenderersWithLayerInChildren(highlightEffect.effectGroupLayer);
				break;
			}
			default:
				rr = target.GetComponentsInChildren<Renderer>();
				break;
			}
			SetupMaterial(rr);
		}

		private void SetupMaterial(Renderer[] rr)
		{
			if (rr == null)
			{
				rr = new Renderer[0];
			}
			if (rms == null || rms.Length < rr.Length)
			{
				rms = new ModelMaterials[rr.Length];
			}
			rmsCount = 0;
			for (int i = 0; i < rr.Length; i++)
			{
				rms[rmsCount] = default;
				Renderer renderer = rr[i];
				rms[rmsCount].renderer = renderer;
				rms[rmsCount].renderWasVisibleDuringSetup = renderer.isVisible;
				if (renderer.transform != target)
				{
					HighlightEffect component = renderer.GetComponent<HighlightEffect>();
					if (component != null && component.enabled)
					{
						component.highlighted = highlighted;
						continue;
					}
				}
				if (OnRendererHighlightStart != null && !OnRendererHighlightStart(renderer))
				{
					rmsCount++;
					continue;
				}
				bool flag = renderer is SkinnedMeshRenderer;
				rms[rmsCount].preserveOriginalMesh = flag || preserveOriginalMesh;
				if (rms[rmsCount].preserveOriginalMesh)
				{
					CheckCommandBuffers();
				}
				if (flag)
				{
					rms[rmsCount].mesh = ((SkinnedMeshRenderer)renderer).sharedMesh;
				}
				else if (Application.isPlaying && renderer.isPartOfStaticBatch)
				{
					MeshCollider component2 = renderer.GetComponent<MeshCollider>();
					if (component2 != null)
					{
						rms[rmsCount].mesh = component2.sharedMesh;
					}
				}
				else
				{
					MeshFilter component3 = renderer.GetComponent<MeshFilter>();
					if (component3 != null)
					{
						rms[rmsCount].mesh = component3.sharedMesh;
					}
				}
				if (rms[rmsCount].mesh == null)
				{
					continue;
				}
				rms[rmsCount].transform = renderer.transform;
				rms[rmsCount].fxMatMask = Fork(fxMatMask, rms[rmsCount].mesh);
				rms[rmsCount].fxMatSeeThroughInner = Fork(fxMatSeeThroughInner, rms[rmsCount].mesh);
				rms[rmsCount].fxMatSeeThroughBorder = Fork(fxMatSeeThroughBorder, rms[rmsCount].mesh);
				rms[rmsCount].fxMatOverlay = Fork(fxMatOverlay, rms[rmsCount].mesh);
				rms[rmsCount].fxMatInnerGlow = Fork(fxMatInnerGlow, rms[rmsCount].mesh);
				rms[rmsCount].fxMatSolidColor = Fork(fxMatSolidColor, rms[rmsCount].mesh);
				rms[rmsCount].originalMesh = rms[rmsCount].mesh;
				if (!rms[rmsCount].preserveOriginalMesh)
				{
					if (innerGlow > 0f || (glow > 0f && glowQuality != QualityLevel.Highest) || (outline > 0f && outlineQuality != QualityLevel.Highest))
					{
						AverageNormals(rmsCount);
					}
					BakeTransform(rmsCount, duplicateMesh: true);
				}
				rmsCount++;
			}
			UpdateMaterialPropertiesNow();
		}

		private Renderer[] FindRenderersWithLayerInScene(LayerMask layer)
		{
			Renderer[] array = UnityEngine.Object.FindObjectsOfType<Renderer>();
			if (tempRR == null)
			{
				tempRR = new List<Renderer>();
			}
			else
			{
				tempRR.Clear();
			}
			foreach (Renderer renderer in array)
			{
				if (((1 << renderer.gameObject.layer) & (int)layer) != 0)
				{
					tempRR.Add(renderer);
				}
			}
			return tempRR.ToArray();
		}

		private Renderer[] FindRenderersWithLayerInChildren(LayerMask layer)
		{
			Renderer[] componentsInChildren = target.GetComponentsInChildren<Renderer>();
			if (tempRR == null)
			{
				tempRR = new List<Renderer>();
			}
			else
			{
				tempRR.Clear();
			}
			foreach (Renderer renderer in componentsInChildren)
			{
				if (((1 << renderer.gameObject.layer) & (int)layer) != 0)
				{
					tempRR.Add(renderer);
				}
			}
			return tempRR.ToArray();
		}

		private void CheckGeometrySupportDependencies()
		{
			InitMaterial(ref fxMatMask, "HighlightPlus/Geometry/Mask");
			InitMaterial(ref fxMatOverlay, "HighlightPlus/Geometry/Overlay");
			InitMaterial(ref fxMatSeeThroughInner, "HighlightPlus/Geometry/SeeThroughInner");
			InitMaterial(ref fxMatSeeThroughBorder, "HighlightPlus/Geometry/SeeThroughBorder");
			InitMaterial(ref fxMatSolidColor, "HighlightPlus/Geometry/SolidColor");
			InitMaterial(ref fxMatClearStencil, "HighlightPlus/ClearStencil");
			InitMaterial(ref fxMatOutlineRef, "HighlightPlus/Geometry/Outline");
			InitMaterial(ref fxMatGlowRef, "HighlightPlus/Geometry/Glow");
			InitMaterial(ref fxMatInnerGlow, "HighlightPlus/Geometry/InnerGlow");
			InitMaterial(ref fxMatTargetRef, "HighlightPlus/Geometry/Target");
			InitMaterial(ref fxMatComposeOutlineRef, "HighlightPlus/Geometry/ComposeOutline");
			InitMaterial(ref fxMatComposeGlowRef, "HighlightPlus/Geometry/ComposeGlow");
			InitMaterial(ref fxMatBlurOutlineRef, "HighlightPlus/Geometry/BlurOutline");
			InitMaterial(ref fxMatBlurGlowRef, "HighlightPlus/Geometry/BlurGlow");
			CheckRequiredCommandBuffers();
		}

		private void CheckRequiredCommandBuffers()
		{
			if (cbGlow == null)
			{
				cbGlow = new CommandBuffer();
				cbGlow.name = "Outer Glow";
			}
		}

		private void CheckCommandBuffers()
		{
			if (cbMask == null)
			{
				cbMask = new CommandBuffer();
				cbMask.name = "Mask";
			}
			if (cbSeeThrough == null)
			{
				cbSeeThrough = new CommandBuffer();
				cbSeeThrough.name = "See Through";
			}
			if (cbOutline == null)
			{
				cbOutline = new CommandBuffer();
				cbOutline.name = "Outline";
			}
			if (cbOverlay == null)
			{
				cbOverlay = new CommandBuffer();
				cbOverlay.name = "Overlay";
			}
			if (cbInnerGlow == null)
			{
				cbInnerGlow = new CommandBuffer();
				cbInnerGlow.name = "Inner Glow";
			}
		}

		private void CheckBlurCommandBuffer()
		{
			if (cbSmoothBlend == null)
			{
				cbSmoothBlend = new CommandBuffer();
				cbSmoothBlend.name = "Smooth Blend";
			}
		}

		private Material[] Fork(Material mat, Mesh mesh)
		{
			if (mesh == null)
			{
				return null;
			}
			int subMeshCount = mesh.subMeshCount;
			Material[] array = new Material[subMeshCount];
			for (int i = 0; i < subMeshCount; i++)
			{
				array[i] = UnityEngine.Object.Instantiate(mat);
			}
			return array;
		}

		private void BakeTransform(int objIndex, bool duplicateMesh)
		{
			if (rms[objIndex].mesh == null)
			{
				return;
			}
			Transform transform = rms[objIndex].transform;
			Vector3 localScale = transform.localScale;
			if (localScale.x >= 0f && localScale.y >= 0f && localScale.z >= 0f)
			{
				rms[objIndex].bakedTransform = false;
				return;
			}
			Mesh mesh = (duplicateMesh ? UnityEngine.Object.Instantiate(rms[objIndex].originalMesh) : rms[objIndex].mesh);
			Vector3[] array = mesh.vertices;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = transform.TransformPoint(array[i]);
			}
			mesh.vertices = array;
			Vector3[] array2 = mesh.normals;
			if (array2 != null)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = transform.TransformVector(array2[j]).normalized;
				}
				mesh.normals = array2;
			}
			mesh.RecalculateBounds();
			rms[objIndex].mesh = mesh;
			rms[objIndex].bakedTransform = true;
			rms[objIndex].currentPosition = transform.position;
			rms[objIndex].currentRotation = transform.eulerAngles;
			rms[objIndex].currentScale = transform.lossyScale;
		}

		public void UpdateMaterialProperties(bool forceNow = false)
		{
			if (forceNow || !Application.isPlaying)
			{
				UpdateMaterialPropertiesNow();
			}
			else
			{
				requireUpdateMaterial = true;
			}
		}

		private void UpdateMaterialPropertiesNow()
		{
			if (rms == null)
			{
				return;
			}
			if (ignore)
			{
				_highlighted = false;
			}
			Color value = seeThroughTintColor;
			value.a = seeThroughTintAlpha;
			if (lastOutlineVisibility != outlineVisibility)
			{
				if (glowQuality == QualityLevel.Highest && outlineQuality == QualityLevel.Highest)
				{
					glowVisibility = outlineVisibility;
				}
				lastOutlineVisibility = outlineVisibility;
			}
			if (outlineWidth < 0f)
			{
				outlineWidth = 0f;
			}
			if (glowWidth < 0f)
			{
				glowWidth = 0f;
			}
			if (overlay < overlayMinIntensity)
			{
				overlay = overlayMinIntensity;
			}
			if (targetFXTransitionDuration <= 0f)
			{
				targetFXTransitionDuration = 0.0001f;
			}
			if (targetFXStayDuration <= 0f)
			{
				targetFXStayDuration = 0.0001f;
			}
			if (seeThroughBorderWidth < 0f)
			{
				seeThroughBorderWidth = 0f;
			}
			bool flag = glow > 0f && glowQuality == QualityLevel.Highest;
			if (flag)
			{
				CheckBlurCommandBuffer();
				fxMatComposeGlow.SetInt("_Cull", cullBackFaces ? 2 : 0);
				fxMatBlurGlow.SetFloat("_BlurScale", glowWidth / (float)glowDownsampling);
				fxMatBlurGlow.SetFloat("_Speed", glowAnimationSpeed);
			}
			if (outline > 0f && outlineQuality == QualityLevel.Highest)
			{
				CheckBlurCommandBuffer();
				fxMatComposeOutline.SetInt("_Cull", cullBackFaces ? 2 : 0);
				fxMatBlurOutline.SetFloat("_BlurScale", outlineWidth / (float)outlineDownsampling);
			}
			for (int i = 0; i < rmsCount; i++)
			{
				if (!(rms[i].mesh != null))
				{
					continue;
				}
				float value2 = ((outlineQuality == QualityLevel.High) ? 0f : (outlineWidth / 100f));
				Material material = fxMatOutline;
				material.SetFloat("_OutlineWidth", value2);
				material.SetVector("_OutlineDirection", Vector3.zero);
				material.SetInt("_OutlineZTest", GetZTestValue(outlineVisibility));
				material.SetInt("_Cull", cullBackFaces ? 2 : 0);
				material.SetFloat("_ConstantWidth", constantWidth ? 1f : 0f);
				Material material2 = fxMatGlow;
				material2.SetVector("_Glow2", new Vector3(outlineWidth / 100f, glowAnimationSpeed, (!glowDithering) ? 1 : 0));
				material2.SetInt("_GlowZTest", GetZTestValue(glowVisibility));
				material2.SetInt("_Cull", cullBackFaces ? 2 : 0);
				material2.SetFloat("_ConstantWidth", constantWidth ? 1f : 0f);
				if (glowBlendMode == GlowBlendMode.AlphaBlend)
				{
					fxMatComposeGlow.SetInt("_SrcBlend", 5);
					fxMatComposeGlow.SetInt("_DstBlend", 10);
				}
				else
				{
					fxMatComposeGlow.SetInt("_SrcBlend", 1);
					fxMatComposeGlow.SetInt("_DstBlend", 1);
				}
				if (targetFX)
				{
					if (targetFXTexture == null)
					{
						targetFXTexture = Resources.Load<Texture2D>("HighlightPlus/target");
					}
					fxMatTarget.mainTexture = targetFXTexture;
				}
				for (int j = 0; j < rms[i].mesh.subMeshCount; j++)
				{
					Renderer renderer = rms[i].renderer;
					if (renderer == null)
					{
						continue;
					}
					Material material3 = null;
					if (renderer.sharedMaterials != null && j < renderer.sharedMaterials.Length)
					{
						material3 = renderer.sharedMaterials[j];
					}
					if (material3 == null)
					{
						continue;
					}
					bool flag2 = material3.HasProperty("_MainTex");
					bool flag3 = (alphaCutOff > 0f) & flag2;
					if (rms[i].fxMatMask != null && rms[i].fxMatMask.Length > j)
					{
						Material material4 = rms[i].fxMatMask[j];
						if (material4 != null)
						{
							if (flag2)
							{
								Texture mainTexture = material3.mainTexture;
								material4.mainTexture = mainTexture;
								material4.mainTextureOffset = material3.mainTextureOffset;
								material4.mainTextureScale = material3.mainTextureScale;
							}
							if (flag3)
							{
								material4.SetFloat("_CutOff", alphaCutOff);
								material4.EnableKeyword("HP_ALPHACLIP");
							}
							else
							{
								material4.DisableKeyword("HP_ALPHACLIP");
							}
							if (depthClip)
							{
								material4.EnableKeyword("HP_DEPTHCLIP");
							}
							else
							{
								material4.DisableKeyword("HP_DEPTHCLIP");
							}
							material4.SetInt("_Cull", cullBackFaces ? 2 : 0);
						}
					}
					if (rms[i].fxMatSeeThroughInner != null && rms[i].fxMatSeeThroughInner.Length > j)
					{
						Material material5 = rms[i].fxMatSeeThroughInner[j];
						if (material5 != null)
						{
							material5.SetFloat("_SeeThrough", seeThroughIntensity);
							material5.SetFloat("_SeeThroughNoise", seeThroughNoise);
							material5.SetColor("_SeeThroughTintColor", value);
							material5.SetFloat("_SeeThroughBorderWidth", (seeThroughBorder * seeThroughBorderWidth > 0f) ? (seeThroughBorderWidth / 100f) : 0f);
							material5.SetFloat("_SeeThroughBorderConstantWidth", constantWidth ? 1f : 0f);
							if (flag2)
							{
								Texture mainTexture2 = material3.mainTexture;
								material5.mainTexture = mainTexture2;
								material5.mainTextureOffset = material3.mainTextureOffset;
								material5.mainTextureScale = material3.mainTextureScale;
							}
							if (flag3)
							{
								material5.SetFloat("_CutOff", alphaCutOff);
								material5.EnableKeyword("HP_ALPHACLIP");
							}
							else
							{
								material5.DisableKeyword("HP_ALPHACLIP");
							}
						}
					}
					if (rms[i].fxMatSeeThroughBorder != null && rms[i].fxMatSeeThroughBorder.Length > j)
					{
						Material material6 = rms[i].fxMatSeeThroughBorder[j];
						if (material6 != null)
						{
							material6.SetColor("_SeeThroughBorderColor", new Color(seeThroughBorderColor.r, seeThroughBorderColor.g, seeThroughBorderColor.b, seeThroughBorder));
							if (flag2)
							{
								Texture mainTexture3 = material3.mainTexture;
								material6.mainTexture = mainTexture3;
								material6.mainTextureOffset = material3.mainTextureOffset;
								material6.mainTextureScale = material3.mainTextureScale;
							}
							if (flag3)
							{
								material6.SetFloat("_CutOff", alphaCutOff);
								material6.EnableKeyword("HP_ALPHACLIP");
							}
							else
							{
								material6.DisableKeyword("HP_ALPHACLIP");
							}
						}
					}
					if (rms[i].fxMatOverlay != null && rms[i].fxMatOverlay.Length > j)
					{
						Material material7 = rms[i].fxMatOverlay[j];
						if (material7 != null)
						{
							if (flag2)
							{
								Texture mainTexture4 = material3.mainTexture;
								material7.mainTexture = mainTexture4;
								material7.mainTextureOffset = material3.mainTextureOffset;
								material7.mainTextureScale = material3.mainTextureScale;
							}
							if (material3.HasProperty("_Color"))
							{
								material7.SetColor("_OverlayBackColor", material3.GetColor("_Color"));
							}
							if (flag3)
							{
								material7.SetFloat("_CutOff", alphaCutOff);
								material7.EnableKeyword("HP_ALPHACLIP");
							}
							else
							{
								material7.DisableKeyword("HP_ALPHACLIP");
							}
						}
					}
					if (rms[i].fxMatInnerGlow != null && rms[i].fxMatInnerGlow.Length > j)
					{
						Material material8 = rms[i].fxMatInnerGlow[j];
						if (material8 != null)
						{
							if (flag2)
							{
								Texture mainTexture5 = material3.mainTexture;
								material8.mainTexture = mainTexture5;
								material8.mainTextureOffset = material3.mainTextureOffset;
								material8.mainTextureScale = material3.mainTextureScale;
							}
							material8.SetFloat("_Width", innerGlowWidth);
							material8.SetInt("_InnerGlowZTest", GetZTestValue(innerGlowVisibility));
							if (flag3)
							{
								material8.SetFloat("_CutOff", alphaCutOff);
								material8.EnableKeyword("HP_ALPHACLIP");
							}
							else
							{
								material8.DisableKeyword("HP_ALPHACLIP");
							}
						}
					}
					if (rms[i].fxMatSolidColor == null || rms[i].fxMatSolidColor.Length <= j)
					{
						continue;
					}
					Material material9 = rms[i].fxMatSolidColor[j];
					if (material9 != null)
					{
						material9.color = glowHQColor;
						material9.SetInt("_Cull", cullBackFaces ? 2 : 0);
						material9.SetInt("_ZTest", GetZTestValue(flag ? glowVisibility : outlineVisibility));
						if (flag2)
						{
							Texture mainTexture6 = material3.mainTexture;
							material9.mainTexture = mainTexture6;
							material9.mainTextureOffset = material3.mainTextureOffset;
							material9.mainTextureScale = material3.mainTextureScale;
						}
						if (flag3)
						{
							material9.SetFloat("_CutOff", alphaCutOff);
							material9.EnableKeyword("HP_ALPHACLIP");
						}
						else
						{
							material9.DisableKeyword("HP_ALPHACLIP");
						}
						if (depthClip)
						{
							material9.EnableKeyword("HP_DEPTHCLIP");
						}
						else
						{
							material9.DisableKeyword("HP_DEPTHCLIP");
						}
					}
				}
			}
		}

		private int GetZTestValue(Visibility param)
		{
			return param switch
			{
				Visibility.AlwaysOnTop => 8, 
				Visibility.OnlyWhenOccluded => 5, 
				_ => 4, 
			};
		}

		private void BuildQuad()
		{
			quadMesh = new Mesh();
			Vector3[] array = new Vector3[4];
			float num = 0.5f;
			float num2 = 0.5f;
			array[0] = new Vector3(0f - num2, 0f - num, 0f);
			array[1] = new Vector3(0f - num2, num, 0f);
			array[2] = new Vector3(num2, 0f - num, 0f);
			array[3] = new Vector3(num2, num, 0f);
			Vector2[] array2 = new Vector2[array.Length];
			array2[0] = new Vector2(0f, 0f);
			array2[1] = new Vector2(0f, 1f);
			array2[2] = new Vector2(1f, 0f);
			array2[3] = new Vector2(1f, 1f);
			int[] triangles = new int[6] { 0, 1, 2, 3, 2, 1 };
			Vector3[] array3 = new Vector3[array.Length];
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i] = Vector3.forward;
			}
			quadMesh.vertices = array;
			quadMesh.uv = array2;
			quadMesh.triangles = triangles;
			quadMesh.normals = array3;
			quadMesh.RecalculateBounds();
		}

		private bool CheckOcclusion(Camera cam)
		{
			if (Time.time - occlusionCheckLastTime < seeThroughOccluderCheckInterval)
			{
				return lastOcclusionTestResult;
			}
			occlusionCheckLastTime = Time.time;
			if (rms.Length == 0 || rms[0].renderer == null)
			{
				return false;
			}
			Vector3 position = cam.transform.position;
			Bounds bounds = rms[0].renderer.bounds;
			for (int i = 1; i < rms.Length; i++)
			{
				if (rms[i].renderer != null)
				{
					bounds.Encapsulate(rms[i].renderer.bounds);
				}
			}
			Vector3 center = bounds.center;
			Vector3 vector = center - position;
			float maxDistance = Vector3.Distance(center, position);
			lastOcclusionTestResult = Physics.BoxCast(center - vector, bounds.extents * seeThroughOccluderThreshold, vector.normalized, Quaternion.identity, maxDistance, seeThroughOccluderMask);
			return lastOcclusionTestResult;
		}

		private void AverageNormals(int objIndex)
		{
			if (rms == null || objIndex >= rms.Length)
			{
				return;
			}
			Mesh mesh = rms[objIndex].mesh;
			if (!smoothMeshes.TryGetValue(mesh, out var value))
			{
				if (!mesh.isReadable)
				{
					return;
				}
				if (normals == null)
				{
					normals = new List<Vector3>();
				}
				else
				{
					normals.Clear();
				}
				mesh.GetNormals(normals);
				int count = normals.Count;
				if (count == 0)
				{
					return;
				}
				if (vertices == null)
				{
					vertices = new List<Vector3>();
				}
				else
				{
					vertices.Clear();
				}
				mesh.GetVertices(vertices);
				int num = vertices.Count;
				if (count < num)
				{
					num = count;
				}
				if (newNormals == null || newNormals.Length < num)
				{
					newNormals = new Vector3[num];
				}
				else
				{
					Vector3 zero = Vector3.zero;
					for (int i = 0; i < num; i++)
					{
						newNormals[i] = zero;
					}
				}
				if (matches == null || matches.Length < num)
				{
					matches = new int[num];
				}
				vv.Clear();
				for (int j = 0; j < num; j++)
				{
					Vector3 key = vertices[j];
					if (!vv.TryGetValue(key, out var value2))
					{
						value2 = (vv[key] = j);
					}
					matches[j] = value2;
				}
				for (int k = 0; k < num; k++)
				{
					int num3 = matches[k];
					newNormals[num3] += normals[k];
				}
				for (int l = 0; l < num; l++)
				{
					int num4 = matches[l];
					normals[l] = newNormals[num4].normalized;
				}
				value = UnityEngine.Object.Instantiate(mesh);
				value.hideFlags = HideFlags.DontSave;
				value.SetNormals(normals);
				smoothMeshes[mesh] = value;
			}
			rms[objIndex].mesh = value;
		}

		public void HitFX(Color color, float fadeOutDuration, float initialIntensity = 1f)
		{
			hitInitialIntensity = initialIntensity;
			hitFadeOutDuration = fadeOutDuration;
			hitColor = color;
			hitStartTime = Time.time;
			hitActive = true;
			if (overlay == 0f)
			{
				UpdateMaterialProperties();
			}
		}

		public static void RegisterOccluder(HighlightSeeThroughOccluder occluder)
		{
			if (!occluders.Contains(occluder))
			{
				occluders.Add(occluder);
			}
		}

		public static void UnregisterOccluder(HighlightSeeThroughOccluder occluder)
		{
			if (occluders.Contains(occluder))
			{
				occluders.Remove(occluder);
			}
		}

		public void RenderOccluders(Camera cam)
		{
			int count = occluders.Count;
			if (count == 0 || rmsCount == 0)
			{
				return;
			}
			Vector3 position = cam.transform.position;
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				HighlightSeeThroughOccluder highlightSeeThroughOccluder = occluders[i];
				if (!(highlightSeeThroughOccluder == null) && highlightSeeThroughOccluder.isActiveAndEnabled && highlightSeeThroughOccluder.detectionMethod == DetectionMethod.RayCast)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Bounds bounds = default;
				for (int j = 0; j < rms.Length; j++)
				{
					if (rms[j].renderer != null)
					{
						if (bounds.size.x == 0f)
						{
							bounds = rms[j].renderer.bounds;
						}
						else
						{
							bounds.Encapsulate(rms[j].renderer.bounds);
						}
					}
				}
				Vector3 center = bounds.center;
				Vector3 vector = center - position;
				float maxDistance = Vector3.Distance(center, position);
				if (hits == null || hits.Length == 0)
				{
					hits = new RaycastHit[64];
				}
				int num = Physics.BoxCastNonAlloc(center - vector, bounds.extents * 0.9f, vector.normalized, hits, Quaternion.identity, maxDistance);
				for (int k = 0; k < num; k++)
				{
					for (int l = 0; l < count; l++)
					{
						if (hits[k].collider.transform == occluders[l].transform)
						{
							cancelSeeThroughThisFrame = true;
							return;
						}
					}
				}
			}
			occludersFrameCount.TryGetValue(cam, out var value);
			int frameCount = Time.frameCount;
			if (frameCount == value)
			{
				return;
			}
			occludersFrameCount[cam] = frameCount;
			if (cbOccluder == null)
			{
				cbOccluder = new CommandBuffer();
				cbOccluder.name = "Occluder";
			}
			if (fxMatOccluder == null)
			{
				InitMaterial(ref fxMatOccluder, "HighlightPlus/Geometry/SeeThroughOccluder");
				if (fxMatOccluder == null)
				{
					return;
				}
			}
			cbOccluder.Clear();
			for (int m = 0; m < count; m++)
			{
				HighlightSeeThroughOccluder highlightSeeThroughOccluder2 = occluders[m];
				if (highlightSeeThroughOccluder2 == null || !highlightSeeThroughOccluder2.isActiveAndEnabled || highlightSeeThroughOccluder2.detectionMethod != DetectionMethod.Stencil || highlightSeeThroughOccluder2.meshData == null || highlightSeeThroughOccluder2.meshData.Length == 0)
				{
					continue;
				}
				for (int n = 0; n < highlightSeeThroughOccluder2.meshData.Length; n++)
				{
					Renderer renderer = highlightSeeThroughOccluder2.meshData[n].renderer;
					if (renderer.isVisible)
					{
						for (int num2 = 0; num2 < highlightSeeThroughOccluder2.meshData[n].subMeshCount; num2++)
						{
							cbOccluder.DrawRenderer(renderer, fxMatOccluder, num2);
						}
					}
				}
			}
			Graphics.ExecuteCommandBuffer(cbOccluder);
		}
	}
}
