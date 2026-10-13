using Service.Abstract;
using UnityEngine;

namespace Service.Delivery
{
	public class JointHandler : Service.Abstract.JointHandler
	{
		private void OnJointBreak(float breakForce)
		{
			TrailerController component = GetComponent<ConfigurableJoint>().connectedBody.GetComponent<TrailerController>();
			Object.Destroy(GetComponent<ConfigurableJoint>());
			component.Detach("Trailer is detached due to collision", showMessage: true);
		}
	}
}
