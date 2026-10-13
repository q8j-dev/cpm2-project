using Cysharp.Threading.Tasks;
using Persistence.Systems;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	public interface IDataVersionsService
	{
		UniTask<DataClassesVersionStorage> GetDataVersions();

		UniTask SaveDataVersions(DataClassesVersionStorage storage);
	}
}
