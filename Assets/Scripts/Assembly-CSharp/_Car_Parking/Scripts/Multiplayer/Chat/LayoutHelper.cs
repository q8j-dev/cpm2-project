using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public static class LayoutHelper
	{
		public static async UniTask ChangeUpdateAsync(this LayoutGroup layoutGroup, CancellationToken ct = default(CancellationToken))
		{
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
			if ((bool)layoutGroup)
			{
				layoutGroup.CalculateLayoutInputHorizontal();
				layoutGroup.CalculateLayoutInputVertical();
				layoutGroup.SetLayoutHorizontal();
				layoutGroup.SetLayoutVertical();
			}
			await UniTask.Yield(ct);
		}
	}
}
