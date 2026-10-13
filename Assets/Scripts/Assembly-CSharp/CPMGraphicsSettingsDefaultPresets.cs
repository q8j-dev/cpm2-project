using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Graphics Settings Preset", menuName = "CPM/Graphics Settings preset")]
public class CPMGraphicsSettingsDefaultPresets : ScriptableObject
{
	public List<CPMGraphicsSettingsPreset> m_Presets = new List<CPMGraphicsSettingsPreset>();
}
