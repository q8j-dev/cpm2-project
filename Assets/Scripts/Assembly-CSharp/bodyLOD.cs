using UnityEngine;
using UnityEngine.Rendering;

public class bodyLOD : MonoBehaviour
{
	public MeshRenderer mainLODBody;

	public MeshRenderer shadowBody;

	public MeshRenderer windowLODBody;

	private void Awake()
	{
		foreach (Transform item in base.transform)
		{
			MeshRenderer component = item.GetComponent<MeshRenderer>();
			if ((bool)component)
			{
				component.shadowCastingMode = ShadowCastingMode.Off;
			}
		}
	}
}
