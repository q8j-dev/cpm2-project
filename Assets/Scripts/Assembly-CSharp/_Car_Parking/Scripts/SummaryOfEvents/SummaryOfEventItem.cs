using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	public class SummaryOfEventItem : MonoBehaviour
	{
		[SerializeField]
		private Button Button = default;

		public event Action<SummaryOfEventItem> OnClickEvent;

		protected virtual void OnEnable()
		{
			Button.onClick.AddListener(ClickCallback);
		}

		protected virtual void OnDisable()
		{
			Button.onClick.RemoveListener(ClickCallback);
		}

		protected virtual void ClickCallback()
		{
			OnClickEvent?.Invoke(this);
		}
	}
}
