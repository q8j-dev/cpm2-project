using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageWithTextElement : MonoBehaviour
{
	[SerializeField]
	private Image Icon;

	[SerializeField]
	private TextMeshProUGUI NameText;

	[SerializeField]
	private TweenPulseAnimator TweenPulseAnimator;

	private bool _isDestroyed;

	public void ChangeState(Color newColor, string text, float duration)
	{
		Icon.color = newColor;
		NameText.color = newColor;
		NameText.text = text;
		if (duration != -1f)
		{
			DelayedClose(duration).Forget();
		}
		base.gameObject.SetActive(value: true);
	}

	private async UniTaskVoid DelayedClose(float duration)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(duration));
		if (!_isDestroyed)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
	}
}
