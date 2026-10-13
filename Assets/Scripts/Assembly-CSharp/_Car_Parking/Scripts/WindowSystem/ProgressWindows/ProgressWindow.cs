using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem.ProgressWindows
{
	public class ProgressWindow : WindowScript
	{
		[SerializeField]
		private Slider ProgressSlider;

		[SerializeField]
		private Transform WheelImage;

		[SerializeField]
		private TextMeshProUGUI ProgressNameText;

		[SerializeField]
		private TextMeshProUGUI ProgressValueText;

		[SerializeField]
		private TextMeshProUGUI ProgressStatusText;

		[SerializeField]
		private float Duration = 1.5f;

		private IDisposable _disposable;

		private ProgressContainer _progressContainer;

		private Tweener _tweener;

		private float _lastProgress;

		private const float ProgressLerpValue = 2f;

		[Inject]
		public void Construct(ProgressContainer progressContainer)
		{
			_progressContainer = progressContainer;
		}

		protected override void Awake()
		{
			base.Awake();
			Reset();
		}

		public void Init(string name)
		{
			ProgressNameText.text = name;
			_disposable?.Dispose();
			_disposable = Observable.EveryUpdate().Subscribe((long _) =>
			{
				Tick();
			});
			_progressContainer.Reset();
			RotateWheel();
		}

		private void Reset()
		{
			ProgressSlider.value = 0f;
			ProgressNameText.text = "";
			ProgressValueText.text = "";
			ProgressStatusText.text = "";
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_disposable?.Dispose();
		}

		private void SetProgress(float progress, string status)
		{
			progress += 0.01f;
			float value = Mathf.Lerp(_lastProgress, progress, Time.deltaTime * 2f);
			value = (_lastProgress = Mathf.Clamp01(value));
			ProgressSlider.value = value;
			ProgressValueText.text = $"{(int)(value * 100f)}%";
			ProgressStatusText.text = status;
		}

		private void Tick()
		{
			SetProgress(_progressContainer.Value, _progressContainer.Status);
			if (_progressContainer.IsEnded)
			{
				Stop();
			}
		}

		private void Stop()
		{
			_disposable?.Dispose();
			_tweener?.Kill();
			Close();
		}

		private void RotateWheel()
		{
			_tweener?.Kill();
			_tweener = WheelImage.DORotate(Vector3.back * 360f, Duration, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear)
				.SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
		}

		public static async UniTask TestProgress(ManagerUI managerUI, ProgressContainer progressContainer)
		{
		}
	}
}
