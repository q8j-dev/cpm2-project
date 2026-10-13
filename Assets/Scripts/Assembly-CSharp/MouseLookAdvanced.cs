using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLookAdvanced : MonoBehaviour
{
	public float sensitivityX = 5f;

	public float sensitivityY = 5f;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -90f;

	public float maximumY = 90f;

	public float smoothSpeed = 20f;

	private float verticalAcceleration;

	private float rotationX;

	private float smoothRotationX;

	private float rotationY;

	private float smoothRotationY;

	private Vector3 vMousePos;

	public float Speed = 100f;

	private bool bActive;
}
