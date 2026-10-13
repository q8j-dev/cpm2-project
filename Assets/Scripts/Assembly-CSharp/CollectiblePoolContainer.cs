using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;

public class CollectiblePoolContainer : MonoBehaviour
{
	[SerializeField]
	private _Car_Parking.Scripts.EventSystem.EventType EventType;

	[SerializeField]
	private GameObject CollectiblePoolPrefab;

	[SerializeField]
	private int CollectiblePoolCapacity;

	private List<IMovableCollectible> _collectiblePool = new List<IMovableCollectible>();

	private const int FrameSkipCount = 10;

	private IPrefabFactory _prefabFactory;

	private IEventService _eventService;

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, IEventService eventService)
	{
		_prefabFactory = prefabFactory;
		_eventService = eventService;
	}

	private void Start()
	{
		if (_eventService.GetEventByName(EventType) != null)
		{
			CreateCollectiblePool().Forget();
		}
	}

	private async UniTaskVoid CreateCollectiblePool()
	{
		await AddCollectible(CollectiblePoolCapacity);
	}

	private async UniTask AddCollectible(int needCollectibleCount)
	{
		int num = 0;
		for (int i = 0; i < needCollectibleCount; i++)
		{
			num++;
			GameObject gameObject = _prefabFactory.Instantiate(CollectiblePoolPrefab, base.transform.position, base.transform.rotation, base.transform);
			IMovableCollectible collectible = gameObject.GetComponent<IMovableCollectible>();
			_collectiblePool.Add(collectible);
			if (num == 10)
			{
				await UniTask.Yield();
				num = 0;
			}
			collectible.ChangeState(state: false);
		}
	}

	public async UniTask<List<IMovableCollectible>> GetCollectiblePool(int needCount)
	{
		List<IMovableCollectible> collectiblePool = new List<IMovableCollectible>();
		if (needCount > _collectiblePool.Count)
		{
			await AddCollectible(needCount - _collectiblePool.Count);
		}
		for (int i = 0; i < needCount; i++)
		{
			collectiblePool.Add(_collectiblePool[i]);
		}
		return collectiblePool;
	}
}
