using UnityEngine;

namespace Gley.TrafficSystem
{
	public delegate void ModifyTriggerSize(float currentSpeed, BoxCollider frontCollider, float maxSpeed, float minTriggerLength, float maxTriggerLength);
}
