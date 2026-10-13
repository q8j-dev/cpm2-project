using UnityEngine;

public class ToRoadPointLookGizmos : MonoBehaviour
{
	[SerializeField]
	private float lineLength = 5f;

	[SerializeField]
	private Color lineColor = Color.green;

	private void OnDrawGizmos()
	{
		Gizmos.color = lineColor;
		Vector3 position = base.transform.position;
		Vector3 vector = position + base.transform.forward * lineLength;
		Gizmos.DrawLine(position, vector);
		Vector3 to = vector - base.transform.forward * 0.5f + base.transform.right * 0.3f;
		Vector3 to2 = vector - base.transform.forward * 0.5f - base.transform.right * 0.3f;
		Gizmos.DrawLine(vector, to);
		Gizmos.DrawLine(vector, to2);
	}
}
