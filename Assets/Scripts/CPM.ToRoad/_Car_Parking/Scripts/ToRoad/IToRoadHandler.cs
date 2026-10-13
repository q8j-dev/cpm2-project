namespace _Car_Parking.Scripts.ToRoad
{
	public interface IToRoadHandler
	{
		ToRoadPriority Priority { get; }

		bool TryResolve(in ToRoadRequest request, out ToRoadResult result);
	}
}
