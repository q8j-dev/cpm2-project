using System.Collections.Generic;
using AK.Wwise;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

public class WwiseAmbienceState : MonoBehaviour, IAmbienceStateZone
{
	public State OntriggerEnterState;

	public State OntriggerExitState;

	[SerializeField]
	private bool IsFilterEnabled;

	private readonly Dictionary<Collider, int> _insideCounts = new Dictionary<Collider, int>();

	private readonly List<Collider> _staleColliders = new List<Collider>();

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("MainCamera"))
		{
			_insideCounts.TryGetValue(other, out var value);
			_insideCounts[other] = value + 1;
			FilterStateUpdate(enable: true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("MainCamera"))
		{
			if (_insideCounts.TryGetValue(other, out var value) && value > 1)
			{
				_insideCounts[other] = value - 1;
			}
			else
			{
				_insideCounts.Remove(other);
			}
			RemoveStaleColliders();
			if (_insideCounts.Count == 0)
			{
				FilterStateUpdate(enable: false);
			}
		}
	}

	private void RemoveStaleColliders()
	{
		_staleColliders.Clear();
		foreach (Collider key in _insideCounts.Keys)
		{
			if (key == null || !key.enabled || !key.gameObject.activeInHierarchy)
			{
				_staleColliders.Add(key);
			}
		}
		foreach (Collider staleCollider in _staleColliders)
		{
			_insideCounts.Remove(staleCollider);
		}
	}

	public void ApplyEnterState()
	{
		OntriggerEnterState.SetValue();
		IsFilterEnabled = true;
	}

	private void FilterStateUpdate(bool enable)
	{
		if (!(enable ? IsFilterEnabled : (!IsFilterEnabled)))
		{
			(enable ? OntriggerEnterState : OntriggerExitState).SetValue();
			IsFilterEnabled = enable;
		}
	}

	private void ForceDisableFilter()
	{
		FilterStateUpdate(enable: false);
	}

	private void OnDisable()
	{
		_insideCounts.Clear();
		ForceDisableFilter();
	}
}
