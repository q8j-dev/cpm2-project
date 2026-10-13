using UnityEngine;

namespace Obi
{
	[RequireComponent(typeof(ObiActor))]
	public class ColorFromPhase : MonoBehaviour
	{
		private ObiActor actor;

		private void Awake()
		{
			actor = GetComponent<ObiActor>();
		}

		private void LateUpdate()
		{
			if (base.isActiveAndEnabled && !(actor.solver == null))
			{
				for (int i = 0; i < actor.solverIndices.Length; i++)
				{
					int index = actor.solverIndices[i];
					int groupFromPhase = ObiUtils.GetGroupFromPhase(actor.solver.phases[index]);
					actor.solver.colors[index] = ObiUtils.colorAlphabet[groupFromPhase % ObiUtils.colorAlphabet.Length];
				}
			}
		}
	}
}
