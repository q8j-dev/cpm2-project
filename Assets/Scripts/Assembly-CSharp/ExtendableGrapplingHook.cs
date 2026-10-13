using System.Collections;
using Obi;
using UnityEngine;

public class ExtendableGrapplingHook : MonoBehaviour
{
	public ObiSolver solver;

	public ObiCollider character;

	public Material material;

	public ObiRopeSection section;

	[Range(0f, 1f)]
	public float hookResolution = 0.5f;

	public float hookExtendRetractSpeed = 2f;

	public float hookShootSpeed = 30f;

	public int particlePoolSize = 100;

	private ObiRope rope;

	private ObiRopeBlueprint blueprint;

	private ObiRopeExtrudedRenderer ropeRenderer;

	private ObiRopeCursor cursor;

	private RaycastHit hookAttachment;

	private void Awake()
	{
		rope = base.gameObject.AddComponent<ObiRope>();
		ropeRenderer = base.gameObject.AddComponent<ObiRopeExtrudedRenderer>();
		ropeRenderer.section = section;
		ropeRenderer.uvScale = new Vector2(1f, 4f);
		ropeRenderer.normalizeV = false;
		ropeRenderer.uvAnchor = 1f;
		rope.GetComponent<MeshRenderer>().material = material;
		blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
		blueprint.resolution = 0.5f;
		blueprint.pooledParticles = particlePoolSize;
		rope.maxBending = 0.02f;
		cursor = rope.gameObject.AddComponent<ObiRopeCursor>();
		cursor.cursorMu = 0f;
		cursor.direction = true;
	}

	private void OnDestroy()
	{
		Object.DestroyImmediate(blueprint);
	}

	private void LaunchHook()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = base.transform.position.z - Camera.main.transform.position.z;
		Vector3 vector = Camera.main.ScreenToWorldPoint(mousePosition);
		if (Physics.Raycast(new Ray(base.transform.position, vector - base.transform.position), out hookAttachment))
		{
			StartCoroutine(AttachHook());
		}
	}

	private IEnumerator AttachHook()
	{
		yield return null;
		ObiConstraints<ObiPinConstraintsBatch> pinConstraints = rope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
		pinConstraints.Clear();
		Vector3 vector = rope.transform.InverseTransformPoint(hookAttachment.point);
		int filter = ObiUtils.MakeFilter(65535, 0);
		blueprint.path.Clear();
		blueprint.path.AddControlPoint(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1f, filter, Color.white, "Hook start");
		blueprint.path.AddControlPoint(vector.normalized * 0.5f, Vector3.zero, Vector3.zero, Vector3.up, 0.1f, 0.1f, 1f, filter, Color.white, "Hook end");
		blueprint.path.FlushEvents();
		yield return blueprint.Generate();
		rope.ropeBlueprint = blueprint;
		yield return null;
		rope.GetComponent<MeshRenderer>().enabled = true;
		for (int i = 0; i < rope.activeParticleCount; i++)
		{
			solver.invMasses[rope.solverIndices[i]] = 0f;
		}
		float currentLength = 0f;
		float magnitude;
		while (true)
		{
			Vector3 vector2 = solver.transform.InverseTransformPoint(rope.transform.position);
			Vector3 vector3 = hookAttachment.point - vector2;
			magnitude = vector3.magnitude;
			vector3.Normalize();
			currentLength += hookShootSpeed * Time.deltaTime;
			if (currentLength >= magnitude)
			{
				break;
			}
			cursor.ChangeLength(Mathf.Min(magnitude, currentLength));
			float num = 0f;
			for (int j = 0; j < rope.elements.Count; j++)
			{
				solver.positions[rope.elements[j].particle1] = vector2 + vector3 * num;
				solver.positions[rope.elements[j].particle2] = vector2 + vector3 * (num + rope.elements[j].restLength);
				num += rope.elements[j].restLength;
			}
			yield return null;
		}
		cursor.ChangeLength(magnitude);
		for (int k = 0; k < rope.activeParticleCount; k++)
		{
			solver.invMasses[rope.solverIndices[k]] = 10f;
		}
		ObiPinConstraintsBatch obiPinConstraintsBatch = new ObiPinConstraintsBatch();
		obiPinConstraintsBatch.AddConstraint(rope.elements[0].particle1, character, base.transform.localPosition, Quaternion.identity, 0f, 0f, float.PositiveInfinity);
		obiPinConstraintsBatch.AddConstraint(rope.elements[rope.elements.Count - 1].particle2, hookAttachment.collider.GetComponent<ObiColliderBase>(), hookAttachment.collider.transform.InverseTransformPoint(hookAttachment.point), Quaternion.identity, 0f, 0f, float.PositiveInfinity);
		obiPinConstraintsBatch.activeConstraintCount = 2;
		pinConstraints.AddBatch(obiPinConstraintsBatch);
		rope.SetConstraintsDirty(Oni.ConstraintType.Pin);
	}

	private void DetachHook()
	{
		rope.ropeBlueprint = null;
		rope.GetComponent<MeshRenderer>().enabled = false;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (!rope.isLoaded)
			{
				LaunchHook();
			}
			else
			{
				DetachHook();
			}
		}
		if (rope.isLoaded)
		{
			if (Input.GetKey(KeyCode.W))
			{
				cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.S))
			{
				cursor.ChangeLength(rope.restLength + hookExtendRetractSpeed * Time.deltaTime);
			}
		}
	}
}
