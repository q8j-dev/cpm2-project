using UnityEngine;
using _Car_Parking.Scripts;

public class PenaltyFreeZoneTrigger : MonoBehaviour
{
	[SerializeField]
	private float TriggerCoolDown = 3f;

	private float _lastPassTime;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerViaName.CarLayer && (bool)other.gameObject.transform.parent && other.gameObject.transform.parent.TryGetComponent<LapRaceCar>(out var component) && !(Time.time - _lastPassTime <= TriggerCoolDown))
		{
			_lastPassTime = Time.time;
			component.ChangeSplineWidthState();
		}
	}
}
