using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{
	[SerializeField]
	private Camera m_MainCamera;

	[SerializeField]
	private float m_SkyboxScale = 1f;

	private Vector3 mainCamStartPos;

	private Vector3 skyboxCamStartPos;

	private void Start()
	{
		if (m_MainCamera == null)
		{
			m_MainCamera = Camera.main;
		}
		mainCamStartPos = m_MainCamera.transform.position;
		skyboxCamStartPos = base.transform.position;
	}

	private void Update()
	{
		Vector3 vector = m_MainCamera.transform.position - mainCamStartPos;
		base.transform.position = skyboxCamStartPos + vector * m_SkyboxScale;
		base.transform.rotation = m_MainCamera.transform.rotation;
	}
}
