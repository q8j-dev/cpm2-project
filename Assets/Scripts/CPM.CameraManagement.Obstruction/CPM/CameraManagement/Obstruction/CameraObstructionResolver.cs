using System.Collections.Generic;
using UnityEngine;

namespace CPM.CameraManagement.Obstruction
{
	public static class CameraObstructionResolver
	{
		public const float DefaultSuddenDropThreshold = 1f;

		public const float DefaultMinHeightFraction = 0.3f;

		public static bool AcceptSuddenDrop(float previousDistance, float candidateDistance, float threshold, ref bool dropPending)
		{
			if (candidateDistance >= previousDistance - threshold)
			{
				dropPending = false;
				return true;
			}
			if (!dropPending)
			{
				dropPending = true;
				return false;
			}
			dropPending = false;
			return true;
		}

		public static float ScaleObstructedHeight(float heightOffset, float obstructedDistance, float freeDistance, float minFraction)
		{
			if (freeDistance <= 0.0001f)
			{
				return heightOffset;
			}
			float a = Mathf.Clamp01(obstructedDistance / freeDistance);
			return heightOffset * Mathf.Max(a, Mathf.Clamp01(minFraction));
		}

		public static float? ResolveDistance(IReadOnlyList<ObstructionCandidate> candidates, float margin, float minDistance)
		{
			float? num = null;
			for (int i = 0; i < candidates.Count; i++)
			{
				ObstructionCandidate obstructionCandidate = candidates[i];
				if (!obstructionCandidate.IsIgnored && !(obstructionCandidate.Distance <= 0f) && (!num.HasValue || obstructionCandidate.Distance < num.Value))
				{
					num = obstructionCandidate.Distance;
				}
			}
			if (!num.HasValue)
			{
				return null;
			}
			return Mathf.Max(num.Value - margin, minDistance);
		}
	}
}
