using System;
using UnityEngine;

[Serializable]
public class CPMGraphicsSettingsConfig
{
	public string m_ConfigVersion;

	public int Quality;

	public CPMGraphicsSettingsData m_SettingsData;

	public FullScreenMode FullScreenMode;

	public int[] Resolution;

	public int DisplayIndex;
}
