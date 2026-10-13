using System.Collections.Generic;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	public interface ICPMCrowdManager
	{
		CPMCrowdNpc Spawn(CPMCrowdNpcData data);

		void SpawnAll(IReadOnlyList<CPMCrowdNpcData> datas);

		void Despawn(CPMCrowdNpc npc);

		void DespawnAll();
	}
}
