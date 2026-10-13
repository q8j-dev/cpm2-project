using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Multiplayer.CarLegitService
{
	[Preserve]
	public interface IProgressBackend
	{
		UniTask<Result<IReadOnlyList<bool>>> CheckOtherCarsLegit(IReadOnlyList<(string firebaseId, string carInstanceId, int carId)> requests, CancellationToken cancellationToken = default(CancellationToken));

		UniTask<Result<bool>> CheckOtherCarLegit(string firebaseId, string carInstanceId, int carId, CancellationToken cancellationToken = default(CancellationToken));

		UniTask<Result<bool>> RegisterCar(string carInstanceId, int carId, CancellationToken cancellationToken = default(CancellationToken));

		UniTask RegisterCarWithBackoff(string carInstanceId, int carId, CancellationToken cancellationToken = default(CancellationToken));

		UniTask<Result<bool>> RemoveCar(string carInstanceId, CancellationToken cancellationToken = default(CancellationToken));
	}
}
