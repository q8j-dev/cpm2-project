using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkidmarkColorConfig", menuName = "Scriptable/SkidmarkColorConfig", order = 11)]
public class SkidmarkColorConfigSO : ScriptableObject
{
	[SerializeField]
	private List<SkidmarkSurfaceColor> _entries = new List<SkidmarkSurfaceColor>();

	public IReadOnlyList<SkidmarkSurfaceColor> Entries => _entries;
}
