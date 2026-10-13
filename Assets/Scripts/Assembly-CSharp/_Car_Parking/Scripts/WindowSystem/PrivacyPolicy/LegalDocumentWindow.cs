using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem.PrivacyPolicy
{
	public class LegalDocumentWindow : WindowScript, IBackableWindow
	{
		public const string PrefabKey = "LegalDocumentWindow";

		[SerializeField]
		private TextMeshProUGUI Title = default;

		[SerializeField]
		private TextMeshProUGUI ContentText = default;

		[SerializeField]
		private ScrollRect ContentScroll = default;

		[SerializeField]
		private Button OpenLinkButton = default;

		[SerializeField]
		private Button AcceptButton = default;

		[SerializeField]
		private TextMeshProUGUI OpenLinkButtonText = default;

		[SerializeField]
		private TextMeshProUGUI AcceptButtonText = default;

		[SerializeField]
		private float ScrollToBottomDuration = 0.3f;

		private const float BottomThreshold = 0.01f;

		private string _url;

		private bool _reachedBottom;

		private bool _isScrolling;

		private Tweener _scrollTween;

		protected override void Start()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ConfirmationWindowSwitch, base.gameObject);
			OpenLinkButton.onClick.AddListener(OpenLink);
			AcceptButton.onClick.AddListener(Accept);
			if ((bool)ContentScroll)
			{
				ContentScroll.onValueChanged.AddListener(OnScrollChanged);
			}
			if ((bool)OpenLinkButtonText)
			{
				OpenLinkButtonText.SetText("Open Link");
			}
			if ((bool)AcceptButtonText)
			{
				AcceptButtonText.SetText("Accept");
			}
			_managerUI.OnShowScreen?.Invoke(obj: false);
			base.Start();
			InitBackButton();
		}

		public void Back()
		{
			Accept();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		protected override void OnDestroy()
		{
			OpenLinkButton.onClick.RemoveAllListeners();
			AcceptButton.onClick.RemoveAllListeners();
			if ((bool)ContentScroll)
			{
				ContentScroll.onValueChanged.RemoveListener(OnScrollChanged);
			}
			_scrollTween?.Kill();
			_managerUI.OnShowScreen?.Invoke(obj: true);
			base.OnDestroy();
		}

		public void Init(string title, string content, string url)
		{
			_url = url;
			Title.SetText(title);
			ContentText.SetText(content);
			_reachedBottom = false;
			if ((bool)ContentScroll)
			{
				ContentScroll.verticalNormalizedPosition = 1f;
			}
		}

		private void OpenLink()
		{
			if (!string.IsNullOrEmpty(_url))
			{
				Application.OpenURL(_url);
			}
		}

		private void Accept()
		{
			if (_reachedBottom || IsAtBottom())
			{
				Close();
			}
			else if (!_isScrolling)
			{
				ScrollToBottom();
			}
		}

		private void OnScrollChanged(Vector2 _)
		{
			if (!_reachedBottom && (bool)ContentScroll && ContentScroll.verticalNormalizedPosition <= 0.01f)
			{
				_reachedBottom = true;
			}
		}

		private bool IsAtBottom()
		{
			if (ContentScroll == null)
			{
				return true;
			}
			RectTransform content = ContentScroll.content;
			if (content == null)
			{
				return true;
			}
			RectTransform rectTransform = ((ContentScroll.viewport != null) ? ContentScroll.viewport : ((RectTransform)ContentScroll.transform));
			if (content.rect.height <= rectTransform.rect.height + 1f)
			{
				return true;
			}
			return ContentScroll.verticalNormalizedPosition <= 0.01f;
		}

		private void ScrollToBottom()
		{
			_isScrolling = true;
			_scrollTween?.Kill();
			_scrollTween = ContentScroll.DOVerticalNormalizedPos(0f, ScrollToBottomDuration).SetEase(Ease.OutCubic).SetUpdate(isIndependentUpdate: true)
				.OnComplete(OnScrollToBottomComplete);
		}

		private void OnScrollToBottomComplete()
		{
			_reachedBottom = true;
			_isScrolling = false;
		}
	}
}
