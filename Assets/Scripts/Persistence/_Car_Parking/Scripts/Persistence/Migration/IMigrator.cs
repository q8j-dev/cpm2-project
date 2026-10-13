using System;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	public interface IMigrator
	{
		UniTask<object> Migrate(object data);

		UniTask<object> Migrate(object data, DataSource dataSource);

		Type GetFirstType();

		Type GetSecondType();

		UniTask Commit(object data, bool external = false);

		UniTask Delete(object data);

		void SetMigrationManager(IMigrationManager migrationManager);

		IMigrationDataProvider GetMigrationProvider();

		void SetPath(string path);
	}
}
