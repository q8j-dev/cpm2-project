using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.NewUI
{
	public abstract class PhoneUISubPanel : MonoBehaviour, IBackableWindow
	{
		[SerializeField]
		private Button BackButton = default;

		[SerializeField]
		protected CanvasGroup CanvasGroup;

		[SerializeField]
		private bool IgnoreBackButton = default;

		public Action BackButtonPressed;

		private Tween _tween;

		protected ManagerUI ManagerUI;

		[Inject]
		public void ConstructBase(ManagerUI managerUI)
		{
			ManagerUI = managerUI;
		}

		private void OnEnable()
		{
			OnEnableEvent();
		}

		public virtual void OnEnableEvent()
		{
			if (!IgnoreBackButton)
			{
				BackButton.SetActive(active: true);
				BackButton.onClick.RemoveAllListeners();
				BackButton.onClick.AddListener(Close);
				InitBackButton();
			}
		}

		public void InitBackButton()
		{
			ManagerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			Close();
		}

		public virtual void Show()
		{
			base.gameObject.SetActive(value: true);
			_tween?.Kill();
			_tween = CanvasGroup.DOFade(1f, 0.5f);
		}

		public virtual void HideWithoutAnimation()
		{
			_tween?.Kill();
			CanvasGroup.alpha = 0f;
			base.gameObject.SetActive(value: false);
		}

		public virtual void Close()
		{
			if (!FlagsStorage.IsMultiFree)
			{
				Time.timeScale = 0f;
			}
			_tween?.Kill();
			_tween = CanvasGroup.DOFade(0f, 0.5f).SetUpdate(isIndependentUpdate: true);
			BackButtonPressed?.Invoke();
			BackButton.SetActive(active: false);
			base.gameObject.SetActive(value: false);
		}

		public virtual void OnDestroyAction()
		{
			_tween?.Kill();
		}

		private void OnDestroy()
		{
			OnDestroyAction();
		}
	}
}
