using System.Collections.Generic;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	public interface IRallyPropManager
	{
		PooledProp Spawn(RallyPropData data);

		void SpawnAll(IReadOnlyList<RallyPropData> datas);

		void Despawn(PooledProp prop);

		void DespawnAll();
	}
}
