using Obi;
using UnityEngine;

[RequireComponent(typeof(ObiSolver))]
public class ColliderHighlighter : MonoBehaviour
{
	private ObiSolver solver;

	private void Awake()
	{
		solver = GetComponent<ObiSolver>();
	}

	private void OnEnable()
	{
		solver.OnCollision += Solver_OnCollision;
	}

	private void OnDisable()
	{
		solver.OnCollision -= Solver_OnCollision;
	}

	private void Solver_OnCollision(object sender, ObiSolver.ObiCollisionEventArgs e)
	{
		ObiColliderWorld instance = ObiColliderWorld.GetInstance();
		Oni.Contact[] data = e.contacts.Data;
		for (int i = 0; i < e.contacts.Count; i++)
		{
			Oni.Contact contact = data[i];
			if (!(contact.distance < 0.01f))
			{
				continue;
			}
			ObiColliderBase owner = instance.colliderHandles[contact.bodyB].owner;
			if (owner != null)
			{
				Blinker component = owner.GetComponent<Blinker>();
				if ((bool)component)
				{
					component.Blink();
				}
			}
		}
	}
}
