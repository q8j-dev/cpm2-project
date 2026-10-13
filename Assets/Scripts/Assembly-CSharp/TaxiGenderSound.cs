using System;
using CarInteractionAnimationsScripts;
using UniRx;
using UnityEngine;

public class TaxiGenderSound : MonoBehaviour
{
	private TaxiClientController _taxiClientController;

	public bool male;

	public AudioClip maleSound;

	public AudioClip femaleSound;

	public float waitTime;

	public float _checkDist;

	public GameObject waitParticle;

	private GameObject _root;

	private PersonAnimationsController _personAnimationsController;

	private Rigidbody _rootRigidbody;

	private CapsuleCollider _rootCapsCollider;

	private IDisposable _soundPlayDisposable;

	private IDisposable _animatorSwitcherDisposable;

	public MainPerson mainPerson;

	private void Start()
	{
		if ((bool)GetComponent<TaxiClientController>())
		{
			_taxiClientController = GetComponent<TaxiClientController>();
			_root = base.gameObject.transform.parent.gameObject;
			_personAnimationsController = _root.GetComponent<PersonAnimationsController>();
			if ((bool)_root.GetComponent<Rigidbody>())
			{
				_rootRigidbody = _root.GetComponent<Rigidbody>();
			}
			if ((bool)_root.GetComponent<CapsuleCollider>())
			{
				_rootCapsCollider = _root.GetComponent<CapsuleCollider>();
			}
			_soundPlayDisposable = Observable.Timer(TimeSpan.FromSeconds(waitTime)).Subscribe((long _) =>
			{
				SoundPlay();
			}).AddTo(this);
			_animatorSwitcherDisposable = Observable.Timer(TimeSpan.FromSeconds(0.0)).Subscribe((long _) =>
			{
				AnimatorComponentSwitcher();
			}).AddTo(this);
			male = (int)mainPerson.Gender == 1;
			if ((bool)_personAnimationsController)
			{
				_personAnimationsController.Animator.enabled = false;
			}
		}
	}

	private void AnimatorComponentSwitcher()
	{
		_animatorSwitcherDisposable?.Dispose();
		_animatorSwitcherDisposable = Observable.Timer(TimeSpan.FromSeconds(UnityEngine.Random.Range(3, 3))).Subscribe((long _) =>
		{
			AnimatorComponentSwitcher();
		}).AddTo(this);
		if (_taxiClientController.isPassenger || !V.DriverPerson)
		{
			return;
		}
		if (Vector3.Distance(V.DriverPerson.transform.position, base.transform.position) < _checkDist)
		{
			if ((bool)_rootCapsCollider)
			{
				_rootCapsCollider.enabled = true;
			}
			if ((bool)_personAnimationsController)
			{
				_personAnimationsController.Animator.enabled = true;
			}
			if ((bool)_rootRigidbody)
			{
				_rootRigidbody.useGravity = true;
			}
		}
		else
		{
			if ((bool)_personAnimationsController)
			{
				_personAnimationsController.Animator.enabled = false;
			}
			if ((bool)_rootRigidbody)
			{
				_rootRigidbody.useGravity = false;
			}
			if ((bool)_rootCapsCollider)
			{
				_rootCapsCollider.enabled = false;
			}
		}
	}

	private void SoundPlay()
	{
		_soundPlayDisposable?.Dispose();
		_soundPlayDisposable = Observable.Timer(TimeSpan.FromSeconds(UnityEngine.Random.Range(10, 25))).Subscribe((long _) =>
		{
			SoundPlay();
		}).AddTo(this);
		if (_taxiClientController.isPassenger || !V.DriverPerson)
		{
			return;
		}
		if (Vector3.Distance(V.DriverPerson.transform.position, base.transform.position) < _checkDist)
		{
			if (male)
			{
				_audioSourceWork(maleSound);
			}
			else
			{
				_audioSourceWork(femaleSound);
			}
			_waitingParticleEnable();
			_waitingAnimationEnable();
			_waitingRotationToCar();
			if ((bool)_personAnimationsController)
			{
				_personAnimationsController.Animator.enabled = true;
			}
		}
		else
		{
			if ((bool)GetComponent<AudioSource>())
			{
				UnityEngine.Object.Destroy(GetComponent<AudioSource>());
			}
			if ((bool)_personAnimationsController)
			{
				_personAnimationsController.Animator.enabled = false;
			}
			_waitingParticleDisable();
			_waitingAnimationDisable();
		}
	}

	private void _audioSourceWork(AudioClip _audioClip)
	{
		AudioSource audioSource = ((!GetComponent<AudioSource>()) ? base.gameObject.AddComponent<AudioSource>() : GetComponent<AudioSource>());
		audioSource.spatialBlend = 1f;
		audioSource.maxDistance = _checkDist;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.volume = 0.5f;
		audioSource.pitch = UnityEngine.Random.Range(0.8f, 1f);
		audioSource.PlayOneShot(_audioClip);
	}

	private void _waitingParticleEnable()
	{
		if (waitParticle != null)
		{
			waitParticle.SetActive(value: true);
		}
	}

	private void _waitingParticleDisable()
	{
		if (waitParticle != null)
		{
			waitParticle.SetActive(value: false);
		}
	}

	private void _waitingAnimationEnable()
	{
		if (_taxiClientController != null && _taxiClientController._animator != null)
		{
			_taxiClientController._animator.SetBool("wait", value: true);
		}
	}

	private void _waitingAnimationDisable()
	{
		if (_taxiClientController != null && _taxiClientController._animator != null)
		{
			_taxiClientController._animator.SetBool("wait", value: false);
		}
	}

	private void _waitingRotationToCar()
	{
		Transform root = base.transform.root;
		Vector3 forward = V.DriverPerson.transform.root.transform.position - root.position;
		forward.y = 0f;
		if (root.rotation != new Quaternion(0f, 0f, 0f, 0f))
		{
			root.rotation = Quaternion.Lerp(root.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 500f);
		}
	}
}
