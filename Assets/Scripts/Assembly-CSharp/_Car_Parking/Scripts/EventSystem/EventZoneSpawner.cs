using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem
{
	public class EventZoneSpawner : MonoBehaviour
	{
		[SerializeField]
		private EventType EvenType = default;

		[SerializeField]
		private GameObject Prefab = default;

		[SerializeField]
		private SceneType SceneType = default;

		[SerializeField]
		private EventZoneCollectiblesPosDatas EventZonesConfig = default;

		private IEventService _eventService;

		private bool _wasSpawn;

		private IPrefabFactory _prefabFactory;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(IEventService eventService, OptionalDependency<IPrefabFactory> prefabFactory)
		{
			_prefabFactory = prefabFactory.Value;
			_eventService = eventService;
		}

		private void OnEnable()
		{
			if (_prefabFactory == null && (bool)V.CoreGameManager && V.CoreGameManager._prefabFactory != null)
			{
				_prefabFactory = V.CoreGameManager._prefabFactory;
			}
			TrySpawn();
		}

		public void TrySpawn()
		{
			if (FlagsStorage.IsLevelScene || _wasSpawn || !Prefab || _prefabFactory == null || _eventService.GetEventByName(EvenType) == null)
			{
				return;
			}
			_wasSpawn = true;
			EventLocationData eventLocationData = EventZonesConfig.CollectibleZones.FirstOrDefault((EventLocationData z) => z.SceneType == SceneType);
			if (eventLocationData == null)
			{
				return;
			}
			foreach (ZoneConfigData collectiblePosition in eventLocationData.CollectiblePositions)
			{
				GameObject obj = _prefabFactory.Instantiate(Prefab, base.transform.position, base.transform.rotation, base.transform);
				obj.transform.SetParent(base.transform, worldPositionStays: false);
				IEventZone[] componentsInChildren = obj.GetComponentsInChildren<IEventZone>();
				for (int num = 0; num < componentsInChildren.Length; num++)
				{
					componentsInChildren[num].Init(EvenType, collectiblePosition.Index, eventLocationData.SceneType);
				}
			}
		}
	}
}
