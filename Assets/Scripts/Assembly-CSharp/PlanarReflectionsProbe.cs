using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
[AddComponentMenu("Rendering/Planar Reflections Probe")]
public class PlanarReflectionsProbe : MonoBehaviour
{
	[Range(1f, 4f)]
	public int targetTextureID = 1;

	[Space(10f)]
	public bool useCustomNormal;

	public Vector3 customNormal;

	[Space(10f)]
	[Range(0.01f, 1f)]
	public float reflectionsQuality = 1f;

	public float farClipPlane = 1000f;

	public bool renderBackground = true;

	[Space(10f)]
	public bool renderInEditor;

	public GameObject _probeGO;

	public Camera _probe;

	private Skybox _probeSkybox;

	private Dictionary<Camera, RenderTexture> _camTextureMap = new Dictionary<Camera, RenderTexture>();

	private ArrayList _ignoredCameras = new ArrayList();

	private void OnEnable()
	{
		RenderPipelineManager.beginCameraRendering += PreRender;
	}

	private void OnDisable()
	{
		FinalizeProbe();
		RenderPipelineManager.beginCameraRendering -= PreRender;
	}

	private void OnDestroy()
	{
		FinalizeProbe();
		RenderPipelineManager.beginCameraRendering -= PreRender;
	}

	private void InitializeProbe()
	{
		_probeGO = new GameObject("", typeof(Camera), typeof(Skybox));
		_probeGO.name = "PRCamera" + _probeGO.GetInstanceID();
		_probeGO.hideFlags = HideFlags.HideAndDontSave;
		_probe = _probeGO.GetComponent<Camera>();
		_probeSkybox = _probeGO.GetComponent<Skybox>();
		_probeSkybox.enabled = false;
		_probeSkybox.material = null;
	}

	private void FinalizeProbe()
	{
		if (!(_probe == null))
		{
			if (Application.isEditor)
			{
				Object.DestroyImmediate(_probeGO);
			}
			else
			{
				Object.Destroy(_probeGO);
			}
		}
	}

	private void CleanupRenderTextures()
	{
		foreach (RenderTexture value in _camTextureMap.Values)
		{
			value.Release();
		}
		_camTextureMap.Clear();
	}

	private bool CheckCamera(Camera cam)
	{
		if (cam.cameraType == CameraType.Reflection)
		{
			return true;
		}
		if (!renderInEditor && cam.cameraType == CameraType.SceneView)
		{
			return true;
		}
		if (_ignoredCameras.Contains(cam))
		{
			return true;
		}
		return false;
	}

	private void PreRender(ScriptableRenderContext context, Camera cam)
	{
		if (!CheckCamera(cam))
		{
			if (_probe == null)
			{
				InitializeProbe();
			}
			Vector3 normal = GetNormal();
			UpdateProbeSettings(cam);
			CreateRenderTexture(cam);
			UpdateProbeTransform(cam, normal);
			CalculateObliqueProjection(normal);
			UniversalRenderPipeline.RenderSingleCamera(context, _probe);
			string globalShaderProperty = "_PlanarReflectionsTex" + targetTextureID;
			_probe.targetTexture.SetGlobalShaderProperty(globalShaderProperty);
			Shader.SetGlobalMatrix("_PlanarReflectionVP", cam.projectionMatrix * cam.worldToCameraMatrix);
			Shader.SetGlobalVector("_PlanarPlanePos", base.transform.position);
			Shader.SetGlobalVector("_PlanarPlaneNormal", GetNormal());
		}
	}

