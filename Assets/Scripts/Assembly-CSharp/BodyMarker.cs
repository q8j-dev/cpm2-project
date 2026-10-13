using System;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

public class BodyMarker : MonoBehaviour
{
	private const float DefaultAnimationLength = 1f;

	private const float AddedDelay = 0.1f;

	private const float EngineHideDelay = 2f;

	private static readonly uint Play_hood_open = AudioWrapper.GetId("Play_hood_open");

	private static readonly uint Play_hood_close = AudioWrapper.GetId("Play_hood_close");

	private static readonly int _open = Animator.StringToHash("open");

	private static readonly int _openTrigger = Animator.StringToHash("openTrigger");

	private static readonly int _closeTrigger = Animator.StringToHash("closeTrigger");

	public Material mainMat;

	public GameObject frontLightLeft;

	public GameObject frontLightRight;

	public GameObject hood;

	public bool hoodState;

	public GameObject engine;

	public SpoilerSystem spoiler;

	public SuspensionParts SuspensionFL;

	public SuspensionParts SuspensionFR;

	public SuspensionParts SuspensionRL;

	public SuspensionParts SuspensionRR;

	[Tooltip("Цельный мост передней оси (опционально): балка едет за средним ходом колёс и кренится при разноимённом ходе.")]
	public Transform FrontSolidAxle;

	[Tooltip("Цельный мост задней оси (опционально), поведение как у переднего.")]
	public Transform RearSolidAxle;

	public GameObject[] AdditionalElements;

	public GameObject[] AdditionalElementsWithoutVinyls;

	[Tooltip("Additional window elements (opening door windows, trunk windows, etc) for vinyls")]
	public GameObject[] AdditionalWindowElements;

	private Animator _hoodAnimator;

	private Animator[] _engineAnimators;

	private IDisposable _hideEngineDisposable;

	private void Awake()
	{
		SetHoodAnimator();
		HideEngineIfHoodClosed();
	}

	private void OnDestroy()
	{
		_hideEngineDisposable?.Dispose();
	}

	private void HideEngineIfHoodClosed()
	{
		if ((bool)engine && !hoodState && (!_hoodAnimator || _hoodAnimator.isActiveAndEnabled))
		{
			HideEngine();
		}
	}

	private void HideEngine()
	{
		if (_engineAnimators != null)
		{
			Animator[] engineAnimators = _engineAnimators;
			foreach (Animator animator in engineAnimators)
			{
				if ((bool)animator)
				{
					animator.enabled = false;
				}
			}
		}
		if ((bool)engine)
		{
			engine.layer = LayerMask.NameToLayer("very short distance");
			engine.SetActive(value: false);
		}
	}

	public void SetHoodAnimator()
	{
		if ((bool)hood && hood.TryGetComponent<Animator>(out var component))
		{
			_hoodAnimator = component;
		}
		if ((bool)engine)
		{
			_engineAnimators = engine.GetComponentsInChildren<Animator>(includeInactive: true);
		}
	}

	public void OpenHood(bool open, bool fast = false)
	{
		if (!_hoodAnimator || !_hoodAnimator.isActiveAndEnabled)
		{
			return;
		}
		if (base.transform.parent.TryGetComponent<CarAnimationsController>(out var carAnimationsController))
		{
			carAnimationsController.OpenDoors[CarDoorType.Hood] = open;
			carAnimationsController.IsAnimatingHood = true;
		}
		_hoodAnimator.SetBool(_open, open);
		_hoodAnimator.SetTrigger(open ? _openTrigger : _closeTrigger);
		hoodState = open;
		if ((bool)engine)
		{
			_hideEngineDisposable?.Dispose();
			_hideEngineDisposable = null;
			if (open)
			{
				engine.SetActive(value: true);
				engine.layer = LayerMask.NameToLayer("CarBody");
				if (_engineAnimators != null)
				{
					Animator[] engineAnimators = _engineAnimators;
					foreach (Animator animator in engineAnimators)
					{
						if ((bool)animator)
						{
							animator.enabled = true;
						}
					}
				}
			}
			else
			{
				_hideEngineDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
				{
					HideEngine();
				});
			}
		}
		if (fast)
		{
			_hoodAnimator.Update(0f);
			AnimatorStateInfo animatorStateInfo = (_hoodAnimator.IsInTransition(0) ? _hoodAnimator.GetNextAnimatorStateInfo(0) : _hoodAnimator.GetCurrentAnimatorStateInfo(0));
			_hoodAnimator.Update(animatorStateInfo.length);
			if ((bool)carAnimationsController)
			{
				carAnimationsController.IsAnimatingHood = false;
			}
			return;
		}
		float num = 1.1f;
		if ((bool)carAnimationsController)
		{
			num = carAnimationsController.GetCurrentAnimationLength(_hoodAnimator) + 0.1f;
		}
		num = ((num <= 0f) ? 0f : num);
		if ((bool)carAnimationsController)
		{
			Observable.Timer(TimeSpan.FromSeconds(num)).Subscribe((long _) =>
			{
				carAnimationsController.IsAnimatingHood = false;
			}).AddTo(this);
		}
		if (base.gameObject.activeInHierarchy)
		{
			AudioWrapper.PostEvent(open ? Play_hood_open : Play_hood_close, base.gameObject);
		}
	}
}
