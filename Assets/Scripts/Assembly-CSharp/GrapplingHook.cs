using System.Collections;
using Obi;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
	public ObiSolver solver;

	public ObiCollider character;

	public float hookExtendRetractSpeed = 2f;

	public Material material;

	public ObiRopeSection section;

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
		yield return 0;
		Vector3 position = rope.transform.InverseTransformPoint(hookAttachment.point);
		int filter = ObiUtils.MakeFilter(65535, 0);
		blueprint.path.Clear();
		blueprint.path.AddControlPoint(Vector3.zero, -position.normalized, position.normalized, Vector3.up, 0.1f, 0.1f, 1f, filter, Color.white, "Hook start");
		blueprint.path.AddControlPoint(position, -position.normalized, position.normalized, Vector3.up, 0.1f, 0.1f, 1f, filter, Color.white, "Hook end");
		blueprint.path.FlushEvents();
		yield return blueprint.Generate();
		rope.ropeBlueprint = blueprint;
		rope.GetComponent<MeshRenderer>().enabled = true;
		ObiConstraints<ObiPinConstraintsBatch> obj = rope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
		obj.Clear();
		ObiPinConstraintsBatch obiPinConstraintsBatch = new ObiPinConstraintsBatch();
		obiPinConstraintsBatch.AddConstraint(rope.solverIndices[0], character, base.transform.localPosition, Quaternion.identity, 0f, 0f, float.PositiveInfinity);
		obiPinConstraintsBatch.AddConstraint(rope.solverIndices[blueprint.activeParticleCount - 1], hookAttachment.collider.GetComponent<ObiColliderBase>(), hookAttachment.collider.transform.InverseTransformPoint(hookAttachment.point), Quaternion.identity, 0f, 0f, float.PositiveInfinity);
		obiPinConstraintsBatch.activeConstraintCount = 2;
		obj.AddBatch(obiPinConstraintsBatch);
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
