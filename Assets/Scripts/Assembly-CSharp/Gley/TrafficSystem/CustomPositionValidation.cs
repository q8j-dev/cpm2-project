using UnityEngine;

namespace Gley.TrafficSystem
{
	public delegate bool CustomPositionValidation(Vector3 vehiclePosition, float vehicleLength, float vehicleHeight, float vehicleWidth, Quaternion vehicleRotation);
}
