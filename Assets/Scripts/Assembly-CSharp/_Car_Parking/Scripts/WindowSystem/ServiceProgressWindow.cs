using System;
using Service;
using Service.Abstract;
using Service.Manager;
using Service.Taxi;
using TMPro;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ServiceProgressWindow : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI Timer;

		[SerializeField]
		private TextMeshProUGUI ItemsCount;

		[SerializeField]
		private TextMeshProUGUI Money;

		private IDisposable _disposable;

		private float _time;

		private Service.Manager.TaskManager TaskManager => Global.TaskManager;

		private void OnDestroy()
		{
			Unsubscribe();
		}

		public void Init()
		{
			if (TaskManager.Task.Type == TYPE.TAXI)
			{
				_time = TaskManager.TaxiManager.TaskTime;
				TaxiManager taxiManager = TaskManager.TaxiManager;
				taxiManager.OnCounterStopped = (Action)Delegate.Combine(taxiManager.OnCounterStopped, new Action(Stop));
			}
			if (TaskManager.Task.Type == TYPE.DELIVERY)
			{
				_time = TaskManager.DeliveryManager.TaskTime;
				Service.Abstract.Task task = TaskManager.Task;
				task.OnItemsCountChanged = (Action)Delegate.Combine(task.OnItemsCountChanged, new Action(SetItemsCount));
				TaskManager.OnFinish += Stop;
				TaskManager.OnCancelTask += Stop;
				ItemsCount.SetActive(active: true);
				Money.SetActive(active: true);
				SetItemsCount();
			}
			Counter();
		}

		private void Counter()
		{
			Timer.SetActive(active: true);
			_disposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Repeat().Subscribe((long _) =>
			{
				_time -= Time.timeScale;
				if (_time < 0f)
				{
					TaskManager.ProgressTime = 0f;
					if (TaskManager.Task.Type == TYPE.TAXI)
					{
						TaskManager.TaxiManager.CancelTask();
						Close();
					}
					_disposable?.Dispose();
				}
				else
				{
					SetTimer(_time);
				}
			})
				.AddTo(this);
		}

		private void SetTimer(float time)
		{
			if (!(time < 0f))
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(time);
				Timer.text = timeSpan.ToString("hh\\:mm\\:ss");
			}
		}

		private void Stop()
		{
			TaskManager.ProgressTime = _time;
			_disposable?.Dispose();
			Close();
		}

		private void SetItemsCount()
		{
			string text = TaskManager.Task.CurrentItemsCount.ToString();
			string text2 = TaskManager.Task.TotalItemsCount.ToString();
			ItemsCount.text = text + "/" + text2;
			Money.text = "$" + TaskManager.Task.Order.Price;
		}

		private void Close()
		{
			if ((bool)Timer)
			{
				Timer.SetActive(active: false);
			}
			if ((bool)ItemsCount)
			{
				ItemsCount.SetActive(active: false);
			}
			if ((bool)Money)
			{
				Money.SetActive(active: false);
			}
			Unsubscribe();
		}

		private void Unsubscribe()
		{
			Service.Manager.TaskManager taskManager = TaskManager;
			if ((bool)taskManager)
			{
				taskManager.OnFinish -= Stop;
				taskManager.OnCancelTask -= Stop;
				if (taskManager.TaxiManager != null)
				{
					TaxiManager taxiManager = taskManager.TaxiManager;
					taxiManager.OnCounterStopped = (Action)Delegate.Remove(taxiManager.OnCounterStopped, new Action(Stop));
				}
				if (taskManager.Task != null)
				{
					Service.Abstract.Task task = taskManager.Task;
					task.OnItemsCountChanged = (Action)Delegate.Remove(task.OnItemsCountChanged, new Action(SetItemsCount));
				}
			}
		}
	}
}
