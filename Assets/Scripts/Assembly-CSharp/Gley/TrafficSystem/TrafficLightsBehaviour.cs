using UnityEngine;

namespace Gley.TrafficSystem
{
	public delegate void TrafficLightsBehaviour(TrafficLightsColor currentRoadColor, GameObject[] redLightObjects, GameObject[] yellowLightObjects, GameObject[] greenLightObjects, string name);
}
