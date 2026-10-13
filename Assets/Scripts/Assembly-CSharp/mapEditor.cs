using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class mapEditor : MonoBehaviour
{
	public Camera cam;

	public RenderTexture texture;

	private void Start()
	{
		cam.Render();
		Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			startNow();
		}).AddTo(this);
	}

	private void startNow()
	{
		TakeSnapshot(this.GetCancellationTokenOnDestroy()).Forget();
	}

	public async UniTask TakeSnapshot(CancellationToken ct = default(CancellationToken))
	{
		try
		{
			await UniTask.WaitForEndOfFrame(this, ct);
			Texture2D texture2D = new Texture2D(1000, 1000, TextureFormat.RGB24, mipChain: false);
			MonoBehaviour.print(texture.width + "  " + texture.height + "  " + texture2D.width);
			texture2D.ReadPixels(new Rect(0f, 0f, texture.width, texture.height), 0, 0);
			texture2D.Apply();
			byte[] bytes = texture2D.EncodeToPNG();
			string text = Application.dataPath + "/../SaveImages/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			MonoBehaviour.print("writing imagesdWssx2xsqa3awqs");
			File.WriteAllBytes(text + "ImageMainMap.png", bytes);
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void Update()
	{
	}
}
