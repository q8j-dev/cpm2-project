using System.Collections.Generic;
using UnityEngine;

public class loadOffroadParts : MonoBehaviour
{
	public int partNumberNow;

	public int maxPart = 24;

	public List<GameObject> allOffroadParts;

	public GameObject Parent;

	public bool stop;

	private void Start()
	{
	}

	private void OnDestroy()
	{
		DestroyAllParts();
	}

	public void DestroyAllParts()
	{
		stop = true;
		foreach (GameObject allOffroadPart in allOffroadParts)
		{
			if ((bool)allOffroadPart)
			{
				Object.Destroy(allOffroadPart);
			}
		}
	}
}
