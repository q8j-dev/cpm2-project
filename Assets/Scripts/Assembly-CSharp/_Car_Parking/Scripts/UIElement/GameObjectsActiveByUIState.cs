namespace _Car_Parking.Scripts.UIElement
{
	public class GameObjectsActiveByUIState : GameObjectsActiveByEnum<GameObjectsActiveByUIState.StateType>
	{
		public enum StateType
		{
			Normal = 0,
			Selected = 1
		}
	}
}
