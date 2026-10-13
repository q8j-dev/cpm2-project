using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.StepProgress
{
	public class SpriteSwapProgressCellView : ProgressCellView
	{
		[SerializeField]
		private Image Icon = default;

		[Tooltip("Connector to the previous cell; the first cell of a row leaves it empty.")]
		[SerializeField]
		private Image Line = default;

		[SerializeField]
		private Sprite FilledSprite = default;

		[SerializeField]
		private Sprite EmptySprite = default;

		[SerializeField]
		private Color FilledLineColor = Color.white;

		[SerializeField]
		private Color EmptyLineColor = Color.gray;

		[Tooltip("Peak of the punch as a fraction of the icon's own scale.")]
		[SerializeField]
		private float PunchStrength = 0.25f;

		[SerializeField]
		[Tooltip("Length of the punch, in seconds.")]
		private float PunchDuration = 0.5f;

		[SerializeField]
		[Tooltip("Punch shape over its duration: 0 is the resting scale, 1 is the peak.")]
		private AnimationCurve PunchCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.35f, 1f), new Keyframe(1f, 0f));

		[SerializeField]
		[Tooltip("Ignore Time.timeScale so the punch keeps playing while the game is paused.")]
		private bool UnscaledTime = true;

		private Vector3 _baseScale = Vector3.one;

		private void Awake()
		{
			if ((bool)Icon)
			{
				_baseScale = Icon.transform.localScale;
			}
		}

		public override void SetFilled(bool filled)
		{
			ApplyState(filled);
			if ((bool)Icon)
			{
				Icon.transform.localScale = _baseScale;
			}
		}

		public override async UniTask PlayFillAsync(CancellationToken token)
		{
			ApplyState(filled: true);
			if (!Icon || PunchDuration <= 0f)
			{
				return;
			}
			using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(), token);
			await PunchAsync(Icon.transform, cts.Token);
		}

		private void ApplyState(bool filled)
		{
			if ((bool)Icon)
			{
				Icon.sprite = (filled ? FilledSprite : EmptySprite);
			}
			if ((bool)Line)
			{
				Line.color = (filled ? FilledLineColor : EmptyLineColor);
			}
		}

		private async UniTask PunchAsync(Transform target, CancellationToken token)
		{
			float elapsed = 0f;
			try
			{
				while (elapsed < PunchDuration)
				{
					await UniTask.Yield(PlayerLoopTiming.Update, token);
					elapsed += (UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
					float time = Mathf.Clamp01(elapsed / PunchDuration);
					target.localScale = _baseScale * (1f + PunchStrength * PunchCurve.Evaluate(time));
				}
			}
			finally
			{
				if ((bool)target)
				{
					target.localScale = _baseScale;
				}
			}
		}
	}
}
