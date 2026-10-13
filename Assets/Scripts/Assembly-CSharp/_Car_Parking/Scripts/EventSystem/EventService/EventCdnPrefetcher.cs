using System;
using System.Collections.Generic;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.CDN;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.EventSystem.EventService
{
	[Preserve]
	public class EventCdnPrefetcher : IInitializable, IDisposable
	{
		private readonly IEventService _eventService;

		private readonly ICDNDownloader _cdnDownloader;

		private readonly PlayerDataStorage _playerDataStorage;

		private readonly List<Event> _events = new List<Event>();

		private bool _isCdnProcess;

		private bool _disposed;

		private int _generation;

		public EventCdnPrefetcher(IEventService eventService, ICDNDownloader cdnDownloader, PlayerDataStorage playerDataStorage)
		{
			_eventService = eventService;
			_cdnDownloader = cdnDownloader;
			_playerDataStorage = playerDataStorage;
		}

		public void Initialize()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			_eventService.OnEventsRetrieved += OnEventsRetrieved;
			_playerDataStorage.OnAccountIdUpdatedEvent += OnAccountIdUpdated;
			if (_eventService.CurrentEvents.Count > 0)
			{
				OnEventsRetrieved();
			}
		}

		public void Dispose()
		{
			_disposed = true;
			_generation++;
			SceneManager.sceneLoaded -= OnSceneLoaded;
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
			_eventService.OnEventsRetrieved -= OnEventsRetrieved;
			_playerDataStorage.OnAccountIdUpdatedEvent -= OnAccountIdUpdated;
			ClearCachedImages();
			_events.Clear();
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			UpdateCdn().Forget();
		}

		private void OnSceneUnloaded(Scene scene)
		{
			UpdateCdn().Forget();
		}

		private void OnEventsRetrieved()
		{
			_events.Clear();
			_events.AddRange(_eventService.CurrentEvents);
			UpdateCdn().Forget();
		}

		private void OnAccountIdUpdated(ObscuredString accountId)
		{
			_generation++;
			ClearCachedImages();
			_events.Clear();
		}

		private async UniTaskVoid UpdateCdn()
		{
			if (_disposed)
			{
				return;
			}
			while (_isCdnProcess)
			{
				await UniTask.Yield();
			}
			if (_disposed || _events.Count == 0)
			{
				return;
			}
			_isCdnProcess = true;
			try
			{
				Event[] snapshot = _events.ToArray();
				int genStart = _generation;
				if (ScenesHelper.IsMenuSceneActive())
				{
					await LoadCachedImages(snapshot);
					if (_disposed || _generation != genStart)
					{
						ClearCachedImages(snapshot);
					}
				}
				else
				{
					ClearCachedImages(snapshot);
				}
			}
			finally
			{
				_isCdnProcess = false;
			}
		}

		private async UniTask LoadCachedImages(Event[] snapshot)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Event obj in snapshot)
			{
				if (!string.IsNullOrEmpty(obj?.Images?.SummaryImageUrl))
				{
					hashSet.Add(obj.Images.SummaryImageUrl);
				}
				if (!string.IsNullOrEmpty(obj?.Images?.BigImageUrl))
				{
					hashSet.Add(obj.Images.BigImageUrl);
				}
			}
			if (hashSet.Count == 0)
			{
				return;
			}
			List<UniTask> list = new List<UniTask>(hashSet.Count);
			foreach (string item in hashSet)
			{
				list.Add(_cdnDownloader.LoadImageAsync(item));
			}
			await UniTask.WhenAll(list);
		}

		private void ClearCachedImages()
		{
			ClearCachedImages(_events.ToArray());
		}

		private void ClearCachedImages(Event[] snapshot)
		{
			foreach (Event obj in snapshot)
			{
				_cdnDownloader.ClearLocalCache(obj?.Images?.SummaryImageUrl);
				_cdnDownloader.ClearLocalCache(obj?.Images?.BigImageUrl);
			}
		}
	}
}
