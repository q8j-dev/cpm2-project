using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PlayerProfile;

namespace _Car_Parking.Scripts.GrantService
{
	public sealed class GrantHandlerAnimations : IGrantHandler
	{
		private readonly IPlayerProfileService _playerProfileService;

		private AnimationDataSO _animationData;

		public RewardType Type => RewardType.Animations;

		[Inject]
		public GrantHandlerAnimations(IPlayerProfileService playerProfileService)
		{
			_playerProfileService = playerProfileService;
		}

		public UniTask<bool> GrantAsync(RewardItemData reward, CancellationToken token)
		{
			int iD = reward.ID;
			AnimationDataSO animationDataSO = ResolveAnimationData();
			if (!animationDataSO)
			{
				return UniTask.FromResult(value: false);
			}
			List<int> list = animationDataSO.CollectYsForBundle(iD);
			if (list.Count == 0)
			{
				return UniTask.FromResult(value: false);
			}
			return UniTask.FromResult(_playerProfileService.AddOwnedAnimations(list));
		}

		private AnimationDataSO ResolveAnimationData()
		{
			if (!_animationData)
			{
				_animationData = Resources.Load<AnimationDataSO>("Configs/AnimationData");
			}
			return _animationData;
		}
	}
}
