using System;
using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.TweenAnimations
{
	public abstract class TweenAnimationBase : MonoBehaviour
	{
		[Serializable]
		public class LoopData
		{
			public bool IsWorking;

			public int Count = -1;

			public LoopType Type = LoopType.Yoyo;
		}

		public bool AutoStart = true;

		public bool AutoDisable = true;

		public float Duration;

		public float Delay;

		public Ease Ease = Ease.Linear;

		public bool IsRelative;

		public LoopData Loop;

		public UpdateType UpdateType;

		public bool IsIndependentUpdate;

		public bool StartTimeRandom;

		[Range(-0.01f, 1f)]
		public float StartTime = -0.01f;

		protected Tween tween;

		private bool _wasInitDefault;

		public bool Autokill = true;

		public void StartTween()
		{
			StopTween();
			if (!_wasInitDefault)
			{
				_wasInitDefault = true;
				InitDefaultData();
			}
			ApplyDefault();
			CreateTween();
			if (tween != null)
			{
				tween.SetAutoKill(Autokill);
				tween.SetUpdate(UpdateType, IsIndependentUpdate);
				if (Delay > 0f)
				{
					tween.SetDelay(Delay);
				}
				tween.SetEase(Ease);
				if (Loop.IsWorking)
				{
					tween.SetLoops(Loop.Count, Loop.Type);
				}
				tween.SetRelative(IsRelative);
				if (StartTime > 0f || StartTimeRandom)
				{
					tween.Goto(Mathf.Lerp(0f, Duration, StartTimeRandom ? UnityEngine.Random.value : StartTime), andPlay: true);
				}
			}
		}

		public void StopTween()
		{
			tween?.Kill();
			tween = null;
		}

		public void Pause()
		{
			if (tween != null && tween.IsPlaying())
			{
				tween.Pause();
			}
		}

		public void Continue()
		{
			if (tween != null && !tween.IsPlaying())
			{
				tween.Play();
			}
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
		}

		protected virtual void OnEnable()
		{
			if (AutoStart)
			{
				StartTween();
			}
		}

		protected virtual void OnDisable()
		{
			if (AutoDisable)
			{
				StopTween();
			}
		}

		public virtual void PlayReverse()
		{
			if (tween != null)
			{
				tween.PlayBackwards();
			}
		}

		protected abstract void CreateTween();

		protected abstract void ApplyDefault();

		protected abstract void InitDefaultData();
	}
}
