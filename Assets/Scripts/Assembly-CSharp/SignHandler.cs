using System;
using CPM.Database;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.SignBehaviour;

public class SignHandler : MonoBehaviour
{
	[SerializeField]
	public Collider _mainCollider;

	[SerializeField]
	[Tooltip("Collider marked as Trigger")]
	public Collider _triggerCollider;

	[SerializeField]
	[Tooltip("Put slicers in order that want the object to be cut")]
	private Slicer[] _slicers = new Slicer[0];

	[SerializeField]
	private SignHandlerBehaviourBaseSO _signHandlerBehaviour = default;

	[SerializeField]
	private BecameInvisibleEventSender[] BecameInvisibleEventSenders = default;

	private Rigidbody _rb;

	private bool _isSignCorrupted;

	public Action<Transform> OnSignBroke { get; set; }

	private bool AllPartsInvisible
	{
		get
		{
			if (BecameInvisibleEventSenders == null)
			{
				return true;
			}
			for (int i = 0; i < BecameInvisibleEventSenders.Length; i++)
			{
				if ((bool)BecameInvisibleEventSenders[i] && BecameInvisibleEventSenders[i].IsVisible)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool IsTriggered { get; private set; }

	private void Awake()
	{
		MeshCollider component = GetComponent<MeshCollider>();
		if ((bool)component && _mainCollider == null)
		{
			_mainCollider = component;
			component.convex = true;
		}
		if (_mainCollider == null)
		{
			_mainCollider = GetComponent<BoxCollider>();
		}
		if (!_triggerCollider)
		{
			_triggerCollider = base.gameObject.AddComponent<BoxCollider>();
		}
		_triggerCollider.isTrigger = true;
		if (_mainCollider == null || _triggerCollider == null)
		{
			RemoveComponents();
			return;
		}
		_mainCollider.enabled = false;
		_triggerCollider.enabled = true;
	}

	private void OnEnable()
	{
		if (BecameInvisibleEventSenders == null)
		{
			return;
		}
		for (int i = 0; i < BecameInvisibleEventSenders.Length; i++)
		{
			if ((bool)BecameInvisibleEventSenders[i])
			{
				BecameInvisibleEventSenders[i].OnBecameInvisibleEvent += OnBecameInvisibleChildCallback;
			}
		}
	}

	private void OnDisable()
	{
		if (BecameInvisibleEventSenders == null)
		{
			return;
		}
		for (int i = 0; i < BecameInvisibleEventSenders.Length; i++)
		{
			if ((bool)BecameInvisibleEventSenders[i])
			{
				BecameInvisibleEventSenders[i].OnBecameInvisibleEvent -= OnBecameInvisibleChildCallback;
			}
		}
	}

	[ContextMenu("ParseColider")]
	public void ParseColider()
	{
		_mainCollider = GetComponent<CapsuleCollider>();
		_triggerCollider = GetComponent<SphereCollider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Car") || _isSignCorrupted || (TryGetComponent<MeshRenderer>(out var component) && !component.isVisible))
		{
			return;
		}
		Rigidbody componentInParent = other.GetComponentInParent<Rigidbody>();
		TryGetComponent<Rigidbody>(out _rb);
		if (_rb == null)
		{
			_rb = base.gameObject.AddComponent<Rigidbody>();
		}
		if (_slicers.Length != 0)
		{
			Slicer[] slicers = _slicers;
			foreach (Slicer slicer in slicers)
			{
				if ((bool)slicer)
				{
					slicer.transform.SetParent(null);
				}
			}
			if ((bool)_slicers[0])
			{
				_slicers[0].ActivateSlicer(componentInParent.velocity, null, base.gameObject);
				FreeDriveDB freeDriveDB = other.GetComponentInParent<CarTriggers>().FreeDriveProvider.FreeDriveDB;
				if ((bool)freeDriveDB)
				{
					++freeDriveDB.slicer_cut;
				}
				_isSignCorrupted = true;
			}
			_slicers = null;
		}
		else
		{
			_rb.mass = 7f;
			_rb.isKinematic = false;
			_rb.AddForce(componentInParent.velocity, ForceMode.Impulse);
			_rb.interpolation = RigidbodyInterpolation.Interpolate;
			if ((bool)_triggerCollider)
			{
				_triggerCollider.enabled = false;
			}
			_mainCollider.enabled = true;
			base.transform.Find("PROC_PLANE")?.gameObject.SetActive(value: false);
			if ((bool)_signHandlerBehaviour)
			{
				_signHandlerBehaviour.Execute(this);
			}
			_isSignCorrupted = true;
			OnSignBroke?.Invoke(base.transform);
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				IsTriggered = true;
			});
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if ((bool)_signHandlerBehaviour)
		{
			_signHandlerBehaviour.OnCollisionEnterExecute(this, other);
		}
	}

	private void OnBecameInvisible()
	{
		if (_isSignCorrupted && AllPartsInvisible)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnBecameInvisibleChildCallback(BecameInvisibleEventSender invisibleEventSender)
	{
		OnBecameInvisible();
	}

	private void RemoveComponents()
	{
		UnityEngine.Object.Destroy(_triggerCollider);
		UnityEngine.Object.Destroy(this);
		UnityEngine.Object.Destroy(_rb);
	}
}
