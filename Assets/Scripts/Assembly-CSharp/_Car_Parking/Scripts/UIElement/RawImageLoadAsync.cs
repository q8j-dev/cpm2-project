using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CDN;

namespace _Car_Parking.Scripts.UIElement
{
	public class RawImageLoadAsync : MonoBehaviour
	{
		[SerializeField]
		private RawImage RawImage;

		[SerializeField]
		private GameObject LoadingImage;

		private CancellationTokenSource _cancellationTokenSourceLoad;

		private ICDNDownloader _cdnDownloader;

		private AsyncOperationHandle<Texture> _addressablesHandle;

		private string _lastPath;

		public RawImage ImageRaw => RawImage;

		public Texture Texture
		{
			get
			{
				if (!RawImage)
				{
					return null;
				}
				return RawImage.texture;
			}
			set
			{
				if ((bool)RawImage)
				{
					RawImage.texture = value;
				}
			}
		}

		public bool ImageActive
		{
			get
			{
				if ((bool)RawImage)
				{
					return RawImage.gameObject.activeSelf;
				}
				return false;
			}
			set
			{
				if ((bool)RawImage)
				{
					RawImage.gameObject.SetActive(value);
				}
				if ((bool)LoadingImage)
				{
					LoadingImage.SetActive(!value);
				}
			}
		}

		[Inject]
		public void Construct(ICDNDownloader cdnDownloader)
		{
			_cdnDownloader = cdnDownloader;
		}

		public void Load(string path)
		{
			if (!(_lastPath == path))
			{
				PrepareToLoad();
				if (!string.IsNullOrEmpty(path))
				{
					_lastPath = path;
					LoadFromResourcesAsync(path, _cancellationTokenSourceLoad.Token).Forget();
				}
			}
		}

		public void LoadFromAddressables(string address)
		{
			if (!(_lastPath == address))
			{
				PrepareToLoad();
				if (!string.IsNullOrEmpty(address))
				{
					_lastPath = address;
					LoadFromAddressablesAsync(address, _cancellationTokenSourceLoad.Token).Forget();
				}
			}
		}

		public void LoadFromSDN(string url)
		{
			if (!(_lastPath == url))
			{
				PrepareToLoad();
				if (!string.IsNullOrEmpty(url))
				{
					_lastPath = url;
					LoadFromSDNAsync(url, _cancellationTokenSourceLoad.Token).Forget();
				}
			}
		}

		private void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceLoad);
			ReleaseAddressablesHandle();
		}

		private void PrepareToLoad()
		{
			_lastPath = "";
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceLoad);
			_cancellationTokenSourceLoad = new CancellationTokenSource();
			ReleaseAddressablesHandle();
			ImageActive = false;
		}

		private void ReleaseAddressablesHandle()
		{
			if (_addressablesHandle.IsValid())
			{
				Addressables.Release(_addressablesHandle);
			}
		}

		private async UniTask LoadFromResourcesAsync(string path, CancellationToken cancellationToken)
		{
			Texture texture = (await Resources.LoadAsync<Texture>(path)) as Texture;
			if (!cancellationToken.IsCancellationRequested)
			{
				ApplyTexture(texture);
			}
		}

		private async UniTaskVoid LoadFromAddressablesAsync(string address, CancellationToken cancellationToken)
		{
			_addressablesHandle = Addressables.LoadAssetAsync<Texture>(address);
			await _addressablesHandle.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested)
			{
				ApplyTexture(_addressablesHandle.Result);
			}
		}

		private async UniTaskVoid LoadFromSDNAsync(string url, CancellationToken cancellationToken)
		{
			Texture2D texture = await _cdnDownloader.LoadImageAsync(url);
			if (!cancellationToken.IsCancellationRequested)
			{
				ApplyTexture(texture);
			}
		}

		private void ApplyTexture(Texture texture)
		{
			if ((bool)texture)
			{
				Texture = texture;
				ImageActive = true;
			}
			else
			{
				ImageActive = false;
			}
		}
	}
}
