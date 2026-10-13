using CPMEngine;
using UnityEngine;
using UnityEngine.Rendering;

public class WheelDebugForces : MonoBehaviour
{
	[SerializeField]
	protected Material Material;

	private WheelProvider _wheelProvider;

	private IWheel _wheel;

	private LineRenderer _longForceLine;

	private LineRenderer _latForceLine;

	private LineRenderer _vertForceLine;

	public Vector3 LongForce;

	public Vector3 LateralForce;

	public Vector3 VerticalForce;

	private LineRenderer LatForceLine
	{
		get
		{
			if (!_latForceLine)
			{
				_latForceLine = CreateForceLine();
			}
			return _latForceLine;
		}
	}

	private LineRenderer LongForceLine
	{
		get
		{
			if (!_longForceLine)
			{
				_longForceLine = CreateForceLine();
			}
			return _longForceLine;
		}
	}

	private LineRenderer VertForceLine
	{
		get
		{
			if (!_vertForceLine)
			{
				_vertForceLine = CreateForceLine();
			}
			return _vertForceLine;
		}
	}

	private WheelProvider WheelProvider
	{
		get
		{
			if (!_wheelProvider)
			{
				TryGetComponent<WheelProvider>(out _wheelProvider);
			}
			return _wheelProvider;
		}
	}

	private LineRenderer CreateForceLine()
	{
		GameObject obj = new GameObject("Forceline");
		obj.hideFlags = HideFlags.HideAndDontSave;
		obj.transform.parent = base.transform;
		LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
		lineRenderer.lightProbeUsage = LightProbeUsage.Off;
		lineRenderer.useWorldSpace = true;
		lineRenderer.positionCount = 2;
		lineRenderer.material = Material;
		return lineRenderer;
	}

	private void DestroyForceLine(LineRenderer forceline)
	{
		if ((bool)forceline)
		{
			Object.Destroy(forceline.gameObject);
		}
	}

	private void LateUpdate()
	{
		IWheel wheel = _wheel;
		if (wheel == null || !wheel.Enabled)
		{
			return;
		}
		LongForce = -_wheel.LongForce;
		LateralForce = -_wheel.LatForce;
		VerticalForce = _wheel.VertForce;
		if (!_wheel.GetPatch(out var _, out var _, out var cnt, out var n))
		{
			LongForceLine.enabled = false;
			LatForceLine.enabled = false;
			VertForceLine.enabled = false;
			return;
		}
		LongForceLine.enabled = true;
		LatForceLine.enabled = true;
		VertForceLine.enabled = true;
		cnt += n * 0.05f;
		float num = 0.0005f;
		float magnitude = LongForce.magnitude;
		if (magnitude > 0.001f)
		{
			LongForceLine.SetPosition(0, cnt);
			LongForceLine.SetPosition(1, cnt + LongForce * num);
			LongForceLine.startWidth = 0.002f;
			LongForceLine.endWidth = magnitude * num * 0.04f;
		}
		float magnitude2 = LateralForce.magnitude;
		if (magnitude2 > 0.001f)
		{
			LatForceLine.SetPosition(0, cnt);
			LatForceLine.SetPosition(1, cnt + LateralForce * num);
			LatForceLine.startWidth = 0.002f;
			LatForceLine.endWidth = magnitude2 * num * 0.04f;
		}
		float magnitude3 = VerticalForce.magnitude;
		if (!(magnitude3 <= 0.001f))
		{
			VertForceLine.SetPosition(0, cnt);
			VertForceLine.SetPosition(1, cnt + VerticalForce * num);
			VertForceLine.startWidth = 0.002f;
			VertForceLine.endWidth = magnitude3 * num * 0.04f;
		}
	}

	private void OnDisable()
	{
		DestroyForceLine(_longForceLine);
		DestroyForceLine(_latForceLine);
		DestroyForceLine(_vertForceLine);
		_wheel = null;
		_wheelProvider = null;
	}

	private void OnEnable()
	{
		if ((bool)WheelProvider)
		{
			_wheel = WheelProvider.Wheel;
		}
	}
}
