using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class PhotoModeInfoPanel : MonoBehaviour, IBackableWindow
	{
		private readonly float _animationDuration = 0.1f;

		[SerializeField]
		private ScrollRect Scroll;

		[SerializeField]
		private TextMeshProUGUI Title;

		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private CanvasGroup PanelCanvasGroup;

		[SerializeField]
		private Transform Panel;

		[SerializeField]
		private GameObject MainTutorPanel;

		[SerializeField]
		private GameObject PanningTutorPanel;

		[SerializeField]
		private VideoPlayerUI[] VideoPlayers;

		private List<VideoClip> _videoClips = new List<VideoClip>();

		private PhotoModeCategoryType _categoryType;

		public ManagerUI _managerUI;

		public event Action OnClose;

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			Close();
		}

		private void Start()
		{
			CloseButton.onClick.AddListener(Close);
			Scroll.onValueChanged.AddListener(OnScrollValueChanged);
		}

		public void Open(List<VideoClip> videoClips, PhotoModeCategoryType categoryType)
		{
			Scroll.DOVerticalNormalizedPos(1f, 0.1f).SetUpdate(isIndependentUpdate: true);
			_categoryType = categoryType;
			_videoClips = videoClips;
			base.gameObject.SetActive(value: true);
			Panel.localScale = Vector3.zero;
			PanelCanvasGroup.alpha = 0f;
			if (_categoryType != PhotoModeCategoryType.Panning)
			{
				ShowPanningTutorial(show: false);
				PlayMainTutorial();
				Title.text = new LocalizedString("PhotoMode.CameraControls");
			}
			else
			{
				ShowPanningTutorial(show: true);
				Title.text = new LocalizedString("PhotoMode.PanningModes");
			}
			Animate(isOpening: true);
			InitBackButton();
		}

		private void Close()
		{
			Animate(isOpening: false, () =>
			{
				base.gameObject.SetActive(value: false);
				if (_categoryType != PhotoModeCategoryType.Panning)
				{
					StopMainTutorial();
				}
				OnClose?.Invoke();
				_managerUI.TryRemoveFromCurrentWindows(this);
			});
		}

		private void ShowPanningTutorial(bool show)
		{
			MainTutorPanel.SetActive(!show);
			PanningTutorPanel.SetActive(show);
		}

		private void PlayMainTutorial()
		{
			if (_videoClips == null || _videoClips.Count == 0)
			{
				Debug.Log("VideoClips not found");
				return;
			}
			for (int i = 0; i < _videoClips.Count; i++)
			{
				if ((bool)VideoPlayers[i])
				{
					if ((bool)_videoClips[i])
					{
						VideoPlayers[i].PrepareAndPlay(_videoClips[i], loop: true);
					}
					else
					{
						Debug.Log($"Video clip {i} doesn't exist");
					}
				}
				else
				{
					Debug.Log($"Video player {i} doesn't exist");
				}
			}
		}

		private void StopMainTutorial()
		{
			VideoPlayerUI[] videoPlayers = VideoPlayers;
			for (int i = 0; i < videoPlayers.Length; i++)
			{
				videoPlayers[i].Stop();
			}
		}

		private void OnScrollValueChanged(Vector2 value)
		{
			if (_categoryType != PhotoModeCategoryType.Panning)
			{
				if (value.y > 0.5f)
				{
					SwitchPlayingVideoPlayers(0, 2);
				}
				else
				{
					SwitchPlayingVideoPlayers(2, 0);
				}
			}
		}

		private void SwitchPlayingVideoPlayers(int toResume, int toPause)
		{
			if (!VideoPlayers[toResume].IsPlaying)
			{
				VideoPlayers[toResume].Resume();
			}
			if (VideoPlayers[toPause].IsPlaying)
			{
				VideoPlayers[toPause].PauseAndDisable();
			}
		}

		private void Animate(bool isOpening, UnityAction callback = null)
		{
			Vector3 endValue = (isOpening ? Vector3.one : Vector3.zero);
			float endValue2 = (isOpening ? 1 : 0);
			Panel.DOKill();
			Panel.DOScale(endValue, _animationDuration).SetUpdate(isIndependentUpdate: true);
			PanelCanvasGroup.DOKill();
			PanelCanvasGroup.DOFade(endValue2, _animationDuration).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
			{
				callback?.Invoke();
			});
		}
	}
}
