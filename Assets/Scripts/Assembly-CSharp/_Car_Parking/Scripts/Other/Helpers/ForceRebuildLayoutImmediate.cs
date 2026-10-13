using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class ForceRebuildLayoutImmediate : MonoBehaviour
	{
		[SerializeField]
		private RectTransform Root;

		private void OnEnable()
		{
			RebuildLayoutAsync().Forget();
		}

		public void RebuildLayout()
		{
			RebuildLayoutAsync().Forget();
		}

		private async UniTaskVoid RebuildLayoutAsync()
		{
			if ((bool)Root)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(Root);
			}
			await UniTask.Yield();
			if ((bool)Root)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(Root);
			}
		}
	}
}
