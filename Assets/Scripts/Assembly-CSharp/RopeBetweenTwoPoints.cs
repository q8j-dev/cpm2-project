using Obi;
using UnityEngine;

public class RopeBetweenTwoPoints : MonoBehaviour
{
	public Transform start;

	public Transform end;

	public ObiSolver solver;

	private void Start()
	{
		Generate();
	}

	private void Generate()
	{
		if (start != null && end != null)
		{
			base.transform.position = (start.position + end.position) / 2f;
			base.transform.rotation = Quaternion.FromToRotation(Vector3.right, end.position - start.position);
			Vector3 vector = base.transform.InverseTransformPoint(start.position);
			Vector3 vector2 = base.transform.InverseTransformPoint(end.position);
			Vector3 normalized = (vector2 - vector).normalized;
			ObiRopeBlueprint obiRopeBlueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
			int filter = ObiUtils.MakeFilter(65535, 0);
			obiRopeBlueprint.path.AddControlPoint(vector, -normalized, normalized, Vector3.up, 0.1f, 0.1f, 1f, filter, Color.white, "start");
			obiRopeBlueprint.path.AddControlPoint(vector2, -normalized, normalized, Vector3.up, 0.1f, 0.1f, 1f, filter, Color.white, "end");
			obiRopeBlueprint.path.FlushEvents();
			obiRopeBlueprint.GenerateImmediate();
			ObiRope obiRope = base.gameObject.AddComponent<ObiRope>();
			ObiRopeExtrudedRenderer obiRopeExtrudedRenderer = base.gameObject.AddComponent<ObiRopeExtrudedRenderer>();
			ObiParticleAttachment obiParticleAttachment = base.gameObject.AddComponent<ObiParticleAttachment>();
			ObiParticleAttachment obiParticleAttachment2 = base.gameObject.AddComponent<ObiParticleAttachment>();
			obiRopeExtrudedRenderer.section = Resources.Load<ObiRopeSection>("DefaultRopeSection");
			obiRope.ropeBlueprint = obiRopeBlueprint;
			obiParticleAttachment.target = start;
			obiParticleAttachment2.target = end;
			obiParticleAttachment.particleGroup = obiRopeBlueprint.groups[0];
			obiParticleAttachment2.particleGroup = obiRopeBlueprint.groups[1];
			base.transform.SetParent(solver.transform);
		}
	}
}
