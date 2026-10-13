using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New Stage Lighting", menuName = "CPM/Stage Lighting preset")]
public class StageLightingPreset : ScriptableObject
{
	public Cubemap Reflection;

	[HideInInspector]
	public SphericalHarmonicsL2 AmbientSH;
}
