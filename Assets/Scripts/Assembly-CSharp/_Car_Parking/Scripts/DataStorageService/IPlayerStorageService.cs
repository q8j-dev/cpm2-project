using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.DataStorageService
{
	[Preserve]
	public interface IPlayerStorageService : IAccountDataProvider
	{
		PlayerStorage Data { get; set; }

		void Save();

		void Initialize();
	}
}
