using Service.Abstract;
using UnityEngine;

namespace Service
{
	public class JointHandler : Service.Abstract.JointHandler
	{
		private Vector3? rearTrigger;

		private Vector3? truckTrigger;

		private void OnJointBreak(float breakForce)
		{
			ClientController clientController = null;
			if (TryGetComponent<ConfigurableJoint>(out var component) && component.connectedBody != null)
			{
				clientController = component.connectedBody.GetComponent<ClientController>();
			}
			if (!(clientController == null))
			{
				Object.Destroy(GetComponent<ConfigurableJoint>());
				clientController.Detach("Trailer is detached due to collision", showMessage: true);
			}
		}

		public void CreateTrigger(TYPE type)
		{
			switch (type)
			{
			case TYPE.DELIVERY:
				base.CreateTrigger();
				trigger.transform.localPosition = -Vector3.forward * 5f;
				rearTrigger = collider.ClosestPoint(trigger.transform.position);
				trigger.transform.position = rearTrigger ?? Vector3.zero;
				break;
			case TYPE.CARGO:
				if (GetComponent<VehicleData>().isTruck)
				{
					base.CreateTrigger();
					truckTrigger = GetComponent<VehicleData>().jointPosition;
					trigger.transform.localPosition = (truckTrigger ?? Vector3.zero) - Vector3.up * 0.2f;
				}
				break;
			}
		}
	}
}
