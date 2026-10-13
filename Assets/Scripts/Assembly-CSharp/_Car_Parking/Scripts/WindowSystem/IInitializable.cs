namespace _Car_Parking.Scripts.WindowSystem
{
	public interface IInitializable<in T>
	{
		void Init(T data);
	}
}
