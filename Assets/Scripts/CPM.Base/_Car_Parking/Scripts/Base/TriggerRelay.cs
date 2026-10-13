using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Base
{
	public sealed class TriggerRelay : MonoBehaviour
	{
		public event Action<Collider, TriggerRelay> OnTriggerEntered;

		private void OnTriggerEnter(Collider other)
		{
			OnTriggerEntered?.Invoke(other, this);
		}
	}
}
