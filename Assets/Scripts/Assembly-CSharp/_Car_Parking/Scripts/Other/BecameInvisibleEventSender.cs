using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class BecameInvisibleEventSender : MonoBehaviour
	{
		public bool IsVisible { get; private set; }

		public event Action<BecameInvisibleEventSender> OnBecameInvisibleEvent;

		private void OnBecameInvisible()
		{
			IsVisible = false;
			OnBecameInvisibleEvent?.Invoke(this);
		}

		private void OnBecameVisible()
		{
			IsVisible = true;
		}
	}
}
