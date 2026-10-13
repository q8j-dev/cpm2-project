using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace Service
{
	[Serializable]
	public class ServiceLabel
	{
		public Timer mapTimer;

		public Timer textTimer;

		public TextMeshProUGUI onBoard;

		public TextMeshProUGUI money;

		public Button resetTrailer;

		public void UpdateLabels(uint money, int count, int total)
		{
			onBoard.text = $"{count}/{total}";
			this.money.text = $"${money}";
		}

		public async UniTaskVoid Disable()
		{
			mapTimer.Disable().Forget();
			textTimer.Disable().Forget();
			await UniTask.Delay(TimeSpan.FromMilliseconds(1000.0));
			onBoard.transform.parent.gameObject.SetActive(value: false);
			money.transform.parent.gameObject.SetActive(value: false);
		}

		public void Enable(TYPE type)
		{
			mapTimer.Enable();
			textTimer.Enable();
			if (type != TYPE.DELIVERY)
			{
				_ = 4;
				return;
			}
			onBoard.transform.parent.gameObject.SetActive(value: true);
			money.transform.parent.gameObject.SetActive(value: true);
		}
	}
}
