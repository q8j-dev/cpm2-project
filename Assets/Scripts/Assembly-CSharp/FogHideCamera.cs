using UnityEngine;

public class FogHideCamera : MonoBehaviour
{
	private bool revertFogState;

	private void OnPreRender()
	{
		revertFogState = RenderSettings.fog;
		RenderSettings.fog = false;
	}

	private void OnPostRender()
	{
		RenderSettings.fog = revertFogState;
	}
}
