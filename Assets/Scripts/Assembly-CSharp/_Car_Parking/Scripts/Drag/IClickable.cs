using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag
{
	public interface IClickable
	{
		void OnItemClick(PartData parData, int index = 0, UIElement uiElement = null);

		void CloseClickable();
	}
}
