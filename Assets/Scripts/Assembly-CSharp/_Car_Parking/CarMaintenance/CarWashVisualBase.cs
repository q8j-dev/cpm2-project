using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.CarMaintenance
{
	public abstract class CarWashVisualBase : MonoBehaviour
	{
		public abstract UniTask PlayAsync(GameObject car, float duration, CancellationToken ct);
	}
}
