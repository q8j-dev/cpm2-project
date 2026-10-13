using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace MVPFree.MapIcons.Presenters
{
	public class ArenaIconClickPresenter : IPresenter
	{
		private readonly MapIconsModel _mapIconsModel;

		private readonly ArenaFlowController _arenaFlow;

		[Inject]
		public ArenaIconClickPresenter(MapIconsModel mapIconsModel, ArenaFlowController arenaFlow)
		{
			_mapIconsModel = mapIconsModel;
			_arenaFlow = arenaFlow;
		}

		public void Subscribe()
		{
			_mapIconsModel.OnMapIconTypeSelectedAction += OnIconSelected;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.OnMapIconTypeSelectedAction -= OnIconSelected;
		}

		private void OnIconSelected(int index, IconMapView iconMapView)
		{
			if ((bool)iconMapView)
			{
				ArenaMapIcons.IsArenaIcon(iconMapView.IconType);
			}
		}
	}
}
