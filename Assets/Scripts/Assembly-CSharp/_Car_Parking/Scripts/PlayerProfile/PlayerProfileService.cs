using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Account;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[UnityEngine.Scripting.Preserve]
	public class PlayerProfileService : IPlayerProfileService
	{
		private const string PendingFavouritesPushKey = "profileFavsPendingPush";

		private readonly IPlayerProfileBackend _backend;

		private readonly IPlayerProfileCache _cache;

		private readonly IAccountController _accountController;

		private PlayerProfileData _data;

		private string _cacheProbedUid;

		private static readonly IReadOnlyList<int> EmptyFavourites = Array.Empty<int>();

		public PlayerProfileData Data
		{
			get
			{
				string firebaseUserId = _accountController.FirebaseUserId;
				if (_data != null && _data.Id == firebaseUserId)
				{
					return _data;
				}
				if (string.IsNullOrEmpty(firebaseUserId) || _cacheProbedUid == firebaseUserId)
				{
					return null;
				}
				_cacheProbedUid = firebaseUserId;
				if (_cache.TryLoad(firebaseUserId, out var data))
				{
					_data = data;
					return _data;
				}
				return null;
			}
			private set
			{
				_data = value;
			}
		}

		public bool IsLoaded => Data != null;

		public long CircuitCoinsEarned => (Data?.Progress?.Features?.CircuitRacing?.CurrentCoins).GetValueOrDefault();

		public long CircuitMoneyEarned => (Data?.Progress?.Features?.CircuitRacing?.CurrentMoney).GetValueOrDefault();

		public IReadOnlyList<int> FavouriteAnimations
		{
			get
			{
				IReadOnlyList<int> readOnlyList = Data?.Progress?.FavouriteItems?.FavouriteAnimations;
				return readOnlyList ?? EmptyFavourites;
			}
		}

		public IReadOnlyList<int> FavouriteEmojis
		{
			get
			{
				IReadOnlyList<int> readOnlyList = Data?.Progress?.FavouriteItems?.FavouriteEmojis;
				return readOnlyList ?? EmptyFavourites;
			}
		}

		[Inject]
		public PlayerProfileService(IPlayerProfileBackend backend, IPlayerProfileCache cache, IAccountController accountController)
		{
			_backend = backend;
			_cache = cache;
			_accountController = accountController;
		}

		public async UniTask<bool> RefreshAsync(CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(_accountController.FirebaseUserId))
			{
				return false;
			}
			if (_data != null && _data.Id != _accountController.FirebaseUserId)
			{
				_data = null;
			}
			Result<PlayerProfileData> result = await _backend.GetProfileAsync(cancellationToken);
			PlayerProfileData value = result.Value;
			if (result.IsSuccess && value != null && !string.IsNullOrEmpty(value.Id) && value.Id == _accountController.FirebaseUserId)
			{
				bool num = MergePendingFavourites(value);
				Data = value;
				_cache.Save(value);
				if (num)
				{
					PushFavouritesAsync(CancellationToken.None).Forget();
				}
				return true;
			}
			if (Data == null && _cache.TryLoad(_accountController.FirebaseUserId, out var data))
			{
				Data = data;
			}
			return false;
		}

		public bool AddCircuitCoinsEarned(long coins)
		{
			return AddCircuitEarned(coins, "AddCircuitCoinsEarned", (CircuitRacingProfileData circuit, long amount) =>
			{
				circuit.CurrentCoins += amount;
			});
		}

		public bool AddCircuitMoneyEarned(long money)
		{
			return AddCircuitEarned(money, "AddCircuitMoneyEarned", (CircuitRacingProfileData circuit, long amount) =>
			{
				circuit.CurrentMoney += amount;
			});
		}

		private bool AddCircuitEarned(long amount, string caller, Action<CircuitRacingProfileData, long> apply)
		{
			if (amount <= 0)
			{
				return true;
			}
			PlayerProfileData data = Data;
			if (data == null)
			{
				return false;
			}
			PlayerProfileData playerProfileData = data;
			ProfileProgressData profileProgressData;
			if (playerProfileData.Progress == null)
			{
				profileProgressData = (playerProfileData.Progress = new ProfileProgressData());
			}
			profileProgressData = data.Progress;
			ProfileFeaturesData profileFeaturesData;
			if (profileProgressData.Features == null)
			{
				profileFeaturesData = (profileProgressData.Features = new ProfileFeaturesData());
			}
			profileFeaturesData = data.Progress.Features;
			if (profileFeaturesData.CircuitRacing == null)
			{
				CircuitRacingProfileData circuitRacingProfileData = (profileFeaturesData.CircuitRacing = new CircuitRacingProfileData());
			}
			apply(data.Progress.Features.CircuitRacing, amount);
			_cache.Save(data);
			return true;
		}

		public bool OwnsPose(int ikPoseId)
		{
			return (Data?.Progress?.Items?.Poses)?.Contains(ikPoseId) ?? false;
		}

		public bool AddOwnedPoses(IReadOnlyCollection<int> ikPoseIds)
		{
			if (ikPoseIds == null || ikPoseIds.Count == 0)
			{
				return true;
			}
			PlayerProfileData data = Data;
			if (data == null)
			{
				return false;
			}
			PlayerProfileData playerProfileData = data;
			ProfileProgressData profileProgressData;
			if (playerProfileData.Progress == null)
			{
				profileProgressData = (playerProfileData.Progress = new ProfileProgressData());
			}
			profileProgressData = data.Progress;
			ProfileItemsData profileItemsData;
			if (profileProgressData.Items == null)
			{
				profileItemsData = (profileProgressData.Items = new ProfileItemsData());
			}
			profileItemsData = data.Progress.Items;
			if (profileItemsData.Poses == null)
			{
				List<int> list = (profileItemsData.Poses = new List<int>());
			}
			List<int> poses = data.Progress.Items.Poses;
			bool flag = false;
			foreach (int ikPoseId in ikPoseIds)
			{
				if (!poses.Contains(ikPoseId))
				{
					poses.Add(ikPoseId);
					flag = true;
				}
			}
			if (flag)
			{
				_cache.Save(data);
			}
			return true;
		}

		public void RemoveOwnedPoses(IReadOnlyCollection<int> ikPoseIds)
		{
			if (ikPoseIds == null || ikPoseIds.Count == 0)
			{
				return;
			}
			List<int> list = Data?.Progress?.Items?.Poses;
			if (list == null)
			{
				return;
			}
			bool flag = false;
			foreach (int ikPoseId in ikPoseIds)
			{
				flag |= list.Remove(ikPoseId);
			}
			if (flag)
			{
				_cache.Save(Data);
			}
		}

		public bool OwnsAnimation(int y)
		{
			return (Data?.Progress?.Items?.Animations)?.Contains(y) ?? false;
		}

		public bool AddOwnedAnimations(IReadOnlyCollection<int> ys)
		{
			if (ys == null || ys.Count == 0)
			{
				return true;
			}
			PlayerProfileData data = Data;
			if (data == null)
			{
				return false;
			}
			PlayerProfileData playerProfileData = data;
			ProfileProgressData profileProgressData;
			if (playerProfileData.Progress == null)
			{
				profileProgressData = (playerProfileData.Progress = new ProfileProgressData());
			}
			profileProgressData = data.Progress;
			ProfileItemsData profileItemsData;
			if (profileProgressData.Items == null)
			{
				profileItemsData = (profileProgressData.Items = new ProfileItemsData());
			}
			profileItemsData = data.Progress.Items;
			if (profileItemsData.Animations == null)
			{
				List<int> list = (profileItemsData.Animations = new List<int>());
			}
			List<int> animations = data.Progress.Items.Animations;
			bool flag = false;
			foreach (int y in ys)
			{
				if (!animations.Contains(y))
				{
					animations.Add(y);
					flag = true;
				}
			}
			if (flag)
			{
				_cache.Save(data);
			}
			return true;
		}

		public void RemoveOwnedAnimations(IReadOnlyCollection<int> ys)
		{
			if (ys == null || ys.Count == 0)
			{
				return;
			}
			List<int> list = Data?.Progress?.Items?.Animations;
			if (list == null)
			{
				return;
			}
			bool flag = false;
			foreach (int y in ys)
			{
				flag |= list.Remove(y);
			}
			if (flag)
			{
				_cache.Save(Data);
			}
		}

		private bool MergePendingFavourites(PlayerProfileData profile)
		{
			if (!PlayerPrefs.HasKey("profileFavsPendingPush"))
			{
				return false;
			}
			if (PlayerPrefs.GetString("profileFavsPendingPush", string.Empty) != profile.Id)
			{
				PlayerPrefs.DeleteKey("profileFavsPendingPush");
				PlayerPrefs.Save();
				return false;
			}
			ProfileFavouriteItemsData profileFavouriteItemsData = Data?.Progress?.FavouriteItems;
			if (profileFavouriteItemsData == null)
			{
				return false;
			}
			if (profile.Progress == null)
			{
				ProfileProgressData profileProgressData = (profile.Progress = new ProfileProgressData());
			}
			profile.Progress.FavouriteItems = profileFavouriteItemsData;
			return true;
		}

		public bool SetFavourites(IReadOnlyCollection<int> favouriteAnimations, IReadOnlyCollection<int> favouriteEmojis)
		{
			PlayerProfileData data = Data;
			if (data == null)
			{
				return false;
			}
			PlayerProfileData playerProfileData = data;
			ProfileProgressData profileProgressData;
			if (playerProfileData.Progress == null)
			{
				profileProgressData = (playerProfileData.Progress = new ProfileProgressData());
			}
			profileProgressData = data.Progress;
			if (profileProgressData.FavouriteItems == null)
			{
				ProfileFavouriteItemsData profileFavouriteItemsData = (profileProgressData.FavouriteItems = new ProfileFavouriteItemsData());
			}
			data.Progress.FavouriteItems.FavouriteAnimations = ((favouriteAnimations != null) ? new List<int>(favouriteAnimations) : new List<int>());
			data.Progress.FavouriteItems.FavouriteEmojis = ((favouriteEmojis != null) ? new List<int>(favouriteEmojis) : new List<int>());
			_cache.Save(data);
			PlayerPrefs.SetString("profileFavsPendingPush", data.Id ?? string.Empty);
			PlayerPrefs.Save();
			return true;
		}

		public async UniTask<bool> PushFavouritesAsync(CancellationToken cancellationToken)
		{
			PlayerProfileData data = Data;
			if (data == null)
			{
				return false;
			}
			ProfileFavouriteItemsData favourites = data.Progress?.FavouriteItems ?? new ProfileFavouriteItemsData
			{
				FavouriteAnimations = new List<int>(),
				FavouriteEmojis = new List<int>()
			};
			Result<bool> result = await _backend.SaveFavouritesAsync(favourites, cancellationToken);
			if (result.IsSuccess)
			{
				PlayerPrefs.DeleteKey("profileFavsPendingPush");
				PlayerPrefs.Save();
			}
			return result.IsSuccess;
		}
	}
}
