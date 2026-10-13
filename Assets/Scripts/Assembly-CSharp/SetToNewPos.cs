using UnityEngine;

public class SetToNewPos : MonoBehaviour
{
	public bool Set;

	public Vector3 pos;

	public Vector3 euler;

	private void Awake()
	{
		if (Application.loadedLevel == 1)
		{
			base.transform.position = pos;
			base.transform.eulerAngles = euler;
		}
	}
}
