using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Service.Taxi
{
	public interface ICollisionDetector
	{
		UniTask<bool> TriggerEnter(Collider other);

		bool TriggerExit(Collider other);
	}
}
