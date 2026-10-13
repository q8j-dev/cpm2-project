using UnityEngine;

public class disableEnable : MonoBehaviour
{
	public GameObject[] obj;

	public GameObject ped;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void enableDis(bool enable)
	{
		GameObject[] array = obj;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(enable);
			}
		}
	}
}
