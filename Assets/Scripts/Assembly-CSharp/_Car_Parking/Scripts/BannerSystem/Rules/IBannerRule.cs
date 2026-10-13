using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.BannerSystem.Rules
{
	public interface IBannerRule
	{
		UniTask<bool> CanShow();
	}
}
