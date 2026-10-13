using UnityEngine;

public class TriggerParticleSystem : MonoBehaviour
{
	public ParticleSystem particleSystem;

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Object Entered Trigger: " + other.name);
		if (particleSystem != null)
		{
			particleSystem.Play();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (particleSystem != null)
		{
			particleSystem.Stop();
		}
	}
}
