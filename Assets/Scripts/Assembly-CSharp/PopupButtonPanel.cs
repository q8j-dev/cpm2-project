using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

public class PopupButtonPanel : MonoBehaviour, IPopupWindow
{
	[SerializeField]
	private Button ActionButton = default;

	[SerializeField]
	private float Duration = default;

	[SerializeField]
	private TextMeshProUGUI Text = default;

	private UiNotificationsManager _notificationsManager;

	private bool _isDestroyed;

	private PopupData _popupData;

	private ManagerUI _managerUI;

	[Inject]
	public void Construct(UiNotificationsManager notificationsManager, ManagerUI managerUI)
	{
		_notificationsManager = notificationsManager;
		_managerUI = managerUI;
	}

	public void Init(PopupData popupData)
	{
		_popupData = popupData;
		Text.text = _popupData.Title;
		PlaySound();
		ActionButton.onClick.AddListener(ShowNextWindow);
		DelayedClose().Forget();
	}

	private void PlaySound()
	{
		SoundEventsDataHelper.PlayUISound(_popupData.AudioKey, base.gameObject);
	}

	private void CloseImmediate()
	{
		_notificationsManager.Remove(_popupData);
	}

	private void ShowNextWindow()
	{
		SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.AcceptSwitch, base.gameObject);
		if (_popupData.NextWindowData != null)
		{
			_managerUI.ShowWindow(_popupData.NextWindowData.NextWindowKeyOnClick, alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, _popupData.NextWindowData.NextWindowAdditionalData).Forget();
		}
		CloseImmediate();
	}

	private async UniTaskVoid DelayedClose()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(Duration));
		if (!_isDestroyed)
		{
			_notificationsManager.Remove(_popupData);
		}
	}

	public void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
	}
}
