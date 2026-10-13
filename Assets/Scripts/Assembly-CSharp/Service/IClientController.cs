using UnityEngine;

namespace Service
{
	public interface IClientController
	{
		void ChangeState(CLIENT_STATE state);

		void SetState(CLIENT_STATE state);

		void Set(GameObject owner, GameObject car);
	}
}
