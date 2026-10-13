using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class SubmitCircleWindow : WindowScript
	{
		[SerializeField]
		private Button SubmitButton;

		[SerializeField]
		private Image FilledCircle;

		public bool justTimer;

		public float _delay = 30f;

		private bool _isClickSubmit;

		private FreeDriveProvider _freeDriveProvider;

		private EventRegistry _eventRegistry;

		private RegistryHandle<ScriptableEvent> _interactHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Interact);

		private RegistryHandle<ScriptableEvent> _cancelHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Cancel);

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, EventRegistry eventRegistry)
		{
			_freeDriveProvider = freeDriveProvider;
			_eventRegistry = eventRegistry;
		}

		protected override void Start()
		{
			base.Start();
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.RequestRaceSwitch, base.gameObject);
			if (!justTimer)
			{
				_freeDriveProvider.FreeDriveControl.notificationCircleRacing = base.gameObject;
				SubmitButton.onClick.AddListener(Submit);
			}
			FilledCircle.DOFillAmount(0f, _delay).OnComplete(Close);
			_freeDriveProvider.QuickCarSettings.Door.AddListener(CloseCancel);
			_freeDriveProvider.QuickCarSettings.DoorPassenger.AddListener(CloseCancel);
			_interactHandle.Get(_eventRegistry).Subscribe(Submit);
			_cancelHandle.Get(_eventRegistry).Subscribe(Close);
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				base.Close();
				if (!_isClickSubmit)
				{
					_freeDriveProvider.FreeDriveControl.CancelButtonNegWindow();
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_freeDriveProvider.QuickCarSettings.Door.RemoveListener(CloseCancel);
			_freeDriveProvider.QuickCarSettings.DoorPassenger.RemoveListener(CloseCancel);
			SubmitButton.onClick.RemoveAllListeners();
			_interactHandle.Get(_eventRegistry)?.Unsubscribe(Submit);
			_cancelHandle.Get(_eventRegistry)?.Unsubscribe(Close);
		}

		private void Submit()
		{
			if ((!V.DriverPerson.TryGetComponent<MainPerson>(out var component) || !component.PersonAnimationsController || !component.PersonAnimationsController.AnimationService.IsAnimatingPlayer()) && !justTimer)
			{
				_isClickSubmit = true;
				_managerUI.ShowRequestWindow();
			}
		}

		private void CloseCancel()
		{
			_isClickSubmit = true;
			Close();
		}
	}
}
