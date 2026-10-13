using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Racing_Scripts.RewardUI
{
	public class RaceRewardItem : MonoBehaviour
	{
		private readonly uint[] _placeEvents = new uint[3]
		{
			CircuitRewardUiElement.FirstPlaceEvent,
			CircuitRewardUiElement.SecondPlaceEvent,
			CircuitRewardUiElement.ThirdPlaceEvent
		};

		[SerializeField]
		private int PlaceEvent;

		[SerializeField]
		private TextMeshProUGUI Reward;

		[SerializeField]
		private TextMeshProUGUI Count;

		[SerializeField]
		private CombinedPriceView CombinedPriceView;

		[SerializeField]
		private GameObject PassIcon;

		[SerializeField]
		private GameObject NotPassIcon;

		private Tween _tween;

		private IEnumerable<int> PlaceIndices => Enumerable.Range(0, _placeEvents.Length);

		public void UpdateText(RallyTimes rallyTimes)
		{
			if (rallyTimes != null && (bool)Reward)
			{
				Reward.text = rallyTimes.Price.ToString();
			}
		}

		public void SetText(int reward, int winCount, CombinedPrice combinedPrice = null)
		{
			if ((bool)Reward)
			{
				Reward.text = reward.ToString();
			}
			if ((bool)Count)
			{
				Count.text = $"X{winCount}";
			}
			if ((bool)CombinedPriceView && combinedPrice != null)
			{
				CombinedPriceView.Init(combinedPrice);
			}
		}

		public void SetState(bool win)
		{
			PassIcon.SetActive(win);
			NotPassIcon.SetActive(!win);
		}

		public void AnimateWin()
		{
			if (PlaceEvent >= 0 && PlaceEvent < _placeEvents.Length)
			{
				SoundEventsDataHelper.PlayUISound(_placeEvents[PlaceEvent], base.gameObject);
			}
			_tween?.Kill();
			_tween = PassIcon.transform.DOScale(1.5f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
		}

		private void OnDestroy()
		{
			_tween?.Kill();
		}
	}
}
