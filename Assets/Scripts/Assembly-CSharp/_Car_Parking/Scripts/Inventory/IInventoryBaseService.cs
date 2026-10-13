namespace _Car_Parking.Scripts.Inventory
{
	public interface IInventoryBaseService<TInventoryItem>
	{
		void Add(TInventoryItem item);

		void Remove(TInventoryItem item);

		void Clear();

		void Save();
	}
}
