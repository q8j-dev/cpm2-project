using System.Collections.Generic;
using System.Linq;
using CPM.PlayerData;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace Car_Parking.Scripts.Racing
{
	public class UITimeBannerController : MonoBehaviour
	{
		[SerializeField]
		private int BannerIndex;

		[SerializeField]
		private TextMeshProUGUI TitleText;

		[SerializeField]
		private TimeBannerUIElement UserTimeBannerUIElement;

		[SerializeField]
		private List<TimeBannerUIElement> TimeBannerUIElement = new List<TimeBannerUIElement>();

		[SerializeField]
		private TimeBannerUIElement[] MvpPanels;

		private BannerData[] _bannerDatas;

		private IPrefabFactory _prefabFactory;

		private MultiPersonControl _cachedUserMultiPersonControl;

		private OptionalDependency<DragRacingController> _dragRacingController;

		[Inject]
		public void Construct(OnlineCollectionsProvider onlineCollectionsProvider, PlayerDataStorage playerDataStorage, FreeDriveProvider freeDriveProvider, OptionalDependency<DragRacingController> dragRacingController)
		{
			_dragRacingController = dragRacingController;
		}

		private void Awake()
		{
			_bannerDatas = new BannerData[TimeBannerUIElement.Count];
			GlobalEventSystem.OnTimeAttackDataUpdate += OnTimeAttackDataUpdate;
		}

		private void OnTimeAttackDataUpdate(TimeAttackContentData timeAttackContentData)
		{
			bool flag = timeAttackContentData.BannerDataType == BannerDataType.All || timeAttackContentData.BannerDataType == BannerDataType.HorizontalBanner;
			if (timeAttackContentData.LeaderBoard != null && flag && BannerIndex == timeAttackContentData.Index)
			{
				FillTable(timeAttackContentData.LeaderBoard, timeAttackContentData.LoseUser);
			}
		}

		public void DisableElements()
		{
			for (int i = 0; i < TimeBannerUIElement.Count; i++)
			{
				TimeBannerUIElement[i].gameObject.SetActive(value: false);
			}
			UserTimeBannerUIElement.gameObject.SetActive(value: false);
			for (int j = 0; j < MvpPanels.Length; j++)
			{
				MvpPanels[j].RemoveUser();
			}
		}

		public void FillTable(List<BannerData> newData, BannerData userOutsideData)
		{
			DisableElements();
			int num = Mathf.Min(newData.Count, TimeBannerUIElement.Count);
			for (int i = 0; i < num; i++)
			{
				if (TimeBannerUIElement[i] != null)
				{
					TimeBannerUIElement[i].Init(newData[i]);
				}
			}
			if (userOutsideData != null)
			{
				UserTimeBannerUIElement?.Init(userOutsideData);
			}
			if (newData.Count > 0)
			{
				PrepareMVPUsers(newData.First());
			}
		}

		public void FillDragRacingTable(List<MainCarValues> cars, DragRacingController.RunDistance distance)
		{
			for (int i = 0; i < cars.Count; i++)
			{
				MainCarValues mainCarValues = cars[i];
				BannerData bannerData = new BannerData
				{
					UserName = mainCarValues.NickName,
					ClassId = mainCarValues.Class,
					IsUser = mainCarValues.IsUser,
					CarId = mainCarValues.CarID,
					Rank = 0,
					Place = i,
					PointValue = _dragRacingController.Value.timeClass,
					TimeAttackMode = TimeAttackMode.Drag
				};
				if (i == 0)
				{
					PrepareMVPUsers(bannerData);
				}
				if (mainCarValues.IsUser && i > 2)
				{
					bannerData.Place = Random.Range(4, 10);
					UserTimeBannerUIElement.Init(bannerData);
				}
				else
				{
					TimeBannerUIElement[i].Init(bannerData);
				}
			}
		}

		private void PrepareMVPUsers(BannerData bannerData)
		{
			for (int i = 0; i < MvpPanels.Length; i++)
			{
				MvpPanels[i].Init(bannerData);
			}
		}

		private void OnDestroy()
		{
			GlobalEventSystem.OnTimeAttackDataUpdate -= OnTimeAttackDataUpdate;
		}
	}
}
