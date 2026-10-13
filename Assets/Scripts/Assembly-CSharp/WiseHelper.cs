using UnityEngine;

public class WiseHelper
{
	public static void SetAudioWiseComponents(GameObject go, bool addColliderTrigger)
	{
		if (addColliderTrigger)
		{
			if (!go.TryGetComponent<SphereCollider>(out var component))
			{
				component = go.AddComponent<SphereCollider>();
			}
			component.isTrigger = true;
		}
		if (!go.GetComponent<AkGameObj>())
		{
			go.AddComponent<AkGameObj>();
		}
	}

	public static int SoundIdForNpc()
	{
		return Random.Range(1, 3);
	}
}
