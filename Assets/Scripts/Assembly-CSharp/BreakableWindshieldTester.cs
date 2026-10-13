using UnityEngine;
using UnityEngine.VFX;

public class BreakableWindshieldTester : MonoBehaviour
{
	public Material GlassMaterial;

	public VisualEffect vfx;

	public void Break()
	{
		GlassMaterial.SetFloat("_Shattered", 1f);
		GlassMaterial.SetFloat("_ShatterHole", 1f);
		vfx.SendEvent("Break");
	}

	public void Reset()
	{
		GlassMaterial.SetFloat("_Shattered", 0f);
		GlassMaterial.SetFloat("_ShatterHole", 0f);
	}
}
