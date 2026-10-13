using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.CDN
{
	public interface ICDNDownloader
	{
		UniTask<Texture2D> LoadImageAsync(string url);

		void ClearLocalCache();

		void ClearLocalCache(string url);

		bool HaveLocalCache(string url);
	}
}
