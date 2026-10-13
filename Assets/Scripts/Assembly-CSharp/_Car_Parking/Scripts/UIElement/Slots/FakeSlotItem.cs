using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class FakeSlotItem : ScrollViewItemInteractiveBase
	{
		public override bool CanDrag => false;

		public override bool CanSelect => false;

		public override bool IsSelected => false;
	}
}
