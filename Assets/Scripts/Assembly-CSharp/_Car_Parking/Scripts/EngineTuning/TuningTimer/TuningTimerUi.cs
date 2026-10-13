using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	public class TuningTimerUi : MonoBehaviour
	{
		public TextMeshProUGUI TextMeshProUGUI1;

		public Image ProgressImage;

		public Button SkipByRewardButton;

		private CanvasGroup _canvasGroup;

		private ITimerTuningService _timerTuningService;

		private Tweener _fadeOp;

		private IEngineTuningTimerService _engineTuningTimerService;

		private ManagerUI _managerUI;

		private bool _showingEngineProgress;

		private bool _installTimerRunning;

		private bool _increaseButtonAllowed = true;

		[Inject]
		public void Construct(ITimerTuningService timerTuningService, IEngineTuningTimerService engineTuningTimerService, ManagerUI managerUI)
		{
			_engineTuningTimerService = engineTuningTimerService;
			_timerTuningService = timerTuningService;
			_managerUI = managerUI;
		}

		private void Start()
		{
			_canvasGroup = ProgressImage.GetComponent<CanvasGroup>();
			_timerTuningService.ProgressEvent += SetProgress;
			_timerTuningService.HideEvent += Hide;
			_engineTuningTimerService.SwitchToEngineProgressEvent += SwitchProgressType;
			TextMeshProUGUI1.text = "";
			ProgressImage.fillAmount = 0f;
			SkipByRewardButton.onClick.AddListener(OpenSpeedUpWindow);
			SkipByRewardButton.gameObject.SetActive(value: false);
			ManagerUI managerUI = _managerUI;
			managerUI.IncreaseTimerButtonShow = (Action<bool>)Delegate.Combine(managerUI.IncreaseTimerButtonShow, new Action<bool>(OnIncreaseTimerButtonShow));
		}

		private void OnIncreaseTimerButtonShow(bool show)
		{
			_increaseButtonAllowed = show;
			UpdateSpeedUpButton();
		}

		private void OpenSpeedUpWindow()
		{
			OpenSpeedUpWindowAsync().Forget();
		}

		private async UniTaskVoid OpenSpeedUpWindowAsync()
		{
			SkipByRewardButton.gameObject.SetActive(value: false);
			WindowScript windowScript = await _managerUI.ShowWindow(WindowScriptType.TuningTimerWindow.ToString(), alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, null, withAnimate: false, updateBackButton: false);
			if (!this)
			{
				return;
			}
			if (windowScript == null)
			{
				UpdateSpeedUpButton();
				return;
			}
			windowScript.InitOnCloseCallback(() =>
			{
				if ((bool)this)
				{
					UpdateSpeedUpButton();
				}
			});
		}

		private void UpdateSpeedUpButton()
		{
			SkipByRewardButton.gameObject.SetActive(_installTimerRunning && _increaseButtonAllowed);
		}

		private void SwitchProgressType(bool showEngineProgress)
		{
			_showingEngineProgress = showEngineProgress;
			UnsubscribeAll();
			Hide(isImmediate: true);
			if (showEngineProgress)
			{
				_engineTuningTimerService.ProgressEvent += SetProgress;
				_engineTuningTimerService.HideEvent += Hide;
			}
			else
			{
				_timerTuningService.ProgressEvent += SetProgress;
				_timerTuningService.HideEvent += Hide;
			}
		}

		private void OnDestroy()
		{
			UnsubscribeAll();
			if (_engineTuningTimerService != null)
			{
				_engineTuningTimerService.SwitchToEngineProgressEvent -= SwitchProgressType;
			}
			if ((bool)_managerUI)
			{
				ManagerUI managerUI = _managerUI;
				managerUI.IncreaseTimerButtonShow = (Action<bool>)Delegate.Remove(managerUI.IncreaseTimerButtonShow, new Action<bool>(OnIncreaseTimerButtonShow));
			}
		}

		private void UnsubscribeAll()
		{
			_timerTuningService.ProgressEvent -= SetProgress;
			_timerTuningService.HideEvent -= Hide;
			_engineTuningTimerService.ProgressEvent -= SetProgress;
			_engineTuningTimerService.HideEvent -= Hide;
		}

		private void Hide(bool isImmediate)
		{
			_installTimerRunning = false;
			UpdateSpeedUpButton();
			_fadeOp?.Kill();
			if (isImmediate)
			{
				_canvasGroup.alpha = 0f;
			}
			else
			{
				_fadeOp = _canvasGroup.DOFade(0f, 1f).SetDelay(1f);
			}
		}

		public void SetProgress(TimerEnums timerEnums, float value)
		{
			bool flag = value < 1f;
			if (flag != _installTimerRunning)
			{
				_installTimerRunning = flag;
				UpdateSpeedUpButton();
			}
			ProgressImage.fillAmount = value;
			string newValue = ((timerEnums == TimerEnums.Engine) ? new LocalizedString("TuningTimerUi.Engine") : new LocalizedString("ServicePoint.Car"));
			string text = new LocalizedString("TuningTimerUi.EntitytextInService");
			TextMeshProUGUI1.text = text.Replace("{0}", newValue) + $"{value * 100f:F0}%";
			if (value < 1f)
			{
				_fadeOp?.Kill();
				_canvasGroup.alpha = 1f;
			}
		}
	}
}
