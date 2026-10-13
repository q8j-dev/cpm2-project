using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AkEnvironment))]
public class AkEnvironmentSceneCleanup : MonoBehaviour
{
	private readonly HashSet<Collider> _insideColliders = new HashSet<Collider>();

	private Collider _myCollider;

	private void Awake()
	{
		_myCollider = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<AkGameObj>() != null)
		{
			_insideColliders.Add(other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		_insideColliders.Remove(other);
	}

	private void OnDisable()
	{
		if (_myCollider == null)
		{
			return;
		}
		foreach (Collider insideCollider in _insideColliders)
		{
			if (insideCollider != null)
			{
				insideCollider.SendMessage("OnTriggerExit", _myCollider, SendMessageOptions.DontRequireReceiver);
			}
		}
		_insideColliders.Clear();
	}
}
