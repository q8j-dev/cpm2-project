using _Car_Parking.Scripts.ToRoad;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public interface IArenaToRoadPolicy
	{
		bool TryResolveToRoad(in ToRoadRequest request, out ToRoadResult result);
	}
}
