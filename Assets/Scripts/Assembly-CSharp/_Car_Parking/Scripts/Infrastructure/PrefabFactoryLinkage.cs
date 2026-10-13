using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	public class PrefabFactoryLinkage
	{
		public PrefabFactoryLinkage(PlayerDatas playerDatas, IPrefabFactory prefabFactory)
		{
			playerDatas.SetPrefabFactory(prefabFactory);
		}
	}
}
