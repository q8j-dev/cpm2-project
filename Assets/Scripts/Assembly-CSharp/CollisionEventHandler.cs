using Obi;
using UnityEngine;

[RequireComponent(typeof(ObiSolver))]
public class CollisionEventHandler : MonoBehaviour
{
	private ObiSolver solver;

	public int contactCount;

	private ObiSolver.ObiCollisionEventArgs frame;

	private void Awake()
	{
		solver = GetComponent<ObiSolver>();
	}

	private void OnEnable()
	{
		solver.OnParticleCollision += Solver_OnCollision;
	}

	private void OnDisable()
	{
		solver.OnParticleCollision -= Solver_OnCollision;
	}

	private void Solver_OnCollision(object sender, ObiSolver.ObiCollisionEventArgs e)
	{
		frame = e;
	}

	private void OnDrawGizmos()
	{
		if (solver == null || frame == null || frame.contacts == null)
		{
			return;
		}
		Gizmos.matrix = solver.transform.localToWorldMatrix;
		contactCount = frame.contacts.Count;
		for (int i = 0; i < frame.contacts.Count; i++)
		{
			Oni.Contact contact = frame.contacts.Data[i];
			Gizmos.color = ((contact.distance <= 0f) ? Color.red : Color.green);
			Vector3 zero = Vector3.zero;
			int simplexStartAndSize = solver.simplexCounts.GetSimplexStartAndSize(contact.bodyB, out var size);
			float num = 0f;
			for (int j = 0; j < size; j++)
			{
				zero += (Vector3)solver.positions[solver.simplices[simplexStartAndSize + j]] * contact.pointB[j];
				num += solver.principalRadii[solver.simplices[simplexStartAndSize + j]].x * contact.pointB[j];
			}
			Vector3 vector = contact.normal;
			Gizmos.DrawSphere(zero + vector * num, 0.01f);
			Gizmos.DrawRay(zero + vector * num, vector.normalized * contact.distance);
		}
	}
}
