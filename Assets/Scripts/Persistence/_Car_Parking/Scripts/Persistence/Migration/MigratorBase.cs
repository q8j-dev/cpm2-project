using System;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	public abstract class MigratorBase : IMigrator
	{
		protected string path;

		protected IMigrationManager migrationManager;

		public abstract UniTask<object> Migrate(object data);

		public abstract UniTask<object> Migrate(object data, DataSource dataSource);

		public abstract Type GetFirstType();

		public abstract Type GetSecondType();

		public abstract UniTask Delete(object data);

		public void SetMigrationManager(IMigrationManager migrationManager)
		{
			this.migrationManager = migrationManager;
		}

		public abstract IMigrationDataProvider GetMigrationProvider();

		public abstract UniTask Commit(object data, bool external = false);

		public void SetPath(string path)
		{
			this.path = path;
		}
	}
}
