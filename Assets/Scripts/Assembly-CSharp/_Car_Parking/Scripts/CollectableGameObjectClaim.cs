using UnityEngine;

namespace _Car_Parking.Scripts
{
	public abstract class CollectableGameObjectClaim : MonoBehaviour
	{
		public abstract void Claim(CollectableGameObject collectableGameObject);
	}
}
