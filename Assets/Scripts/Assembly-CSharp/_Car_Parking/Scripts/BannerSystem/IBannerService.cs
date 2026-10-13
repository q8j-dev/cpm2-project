using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.BannerSystem
{
	public interface IBannerService
	{
		UniTask<bool> ProvideShowBanner(BannerType typeForShow, Action showCallback, CancellationToken cancellationToken);
	}
}
