using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Art.UI.Prefabs.NewUI.Scripts1.Elements
{
	public class ButtonHelper : MonoBehaviour
	{
		[SerializeField]
		private GameObject ChildGameObject;

		[SerializeField]
		private Button CurrentButton;

		[SerializeField]
		private bool AnimateSize;

		[SerializeField]
		private Transform ChildTransform;

		[SerializeField]
		private float TargetScale = 0.9f;

		[SerializeField]
		private bool AnimateAlpha;

		[SerializeField]
		private Image ChildImage;

		[SerializeField]
		private float TargetAlpha;

		[SerializeField]
		private float AnimationDuration = 0.2f;

		private Vector3 _initialScale = Vector3.one;

		private float _initialAlpha = 1f;

		private void Awake()
		{
			if (AnimateSize && (bool)ChildTransform)
			{
				_initialScale = ChildTransform.localScale;
			}
			if (AnimateAlpha && (bool)ChildImage)
			{
				_initialAlpha = ChildImage.color.a;
			}
		}

		private void Start()
		{
			CurrentButton.onClick.AddListener(EnableChild);
		}

		private void EnableChild()
		{
			CurrentButton.interactable = false;
			ChildGameObject.SetActive(value: true);
			if (AnimateAlpha || AnimateSize)
			{
				PlayChildTween();
			}
		}

		private void PlayChildTween()
		{
			KillChildTweens();
			if (AnimateSize && (bool)ChildTransform)
			{
				ChildTransform.localScale = _initialScale;
				ChildTransform.DOScale(TargetScale, AnimationDuration).SetUpdate(isIndependentUpdate: true);
			}
			if (AnimateAlpha && (bool)ChildImage)
			{
				Color color = ChildImage.color;
				color.a = _initialAlpha;
				ChildImage.color = color;
				ChildImage.DOFade(TargetAlpha, AnimationDuration).SetUpdate(isIndependentUpdate: true);
			}
		}

		private void OnEnable()
		{
			ChildGameObject.SetActive(value: false);
			CurrentButton.interactable = true;
		}

		private void OnDisable()
		{
			KillChildTweens();
		}

		private void KillChildTweens()
		{
			if ((bool)ChildTransform)
			{
				ChildTransform.DOKill();
			}
			if ((bool)ChildImage)
			{
				ChildImage.DOKill();
			}
		}
	}
}
