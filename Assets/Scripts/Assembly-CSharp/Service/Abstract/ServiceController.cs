using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.WindowSystem;

namespace Service.Abstract
{
	[Serializable]
	public abstract class ServiceController : MonoBehaviour, IServiceController
	{
		protected int? currentTask;

		protected int index;

		protected int time = -1;

		protected CancellationTokenSource token;

		protected Dictionary<int, Transform> orders = new Dictionary<int, Transform>();

		protected RawImage mapTexture;

		[SerializeField]
		protected SpawnPointsScriptableObject positionsSO;

		[SerializeField]
		protected Button startButton;

		[SerializeField]
		protected Transform parentList;

		[SerializeField]
		protected GameObject orderPrefab;

		[SerializeField]
		protected List<Location> locations;

		[SerializeField]
		protected List<GameObject> windows;

		public PhoneWindow PhoneWindow { get; set; }

		public Order CurrentOrder { get; set; }

		public virtual void AddTask()
		{
		}

		public virtual void RefreshList()
		{
			time = 50;
			PhoneWindow.ShowNonInteractableAcceptButton();
			orders.Clear();
			index = 0;
		}

		public virtual void ClickStartButton(int? taskID)
		{
			GPSTracker.ClearPath(forget: true);
		}

		protected async UniTaskVoid Timer()
		{
			while (time >= 0)
			{
				time--;
				await UniTask.Delay(TimeSpan.FromMilliseconds(1000.0));
			}
		}

		public virtual void CreateOrders()
		{
			RefreshList();
			Timer().Forget();
		}

		protected virtual void HighlightTask(Transform orderTransform)
		{
			foreach (Transform item in orderTransform.parent)
			{
				item.GetChild(0).GetComponent<Image>().color = Color.white;
				for (int i = 0; i <= 1; i++)
				{
					item.GetChild(1).GetChild(i).GetComponent<TextMeshProUGUI>()
						.color = new Color(109f / 255f, 160f / 255f, 229f / 255f);
				}
				for (int j = 2; j <= 4; j++)
				{
					item.GetChild(1).GetChild(j).GetComponent<TextMeshProUGUI>()
						.color = Color.black;
				}
			}
			orderTransform.GetChild(0).GetComponent<Image>().color = new Color(251f / 255f, 180f / 255f, 89f / 255f);
			for (int k = 2; k <= 4; k++)
			{
				orderTransform.GetChild(1).GetChild(k).GetComponent<TextMeshProUGUI>()
					.color = Color.white;
			}
		}

		public virtual void OnSceneLoaded()
		{
		}

		public virtual void ShowWindow(int i)
		{
			foreach (GameObject window in windows)
			{
				window.SetActive(value: false);
			}
			windows[i].SetActive(value: true);
		}

		protected virtual void RemoveTask(int taskID)
		{
		}

		public virtual void OnTaskFinished()
		{
		}

		protected virtual void LocateOrders()
		{
		}
	}
}
