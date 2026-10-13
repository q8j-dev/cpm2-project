using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

namespace Service.Abstract
{
	[Serializable]
	public abstract class TaskManager : MonoBehaviour, ITaskManager
	{
		[SerializeField]
		public Dictionary<int, ClientController> clients = new Dictionary<int, ClientController>();

		[SerializeField]
		public ServiceController serviceController;

		protected Queue<CancellationTokenSource> cancelToken = new Queue<CancellationTokenSource>();

		protected ManagerUI ManagerUI;

		private ServiceProgressWindow _serviceProgressWindow;

		public float ProgressTime { get; set; }

		public float TaskTime { get; protected set; }

		public float NotificationWaitTime { get; protected set; }

		public string NotificationText { get; protected set; }

		public string NotificationTitle { get; protected set; }

		public Action OnCounterStopped { get; set; }

		[Inject]
		public void Construct(ManagerUI managerUI, OptionalDependency<ParkingMenuUI> parkingMenuUI)
		{
			ManagerUI = managerUI;
			if (parkingMenuUI.HasValue)
			{
				_serviceProgressWindow = parkingMenuUI.Value.ServiceProgressWindow;
			}
		}

		public virtual void OnUserDisconnect(int ownerID)
		{
			EndTaskForOther(ownerID);
		}

		public virtual void EndTaskForOther(int ownerID)
		{
			if (clients.TryGetValue(ownerID, out var value) && (bool)value)
			{
				UnityEngine.Object.Destroy(value.gameObject);
			}
			clients.Remove(ownerID);
		}

		public virtual void Counter(float taskTime)
		{
			TaskTime = taskTime;
			OnCounterStopped?.Invoke();
			_serviceProgressWindow.Init();
		}

		public virtual void ShowResult(string title, string text, float delay)
		{
			NotificationTitle = title;
			NotificationText = text;
			NotificationWaitTime = delay;
			ManagerUI.ShowServiceResultWindow().Forget();
		}

		private void OnDestroy()
		{
			OnCounterStopped = null;
		}
	}
}
