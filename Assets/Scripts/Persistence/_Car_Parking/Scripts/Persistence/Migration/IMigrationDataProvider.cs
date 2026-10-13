using System.Collections.Generic;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	public interface IMigrationDataProvider
	{
		List<object> GetMigrationInputData();
	}
}
