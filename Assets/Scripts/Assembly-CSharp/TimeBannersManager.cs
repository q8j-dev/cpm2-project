using CPM.PlayerData;
using Car_Parking.Scripts.Racing;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class TimeBannersManager : MonoBehaviour
{
	[SerializeField]
	private TimeBannerController[] TimeBannerControllers;

	private PlayerDataStorage _playerDataStorage;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private FreeDriveProvider _freeDriveProvider;

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveProvider freeDriveProvider)
	{
		_playerDataStorage = playerDataStorage;
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_freeDriveProvider = freeDriveProvider;
	}

	private void Awake()
	{
		for (int i = 0; i < TimeBannerControllers.Length; i++)
		{
			TimeBannerControllers[i].SetReferences(_playerDataStorage, _onlineCollectionsProvider, _freeDriveProvider);
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < TimeBannerControllers.Length; i++)
		{
			TimeBannerControllers[i].EnableBannersAsync().Forget();
		}
	}

	public void UpdateTablos(V.placeToSpawn placeToSpawn)
	{
		for (int i = 0; i < TimeBannerControllers.Length; i++)
		{
			if (TimeBannerControllers[i].PlaceToSpawn == placeToSpawn)
			{
				TimeBannerControllers[i].UpdateContentData(BannerDataType.All);
			}
		}
	}

	public TimeBannerController GetTimeBannersManager(int index)
	{
		return TimeBannerControllers[index];
	}
}
