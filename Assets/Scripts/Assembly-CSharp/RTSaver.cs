using System.IO;
using UnityEngine;

public class RTSaver : MonoBehaviour
{
	public RenderTexture rt;

	public void SaveTexture()
	{
		byte[] bytes = toTexture2D(rt).EncodeToPNG();
		File.WriteAllBytes(Path.Combine(Application.dataPath, rt.name + ".png"), bytes);
	}

	private Texture2D toTexture2D(RenderTexture rTex)
	{
		Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.RGBAHalf, mipChain: false);
		RenderTexture.active = rTex;
		texture2D.ReadPixels(new Rect(0f, 0f, rTex.width, rTex.height), 0, 0);
		texture2D.Apply();
		return texture2D;
	}

	private void Start()
	{
		SaveTexture();
	}

	private void Update()
	{
	}
}
