using UnityEngine;

public class superMarketBuilding : MonoBehaviour
{
	public Light[] lights;

	private void Start()
	{
		EnableLights(enable: false);
	}

	public void EnableLights(bool enable)
	{
		Light[] array = lights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = enable;
		}
	}
}
