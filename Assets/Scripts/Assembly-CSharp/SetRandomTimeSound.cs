using UnityEngine;

public class SetRandomTimeSound : MonoBehaviour
{
	private void Start()
	{
		AudioSource component = base.gameObject.GetComponent<AudioSource>();
		if ((bool)component)
		{
			float num = Random.Range(0.1f, 0.9f);
			component.time = component.clip.length * num;
		}
	}
}
