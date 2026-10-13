using System;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	[Preserve]
	public class DataVersionsService : IDataVersionsService
	{
		public const string GetDataVersionsFn = "GetDataVersions";

		public const string SaveDataVersionsFn = "SaveDataVersions";

		public async UniTask<DataClassesVersionStorage> GetDataVersions()
		{
			throw new NotImplementedException();
		}

		public async UniTask SaveDataVersions(DataClassesVersionStorage storage)
		{
			throw new NotImplementedException();
		}
	}
}
