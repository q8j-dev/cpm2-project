using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

internal class NonCachingLoadExample : MonoBehaviour
{
	public string BundleURL;

	public string AssetName;

	private async UniTaskVoid Start()
	{
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		try
		{
			WWW www = new WWW(BundleURL);
			try
			{
				await UniTask.WaitUntil(() => www.isDone, PlayerLoopTiming.Update, cancellationTokenOnDestroy);
				if (www.error != null)
				{
					throw new Exception("WWW download had an error:" + www.error);
				}
				AssetBundle assetBundle = www.assetBundle;
				if (AssetName == "")
				{
					AssetBundle.LoadFromFileAsync(BundleURL);
				}
				else
				{
					UnityEngine.Object.Instantiate(assetBundle.LoadAsset(AssetName));
				}
				assetBundle.Unload(unloadAllLoadedObjects: false);
			}
			finally
			{
				if (www != null)
				{
					((IDisposable)www).Dispose();
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}
}
