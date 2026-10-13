using System.Diagnostics;
using CPM.Database;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	public class MigrationHandler
	{
		public async UniTask<MigrateResult> ExecuteAsync(MigrationAnalytic migrationAnalytic, DataBase dataBase)
		{
			migrationAnalytic.LogMigration(MigrationStatus.START);
			Stopwatch migrationStopwatch = Stopwatch.StartNew();
			MigrateResult migrateResult = await new MigrateOldAnon(dataBase).Migrate();
			migrationStopwatch.Stop();
			int num = (int)migrationStopwatch.ElapsedMilliseconds;
			if (migrateResult.MigrationMessageType == MigrationMessageType.NetworkError)
			{
				migrationAnalytic.LogMigration(MigrationStatus.UNEXPECTED, migrateResult.Message, num);
			}
			else
			{
				HandleMigrationResult(migrateResult, num, migrationAnalytic);
			}
			return migrateResult;
		}

		private void HandleMigrationResult(MigrateResult migrateResult, int migrationDuration, MigrationAnalytic migrationAnalytic)
		{
			if (migrateResult.IsCorrectAnswer)
			{
				migrationAnalytic.LogMigration(migrateResult.IsMigrated ? MigrationStatus.COMPLETED : MigrationStatus.REJECT, migrateResult.Message, migrationDuration);
			}
			else
			{
				migrationAnalytic.LogMigration(MigrationStatus.UNEXPECTED, migrateResult.Message, migrationDuration);
			}
		}
	}
}
