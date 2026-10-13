using UnityEngine;

public class disablePartsCheckFPS : MonoBehaviour
{
	public Transform rails;

	public Transform VillageRight;

	public Transform ViillageLeft;

	public Transform villageFenceR;

	public Transform villageFenceL;

	private void Start()
	{
	}

	private void OnGUI()
	{
		int num = 1;
		if (GUI.Button(new Rect(400f, 70 * num, 100f, 30f), "Rails"))
		{
			rails.gameObject.SetActive(!rails.gameObject.active);
		}
		num++;
		if (GUI.Button(new Rect(400f, 70 * num, 100f, 30f), "VillageRight"))
		{
			VillageRight.gameObject.SetActive(!VillageRight.gameObject.active);
		}
		num++;
		if (GUI.Button(new Rect(400f, 70 * num, 100f, 30f), "ViillageLeft"))
		{
			ViillageLeft.gameObject.SetActive(!ViillageLeft.gameObject.active);
		}
		num++;
		if (GUI.Button(new Rect(400f, 70 * num, 100f, 30f), "villageFenceR"))
		{
			villageFenceR.gameObject.SetActive(!villageFenceR.gameObject.active);
		}
		num++;
		if (GUI.Button(new Rect(400f, 70 * num, 100f, 30f), "villageFenceL"))
		{
			villageFenceL.gameObject.SetActive(!villageFenceL.gameObject.active);
		}
	}
}