	private void UpdateProbeSettings(Camera cam)
	{
		_probe.CopyFrom(cam);
		_probe.enabled = false;
		_probe.cameraType = CameraType.Reflection;
		_probe.usePhysicalProperties = false;
		_probe.farClipPlane = farClipPlane;
		_probeSkybox.material = null;
		_probeSkybox.enabled = false;
		if (renderBackground)
		{
			_probe.clearFlags = cam.clearFlags;
			if ((bool)cam.GetComponent<Skybox>())
			{
				Skybox component = cam.GetComponent<Skybox>();
				_probeSkybox.material = component.material;
				_probeSkybox.enabled = component.enabled;
			}
		}
		else
		{
			_probe.clearFlags = CameraClearFlags.Nothing;
		}
	}

	private void CreateRenderTexture(Camera cam)
	{
		int num = (int)((float)cam.pixelWidth * reflectionsQuality);
		int num2 = (int)((float)cam.pixelHeight * reflectionsQuality);
		RenderTexture valueOrDefault = _camTextureMap.GetValueOrDefault(cam, null);
		if (!valueOrDefault || valueOrDefault.width != num || valueOrDefault.height != num2)
		{
			if ((bool)valueOrDefault)
			{
				_camTextureMap.Remove(cam);
				valueOrDefault.Release();
			}
			_probe.targetTexture = new RenderTexture(num, num2, 24);
			_probe.targetTexture.Create();
			_camTextureMap.Add(cam, _probe.targetTexture);
		}
		else
		{
			_probe.targetTexture = valueOrDefault;
		}
	}

	private Vector3 GetNormal()
	{
		if (!useCustomNormal)
		{
			return base.transform.forward;
		}
		if (customNormal.Equals(Vector3.zero))
		{
			return Vector3.up;
		}
		return customNormal.normalized;
	}

	private void UpdateProbeTransform(Camera cam, Vector3 normal)
	{
		Vector3 vector = normal * Vector3.Dot(normal, cam.transform.position - base.transform.position);
		_probe.transform.position = cam.transform.position - 2f * vector;
		Vector3 vector2 = Vector3.Reflect(cam.transform.forward, normal);
		Vector3 worldUp = Vector3.Reflect(cam.transform.up, normal);
		_probe.transform.LookAt(_probe.transform.position + vector2, worldUp);
	}

	private void CalculateObliqueProjection(Vector3 normal)
	{
		Matrix4x4 worldToCameraMatrix = _probe.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(base.transform.position);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal);
		Vector4 clipPlane = new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
		_probe.projectionMatrix = _probe.CalculateObliqueMatrix(clipPlane);
	}

	public void IgnoreCamera(Camera cam)
	{
		if (!_ignoredCameras.Contains(cam))
		{
			_ignoredCameras.Add(cam);
		}
	}

	public void UnignoreCamera(Camera cam)
	{
		if (_ignoredCameras.Contains(cam))
		{
			_ignoredCameras.Remove(cam);
		}
	}

	public void ClearIgnoredList()
	{
		_ignoredCameras.Clear();
	}

	public bool IsIgnoring(Camera cam)
	{
		return _ignoredCameras.Contains(cam);
	}

	public static PlanarReflectionsProbe[] FindProbesRenderingTo(int id)
	{
		PlanarReflectionsProbe[] array = Object.FindObjectsOfType<PlanarReflectionsProbe>();
		ArrayList arrayList = new ArrayList();
		PlanarReflectionsProbe[] array2 = array;
		foreach (PlanarReflectionsProbe planarReflectionsProbe in array2)
		{
			if (planarReflectionsProbe.targetTextureID == id)
			{
				arrayList.Add(planarReflectionsProbe);
			}
		}
		return (PlanarReflectionsProbe[])arrayList.ToArray(typeof(PlanarReflectionsProbe));
	}

	public static PlanarReflectionsProbe FindProbeRenderingTo(int id)
	{
		PlanarReflectionsProbe[] array = Object.FindObjectsOfType<PlanarReflectionsProbe>();
		foreach (PlanarReflectionsProbe planarReflectionsProbe in array)
		{
			if (planarReflectionsProbe.targetTextureID == id)
			{
				return planarReflectionsProbe;
			}
		}
		return null;
	}
}
