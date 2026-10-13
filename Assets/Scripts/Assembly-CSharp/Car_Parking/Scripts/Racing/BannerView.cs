using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace Car_Parking.Scripts.Racing
{
	public class BannerView : MonoBehaviour
	{
		[SerializeField]
		private int BannerIndex = default;

		[SerializeField]
		private BannerViewElement[] BannerContents = default;

		private List<BannerContent> _contentCollection = new List<BannerContent>();

		private int _currentContentIndex;

		private bool _shouldStop;

		private void Awake()
		{
			GlobalEventSystem.OnTimeAttackDataUpdate += OnTimeAttackDataUpdate;
		}

		private void OnTimeAttackDataUpdate(TimeAttackContentData timeAttackContentData)
		{
			if ((timeAttackContentData.BannerDataType == BannerDataType.All || timeAttackContentData.BannerDataType == BannerDataType.HorizontalBanner) && timeAttackContentData.HorizontalBannerTexts != null && BannerIndex == timeAttackContentData.Index)
			{
				ResetAllAnimations();
				_contentCollection = timeAttackContentData.HorizontalBannerTexts;
				_currentContentIndex = 0;
				_shouldStop = false;
				StartScenario();
			}
		}

		public void Init(string contentText, BannerContentType contentType)
		{
			AddContent(contentText, contentType);
			_currentContentIndex = 0;
			StartScenario();
		}

		private void AddContent(string newContentText, BannerContentType contentType)
		{
			BannerContent bannerContent = _contentCollection.FirstOrDefault((BannerContent c) => c.BannerContentType == contentType);
			if (bannerContent != null)
			{
				bannerContent.text = newContentText;
				return;
			}
			_contentCollection.Add(new BannerContent
			{
				text = newContentText,
				BannerContentType = contentType
			});
		}

		private void StartScenario()
		{
			if (_contentCollection.Count > 0)
			{
				ShowContent(_contentCollection[_currentContentIndex]);
			}
		}

		private async UniTaskVoid OnCurrentAnimationEnd()
		{
			await UniTask.Delay(300);
			if (!_shouldStop)
			{
				_currentContentIndex = (_currentContentIndex + 1) % _contentCollection.Count;
				ShowContent(_contentCollection[_currentContentIndex]);
			}
		}

		private void ResetAllAnimations()
		{
			BannerViewElement[] bannerContents = BannerContents;
			for (int i = 0; i < bannerContents.Length; i++)
			{
				bannerContents[i].AnimatedBanner.gameObject.SetActive(value: false);
			}
		}

		public void ShowContent(BannerContent content)
		{
			ResetAllAnimations();
			for (int i = 0; i < BannerContents.Length; i++)
			{
				if (BannerContents[i].BannerContentType == content.BannerContentType)
				{
					BannerContents[i].AnimatedBanner.Init(content.text, () =>
					{
						OnCurrentAnimationEnd().Forget();
					});
				}
			}
		}

		private void OnDisable()
		{
			_shouldStop = true;
			ResetAllAnimations();
		}

		private void OnDestroy()
		{
			_shouldStop = true;
			GlobalEventSystem.OnTimeAttackDataUpdate -= OnTimeAttackDataUpdate;
		}
	}
}
