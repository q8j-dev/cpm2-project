using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Service
{
	public class Timer : MonoBehaviour
	{
		[SerializeField]
		private TIMER_TYPE type;

		[SerializeField]
		private TextMeshProUGUI text;

		[SerializeField]
		private Image sprite;

		[SerializeField]
		private Gradient gradient;

		private void Start()
		{
			base.gameObject.SetActive(value: false);
		}

		public void ChangeValue(float time, float ratio)
		{
			if (!(time < 0f))
			{
				switch (type)
				{
				case TIMER_TYPE.TEXT:
				{
					TimeSpan timeSpan = TimeSpan.FromSeconds(time);
					text.text = timeSpan.ToString("hh\\:mm\\:ss");
					break;
				}
				case TIMER_TYPE.CIRCLE:
					sprite.color = gradient.Evaluate(1f - ratio);
					sprite.fillAmount = ratio;
					break;
				}
			}
		}

		public async UniTaskVoid Disable()
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(1000.0));
			base.gameObject.SetActive(value: false);
		}

		public void Enable()
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
		}
	}
}
