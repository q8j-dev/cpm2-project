using System;
using CPMEngine;
using DG.Tweening;
using Service;
using Service.Manager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PopupNotificationWindow : WindowScript
	{
		public const float AchievementAndRankShowTime = 5f;

		[Header("Texts")]
		[SerializeField]
		private TextMeshProUGUI Title = default;

		[SerializeField]
		private TextMeshProUGUI Content = default;

		[SerializeField]
		private TextMeshProUGUI ImageText = default;

		[Header("Service icons to change")]
		[SerializeField]
		private Sprite TaxiIcon = default;

		[SerializeField]
		private Sprite DeliveryIcon = default;

		[SerializeField]
		private Sprite CargoIcon = default;

		[SerializeField]
		[Header("Movable elements")]
		private RectTransform RectToMove = default;

		[Header("Other")]
		[SerializeField]
		private Button HideButton = default;

		[SerializeField]
		private Image Icon = default;

		[SerializeField]
		private RawImage BckgRawImage = default;

		[SerializeField]
		private RawImage MainRawImage = default;

		private float _waitTime;

		private IDisposable _disposable;

		private readonly float _defaultXPos = 700f;

		private readonly float _moveDuration = 0.5f;

		private TaskManager TaskManager => Global.TaskManager;

		protected override void Start()
		{
			base.Start();
			HideButton.onClick.AddListener(Hide);
			RectToMove.anchoredPosition = new Vector2(_defaultXPos, 0f);
			Show();
		}

		public void InitServiceResultWindow()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhoneNotifySwitch, base.gameObject);
			SetInfo();
			PrepareWindow();
		}

		public void InitAchievementNotification(string name, string description, string title, Texture midTexture, Texture bckgTexture)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.AchivementUnlockedSwitch, base.gameObject);
			_waitTime = 5f;
			PrepareWindow(isFromAchievement: true);
			BckgRawImage.texture = bckgTexture;
			MainRawImage.texture = midTexture;
			Title.text = title;
			Content.text = new LocalizedString(description);
			ImageText.text = new LocalizedString(name);
		}

		public void InitDailyTaskNotification(string name, string description, string title, Texture midTexture, Texture bckgTexture)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.AchivementUnlockedSwitch, base.gameObject);
			_waitTime = 5f;
			PrepareWindow(isFromAchievement: true);
			BckgRawImage.texture = midTexture;
			MainRawImage.gameObject.SetActive(value: false);
			Title.text = title;
			Content.text = new LocalizedString(description);
			ImageText.text = new LocalizedString(name);
		}

		public void InitRankNotification(int newRank, string title, Texture midTexture)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.AchivementUnlockedSwitch, base.gameObject);
			_waitTime = 5f;
			string rankNameByIndex = Utils.GetRankNameByIndex(newRank);
			MainRawImage.texture = midTexture;
			Title.text = title;
			Content.text = string.Concat(new LocalizedString("AchievementNotifcationWindow.YourNewRank"), " ", rankNameByIndex);
			ImageText.text = new LocalizedString(rankNameByIndex);
		}

		private void PrepareWindow(bool isFromAchievement = false)
		{
			Icon.gameObject.SetActive(!isFromAchievement);
			ImageText.gameObject.SetActive(isFromAchievement);
			BckgRawImage.gameObject.SetActive(isFromAchievement);
			MainRawImage.gameObject.SetActive(isFromAchievement);
		}

		private void SetInfo()
		{
			switch (TaskManager.LastTaskType)
			{
			case TYPE.TAXI:
				Icon.sprite = TaxiIcon;
				Title.text = TaskManager.TaxiManager.NotificationTitle;
				Content.text = TaskManager.TaxiManager.NotificationText;
				_waitTime = TaskManager.TaxiManager.NotificationWaitTime;
				break;
			case TYPE.DELIVERY:
				Icon.sprite = DeliveryIcon;
				Title.text = TaskManager.DeliveryManager.NotificationTitle;
				Content.text = TaskManager.DeliveryManager.NotificationText;
				_waitTime = TaskManager.DeliveryManager.NotificationWaitTime;
				break;
			case TYPE.CARGO:
				Icon.sprite = CargoIcon;
				Title.text = TaskManager.CargoManager.NotificationTitle;
				Content.text = TaskManager.CargoManager.NotificationText;
				_waitTime = TaskManager.CargoManager.NotificationWaitTime;
				break;
			case (TYPE)3:
				break;
			}
		}

		private void Show()
		{
			RectToMove.DOAnchorPosX(0f, _moveDuration).SetEase(Ease.InBack).OnComplete(() =>
			{
				_disposable = Observable.Interval(TimeSpan.FromSeconds(Convert.ToDouble(_waitTime))).Subscribe((long _) =>
				{
					Hide();
				}).AddTo(this);
			});
		}

		private void Hide()
		{
			_disposable?.Dispose();
			RectToMove.DOAnchorPosX(_defaultXPos, _moveDuration).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true)
				.OnComplete(() =>
				{
					Close();
				});
		}
	}
}
