using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LensFlareComponentSRP))]
public class LensFlareSwitch : MonoBehaviour
{
	public LensFlareComponentSRP lenflare;

	public LensFlareDataSRP Original;

	public LensFlareDataSRP Alternative;

	public void ReplaceFlare(bool original = true)
	{
		lenflare.lensFlareData = (original ? Original : Alternative);
	}
}
