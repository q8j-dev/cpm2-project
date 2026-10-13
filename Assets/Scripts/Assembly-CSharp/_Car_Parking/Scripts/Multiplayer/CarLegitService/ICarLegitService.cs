using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Multiplayer.CarLegitService
{
	public interface ICarLegitService
	{
		UniTask<bool> IsCarLegitAsync(string firebaseId, string carInstanceId, int carId, CancellationToken ct, bool flushImmediately);

		UniTask<bool> IsCheckEnabledAsync(CancellationToken ct);
	}
}
