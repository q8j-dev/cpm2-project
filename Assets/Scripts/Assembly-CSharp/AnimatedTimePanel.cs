using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using VContainer;
using _Car_Parking.Scripts.Drag;

public class AnimatedTimePanel : MonoBehaviour, IPopupWindow
{
	private Tween _tween;

	public float Duration;

	[SerializeField]
	private float ShrinkTime = 0.2f;

	[SerializeField]
	private float ValueShowTime = 0.2f;

	[SerializeField]
	private RectTransform StartTransform;

	[SerializeField]
	private float StartScale;

	[SerializeField]
	private float EndScale;

	[SerializeField]
	private Transform TargetCircle;

	[SerializeField]
	private GameObject TargetCircleParent;

	[SerializeField]
	private GameObject Panel;

	[SerializeField]
	private TextMeshProUGUI ValueText;

	[SerializeField]
	private TextMeshProUGUI DescriptionText;

	[SerializeField]
	private CanvasGroup ValueCanvasGroup;

	[SerializeField]
	private float StartAlpha;

	[SerializeField]
	private float EndAlpha;

	[SerializeField]
	private RectTransform ParentRectTransform;

	[SerializeField]
	private CanvasGroup[] CanvasGroups;

	[SerializeField]
	private Image LeftBackgroundImage;

	[SerializeField]
	private UnityEngine.UI.Extensions.Gradient Gradient;

	[SerializeField]
	private Image RightBackgroundImage;

	[SerializeField]
	private BlinkAnimationComponent BlinkAnimationComponent;

	private Tween _flashTween;

	private bool _isDestroyed;

	private UiNotificationsManager _uiNotificationsManager;

	private PopupData _popupData;

	[Inject]
	public void Construct(UiNotificationsManager uiNotificationsManager)
	{
		_uiNotificationsManager = uiNotificationsManager;
	}

	public void Init(PopupData popupData)
	{
		SetBackgroundColor(popupData.BackgroundColor);
		_popupData = popupData;
		if ((bool)ParentRectTransform)
		{
			EndScale = ParentRectTransform.rect.size.x;
		}
		if ((bool)TargetCircleParent)
		{
			TargetCircleParent.gameObject.SetActive(value: true);
		}
		if ((bool)TargetCircle)
		{
			TargetCircle.gameObject.SetActive(value: true);
		}
		Panel.SetActive(value: true);
		if ((bool)ValueText)
		{
			ValueText.text = popupData.Title;
			ValueText.gameObject.SetActive(!string.IsNullOrEmpty(popupData.Title));
		}
		if ((bool)DescriptionText)
		{
			DescriptionText.text = popupData.Description;
		}
		BlinkAnimationComponent.gameObject.SetActive(value: true);
		ShowAnimation();
		PlaySound(popupData.AudioKey);
		DelayedClose().Forget();
	}

	private void SetBackgroundColor(PopupColorData colorData)
	{
		if (colorData != null)
		{
			LeftBackgroundImage.color = ColorHelperUtility.GetColor(colorData.ColorKey);
			if ((bool)Gradient && colorData.UseGradient)
			{
				Gradient.SetVertex(ColorHelperUtility.GetColor(colorData.GradientStartKey), ColorHelperUtility.GetColor(colorData.GradientEndKey));
				Gradient.enabled = true;
			}
		}
	}

	private async UniTaskVoid DelayedClose()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(Duration));
		if (!_isDestroyed)
		{
			HideAnimation();
		}
	}

	public void PlaySound(uint key)
	{
		SoundEventsDataHelper.PlayUISound(key, base.gameObject);
	}

	public void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void ChangeBackgrounds(Sprite rightBackground, Sprite leftBackground)
	{
		if ((bool)LeftBackgroundImage)
		{
			LeftBackgroundImage.sprite = leftBackground;
		}
		if ((bool)RightBackgroundImage)
		{
			RightBackgroundImage.sprite = rightBackground;
		}
	}

	public void ReInit(string text, float endScale = -1f)
	{
		if (endScale < 0f)
		{
			endScale = EndScale;
		}
		EndScale = endScale;
		if ((bool)ValueText)
		{
			ValueText.text = text;
		}
		_flashTween?.Kill();
		ShowAnimation(reuse: true);
	}

	public void ShowAnimation(bool reuse = false)
	{
		_tween?.Kill();
		Sequence sequence = DOTween.Sequence().SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
		if (!reuse)
		{
			sequence.Append(TargetCircle.DORotate(new Vector3(0f, 180f, 0f), 1f, RotateMode.WorldAxisAdd).SetEase(Ease.OutQuad));
		}
		sequence.Append(StartTransform.DOSizeDelta(new Vector2(EndScale, StartTransform.sizeDelta.y), ShrinkTime).SetEase(Ease.OutQuad));
		sequence.Append(ValueCanvasGroup.DOFade(EndAlpha, ValueShowTime));
		_tween = sequence;
	}

	public void HideAnimation()
	{
		_tween?.Kill();
		Sequence s = (Sequence)(_tween = DOTween.Sequence().SetLink(base.gameObject, LinkBehaviour.KillOnDestroy));
		s.Append(ValueCanvasGroup.DOFade(StartAlpha, ValueShowTime));
		s.Append(StartTransform.DOSizeDelta(new Vector2(StartScale, StartTransform.sizeDelta.y), ShrinkTime).SetEase(Ease.OutQuad));
		s.AppendCallback(() =>
		{
			base.gameObject.SetActive(value: false);
			_uiNotificationsManager.Remove(_popupData);
		});
	}

	public async UniTaskVoid DelayedSetAlpha(float alpha, float delay = 0f)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(delay));
		if (!_isDestroyed)
		{
			for (int i = 0; i < CanvasGroups.Length; i++)
			{
				CanvasGroups[i].DOFade(alpha, 0.1f);
			}
		}
	}

	private void OnDisable()
	{
		_tween?.Kill();
	}

	private void OnDestroy()
	{
		_isDestroyed = true;
	}
}
