using System.Collections.Generic;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhoneGameActivityCanvasList : NavCanvas
	{
		public const string WindowId = "List";

		[SerializeField]
		private ActivityListItem ItemPrefab;

		[SerializeField]
		private Transform Content;

		private IReadOnlyList<GameActivityEntry> _entries;

		private IPrefabFactory _prefabFactory;

		public override string Id => "List";

		public override string Title => "No title";

		[Inject]
		public void Construct(IPrefabFactory prefabFactory)
		{
			_prefabFactory = prefabFactory;
		}

		public override void Open(object data)
		{
			_entries = (IReadOnlyList<GameActivityEntry>)data;
			Content.RemoveAllChild();
			foreach (GameActivityEntry entry in _entries)
			{
				ActivityListItem activityListItem = _prefabFactory.Instantiate(ItemPrefab, Content);
				activityListItem.OnButtonClick += OnEntryClicked;
				activityListItem.Render(entry);
			}
		}

		private void OnEntryClicked(GameActivityEntry entry)
		{
			base.Navigator.OpenWindow("Info", entry);
		}
	}
}
