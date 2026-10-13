using System;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public sealed class RainDryZone : MonoBehaviour
{
	[Tooltip("Local-space box size, used only when there is no BoxCollider on this object.")]
	public Vector3 size = new Vector3(12f, 8f, 12f);

	[Tooltip("Local-space box center, used only when there is no BoxCollider on this object.")]
	public Vector3 center = Vector3.zero;

	[Header("Edge fade")]
	[Tooltip("Use this zone's own soft-edge settings instead of the global defaults (RainDryZoneManager.EdgeFeather / EdgeWobble). Widen it at a tunnel mouth, tighten it where the cover really does end abruptly.")]
	public bool overrideEdgeFade;

	[Tooltip("Width (metres) the puddles fade over at each face, per box-local axis. The fade is centred on the face: half of it falls inside the box, half outside. Keep Y small — the box floor usually sits just under the road.")]
	public Vector3 edgeFeather = new Vector3(8f, 2f, 8f);

	[Tooltip("How far (metres) the boundary wanders, so the puddles stop along a wavy line instead of a straight box edge. 0 = perfectly straight.")]
	[Min(0f)]
	public float edgeWobble = 1.2f;

	private void OnEnable()
	{
		RainDryZoneManager.Register(this);
	}

	private void OnDisable()
	{
		RainDryZoneManager.Unregister(this);
	}

	public void GetWorldBox(out Vector3 worldCenter, out Vector3 halfExtents, out float yawRad)
	{
		Vector3 position = center;
		Vector3 vector = size;
		if (TryGetComponent<BoxCollider>(out var component))
		{
			position = component.center;
			vector = component.size;
		}
		Transform transform = base.transform;
		worldCenter = transform.TransformPoint(position);
		Vector3 lossyScale = transform.lossyScale;
		halfExtents = new Vector3(Mathf.Abs(vector.x * lossyScale.x), Mathf.Abs(vector.y * lossyScale.y), Mathf.Abs(vector.z * lossyScale.z)) * 0.5f;
		yawRad = transform.rotation.eulerAngles.y * ((float)Math.PI / 180f);
	}

	public void GetEdgeFade(Vector3 defFeather, float defWobble, out Vector3 feather, out float wobble)
	{
		feather = (overrideEdgeFade ? edgeFeather : defFeather);
		feather = Vector3.Max(feather, new Vector3(0.001f, 0.001f, 0.001f));
		wobble = Mathf.Max(0f, overrideEdgeFade ? edgeWobble : defWobble);
	}
}
