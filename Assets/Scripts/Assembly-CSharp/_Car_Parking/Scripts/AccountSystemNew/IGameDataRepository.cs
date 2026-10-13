using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	public interface IGameDataRepository
	{
		UniTask ExecuteAsync(GameDataRepositoryDataType dataType);

		void Clear();
	}
}
