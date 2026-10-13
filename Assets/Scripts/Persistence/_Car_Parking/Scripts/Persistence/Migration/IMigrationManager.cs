using System;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	public interface IMigrationManager
	{
		UniTask<object> MigrateData(Type from, Type to, Action<float> progressCallback, bool commit = false, bool deleteOld = false);

		UniTask<object> MigrateData(object data, Type from, Type to, object inputData = null, bool commit = false, bool deleteOld = false);

		void Initialize();
	}
}
