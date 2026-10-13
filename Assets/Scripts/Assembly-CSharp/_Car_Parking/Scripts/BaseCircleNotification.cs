using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts
{
	public abstract class BaseCircleNotification : UIScriptAsyncResult<EmptyData>
	{
		private const float Delay = 30f;

		[SerializeField]
		protected Image FilledCircle;

		private IPrefabFactory _prefabFactory;

		private Action<RequestAnswer> OnRequestAnswer;

		private string _message;

		private int _carId;

		private int _engineType;

		private int _shortTuningData;

		private bool _isCarExchange;

		protected int _price;

		protected FreeDriveProvider FreeDriveProvider;

		protected VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		private ManagerUI _managerUI;

		private Tweener _tween;

		private RequestWindowTemplate _requestWindowTemplate;

		[Inject]
		private void Construct(IPrefabFactory prefabFactory, ManagerUI managerUI, FreeDriveProvider freeDriveProvider, VirtualPurchaseAnalytic virtualPurchaseAnalytic)
		{
			FreeDriveProvider = freeDriveProvider;
			_prefabFactory = prefabFactory;
			_managerUI = managerUI;
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
		}

		protected virtual void Start()
		{
			OnRequestAnswer = (Action<RequestAnswer>)Delegate.Combine(OnRequestAnswer, new Action<RequestAnswer>(HandleRequestAnswer));
			_tween = FilledCircle.DOFillAmount(0f, 30f).OnComplete(TimeOut);
			GetComponent<Button>().onClick.AddListener(CreateRequestWindow);
			_managerUI.CircleNotification = this;
		}

		private void OnEnable()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.RequestRaceSwitch, base.gameObject);
		}

		protected override void OnDestroy()
		{
			OnRequestAnswer = (Action<RequestAnswer>)Delegate.Remove(OnRequestAnswer, new Action<RequestAnswer>(HandleRequestAnswer));
			base.OnDestroy();
		}

		public void Init(string message, int shorTuningData, int carId, int price, int engineType)
		{
			_message = message;
			_shortTuningData = shorTuningData;
			_carId = carId;
			_isCarExchange = true;
			_price = price;
			_engineType = engineType;
		}

		public void Init(string message, int carId)
		{
			_carId = carId;
			_message = message;
			_isCarExchange = false;
		}

		public void RejectAndClose()
		{
			if ((bool)_requestWindowTemplate)
			{
				_requestWindowTemplate.Close();
			}
			Close();
		}

		protected abstract void HandleRequestAnswer(RequestAnswer requestAnswer);

		private async void CreateRequestWindow()
		{
			base.gameObject.SetActive(value: false);
			_tween.Kill();
			if (_isCarExchange)
			{
				TrySetResult(CodeType.Success);
				Close();
				return;
			}
			GameObject obj = await _managerUI.LoadWindowPrefab("RequestWindowTemplate");
			_requestWindowTemplate = _prefabFactory.Instantiate(obj, _managerUI.GetWindowsParent()).GetComponent<RequestWindowTemplate>();
			_managerUI.RequestWindowTemplate = _requestWindowTemplate;
			_requestWindowTemplate.Init(_message, _carId, OnRequestAnswer);
		}

		protected virtual void TimeOut()
		{
			Close();
		}

		protected void Close()
		{
			_tween.Kill();
			_requestWindowTemplate = null;
			_managerUI.CircleNotification = null;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
