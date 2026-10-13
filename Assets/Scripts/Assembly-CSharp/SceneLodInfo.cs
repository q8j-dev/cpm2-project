using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneLodInfo
{
	public LODGroup LOD;

	public List<SceneLodRendererInfo> Renderers;
}
