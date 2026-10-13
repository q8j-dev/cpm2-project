using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.PlayerProfile;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	public class EmoteFavoritesService
	{
		private readonly IPlayerProfileService _profile;

		private bool _dirty;

		public IReadOnlyList<int> FavoriteMoves => _profile.FavouriteAnimations;

		public IReadOnlyList<int> FavoriteEmojis => _profile.FavouriteEmojis;

		public event Action Changed;

		[Inject]
		public EmoteFavoritesService(IPlayerProfileService profile)
		{
			_profile = profile;
		}

		public bool IsFavoriteMove(int id)
		{
			return Contains(_profile.FavouriteAnimations, id);
		}

		public bool IsFavoriteEmoji(int id)
		{
			return Contains(_profile.FavouriteEmojis, id);
		}

		public void ToggleMove(int id)
		{
			Toggle(id, toggleMoves: true);
		}

		public void ToggleEmoji(int id)
		{
			Toggle(id, toggleMoves: false);
		}

		public void SaveChangesOnClose()
		{
			if (_dirty)
			{
				_dirty = false;
				PushAsync().Forget();
			}
		}

		private async UniTaskVoid PushAsync()
		{
			if (!(await _profile.PushFavouritesAsync(CancellationToken.None)))
			{
				_dirty = true;
			}
		}

		private void Toggle(int id, bool toggleMoves)
		{
			List<int> list = new List<int>(_profile.FavouriteAnimations);
			List<int> list2 = new List<int>(_profile.FavouriteEmojis);
			List<int> list3 = (toggleMoves ? list : list2);
			if (!list3.Remove(id))
			{
				list3.Add(id);
			}
			if (_profile.SetFavourites(list, list2))
			{
				if (toggleMoves)
				{
					EmoteLockerPrefs.SetPreset(new List<int>(list));
				}
				_dirty = true;
				Changed?.Invoke();
			}
		}

		private static bool Contains(IReadOnlyList<int> list, int id)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == id)
				{
					return true;
				}
			}
			return false;
		}
	}
}
