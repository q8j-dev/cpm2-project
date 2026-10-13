using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts
{
	public interface ICheckCarDataBase
	{
		bool Enabled { get; set; }

		UniTask CheckDataBaseAndRestartIfNeedsAsync();
	}
}
