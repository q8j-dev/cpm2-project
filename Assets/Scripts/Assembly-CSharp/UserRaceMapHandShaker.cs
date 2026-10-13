using UnityEngine;
using _Car_Parking.Scripts;

public class UserRaceMapHandShaker : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerViaName.CarLayer && (bool)other.transform.parent && other.transform.parent.TryGetComponent<LapRaceCar>(out var component) && component.IsUser())
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
