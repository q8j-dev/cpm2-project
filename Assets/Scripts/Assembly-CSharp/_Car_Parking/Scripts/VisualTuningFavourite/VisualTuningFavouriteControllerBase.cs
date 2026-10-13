using System;
using UnityEngine;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	public abstract class VisualTuningFavouriteControllerBase : MonoBehaviour
	{
		private int _currenFavouriteCount;

		public Action<bool> OnFavouriteSwitchPanelEnabled;

		public int SelectedItemId { get; private set; }

		public int CurrentFavouriteCount
		{
			get
			{
				return _currenFavouriteCount;
			}
			set
			{
				_currenFavouriteCount = Mathf.Clamp(value, 0, 10);
				OnFavouriteCountChanged?.Invoke(_currenFavouriteCount);
			}
		}

		public event Action OnResetToAllView;

		public event Action<int> OnFavouriteCountChanged;

		public event Action<bool> OnItemSelected;

		protected abstract string GetFavouriteKey(int id);

		protected abstract void UpdateFavourite();

		public abstract void SwitchViewContainers(bool showAllItems);

		public bool AddRemoveItemToFavourite(bool addToFavourite)
		{
			bool result = false;
			if (addToFavourite)
			{
				if (CurrentFavouriteCount < 10)
				{
					PlayerPrefs.SetInt(GetFavouriteKey(SelectedItemId), 1);
					CurrentFavouriteCount++;
					result = true;
				}
				else
				{
					V.CoreGameManager.SetMessage(new LocalizedString("ExteriorFavouriteController.FavouriteIsFull"), 1f);
				}
			}
			else
			{
				PlayerPrefs.SetInt(GetFavouriteKey(SelectedItemId), 0);
				CurrentFavouriteCount--;
			}
			UpdateFavourite();
			return result;
		}

		public void OnSelectItem(int id)
		{
			SelectedItemId = id;
			bool obj = IsItemFavourite(SelectedItemId);
			OnItemSelected?.Invoke(obj);
		}

		public void ResetToAllView()
		{
			OnResetToAllView?.Invoke();
		}

		public bool IsItemFavourite(int itemId)
		{
			return PlayerPrefs.GetInt(GetFavouriteKey(itemId), 0) == 1;
		}
	}
}
