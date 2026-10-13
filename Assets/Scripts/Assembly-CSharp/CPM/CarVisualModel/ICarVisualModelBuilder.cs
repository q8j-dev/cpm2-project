using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;

namespace CPM.CarVisualModel
{
	public interface ICarVisualModelBuilder
	{
		UniTask<CarVisualModelHandle> BuildAsync(int carId, CancellationToken token);

		UniTask<CarVisualModelHandle> BuildAsync(CarInfo carInfo, CancellationToken token);
	}
}
