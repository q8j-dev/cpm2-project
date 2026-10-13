using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace VLB
{
	[ExecuteInEditMode]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-lightbeam/")]
	[AddComponentMenu("")]
	public class BeamGeometry : MonoBehaviour
	{
		private VolumetricLightBeam m_Master;

		private Matrix4x4 m_ColorGradientMatrix;

		private MeshType m_CurrentMeshType;

		private Material m_CustomMaterial;

		private Plane m_ClippingPlaneWS;

		public MeshRenderer meshRenderer { get; private set; }

		public MeshFilter meshFilter { get; private set; }

		public Mesh coneMesh { get; private set; }

		public bool visible
		{
			get
			{
				return meshRenderer.enabled;
			}
			set
			{
				meshRenderer.enabled = value;
			}
		}

		public int sortingLayerID
		{
			get
			{
				return meshRenderer.sortingLayerID;
			}
			set
			{
				meshRenderer.sortingLayerID = value;
			}
		}

		public int sortingOrder
		{
			get
			{
				return meshRenderer.sortingOrder;
			}
			set
			{
				meshRenderer.sortingOrder = value;
			}
		}

		public static bool isCustomRenderPipelineSupported => true;

		private bool isNoiseEnabled
		{
			get
			{
				if (m_Master.isNoiseEnabled && m_Master.noiseIntensity > 0f)
				{
					return Noise3D.isSupported;
				}
				return false;
			}
		}

		private bool isClippingPlaneEnabled => m_ClippingPlaneWS.normal.sqrMagnitude > 0f;

		private bool isDepthBlendEnabled
		{
			get
			{
				if (!GpuInstancing.forceEnableDepthBlend)
				{
					return m_Master.depthBlendDistance > 0f;
				}
				return true;
			}
		}

		private float ComputeFadeOutFactor(Transform camTransform)
		{
			if (m_Master.isFadeOutEnabled)
			{
				float value = Vector3.SqrMagnitude(meshRenderer.bounds.center - camTransform.position);
				return Mathf.InverseLerp(m_Master.fadeOutEnd * m_Master.fadeOutEnd, m_Master.fadeOutBegin * m_Master.fadeOutBegin, value);
			}
			return 1f;
		}

		private IEnumerator CoUpdateFadeOut()
		{
			while (m_Master.isFadeOutEnabled)
			{
				ComputeFadeOutFactor();
				yield return null;
			}
			SetFadeOutFactorProp(1f);
		}

		private void ComputeFadeOutFactor()
		{
			Transform fadeOutCameraTransform = Config.Instance.fadeOutCameraTransform;
			if ((bool)fadeOutCameraTransform)
			{
				float num = ComputeFadeOutFactor(fadeOutCameraTransform);
				if (num > 0f)
				{
					meshRenderer.enabled = true;
					SetFadeOutFactorProp(num);
				}
				else
				{
					meshRenderer.enabled = false;
				}
			}
			else
			{
				SetFadeOutFactorProp(1f);
			}
		}

		private void SetFadeOutFactorProp(float value)
		{
			MaterialChangeStart();
			SetMaterialProp(ShaderProperties.FadeOutFactor, value);
			MaterialChangeStop();
		}

		private void RestartFadeOutCoroutine()
		{
			StopAllCoroutines();
			if ((bool)m_Master && m_Master.isFadeOutEnabled)
			{
				StartCoroutine(CoUpdateFadeOut());
			}
		}

		private void Start()
		{
			if (!m_Master)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			if ((bool)m_CustomMaterial)
			{
				UnityEngine.Object.DestroyImmediate(m_CustomMaterial);
				m_CustomMaterial = null;
			}
		}

		private static bool IsUsingCustomRenderPipeline()
		{
			if (RenderPipelineManager.currentPipeline == null)
			{
				return GraphicsSettings.defaultRenderPipeline != null;
			}
			return true;
		}

		private void OnDisable()
		{
			if (IsUsingCustomRenderPipeline())
			{
				RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
			}
		}

		private void OnEnable()
		{
			RestartFadeOutCoroutine();
			if (IsUsingCustomRenderPipeline())
			{
				RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
			}
		}

		public void Initialize(VolumetricLightBeam master)
		{
			HideFlags proceduralObjectsHideFlags = Consts.ProceduralObjectsHideFlags;
			m_Master = master;
			base.transform.SetParent(master.transform, worldPositionStays: false);
			meshRenderer = base.gameObject.GetOrAddComponent<MeshRenderer>();
			meshRenderer.hideFlags = proceduralObjectsHideFlags;
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			if (Config.Instance.actualRenderingMode != RenderingMode.GPUInstancing)
			{
				m_CustomMaterial = MaterialManager.NewMaterial(gpuInstanced: false);
				ApplyMaterial();
			}
			if (SortingLayer.IsValid(m_Master.sortingLayerID))
			{
				sortingLayerID = m_Master.sortingLayerID;
			}
			else
			{
				Debug.LogError($"Beam '{Utils.GetPath(m_Master.transform)}' has an invalid sortingLayerID ({m_Master.sortingLayerID}). Please fix it by setting a valid layer.");
			}
			sortingOrder = m_Master.sortingOrder;
			meshFilter = base.gameObject.GetOrAddComponent<MeshFilter>();
			meshFilter.hideFlags = proceduralObjectsHideFlags;
			base.gameObject.hideFlags = proceduralObjectsHideFlags;
			RestartFadeOutCoroutine();
		}

		public void RegenerateMesh()
		{
			if (Config.Instance.geometryOverrideLayer)
			{
				base.gameObject.layer = Config.Instance.geometryLayerID;
			}
			else
			{
				base.gameObject.layer = m_Master.gameObject.layer;
			}
			base.gameObject.tag = Config.Instance.geometryTag;
			if ((bool)coneMesh && m_CurrentMeshType == MeshType.Custom)
			{
				UnityEngine.Object.DestroyImmediate(coneMesh);
			}
			m_CurrentMeshType = m_Master.geomMeshType;
			switch (m_Master.geomMeshType)
			{
			case MeshType.Custom:
				coneMesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, m_Master.geomCustomSides, m_Master.geomCustomSegments, m_Master.geomCap, Config.Instance.useSinglePassShader);
				coneMesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				meshFilter.mesh = coneMesh;
				break;
			case MeshType.Shared:
				coneMesh = GlobalMesh.Get();
				meshFilter.sharedMesh = coneMesh;
				break;
			default:
				Debug.LogError("Unsupported MeshType");
				break;
			}
			UpdateMaterialAndBounds();
		}

		private void ComputeLocalMatrix()
		{
			float num = Mathf.Max(m_Master.coneRadiusStart, m_Master.coneRadiusEnd);
			base.transform.localScale = new Vector3(num, num, m_Master.fallOffEnd);
		}

		private bool ApplyMaterial()
		{
			MaterialManager.ColorGradient colorGradient = MaterialManager.ColorGradient.Off;
			if (m_Master.colorMode == ColorMode.Gradient)
			{
				colorGradient = ((Utils.GetFloatPackingPrecision() != Utils.FloatPackingPrecision.High) ? MaterialManager.ColorGradient.MatrixLow : MaterialManager.ColorGradient.MatrixHigh);
			}
			MaterialManager.StaticProperties staticProperties = new MaterialManager.StaticProperties
			{
				blendingMode = (MaterialManager.BlendingMode)m_Master.blendingMode,
				noise3D = (isNoiseEnabled ? MaterialManager.Noise3D.On : MaterialManager.Noise3D.Off),
				depthBlend = (isDepthBlendEnabled ? MaterialManager.DepthBlend.On : MaterialManager.DepthBlend.Off),
				colorGradient = colorGradient,
				clippingPlane = (isClippingPlaneEnabled ? MaterialManager.ClippingPlane.On : MaterialManager.ClippingPlane.Off)
			};
			Material material = null;
			if (Config.Instance.actualRenderingMode != RenderingMode.GPUInstancing)
			{
				material = m_CustomMaterial;
				if ((bool)material)
				{
					staticProperties.ApplyToMaterial(material);
				}
			}
			else
			{
				material = MaterialManager.GetInstancedMaterial(m_Master._INTERNAL_InstancedMaterialGroupID, staticProperties);
			}
			meshRenderer.material = material;
			return material != null;
		}

		private void SetMaterialProp(int nameID, float value)
		{
			if ((bool)m_CustomMaterial)
			{
				m_CustomMaterial.SetFloat(nameID, value);
			}
			else
			{
				MaterialManager.materialPropertyBlock.SetFloat(nameID, value);
			}
		}

		private void SetMaterialProp(int nameID, Vector4 value)
		{
			if ((bool)m_CustomMaterial)
			{
				m_CustomMaterial.SetVector(nameID, value);
			}
			else
			{
				MaterialManager.materialPropertyBlock.SetVector(nameID, value);
			}
		}

		private void SetMaterialProp(int nameID, Color value)
		{
			if ((bool)m_CustomMaterial)
			{
				m_CustomMaterial.SetColor(nameID, value);
			}
			else
			{
				MaterialManager.materialPropertyBlock.SetColor(nameID, value);
			}
		}

		private void SetMaterialProp(int nameID, Matrix4x4 value)
		{
			if ((bool)m_CustomMaterial)
			{
				m_CustomMaterial.SetMatrix(nameID, value);
			}
			else
			{
				MaterialManager.materialPropertyBlock.SetMatrix(nameID, value);
			}
		}

		private void MaterialChangeStart()
		{
			if (m_CustomMaterial == null)
			{
				meshRenderer.GetPropertyBlock(MaterialManager.materialPropertyBlock);
			}
		}

		private void MaterialChangeStop()
		{
			if (m_CustomMaterial == null)
			{
				meshRenderer.SetPropertyBlock(MaterialManager.materialPropertyBlock);
			}
		}

		private void SendMaterialClippingPlaneProp()
		{
			SetMaterialProp(ShaderProperties.ClippingPlaneWS, new Vector4(m_ClippingPlaneWS.normal.x, m_ClippingPlaneWS.normal.y, m_ClippingPlaneWS.normal.z, m_ClippingPlaneWS.distance));
		}

		public void UpdateMaterialAndBounds()
		{
			if (ApplyMaterial())
			{
				MaterialChangeStart();
				if (isClippingPlaneEnabled && m_CustomMaterial == null)
				{
					SendMaterialClippingPlaneProp();
				}
				float f = m_Master.coneAngle * ((float)Math.PI / 180f) / 2f;
				SetMaterialProp(ShaderProperties.ConeSlopeCosSin, new Vector2(Mathf.Cos(f), Mathf.Sin(f)));
				SetMaterialProp(value: new Vector2(Mathf.Max(m_Master.coneRadiusStart, 0.0001f), Mathf.Max(m_Master.coneRadiusEnd, 0.0001f)), nameID: ShaderProperties.ConeRadius);
				float value = Mathf.Sign(m_Master.coneApexOffsetZ) * Mathf.Max(Mathf.Abs(m_Master.coneApexOffsetZ), 0.0001f);
				SetMaterialProp(ShaderProperties.ConeApexOffsetZ, value);
				if (m_Master.colorMode == ColorMode.Flat)
				{
					SetMaterialProp(ShaderProperties.ColorFlat, m_Master.color);
				}
				else
				{
					Utils.FloatPackingPrecision floatPackingPrecision = Utils.GetFloatPackingPrecision();
					m_ColorGradientMatrix = m_Master.colorGradient.SampleInMatrix((int)floatPackingPrecision);
				}
				SetMaterialProp(ShaderProperties.AlphaInside, m_Master.intensityInside);
				SetMaterialProp(ShaderProperties.AlphaOutside, m_Master.intensityOutside);
				SetMaterialProp(ShaderProperties.AttenuationLerpLinearQuad, m_Master.attenuationLerpLinearQuad);
				SetMaterialProp(ShaderProperties.DistanceFadeStart, m_Master.fallOffStart);
				SetMaterialProp(ShaderProperties.DistanceFadeEnd, m_Master.fallOffEnd);
				SetMaterialProp(ShaderProperties.DistanceCamClipping, m_Master.cameraClippingDistance);
				SetMaterialProp(ShaderProperties.FresnelPow, Mathf.Max(0.001f, m_Master.fresnelPow));
				SetMaterialProp(ShaderProperties.GlareBehind, m_Master.glareBehind);
				SetMaterialProp(ShaderProperties.GlareFrontal, m_Master.glareFrontal);
				SetMaterialProp(ShaderProperties.DrawCap, m_Master.geomCap ? 1 : 0);
				if (isDepthBlendEnabled)
				{
					SetMaterialProp(ShaderProperties.DepthBlendDistance, m_Master.depthBlendDistance);
				}
				if (isNoiseEnabled)
				{
					Noise3D.LoadIfNeeded();
					SetMaterialProp(ShaderProperties.NoiseLocal, new Vector4(m_Master.noiseVelocityLocal.x, m_Master.noiseVelocityLocal.y, m_Master.noiseVelocityLocal.z, m_Master.noiseScaleLocal));
					SetMaterialProp(ShaderProperties.NoiseParam, new Vector4(m_Master.noiseIntensity, m_Master.noiseVelocityUseGlobal ? 1f : 0f, m_Master.noiseScaleUseGlobal ? 1f : 0f, (m_Master.noiseMode == NoiseMode.WorldSpace) ? 0f : 1f));
				}
				ComputeLocalMatrix();
				if (IsUsingCustomRenderPipeline() && Config.Instance.actualRenderingMode == RenderingMode.GPUInstancing)
				{
					SetMaterialProp(ShaderProperties.LocalToWorldMatrix, base.transform.localToWorldMatrix);
					SetMaterialProp(ShaderProperties.WorldToLocalMatrix, base.transform.worldToLocalMatrix);
				}
				MaterialChangeStop();
			}
		}

		public void SetClippingPlane(Plane planeWS)
		{
			m_ClippingPlaneWS = planeWS;
			if ((bool)m_CustomMaterial)
			{
				m_CustomMaterial.EnableKeyword("VLB_CLIPPING_PLANE");
				SendMaterialClippingPlaneProp();
			}
			else
			{
				UpdateMaterialAndBounds();
			}
		}

		public void SetClippingPlaneOff()
		{
			m_ClippingPlaneWS = default;
			if ((bool)m_CustomMaterial)
			{
				m_CustomMaterial.DisableKeyword("VLB_CLIPPING_PLANE");
			}
			else
			{
				UpdateMaterialAndBounds();
			}
		}

		private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
		{
			UpdateCameraRelatedProperties(cam);
		}

		private void OnWillRenderObject()
		{
			if (!IsUsingCustomRenderPipeline())
			{
				Camera current = Camera.current;
				if (current != null)
				{
					UpdateCameraRelatedProperties(current);
				}
			}
		}

		private static bool IsEditorCamera(Camera cam)
		{
			return false;
		}

		private void UpdateCameraRelatedProperties(Camera cam)
		{
			if ((bool)cam && (bool)m_Master)
			{
				MaterialChangeStart();
				Vector3 posOS = m_Master.transform.InverseTransformPoint(cam.transform.position);
				Vector3 normalized = base.transform.InverseTransformDirection(cam.transform.forward).normalized;
				float w = (cam.orthographic ? (-1f) : m_Master.GetInsideBeamFactorFromObjectSpacePos(posOS));
				SetMaterialProp(ShaderProperties.CameraParams, new Vector4(normalized.x, normalized.y, normalized.z, w));
				if (IsUsingCustomRenderPipeline())
				{
					Vector2 vector = (cam.allowDynamicResolution ? new Vector2(ScalableBufferManager.widthScaleFactor, ScalableBufferManager.heightScaleFactor) : Vector2.one);
					SetMaterialProp(ShaderProperties.CameraBufferSizeSRP, vector);
				}
				if (m_Master.colorMode == ColorMode.Gradient)
				{
					SetMaterialProp(ShaderProperties.ColorGradientMatrix, m_ColorGradientMatrix);
				}
				MaterialChangeStop();
				if (m_Master.depthBlendDistance > 0f)
				{
					cam.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
		}
	}
}
