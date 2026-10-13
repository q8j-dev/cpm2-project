using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts;

public class TireSpinEffect : MonoBehaviour
{
	private WheelProvider _wheel;

	private static readonly int BlurAmount = Shader.PropertyToID("_BlurAmount");

	private MeshRenderer _meshRenderer;

	private static readonly Vector3 RightAxis = Vector3.right;

	private float _angularVelocity;

	private MaterialPropertyBlock _mpb;

	private MeshFilter _meshFilter;

	private float _blurThreshold = 0.1f;

	public void Init(WheelProvider wheel, Mesh tyreMesh)
	{
		TryGetComponent<MeshRenderer>(out _meshRenderer);
		TryGetComponent<MeshFilter>(out _meshFilter);
		if ((bool)_meshFilter)
		{
			_meshFilter.mesh = tyreMesh;
			_mpb = new MaterialPropertyBlock();
			_meshRenderer.GetPropertyBlock(_mpb);
			_wheel = wheel;
		}
	}

	private void Start()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerViaName.WheelLayer;
		}
	}

	private void LateUpdate()
	{
		if (CPMGraphicsSettingsManager.Instance.Quality != 3 || !_meshRenderer || !_wheel)
		{
			return;
		}
		base.transform.localRotation = Quaternion.AngleAxis((0f - _wheel.Wheel.RollAngle) * 57.29578f, RightAxis);
		if ((bool)_meshRenderer)
		{
			float num = ((_wheel.Wheel.StaticMode || !_wheel.Wheel.Enabled) ? 0f : _wheel.Wheel.AngularVelocity);
			if (!(Mathf.Abs(num - _angularVelocity) < _blurThreshold))
			{
				_angularVelocity = num;
				_mpb.SetFloat(BlurAmount, Mathf.Abs(_angularVelocity) * Time.deltaTime);
				_meshRenderer.SetPropertyBlock(_mpb);
			}
		}
	}
}
