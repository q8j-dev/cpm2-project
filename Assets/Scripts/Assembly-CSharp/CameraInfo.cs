using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("Camera/CameraInfo")]
public class CameraInfo : MonoBehaviour
{
	private bool m_d3d;

	public static Matrix4x4 ViewMatrix { get; private set; }

	public static Matrix4x4 ProjectionMatrix { get; private set; }

	public static Matrix4x4 ViewProjectionMatrix { get; private set; }

	public static Matrix4x4 PrevViewMatrix { get; private set; }

	public static Matrix4x4 PrevProjectionMatrix { get; private set; }

	public static Matrix4x4 PrevViewProjMatrix { get; private set; }

	protected void Awake()
	{
		m_d3d = SystemInfo.graphicsDeviceVersion.IndexOf("Direct3D") > -1;
		ViewMatrix = Matrix4x4.identity;
		ProjectionMatrix = Matrix4x4.identity;
		ViewProjectionMatrix = Matrix4x4.identity;
		PrevViewMatrix = Matrix4x4.identity;
		PrevProjectionMatrix = Matrix4x4.identity;
		PrevViewProjMatrix = Matrix4x4.identity;
		UpdateCurrentMatrices();
	}

	protected void Update()
	{
		PrevViewMatrix = ViewMatrix;
		PrevProjectionMatrix = ProjectionMatrix;
		PrevViewProjMatrix = ViewProjectionMatrix;
		UpdateCurrentMatrices();
	}

	private void UpdateCurrentMatrices()
	{
		ViewMatrix = GetComponent<Camera>().worldToCameraMatrix;
		Matrix4x4 projectionMatrix = GetComponent<Camera>().projectionMatrix;
		if (m_d3d)
		{
			for (int i = 0; i < 4; i++)
			{
				projectionMatrix[1, i] = 0f - projectionMatrix[1, i];
			}
			for (int j = 0; j < 4; j++)
			{
				projectionMatrix[2, j] = projectionMatrix[2, j] * 0.5f + projectionMatrix[3, j] * 0.5f;
			}
		}
		ProjectionMatrix = projectionMatrix;
		ViewProjectionMatrix = ProjectionMatrix * ViewMatrix;
	}
}
