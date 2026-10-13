using UnityEngine;

public class RacingCheckPoint : MonoBehaviour
{
	public RacingCheckPoint previousCheckpoint;

	public RacingCheckPoint nextCheckpoint;

	public bool carPassed;

	private void Start()
	{
		if ((bool)GetComponent<MeshRenderer>())
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
	}

	private void OnDrawGizmos()
	{
		if (previousCheckpoint != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, previousCheckpoint.transform.position);
		}
		if (nextCheckpoint != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, nextCheckpoint.transform.position);
		}
	}
}
