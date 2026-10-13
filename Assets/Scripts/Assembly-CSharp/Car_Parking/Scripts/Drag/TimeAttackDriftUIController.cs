using System;
using Car_Parking.Scripts.Racing;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace Car_Parking.Scripts.Drag
{
	public class TimeAttackDriftUIController : MonoBehaviour
	{
		[SerializeField]
		private GameObject DriftPanel;

		[SerializeField]
		private GameObject DriftTotalScorePanel;

		[SerializeField]
		private GameObject TimePanel;

		[SerializeField]
		private TextMeshProUGUI CurrentComboPointsText;

		[SerializeField]
		private TextMeshProUGUI CurrentPointsText;

		[SerializeField]
		private TextMeshProUGUI TotalPointsText;

		[SerializeField]
		private TextMeshProUGUI BestPointsText;

		[SerializeField]
		private Slider ProgressBar;

		[SerializeField]
		private TextMeshProUGUI DriftComboTween;

		[SerializeField]
		private CanvasGroup DriftTextTweenCanvasGroup;

		[SerializeField]
		private Image DriftPanelBckg;

		[SerializeField]
		private Color DefaultColor;

		[SerializeField]
		private Color StopColor;

		[SerializeField]
		private Color EarnPointsColor;

		[SerializeField]
		private DriftUIPanel DriftUIPanel;

		private IDisposable _driftEveryFrameTick;

		private Tween _tween;

		private Tween _comboDriftTween;

		private Tween _changeColorTween;

		private Tween _driftComboAddedTween;

		private DriftController _driftController;

		private FreeDriveProvider _freeDriveProvider;

		private float m_driftFailedTimer;

		private FreeTimeAttackController _timeAttackController;

		private int _lastComboCount;

		private CanvasGroup _driftSignCanvasGroup;

		private MenuLevelParent _menuLevelParent;

		private IDisposable _driftPanelStateChecker;

		[UnityEngine.Scripting.Preserve]
		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, FreeTimeAttackController timeAttackController, MenuLevelParent menuLevelParent)
		{
			_freeDriveProvider = freeDriveProvider;
			_timeAttackController = timeAttackController;
			_menuLevelParent = menuLevelParent;
		}

		private void Start()
		{
			ProgressBar.maxValue = 1000f;
			_driftController = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<DriftController>();
			if ((bool)_driftController)
			{
				_driftSignCanvasGroup = _menuLevelParent.driftSign.GetComponent<CanvasGroup>();
				DriftSignIconShow(show: false);
				DriftController driftController = _driftController;
				driftController.OnComboAdded = (Action<float>)Delegate.Combine(driftController.OnComboAdded, new Action<float>(OnComboAdded));
				TimePanel.SetActive(value: false);
				DriftController driftController2 = _driftController;
				driftController2.EarnedPointsEvent = (Action<bool>)Delegate.Combine(driftController2.EarnedPointsEvent, new Action<bool>(UpdateTotalPoints));
				_driftEveryFrameTick = Observable.EveryUpdate().Subscribe((long _) =>
				{
					UpdateTick();
				}).AddTo(this);
				DriftController driftController3 = _driftController;
				driftController3.DriftFailedEvent = (Action)Delegate.Combine(driftController3.DriftFailedEvent, new Action(StopProgress));
				DriftUIPanel.RegisterDriftController(_driftController);
				_driftPanelStateChecker = Observable.Interval(TimeSpan.FromSeconds(0.30000001192092896)).Subscribe((long l) =>
				{
					CheckDriftPanelState();
				}).AddTo(this);
			}
			else
			{
				DriftTotalScorePanel.SetActive(value: false);
			}
		}

		private void OnComboAdded(float driftValue)
		{
			DriftTextTweenCanvasGroup.alpha = 1f;
			_ = (string)new LocalizedString("TimeAttack.DriftComboForChange");
			DriftComboTween.text = $"{DriftComboTween.text} \n + <color=yellow>{driftValue} DP</color=yellow> Change";
			_comboDriftTween?.Kill();
			_comboDriftTween = DriftTextTweenCanvasGroup.DOFade(0f, 1f).SetDelay(1f).OnComplete(() =>
			{
				DriftComboTween.text = "";
			});
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.UIEventDriftBonus, base.gameObject);
		}

		private void OnDisable()
		{
			if ((bool)_driftController)
			{
				_driftEveryFrameTick?.Dispose();
				DriftController driftController = _driftController;
				driftController.DriftFailedEvent = (Action)Delegate.Remove(driftController.DriftFailedEvent, new Action(StopProgress));
				DriftController driftController2 = _driftController;
				driftController2.EarnedPointsEvent = (Action<bool>)Delegate.Remove(driftController2.EarnedPointsEvent, new Action<bool>(UpdateTotalPoints));
				DriftController driftController3 = _driftController;
				driftController3.OnComboAdded = (Action<float>)Delegate.Remove(driftController3.OnComboAdded, new Action<float>(OnComboAdded));
			}
			DriftSignIconShow(show: true);
		}

		public void UpdateTick()
		{
			if (m_driftFailedTimer > 0f)
			{
				m_driftFailedTimer -= Time.deltaTime;
			}
			if (_driftController.IsDrift || _driftController.DriftTime > 0f)
			{
				float comboReadyTimeLimit = _driftController.comboReadyTimeLimit;
				float driftTimeSide = _driftController.DriftTimeSide;
				if (_driftController.DriftTime > 1f && driftTimeSide < comboReadyTimeLimit && _driftController.CurrentState == DriftController.DriftState.Drift)
				{
					EnableDriftPanel();
					UpdateProgress();
				}
			}
			int num = (int)_driftController.CurrentComboCount;
			if (num > _lastComboCount && _lastComboCount > 0)
			{
				Pulse(CurrentComboPointsText.rectTransform);
			}
			_lastComboCount = num;
			CurrentComboPointsText.text = $"x{num}";
			CurrentPointsText.text = $"{(int)_driftController.Points} DP";
		}

		private void CheckDriftPanelState()
		{
			DriftPanel.SetActive(_driftController.Points > 0f);
		}

		private void UpdateProgress()
		{
			float value = _driftController.Points % 1000f;
			ProgressBar.value = value;
		}

		private void Pulse(RectTransform rectTransform)
		{
			_driftComboAddedTween?.Kill();
			_driftComboAddedTween = rectTransform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f);
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.UIEventDriftCount, base.gameObject);
		}

		private void StopProgress()
		{
			_lastComboCount = 0;
			_changeColorTween?.Kill();
			_changeColorTween = DriftPanelBckg.DOColor(StopColor, 0.1f).OnComplete(() =>
			{
				if (DriftPanel.activeSelf)
				{
					SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.UIEventDriftEnd, base.gameObject);
				}
				DriftPanel.gameObject.SetActive(value: false);
			});
		}

		private void EnableDriftPanel()
		{
			if (!DriftPanel.activeSelf)
			{
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ShortNotification, base.gameObject);
			}
			DriftPanelBckg.color = DefaultColor;
			DriftPanel.gameObject.SetActive(value: true);
		}

		private void DriftSignIconShow(bool show)
		{
			if ((bool)_driftSignCanvasGroup)
			{
				_driftSignCanvasGroup.alpha = (show ? 1 : 0);
			}
		}

		private void UpdateTotalPoints(bool resetCombo)
		{
			if (!resetCombo)
			{
				_changeColorTween?.Kill();
				_changeColorTween = DriftPanelBckg.DOColor(EarnPointsColor, 0.1f);
			}
			TotalPointsText.text = $"{(int)_driftController.TotalDriftPoints} DP";
		}

		private void OnDestroy()
		{
			_driftPanelStateChecker?.Dispose();
		}
	}
}
