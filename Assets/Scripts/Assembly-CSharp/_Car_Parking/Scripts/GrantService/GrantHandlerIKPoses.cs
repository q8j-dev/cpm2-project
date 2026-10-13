using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Animations.Emotes;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PlayerProfile;

namespace _Car_Parking.Scripts.GrantService
{
	public sealed class GrantHandlerIKPoses : IGrantHandler
	{
		private readonly IPlayerProfileService _playerProfileService;

		private PoseCatalogSO _poseCatalog;

		public RewardType Type => RewardType.Poses;

		[Inject]
		public GrantHandlerIKPoses(IPlayerProfileService playerProfileService)
		{
			_playerProfileService = playerProfileService;
		}

		public UniTask<bool> GrantAsync(RewardItemData reward, CancellationToken token)
		{
			int iD = reward.ID;
			PoseCatalogSO poseCatalogSO = ResolvePoseCatalog();
			if (!poseCatalogSO)
			{
				return UniTask.FromResult(value: false);
			}
			List<int> list = poseCatalogSO.CollectIdsForBundle(iD);
			if (list.Count == 0)
			{
				return UniTask.FromResult(value: false);
			}
			return UniTask.FromResult(_playerProfileService.AddOwnedPoses(list));
		}

		private PoseCatalogSO ResolvePoseCatalog()
		{
			if (!_poseCatalog)
			{
				_poseCatalog = Resources.Load<PoseCatalogSO>("Configs/PoseCatalog");
			}
			return _poseCatalog;
		}
	}
}
