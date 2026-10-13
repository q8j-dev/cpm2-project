using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

public class NotificationWindow : WindowScript
{
	[SerializeField]
	private TextMeshProUGUI NotificationText;

	[SerializeField]
	private Image Background;

	[SerializeField]
	private Image Spark;

	[SerializeField]
	private Sprite Success;

	[SerializeField]
	private Sprite Error;

	[SerializeField]
	private float BackgroundShowDuration = 2f;

	[SerializeField]
	private float SparkFadeInDuration = 0.5f;

	[SerializeField]
	private float SparkFadeOutDuration = 0.5f;

	[SerializeField]
	private float SparkScale = 1f;

	[SerializeField]
	private Ease SparkEase = Ease.Linear;

	[SerializeField]
	private Ease BackgroundEase = Ease.Linear;

	[SerializeField]
	private Image BackgroindIcon;

	private float _delay;

	protected override bool ForceHideCursorOnClose => false;

	public void Init(string text, float delay, NotificationType type = NotificationType.Success, uint soundSwitchName = 0u, Sprite icon = null, bool showBackground = true, float sparkFadeInDuration = -1f, float sparkFadeOutDuration = -1f)
	{
		if (sparkFadeInDuration > 0f)
		{
			SparkFadeInDuration = sparkFadeInDuration;
		}
		if (sparkFadeOutDuration > 0f)
		{
			SparkFadeOutDuration = sparkFadeOutDuration;
		}
		NotificationText.text = text;
		Background.sprite = ((type == NotificationType.Success) ? Success : Error);
		Background.gameObject.SetActive(showBackground);
		if ((bool)icon)
		{
			BackgroindIcon.gameObject.SetActive(value: true);
			BackgroindIcon.sprite = icon;
			BackgroindIcon.color = ((type == NotificationType.Success) ? Color.yellow : Color.red);
		}
		DelayedClose(delay).Forget();
		ShowAnimation();
		PlaySound(soundSwitchName);
	}

	private void PlaySound(uint soundSwitchName = 0u)
	{
		uint num = 0u;
		bool num2 = soundSwitchName == 0;
		if (!num2)
		{
			num = soundSwitchName;
		}
		SoundEventsDataHelper.PlayUISound(num2 ? SoundEventsDataHelper.ShortNotifySwitchState : num, base.gameObject);
	}

	private async UniTaskVoid DelayedClose(float delay)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(delay), DelayType.UnscaledDeltaTime);
		if ((bool)NotificationText)
		{
			Close();
		}
	}

	private void ShowAnimation()
	{
		Color color = new Color(1f, 1f, 1f, 0f);
		Background.color = color;
		Spark.color = color;
		Spark.transform.localScale = Vector3.zero;
		NotificationText.color = color;
		Background.transform.localScale = new Vector3(0f, 1f, 1f);
		Spark.DOFade(1f, SparkFadeInDuration).SetUpdate(isIndependentUpdate: true).SetLink(base.gameObject, LinkBehaviour.KillOnDestroy)
			.OnComplete(() =>
			{
				Spark.DOFade(0f, SparkFadeOutDuration).SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
			});
		Spark.transform.DOScale(SparkScale, SparkFadeInDuration).SetUpdate(isIndependentUpdate: true).SetEase(SparkEase)
			.SetLink(base.gameObject, LinkBehaviour.KillOnDestroy)
			.OnComplete(() =>
			{
				Background.transform.DOScaleX(1f, BackgroundShowDuration).SetUpdate(isIndependentUpdate: true).SetEase(BackgroundEase)
					.SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
				Background.DOFade(1f, BackgroundShowDuration).SetUpdate(isIndependentUpdate: true).SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
				NotificationText.DOFade(1f, BackgroundShowDuration).SetUpdate(isIndependentUpdate: true).SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
			});
	}
}
