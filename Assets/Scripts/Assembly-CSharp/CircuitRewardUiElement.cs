using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Audio;

public class CircuitRewardUiElement : MonoBehaviour
{
	public static readonly uint FirstPlaceEvent = AudioWrapper.GetId("Race_1st");

	public static readonly uint SecondPlaceEvent = AudioWrapper.GetId("Race_2d");

	public static readonly uint ThirdPlaceEvent = AudioWrapper.GetId("Race_3d");

	public static readonly uint LoosEvent = AudioWrapper.GetId("Race_Looser");

	private uint[] _placeEvents = new uint[3] { FirstPlaceEvent, SecondPlaceEvent, ThirdPlaceEvent };

	[SerializeField]
	private uint PlaceEvent;

	[SerializeField]
	private TextMeshProUGUI MoneyText;

	[SerializeField]
	private Image CupWinImage;

	[SerializeField]
	private Image CupLoseImage;

	[SerializeField]
	private Image RewardWinImage;

	[SerializeField]
	private Image RewardLoseImage;

	[Header("Animation")]
	[SerializeField]
	private float CupStartScale = 5f;

	[SerializeField]
	private float CupStartFade = 0.5f;

	[SerializeField]
	private float TweenDuration = 0.3f;

	[SerializeField]
	private Ease CupScalingEase = Ease.InBack;

	public void Init(int price)
	{
		MoneyText.text = price.ToString();
		CupWinImage.SetActive(active: false);
		CupLoseImage.SetActive(active: false);
		RewardWinImage.SetActive(active: false);
		RewardLoseImage.SetActive(active: false);
	}

	public async UniTask WinAnimation(CancellationToken cancellationToken)
	{
		SoundEventsDataHelper.PlayUISound(PlaceEvent, base.gameObject);
		Color color = CupWinImage.color;
		CupWinImage.color = new Color(color.r, color.g, color.b, CupStartFade);
		CupWinImage.transform.localScale = Vector3.one * CupStartScale;
		CupWinImage.SetActive(active: true);
		CupWinImage.DOFade(1f, TweenDuration).SetLink(CupWinImage.gameObject);
		CupWinImage.transform.DOScale(1f, TweenDuration).SetEase(CupScalingEase).SetLink(CupWinImage.gameObject);
		Color color2 = RewardWinImage.color;
		RewardWinImage.color = new Color(color2.r, color2.g, color2.b, 0f);
		RewardWinImage.SetActive(active: true);
		RewardWinImage.DOFade(1f, TweenDuration).SetLink(RewardLoseImage.gameObject);
		await UniTask.Delay(TimeSpan.FromSeconds(TweenDuration), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
	}

	public async UniTask LoseAnimation(CancellationToken cancellationToken)
	{
		SoundEventsDataHelper.PlayUISound(LoosEvent, base.gameObject);
		Color color = CupLoseImage.color;
		CupLoseImage.color = new Color(color.r, color.g, color.b, 0f);
		CupLoseImage.SetActive(active: true);
		CupLoseImage.DOFade(1f, TweenDuration).SetLink(CupWinImage.gameObject);
		Color color2 = RewardLoseImage.color;
		RewardLoseImage.color = new Color(color2.r, color2.g, color2.b, 0f);
		RewardLoseImage.SetActive(active: true);
		RewardLoseImage.DOFade(1f, TweenDuration).SetLink(RewardLoseImage.gameObject);
		await UniTask.Delay(TimeSpan.FromSeconds(TweenDuration), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
	}
}
