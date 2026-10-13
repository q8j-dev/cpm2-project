using System.Collections.Generic;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhoneGameActivityCanvasInfo : NavCanvas
	{
		public const string WindowId = "Info";

		[SerializeField]
		private Transform ViewContainer = default;

		[SerializeField]
		private GameActivityDetailView[] ViewPrefabs = default;

		private IPrefabFactory _prefabFactory;

		private readonly Dictionary<GameActivityType, GameActivityDetailView> _views = new Dictionary<GameActivityType, GameActivityDetailView>();

		private GameActivityDetailView _current;

		private GameActivityEntry _entry;

		private ArenaModeRegistry _arenaModeRegistry;

		public override string Id => "Info";

		public override string Title
		{
			get
			{
				if (!_arenaModeRegistry.TryGetActivityTitle(_entry, out var title))
				{
					return base.Title;
				}
				return title;
			}
		}

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, ArenaModeRegistry arenaModeRegistry)
		{
			_prefabFactory = prefabFactory;
			_arenaModeRegistry = arenaModeRegistry;
		}

		public override void Open(object data)
		{
			_entry = (GameActivityEntry)data;
			GameActivityDetailView gameActivityDetailView = Resolve(_entry.ActivityType);
			if ((bool)gameActivityDetailView)
			{
				if ((bool)_current && _current != gameActivityDetailView)
				{
					_current.gameObject.SetActive(value: false);
				}
				_current = gameActivityDetailView;
				gameActivityDetailView.gameObject.SetActive(value: true);
				gameActivityDetailView.Render(_entry, base.Navigator);
			}
		}

		private GameActivityDetailView Resolve(GameActivityType type)
		{
			if (_views.TryGetValue(type, out var value))
			{
				return value;
			}
			GameActivityDetailView gameActivityDetailView = FindPrefab(type);
			if (!gameActivityDetailView)
			{
				Debug.LogError(string.Format("[{0}] No detail view prefab for {1}", "PhoneGameActivityCanvasInfo", type));
				return null;
			}
			value = _prefabFactory.Instantiate(gameActivityDetailView, ViewContainer);
			_views.Add(type, value);
			return value;
		}

		private GameActivityDetailView FindPrefab(GameActivityType type)
		{
			GameActivityDetailView[] viewPrefabs = ViewPrefabs;
			foreach (GameActivityDetailView gameActivityDetailView in viewPrefabs)
			{
				if ((bool)gameActivityDetailView && gameActivityDetailView.Type == type)
				{
					return gameActivityDetailView;
				}
			}
			return null;
		}
	}
}
