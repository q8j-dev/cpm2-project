using Obi;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
	public Transform headReferenceFrame;

	public float headSpeed = 20f;

	public float upSpeed = 40f;

	public float slitherSpeed = 10f;

	private ObiRope rope;

	private ObiSolver solver;

	private float[] traction;

	private Vector3[] surfaceNormal;

	private void Start()
	{
		rope = GetComponent<ObiRope>();
		solver = rope.solver;
		traction = new float[rope.activeParticleCount];
		surfaceNormal = new Vector3[rope.activeParticleCount];
		solver.OnBeginStep += ResetSurfaceInfo;
		solver.OnCollision += AnalyzeContacts;
		solver.OnParticleCollision += AnalyzeContacts;
	}

	private void OnDestroy()
	{
		solver.OnBeginStep -= ResetSurfaceInfo;
		solver.OnCollision -= AnalyzeContacts;
		solver.OnParticleCollision -= AnalyzeContacts;
	}

	private void ResetSurfaceInfo(ObiSolver s, float stepTime)
	{
		for (int i = 0; i < traction.Length; i++)
		{
			traction[i] = 0f;
			surfaceNormal[i] = Vector3.zero;
		}
	}

	private void AnalyzeContacts(object sender, ObiSolver.ObiCollisionEventArgs e)
	{
		for (int i = 0; i < e.contacts.Count; i++)
		{
			Oni.Contact contact = e.contacts.Data[i];
			if (contact.distance < 0.005f)
			{
				int num = solver.simplices[contact.bodyA];
				ObiSolver.ParticleInActor particleInActor = solver.particleToActor[num];
				if (particleInActor.actor == rope)
				{
					traction[particleInActor.indexInActor] = 1f;
					surfaceNormal[particleInActor.indexInActor] += (Vector3)contact.normal;
				}
			}
		}
	}

	public void Update()
	{
		if (Input.GetKey(KeyCode.J))
		{
			for (int i = 1; i < rope.activeParticleCount; i++)
			{
				int index = rope.solverIndices[i];
				int index2 = rope.solverIndices[i - 1];
				Vector4 vector = Vector3.ProjectOnPlane(solver.positions[index2] - solver.positions[index], surfaceNormal[i]).normalized;
				solver.velocities[index] += vector * traction[i] * slitherSpeed * Time.deltaTime;
			}
		}
		int index3 = rope.solverIndices[0];
		if (headReferenceFrame != null)
		{
			Vector3 zero = Vector3.zero;
			if (Input.GetKey(KeyCode.W))
			{
				zero += headReferenceFrame.forward * headSpeed;
			}
			if (Input.GetKey(KeyCode.A))
			{
				zero += -headReferenceFrame.right * headSpeed;
			}
			if (Input.GetKey(KeyCode.S))
			{
				zero += -headReferenceFrame.forward * headSpeed;
			}
			if (Input.GetKey(KeyCode.D))
			{
				zero += headReferenceFrame.right * headSpeed;
			}
			zero.y = 0f;
			solver.velocities[index3] += (Vector4)zero * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.Space))
		{
			solver.velocities[index3] += (Vector4)Vector3.up * Time.deltaTime * upSpeed;
		}
	}
}
