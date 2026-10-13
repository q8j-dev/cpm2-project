namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public abstract class PackerBase<T>
	{
		public abstract void Pack(ref long current, T data);

		public abstract T Unpack(ref long current);
	}
}
