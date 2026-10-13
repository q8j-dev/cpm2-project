using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.EventSystem.View.ButtonActions;
using _Car_Parking.Scripts.EventSystem.View.Elements;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class BannerView : MonoBehaviour, IBigBannerView
	{
		private readonly List<IBannerElement> _elements = new List<IBannerElement>();

		private readonly List<BannerActionButton> _buttons = new List<BannerActionButton>();

		private readonly Dictionary<BannerButtonKind, IBannerButtonAction> _actions = new Dictionary<BannerButtonKind, IBannerButtonAction>();

		private IEventService _eventService;

		private ServerTimeService _serverTimeService;

		private EventsAnalytic _eventsAnalytic;

		private _Car_Parking.Scripts.EventSystem.Data.Event _event;

		[field: SerializeField]
		public EventType EventType { get; private set; } = EventType.None;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		private void Construct(IEventService eventService, ServerTimeService serverTimeService, EventsAnalytic eventsAnalytic, IEnumerable<IBannerButtonAction> buttonActions)
		{
			_eventService = eventService;
			_serverTimeService = serverTimeService;
			_eventsAnalytic = eventsAnalytic;
			foreach (IBannerButtonAction buttonAction in buttonActions)
			{
				_actions[buttonAction.Kind] = buttonAction;
			}
		}

		public void InitBanner(IBigBannerView.BannerData bannerData)
		{
			_event = _eventService.GetEventByName(bannerData?.EventType ?? EventType.None);
			if (_event != null)
			{
				CollectElements();
				BannerContext context = new BannerContext(_event, _serverTimeService);
				for (int i = 0; i < _elements.Count; i++)
				{
					_elements[i].Bind(context);
				}
				LogFirstShow();
			}
		}

		public bool IsCorrectPrefab(IBigBannerView.BannerData bannerData)
		{
			if (bannerData != null && bannerData.EventType != EventType.None)
			{
				return bannerData.EventType == EventType;
			}
			return false;
		}

		public void Shown()
		{
		}

		private void OnDestroy()
		{
			for (int i = 0; i < _buttons.Count; i++)
			{
				if ((bool)_buttons[i])
				{
					_buttons[i].Clicked -= ButtonClickedCallback;
				}
			}
		}

		private void CollectElements()
		{
			_elements.Clear();
			_buttons.Clear();
			GetComponentsInChildren(includeInactive: true, _elements);
			GetComponentsInChildren(includeInactive: true, _buttons);
			for (int i = 0; i < _buttons.Count; i++)
			{
				_actions.ContainsKey(_buttons[i].ButtonKind);
				_buttons[i].Clicked += ButtonClickedCallback;
			}
			Has(BannerButtonKind.Close);
		}

		private bool Has(BannerButtonKind kind)
		{
			for (int i = 0; i < _buttons.Count; i++)
			{
				if (_buttons[i].ButtonKind == kind)
				{
					return true;
				}
			}
			return false;
		}

		private void ButtonClickedCallback(BannerActionButton button)
		{
			if ((bool)button && _actions.TryGetValue(button.ButtonKind, out var value))
			{
				value.Execute(new BannerActionContext(_event, RequestClose));
			}
		}

		private void RequestClose()
		{
			EventView.Closed?.Invoke();
		}

		private void LogFirstShow()
		{
			string key = "event_first_show_" + _event.Type;
			if (PlayerPrefs.GetInt(key, 0) == 0)
			{
				PlayerPrefs.SetInt(key, 1);
				_eventsAnalytic.LogEventFirstShow(_event);
			}
		}
	}
}
