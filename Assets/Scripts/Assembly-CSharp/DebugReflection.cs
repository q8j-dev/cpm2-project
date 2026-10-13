using UnityEngine;
using UnityEngine.Rendering;

public class DebugReflection : MonoBehaviour
{
	private int cubemapSize = 128;

	private bool oneFacePerFrame = default;

	private Camera cam;

	private RenderTexture renderTexture;

	private void Start()
	{
		UpdateCubemap(63);
	}

	private void OnDisable()
	{
		Object.DestroyImmediate(cam);
		Object.DestroyImmediate(renderTexture);
		Shader.DisableKeyword("_CARREFLECTION");
	}

	private void LateUpdate()
	{
		if (oneFacePerFrame)
		{
			int num = Time.frameCount % 6;
			int faceMask = 1 << num;
			UpdateCubemap(faceMask);
		}
		else
		{
			UpdateCubemap(63);
		}
	}

	private void UpdateCubemap(int faceMask)
	{
		if (!cam)
		{
			GameObject gameObject = new GameObject("CubemapCamera", typeof(Camera));
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.transform.position = base.transform.position;
			gameObject.transform.rotation = Quaternion.identity;
			cam = gameObject.GetComponent<Camera>();
			cam.farClipPlane = 100f;
			cam.enabled = false;
		}
		if (!renderTexture)
		{
			renderTexture = new RenderTexture(cubemapSize, cubemapSize, 16);
			renderTexture.format = RenderTextureFormat.DefaultHDR;
			renderTexture.useMipMap = true;
			renderTexture.dimension = TextureDimension.Cube;
			renderTexture.hideFlags = HideFlags.HideAndDontSave;
			Shader.EnableKeyword("_CARREFLECTION");
			Shader.SetGlobalTexture("_ReflectionCube", renderTexture);
		}
		cam.transform.position = base.transform.position;
		cam.RenderToCubemap(renderTexture, faceMask);
	}
}
