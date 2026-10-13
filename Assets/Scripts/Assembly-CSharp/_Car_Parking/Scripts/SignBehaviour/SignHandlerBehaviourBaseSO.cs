using UnityEngine;

namespace _Car_Parking.Scripts.SignBehaviour
{
	public abstract class SignHandlerBehaviourBaseSO : ScriptableObject
	{
		public abstract void Execute(SignHandler signHandler);

		public abstract void OnCollisionEnterExecute(SignHandler signHandler, Collision other);
	}
}
