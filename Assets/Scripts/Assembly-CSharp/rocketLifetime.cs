using UnityEngine;

public class rocketLifetime : MonoBehaviour
{
	private float lifeTime;

	private void Update()
	{
		lifeTime += Time.deltaTime;
		if (lifeTime > 0.75f)
		{
			Object.DestroyObject(base.gameObject);
		}
	}
}
