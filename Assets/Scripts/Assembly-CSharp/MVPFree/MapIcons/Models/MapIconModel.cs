using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Views;

namespace MVPFree.MapIcons.Models
{
	public class MapIconModel
	{
		public IconMapView IconMapView;

		public IconType IconType;

		public int Id;

		public MapIconModel(IconMapView iconMapView, IconType iconType, int id)
		{
			IconMapView = iconMapView;
			IconType = iconType;
			Id = id;
		}
	}
}
