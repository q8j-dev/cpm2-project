using System;
using System.Threading;
using AK.Wwise;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.UI
{
	public class CountdownUI : MonoBehaviour
	{
		[SerializeField]
		[Header("References")]
		private TMP_Text Label;

		[Tooltip("Optional root toggled on/off around the countdown. Defaults to this GameObject.")]
		[SerializeField]
		private GameObject Root;

		[Header("Sequence")]
		[Tooltip("First number shown. 3 → 2 → 1.")]
		[SerializeField]
		private int StartFrom = 3;

		[SerializeField]
		[Tooltip("Text shown after the last number. Leave empty to skip.")]
		private string FinalText = "GO!";

		[SerializeField]
		[Tooltip("Time for a number to pop in (scale/fade up).")]
		[Header("Timing (seconds)")]
		private float AppearDuration = 0.25f;

		[Tooltip("Time a number stays fully visible before fading out.")]
		[SerializeField]
		private float HoldDuration = 0.45f;

		[Tooltip("Time for a number to fade/scale out.")]
		[SerializeField]
		private float DisappearDuration = 0.3f;

		[Tooltip("Extra pause between steps. Set 0 for a continuous rhythm.")]
		[SerializeField]
		private float GapDuration;

		[Tooltip("Cap on the animated countdown. If the requested duration exceeds it, the excess is spent waiting up front, then the countdown runs for MaxDuration. Negative → no cap.")]
		[SerializeField]
		private float MaxDuration = -1f;

		[Header("Sound")]
		[Tooltip("Posted once per step - on 3, on 2, on 1 and again on GO! - so the event itself decides what each beat sounds like. Left empty the countdown is silent: this component is shared, so only the modes that want the beeps assign it.")]
		[SerializeField]
		private AK.Wwise.Event StartSound;

		[SerializeField]
		[Header("Animation")]
		private float StartScale = 2.2f;

		[SerializeField]
		private float EndScale = 1f;

		[SerializeField]
		private float ExitScale = 0.6f;

		[Tooltip("Eval 0→1 drives the appear interpolation.")]
		[SerializeField]
		private AnimationCurve AppearCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField]
		[Tooltip("Eval 0→1 drives the disappear interpolation.")]
		private AnimationCurve DisappearCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		private const int StopFadeMs = 150;

		private const uint InvalidPlayingId = 0u;

		private RectTransform _labelRect;

		private uint _stepPlayingId;

		private void Awake()
		{
			if (!Root)
			{
				Root = base.gameObject;
			}
			if ((bool)Label)
			{
				_labelRect = Label.rectTransform;
			}
		}

		public async UniTask PlayAsync(float duration = -1f, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!Label)
			{
				Debug.LogError("CountdownUI: Label is not assigned.", this);
				return;
			}
			using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.GetCancellationTokenOnDestroy());
			CancellationToken token = linked.Token;
			float num = 0f;
			float num2 = duration;
			if (duration >= 0f && MaxDuration >= 0f && duration > MaxDuration)
			{
				num = duration - MaxDuration;
				num2 = MaxDuration;
			}
			float timeScale = 1f;
			if (num2 >= 0f)
			{
				float num3 = IntrinsicDuration();
				timeScale = ((num3 > 0f) ? (num2 / num3) : 0f);
			}
			_stepPlayingId = 0u;
			try
			{
				Root.SetActive(value: true);
				if (num > 0f)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(num), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
				}
				for (int number = StartFrom; number >= 1; number--)
				{
					await ShowStepAsync(number.ToString(), timeScale, token);
				}
				if (!string.IsNullOrEmpty(FinalText))
				{
					await ShowStepAsync(FinalText, timeScale, token);
				}
			}
			finally
			{
				if ((bool)Root)
				{
					Root.SetActive(value: false);
				}
				if (_stepPlayingId != 0 && token.IsCancellationRequested)
				{
					AudioWrapper.StopPlayingId(_stepPlayingId, 150);
				}
				_stepPlayingId = 0u;
			}
		}

		private uint PostStartSound()
		{
			if (StartSound == null || !StartSound.IsValid())
			{
				return 0u;
			}
			return AudioWrapper.PostEvent(StartSound.Id, base.gameObject);
		}

		private float IntrinsicDuration()
		{
			return (float)(Mathf.Max(0, StartFrom) + ((!string.IsNullOrEmpty(FinalText)) ? 1 : 0)) * (AppearDuration + HoldDuration + DisappearDuration + GapDuration);
		}

		private async UniTask ShowStepAsync(string text, float timeScale, CancellationToken token)
		{
			Label.text = text;
			_stepPlayingId = PostStartSound();
			await AnimateAsync(AppearDuration * timeScale, AppearCurve, StartScale, EndScale, 0f, 1f, token);
			float num = HoldDuration * timeScale;
			if (num > 0f)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(num), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
			await AnimateAsync(DisappearDuration * timeScale, DisappearCurve, EndScale, ExitScale, 1f, 0f, token);
			float num2 = GapDuration * timeScale;
			if (num2 > 0f)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(num2), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
		}

		private async UniTask AnimateAsync(float duration, AnimationCurve curve, float fromScale, float toScale, float fromAlpha, float toAlpha, CancellationToken token)
		{
			if (duration <= 0f)
			{
				Apply(toScale, toAlpha);
				return;
			}
			float elapsed = 0f;
			while (elapsed < duration)
			{
				token.ThrowIfCancellationRequested();
				float t = curve.Evaluate(elapsed / duration);
				Apply(Mathf.LerpUnclamped(fromScale, toScale, t), Mathf.Lerp(fromAlpha, toAlpha, t));
				elapsed += Time.deltaTime;
				await UniTask.Yield(PlayerLoopTiming.Update, token);
			}
			Apply(toScale, toAlpha);
		}

		private void Apply(float scale, float alpha)
		{
			if ((bool)_labelRect)
			{
				_labelRect.localScale = Vector3.one * scale;
			}
			Label.alpha = alpha;
		}
	}
}
