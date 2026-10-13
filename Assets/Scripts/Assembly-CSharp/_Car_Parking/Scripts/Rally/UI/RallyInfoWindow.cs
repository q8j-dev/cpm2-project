using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.FirstGameSetup;

namespace _Car_Parking.Scripts.Rally.UI
{
	public class RallyInfoWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private ButtonEnableDisableColor PrevButton;

		[SerializeField]
		private ButtonEnableDisableColor NextButton;

		[SerializeField]
		private RectTransform Content;

		[SerializeField]
		private RectTransform Viewport;

		[SerializeField]
		private RectTransform[] Pages;

		[SerializeField]
		private Ease EaseType = Ease.OutCubic;

		[SerializeField]
		private float Duration = 0.3f;

		private int _currentPage;

		private Tween _slideTween;

		protected override void Start()
		{
			base.Start();
			CloseButton.onClick.AddListener(Close);
			PrevButton.Button.onClick.AddListener(PrevPage);
			NextButton.Button.onClick.AddListener(NextPage);
			_currentPage = 0;
			AnimateToPage(instant: true);
			UpdateButtons();
			_managerUI.OnRallyPopupShowed?.Invoke(obj: true);
			InitBackButton();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			Close();
		}

		protected override void OnDestroy()
		{
			_slideTween?.Kill();
			_managerUI.OnRallyPopupShowed?.Invoke(obj: false);
			if ((bool)CloseButton)
			{
				CloseButton.onClick.RemoveListener(Close);
			}
			if ((bool)PrevButton && (bool)PrevButton.Button)
			{
				PrevButton.Button.onClick.RemoveListener(PrevPage);
			}
			if ((bool)NextButton && (bool)NextButton.Button)
			{
				NextButton.Button.onClick.RemoveListener(NextPage);
			}
			base.OnDestroy();
		}

		private void NextPage()
		{
			if (_currentPage < Pages.Length - 1)
			{
				_currentPage++;
				AnimateToPage(instant: false);
				UpdateButtons();
			}
		}

		private void PrevPage()
		{
			if (_currentPage > 0)
			{
				_currentPage--;
				AnimateToPage(instant: false);
				UpdateButtons();
			}
		}

		private void AnimateToPage(bool instant)
		{
			if (!Content || !Viewport)
			{
				Debug.LogError("RallyInfoWindow: Content or Viewport is not assigned.");
				return;
			}
			float num = (float)(-_currentPage) * Viewport.rect.width;
			_slideTween?.Kill();
			if (instant)
			{
				Content.anchoredPosition = new Vector2(num, Content.anchoredPosition.y);
			}
			else
			{
				_slideTween = Content.DOAnchorPosX(num, Duration).SetEase(EaseType).SetUpdate(isIndependentUpdate: true);
			}
		}

		private void UpdateButtons()
		{
			if (_currentPage <= 0)
			{
				PrevButton.Disable();
			}
			else
			{
				PrevButton.Enable();
			}
			if (_currentPage >= Pages.Length - 1)
			{
				NextButton.Disable();
			}
			else
			{
				NextButton.Enable();
			}
		}
	}
}
