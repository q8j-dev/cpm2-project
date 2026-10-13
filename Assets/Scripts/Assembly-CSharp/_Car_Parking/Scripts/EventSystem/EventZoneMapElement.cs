using System;
using CPMEngine;
using MVPFree.MapIcons.Models;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.EventSystem
{
	public class EventZoneMapElement : MonoBehaviour, IEventGameObjectInit
	{
		private const float MaxCooldownTime = 120f;

		public ZoneState ZoneState;

		public SceneType SceneType;

		public int Index;

		[SerializeField]
		private SpriteRenderer MapZoneSpriteRenderer = default;

		[SerializeField]
		private SpriteRenderer PasSpriteRenderer = default;

		[SerializeField]
		private TMP_Text TimeText = default;

		private TimedActivityZoneController _timedActivityZoneController;

		public Action OnCooldownEnd;

		private float _deactivateTime;

		private IDisposable _cooldownTimer;

		private EventType _eventType;

		[SerializeField]
		private Color PasCooldownTimerColor = Color.green;

		[SerializeField]
		private Vector3 PasCooldownTimerPos = default;

		[SerializeField]
		private Color NonPasCooldownTimerColor = Color.red;

		[SerializeField]
		private Vector3 NonPasCooldownTimerPos = default;

		private MapIconsModel _mapIconsModel;

		private FreeDriveProvider _freeDriveProvider;

		[Inject]
		public void Construct(TimedActivityZoneController timedActivityZoneController, MapIconsModel mapIconsModel, FreeDriveProvider freeDriveProvider)
		{
			_timedActivityZoneController = timedActivityZoneController;
			_mapIconsModel = mapIconsModel;
			_freeDriveProvider = freeDriveProvider;
		}

		private void OnEnable()
		{
			MapZoneSpriteRenderer.sortingLayerID = 0;
			ChangeState(ZoneState.None);
		}

		public void ChangeStatusIcon(bool state)
		{
			PasSpriteRenderer.gameObject.SetActive(state);
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				_mapIconsModel.EventsIcons[_eventType][Index].TimeIcon.SetActive(!state);
			}).AddTo(this);
		}

		private void UpdateMapZoneVisible()
		{
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				SwitchMapZoneVisible(_mapIconsModel.IconStatusModel.GetIconVisibleStatus(_mapIconsModel.EventsIcons[_eventType][Index].IconType));
			}).AddTo(this);
		}

		public void ChangeState(ZoneState newState)
		{
			ZoneState = newState;
			switch (ZoneState)
			{
			case ZoneState.None:
				MapZoneSpriteRenderer.color = Color.cyan;
				UpdateMapZoneVisible();
				break;
			case ZoneState.Prepared:
				UpdateMapZoneVisible();
				MapZoneSpriteRenderer.color = Color.cyan;
				break;
			case ZoneState.Started:
				MapZoneSpriteRenderer.color = Color.cyan;
				SwitchMapZoneVisible(isOn: true);
				_freeDriveProvider.MapCamera.zoomIcon(300f);
				break;
			case ZoneState.Pause:
				MapZoneSpriteRenderer.color = Color.red;
				break;
			case ZoneState.Cooldown:
			{
				(int, int) currentProgress = _timedActivityZoneController.GetCurrentProgress();
				bool flag = _timedActivityZoneController.CheckZoneProgress(Index) || currentProgress.Item1 == currentProgress.Item2;
				_deactivateTime = Time.time;
				MapZoneSpriteRenderer.color = (flag ? Color.green : Color.white);
				TimeText.rectTransform.localPosition = (flag ? PasCooldownTimerPos : NonPasCooldownTimerPos);
				TimeText.color = (flag ? PasCooldownTimerColor : NonPasCooldownTimerColor);
				TimeText.gameObject.SetActive(value: true);
				PasSpriteRenderer.gameObject.SetActive(flag);
				_mapIconsModel.EventsIcons[_eventType][Index].TimeIcon.SetActive(!flag);
				_cooldownTimer?.Dispose();
				_cooldownTimer = Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					CheckCooldown();
				});
				break;
			}
			}
			TimeText.gameObject.SetActive(ZoneState == ZoneState.Cooldown);
		}

		private void Start()
		{
			MapZoneSpriteRenderer.sortingLayerID = 0;
			_timedActivityZoneController.RegisterZoneOnMap(this).Forget();
			ChangeStatusIcon(_timedActivityZoneController.CheckZoneProgress(Index));
		}

		public void Init(EventType eventID)
		{
			_eventType = eventID;
			base.gameObject.SetActive(value: true);
		}

		private void CheckCooldown()
		{
			if (ZoneState != ZoneState.Cooldown)
			{
				return;
			}
			if (Time.time - _deactivateTime > 120f)
			{
				ChangeState(ZoneState.None);
				OnCooldownEnd?.Invoke();
				return;
			}
			float num = 120f - (Time.time - _deactivateTime);
			if (num >= 0f)
			{
				TimeText.text = Utils.ConvertToTime(num);
			}
		}

		public void SwitchMapZoneVisible(bool isOn)
		{
			if (!isOn)
			{
				ZoneState zoneState = ZoneState;
				if (zoneState == ZoneState.Started || zoneState == ZoneState.Cooldown)
				{
					return;
				}
			}
			_mapIconsModel.EventsIcons[_eventType][Index].UpdateRootVisible(isOn);
			MapZoneSpriteRenderer.gameObject.SetActive(isOn);
		}

		private void OnDestroy()
		{
			_cooldownTimer?.Dispose();
			_timedActivityZoneController.OnDestroy();
		}
	}
}
