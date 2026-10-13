using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.ArtistTools.Aspects;
using _Car_Parking.Scripts.ArtistTools.BakedData;
using _Car_Parking.Scripts.ArtistTools.Trackables;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.ArtistTools
{
	public class TrackablesRegisterTool : MonoBehaviour
	{
		[SerializeField]
		private List<TrackableWithAspects> Trackables = new List<TrackableWithAspects>();

		public Dictionary<TextAsset, List<BakedDirectionPoint>> CachedBakedPoints = new Dictionary<TextAsset, List<BakedDirectionPoint>>();

		private GlobalCollectionsProvider _provider;

		[Inject]
		public void Construct(GlobalCollectionsProvider provider)
		{
			_provider = provider;
		}

		private void Start()
		{
			StartAsync().Forget();
		}

		private async UniTask StartAsync()
		{
			await UniTask.Yield();
			await UniTask.Yield();
			if (_provider == null || Trackables.IsNullOrEmpty())
			{
				return;
			}
			_ = DateTime.Now;
			foreach (TrackableWithAspects trackable in Trackables)
			{
				if ((bool)trackable)
				{
					_provider?.RegisterTrackable(trackable);
					if (trackable.HasAspect<IRenameAspect>(out var aspect))
					{
						aspect.RenameImpl();
					}
					trackable.OnDestroyEvent += HandleDestroyEvent;
					if (trackable.HasAspect<IDestroyAspect>(out var aspect2))
					{
						trackable.OnDestroyEvent += aspect2.OnDestroyImpl;
					}
				}
			}
			foreach (TrackableWithAspects trackable2 in Trackables)
			{
				if ((bool)trackable2 && trackable2.HasAspect<IStartAspect>(out var aspect3))
				{
					await aspect3.StartImpl(this);
				}
			}
			CachedBakedPoints.Clear();
			CachedBakedPoints = null;
			Trackables?.Clear();
			Trackables = null;
		}

		private void HandleDestroyEvent(TrackableWithAspects obj)
		{
			if ((bool)obj)
			{
				if (obj.HasAspect<IDestroyAspect>(out var aspect))
				{
					obj.OnDestroyEvent -= aspect.OnDestroyImpl;
				}
				obj.OnDestroyEvent -= HandleDestroyEvent;
				_provider?.UnregisterTrackable(obj);
			}
		}
	}
}
