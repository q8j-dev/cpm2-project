using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

public class StageRender : MonoBehaviour
{
	private GameObject Target;

	public Transform ObjectRoot;

	private RenderTexture RT;

	public Camera RTCam;

	public StageLightingPreset LightingPreset;

	private MaterialPropertyBlock block;

	private ReflectionProbe Reflection;

	private float WindowWidth = 1f;

	private float WindowHeigth = 1f;

	private IPrefabFactory _prefabFactory;

	private AsyncOperationHandle<GameObject> _lastHandler;

	private static int[] _idSHA = new int[3]
	{
		Shader.PropertyToID("unity_SHAr"),
		Shader.PropertyToID("unity_SHAg"),
		Shader.PropertyToID("unity_SHAb")
	};

	private static int[] _idSHB = new int[3]
	{
		Shader.PropertyToID("unity_SHBr"),
		Shader.PropertyToID("unity_SHBg"),
		Shader.PropertyToID("unity_SHBb")
	};

	private static int _idSHC = Shader.PropertyToID("unity_SHC");

	[Inject]
	public void Construct(IPrefabFactory prefabFactory)
	{
		_prefabFactory = prefabFactory;
	}

	private void Start()
	{
		Reflection = GetComponent<ReflectionProbe>();
		if (!Reflection)
		{
			CreateReflection();
		}
		base.transform.position = Vector3.down * 1000f;
	}

	private void OnDestroy()
	{
		if (CPMGraphicsSettingsManager.Instance.ReflectionProbes.Contains(Reflection))
		{
			CPMGraphicsSettingsManager.Instance.ReflectionProbes.Remove(Reflection);
		}
	}

	private void CreateReflection()
	{
		Reflection = base.gameObject.AddComponent<ReflectionProbe>();
		Reflection.mode = ReflectionProbeMode.Custom;
		Reflection.customBakedTexture = LightingPreset.Reflection;
		Reflection.blendDistance = 0f;
		Reflection.importance = 2;
		CPMGraphicsSettingsManager.Instance.ReflectionProbes.Add(Reflection);
	}

	private Vector3 AdjustForRenderTexture(Vector3 screenPoint, int rtWidth, int rtHeight)
	{
		float num = Screen.width;
		float num2 = Screen.height;
		float x = screenPoint.x * ((float)rtWidth / num);
		float y = screenPoint.y * ((float)rtHeight / num2);
		return new Vector3(x, y, screenPoint.z);
	}

	private void CreateRT(int Width, int Height)
	{
		RT = new RenderTexture(Width, Height, 16, RenderTextureFormat.ARGBHalf);
		RT.useDynamicScale = false;
	}

	public async UniTask<RenderTexture> RenderObjectStage(string path, int Width, int Height, CancellationToken token)
	{
		if ((bool)RT)
		{
			if (RT.width != Width || RT.height != Height)
			{
				RTCam.targetTexture = null;
				Object.DestroyImmediate(RT);
				CreateRT(Width, Height);
			}
		}
		else
		{
			CreateRT(Width, Height);
		}
		if ((bool)Target)
		{
			Object.DestroyImmediate(Target);
		}
		WindowWidth = Width;
		WindowHeigth = Height;
		if (_lastHandler.IsValid())
		{
			Addressables.Release(_lastHandler);
		}
		_lastHandler = Addressables.LoadAssetAsync<GameObject>(path);
		GameObject obj = await _lastHandler;
		if (token.IsCancellationRequested)
		{
			return null;
		}
		ObjectRoot.rotation = ((path == "Intercooler_0") ? Quaternion.Euler(0f, -90f, 0f) : ((Quaternion)quaternion.identity));
		Target = _prefabFactory.Instantiate(obj, ObjectRoot);
		Target.transform.position = Vector3.zero;
		Target.transform.localRotation = Quaternion.Euler(Vector3.zero);
		Renderer componentInChildren = Target.GetComponentInChildren<Renderer>();
		Bounds bounds = componentInChildren.bounds;
		block = new MaterialPropertyBlock();
		ApplySphericalHarmonicsBlock(block, LightingPreset.AmbientSH);
		Renderer[] componentsInChildren = Target.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.lightProbeUsage = LightProbeUsage.CustomProvided;
			renderer.SetPropertyBlock(block);
			bounds.Encapsulate(renderer.bounds);
		}
		Target.transform.localPosition = new Vector3(0f, 0f - bounds.center.y, 0f);
		bounds = componentInChildren.bounds;
		componentsInChildren = Target.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer2 in componentsInChildren)
		{
			bounds.Encapsulate(renderer2.bounds);
		}
		RTCam.targetTexture = RT;
		RTCam.enabled = true;
		RTCam.Render();
		Vector3 vector = AdjustForRenderTexture(RTCam.WorldToScreenPoint(bounds.min), Width, Height);
		WindowWidth = AdjustForRenderTexture(RTCam.WorldToScreenPoint(bounds.max), Width, Height).x - vector.x;
		return RT;
	}

	public void Cleanup()
	{
		if ((bool)RTCam)
		{
			RTCam.targetTexture = null;
			if ((bool)RT)
			{
				Object.DestroyImmediate(RT);
			}
			if ((bool)Target)
			{
				Object.DestroyImmediate(Target);
			}
			RTCam.enabled = false;
		}
		if (_lastHandler.IsValid())
		{
			Addressables.Release(_lastHandler);
		}
	}

	public void SetInput(Vector2 RelativeCursorPos, float RotationPower = 90f)
	{
		ObjectRoot.Rotate(Vector3.up, (0f - RelativeCursorPos.x) / WindowWidth * RotationPower);
		ObjectRoot.Rotate(Vector3.right * (RelativeCursorPos.y / WindowHeigth) * RotationPower, Space.World);
	}

	private void ApplySphericalHarmonicsBlock(MaterialPropertyBlock m, SphericalHarmonicsL2 sh)
	{
		for (int i = 0; i < 3; i++)
		{
			m.SetVector(_idSHA[i], new Vector4(sh[i, 3], sh[i, 1], sh[i, 2], sh[i, 0] - sh[i, 6]));
		}
		for (int j = 0; j < 3; j++)
		{
			m.SetVector(_idSHB[j], new Vector4(sh[j, 4], sh[j, 6], sh[j, 5] * 3f, sh[j, 7]));
		}
		m.SetVector(_idSHC, new Vector4(sh[0, 8], sh[2, 8], sh[1, 8], 1f));
	}
}
