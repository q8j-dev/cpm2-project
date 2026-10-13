using Cysharp.Threading.Tasks;
using Service.Abstract;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace Service.Taxi
{
	public class OuterCollisionDetector : CollisionDetector
	{
		private const string FemaleEvent = "Play_taxi_femal";

		private const string MaleEvent = "Play_taxi_male";

		private void OnTriggerEnter(Collider other)
		{
			_ = TriggerEnter(other);
		}

		public override async UniTask<bool> TriggerEnter(Collider other)
		{
			if (!isValid(other))
			{
				return false;
			}
			AudioWrapper.PostEvent(((int)GetComponentInParent<MainPerson>().Gender == 0) ? "Play_taxi_femal" : "Play_taxi_male", base.gameObject);
			Debug.Log("Var");
			GetComponentInParent<ClientController>().ChangeState(CLIENT_STATE.WAIT).Forget();
			Debug.Log("Var4");
			return true;
		}

		private void OnTriggerExit(Collider other)
		{
			TriggerExit(other);
		}

		public override bool TriggerExit(Collider other)
		{
			if (!base.TriggerExit(other))
			{
				return false;
			}
			GetComponentInParent<ClientController>().ChangeState(CLIENT_STATE.NONE).Forget();
			return true;
		}
	}
}
