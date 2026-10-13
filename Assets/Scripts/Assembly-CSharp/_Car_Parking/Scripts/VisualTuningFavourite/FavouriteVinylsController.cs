using CPM.Scripts.VinylDrawer;
using UnityEngine;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	public class FavouriteVinylsController : VisualTuningFavouriteControllerBase
	{
		[SerializeField]
		private VinylsEditor VinylsEditor;

		private bool _isShowAll;

		public bool IsShowAll => _isShowAll;

		private void Start()
		{
			_isShowAll = true;
			base.CurrentFavouriteCount = Extensions.GetFavouriteVinyls(V.AllVinyls).Count;
		}

		protected override string GetFavouriteKey(int id)
		{
			return "FavouriteVinyl" + id;
		}

		protected override void UpdateFavourite()
		{
			VinylsEditor.UpdateFavouriteVinyls();
		}

		public override void SwitchViewContainers(bool showAllItems)
		{
			_isShowAll = showAllItems;
			if (showAllItems)
			{
				VinylsEditor.ShowAllVinyls();
			}
			else
			{
				VinylsEditor.ShowFavouriteVinyls();
			}
		}
	}
}
