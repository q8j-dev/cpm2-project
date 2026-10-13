using System;
using UnityEngine;

namespace Traffic
{
	[Serializable]
	public struct ScenePoints
	{
		public int sceneIndex;

		public Vector3[] pointPositions;

		public ScenePoints(int sceneIndex = 0)
		{
			this.sceneIndex = sceneIndex;
			pointPositions = Array.Empty<Vector3>();
		}
	}
}
