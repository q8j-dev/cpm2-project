using UnityEngine;

[ExecuteAlways]
public class ScreenSpacePlacement : MonoBehaviour
{
	[SerializeField]
	private Camera m_Cam = default;

	[SerializeField]
	private Transform m_FlareObject = default;

	private void OnGUI()
	{
		Event current = Event.current;
		Vector2 vector = new Vector2
		{
			x = current.mousePosition.x,
			y = (float)m_Cam.pixelHeight - current.mousePosition.y
		};
		if (m_FlareObject != null && vector.x > 0f && vector.y > 0f && vector.x < (float)m_Cam.pixelWidth && vector.y < (float)m_Cam.pixelHeight)
		{
			Vector3 position = m_Cam.ScreenToWorldPoint(new Vector3(vector.x, vector.y, m_Cam.nearClipPlane));
			m_FlareObject.position = position;
		}
	}
}
