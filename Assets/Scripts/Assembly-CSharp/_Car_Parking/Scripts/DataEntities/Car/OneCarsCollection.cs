using System.Collections.Generic;

namespace _Car_Parking.Scripts.DataEntities.Car
{
	public class OneCarsCollection<T>
	{
		public List<T> cars;

		public HashSet<int> migratedCars;
	}
}
