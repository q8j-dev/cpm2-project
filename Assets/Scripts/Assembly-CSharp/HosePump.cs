using Obi;
using UnityEngine;

[RequireComponent(typeof(ObiRope))]
public class HosePump : MonoBehaviour
{
	[Header("Bulge controls")]
	public float pumpSpeed = 5f;

	public float bulgeFrequency = 3f;

	public float baseThickness = 0.04f;

	public float bulgeThickness = 0.06f;

	public Color bulgeColor = Color.cyan;

	[Header("Flow controls")]
	public ParticleSystem waterEmitter;

	public float flowSpeedMin = 0.5f;

	public float flowSpeedMax = 7f;

	public float minEmitRate = 100f;

	public float maxEmitRate = 1000f;

	private ObiRope rope;

	public ObiPathSmoother smoother;

	private float time;

	private void OnEnable()
	{
		rope = GetComponent<ObiRope>();
		smoother = GetComponent<ObiPathSmoother>();
		rope.OnBeginStep += Rope_OnBeginStep;
	}

	private void OnDisable()
	{
		rope.OnBeginStep -= Rope_OnBeginStep;
	}

	private void Rope_OnBeginStep(ObiActor actor, float stepTime)
	{
		time += stepTime * pumpSpeed;
		float num = 0f;
		float t = 0f;
		for (int i = 0; i < rope.solverIndices.Length; i++)
		{
			int index = rope.solverIndices[i];
			if (i > 0)
			{
				int index2 = rope.solverIndices[i - 1];
				num += Vector3.Distance(rope.solver.positions[index], rope.solver.positions[index2]);
			}
			t = Mathf.Max(0f, Mathf.Sin(num * bulgeFrequency - time));
			rope.solver.principalRadii[index] = Vector3.one * Mathf.Lerp(baseThickness, bulgeThickness, t);
			rope.solver.colors[index] = Color.Lerp(Color.white, bulgeColor, t);
		}
		if (waterEmitter != null)
		{
			ParticleSystem.MainModule main = waterEmitter.main;
			main.startSpeed = Mathf.Lerp(flowSpeedMin, flowSpeedMax, t);
			ParticleSystem.EmissionModule emission = waterEmitter.emission;
			emission.rateOverTime = Mathf.Lerp(minEmitRate, maxEmitRate, t);
		}
	}

	public void LateUpdate()
	{
		if (smoother != null && waterEmitter != null)
		{
			ObiPathFrame sectionAt = smoother.GetSectionAt(1f);
			waterEmitter.transform.position = base.transform.TransformPoint(sectionAt.position);
			waterEmitter.transform.rotation = base.transform.rotation * Quaternion.LookRotation(sectionAt.tangent, sectionAt.binormal);
		}
	}
}
