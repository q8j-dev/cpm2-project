using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class EventButtonPhone : MonoBehaviour
	{
		[SerializeField]
		private RemainingTimeView TimeView = default;

		[SerializeField]
		private RawImageLoadAsync Icon = default;

		[SerializeField]
		private TMP_Text Title = default;

		[SerializeField]
		private Button Button = default;

		private _Car_Parking.Scripts.EventSystem.Data.Event _event;

		public event Action<_Car_Parking.Scripts.EventSystem.Data.Event> OnButtonClick;

		private void OnEnable()
		{
			Button.onClick.AddListener(OnButtonClicked);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(OnButtonClicked);
		}

		private void OnDestroy()
		{
			OnButtonClick = null;
		}

		public void Init(_Car_Parking.Scripts.EventSystem.Data.Event eventData)
		{
			_event = eventData;
			if ((bool)Title)
			{
				Title.text = new LocalizedString(eventData.NameLocalizeKey);
			}
			if ((bool)Icon)
			{
				Icon.LoadFromSDN(eventData.Images?.SummaryImageUrl);
			}
			if ((bool)TimeView)
			{
				TimeView.Init(eventData.EndTime);
			}
		}

		private void OnButtonClicked()
		{
			OnButtonClick?.Invoke(_event);
		}
	}
}
