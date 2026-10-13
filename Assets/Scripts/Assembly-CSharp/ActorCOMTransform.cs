using Obi;
using UnityEngine;

public class ActorCOMTransform : MonoBehaviour
{
	public Vector3 offset;

	public ObiActor actor;

	public void Update()
	{
		if (actor != null && actor.isLoaded)
		{
			actor.GetMass(out var com);
			base.transform.position = actor.solver.transform.TransformPoint(com) + offset;
		}
	}
}
