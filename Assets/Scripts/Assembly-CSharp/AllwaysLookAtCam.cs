using System;
using UniRx;
using UnityEngine;

public class AllwaysLookAtCam : MonoBehaviour
{
	public Transform cam;

	public bool nowLooking;

	public bool FixZ;

	public bool FixX;

	public bool FixY;

	public bool DisableOnClose;

	public float TresholdDistance = 200f;

	public float d;

	public Renderer meshRenderer;

	public bool disableRender = true;

	[Header("Bobbing")]
	public bool EnableBobbing;

	public float BobbingAmplitude = 0.1f;

	public float BobbingSpeed = 1f;

	public AnimationCurve BobbingCurve = new AnimationCurve(new Keyframe(0f, -1f), new Keyframe(0.5f, 1f), new Keyframe(1f, -1f));

	[Header("Distance Fade")]
	[SerializeField]
	private CanvasGroup _canvasGroup;

	public bool EnableDistanceFade;

	public float FadeStartDistance = 30f;

	public float FadeEndDistance = 50f;

	private Vector3 _initialLocalPosition;

	private float _targetAlpha = 1f;

	private void Start()
	{
		if (!cam)
		{
			cam = V.CoreGameManager.transform;
		}
		if ((bool)cam)
		{
			Observable.Timer(TimeSpan.FromSeconds(UnityEngine.Random.Range(0f, 2.5f)), TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				CheckDistance();
			}).AddTo(this);
		}
		meshRenderer = GetComponent<MeshRenderer>();
		_initialLocalPosition = base.transform.localPosition;
		RotateObject();
	}

	private void CheckDistance()
	{
		if ((bool)cam)
		{
			float sqrMagnitude = (cam.transform.position - base.transform.position).sqrMagnitude;
			nowLooking = sqrMagnitude <= TresholdDistance * TresholdDistance;
			if (DisableOnClose)
			{
				base.gameObject.SetActive(sqrMagnitude > 169f);
			}
			if (EnableDistanceFade && (bool)_canvasGroup)
			{
				d = Mathf.Sqrt(sqrMagnitude);
				_targetAlpha = Mathf.InverseLerp(FadeEndDistance, FadeStartDistance, d);
			}
			if ((bool)meshRenderer && disableRender)
			{
				base.enabled = meshRenderer.isVisible;
			}
			else
			{
				base.enabled = sqrMagnitude < 10000f;
			}
		}
	}

	private void Update()
	{
		if ((bool)cam && nowLooking)
		{
			RotateObject();
			if (EnableBobbing)
			{
				ApplyBobbing();
			}
			if (EnableDistanceFade && (bool)_canvasGroup && !Mathf.Approximately(_canvasGroup.alpha, _targetAlpha))
			{
				_canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _targetAlpha, Time.deltaTime * 2f);
			}
		}
	}

	private void RotateObject()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		base.transform.LookAt(cam);
		if (FixX || FixY || FixZ)
		{
			Vector3 eulerAngles2 = base.transform.eulerAngles;
			if (FixZ)
			{
				eulerAngles2.z = eulerAngles.z;
			}
			if (FixX)
			{
				eulerAngles2.x = eulerAngles.x;
			}
			if (FixY)
			{
				eulerAngles2.y = eulerAngles.y;
			}
			base.transform.eulerAngles = eulerAngles2;
		}
	}

	private void ApplyBobbing()
	{
		float time = Mathf.Repeat(Time.time * BobbingSpeed, 1f);
		float num = BobbingCurve.Evaluate(time) * BobbingAmplitude;
		base.transform.localPosition = _initialLocalPosition + Vector3.up * num;
	}
}
