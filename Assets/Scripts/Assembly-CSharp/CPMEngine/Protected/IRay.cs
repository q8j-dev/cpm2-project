using UnityEngine;

namespace CPMEngine.Protected
{
	public interface IRay
	{
		bool GetRay(Vector3 p1, Vector3 p2, out RayInfo rayInfo);

		void Destroy();
	}
}
