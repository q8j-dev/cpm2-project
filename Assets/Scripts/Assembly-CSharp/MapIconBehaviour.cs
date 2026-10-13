using System;
using UnityEngine;

public class MapIconBehaviour : MonoBehaviour
{
	[SerializeField]
	private float ScaleIconKoefficient = 2f;

	[SerializeField]
	private float MinDirectionZ = -70f;

	[SerializeField]
	private float MaxDirectionZ = -30f;

	[SerializeField]
	private RacingUIMap RacingUIMap = default;

	private const float DistanceCamToGround = 53f;

	private SpriteRenderer[] _iconImages;

	private Camera _targetCamera;

	[field: SerializeField]
	public RacingTypeIcon RacingTypeIcon { get; private set; }

	private void Start()
	{
		if ((bool)RacingUIMap)
		{
			_targetCamera = RacingUIMap.GetCamera();
		}
		_iconImages = base.gameObject.GetComponentsInChildren<SpriteRenderer>();
	}

	private void Update()
	{
		LookAtCamera();
		SetScaleIcon();
		SetAlphaIcon();
	}

	private void LookAtCamera()
	{
		if ((bool)_targetCamera)
		{
			Vector3 vector = _targetCamera.transform.position - base.transform.position;
			vector.x = 0f;
			vector.z = Math.Clamp(vector.z, MinDirectionZ, MaxDirectionZ);
			if (!(vector == Vector3.zero))
			{
				base.transform.rotation = Quaternion.LookRotation(vector);
			}
		}
	}

	private void SetScaleIcon()
	{
		if ((bool)_targetCamera)
		{
			float num = _targetCamera.fieldOfView * ScaleIconKoefficient / 100f;
			base.transform.localScale = new Vector3(num, num, num);
		}
	}

	private void SetAlphaIcon()
	{
		if ((bool)_targetCamera)
		{
			Vector3 b = _targetCamera.transform.position + _targetCamera.transform.forward * 53f;
			float num = Vector3.Distance(base.transform.position, b);
			float a = Mathf.SmoothStep(1f, 0.2f, (num - 10f) / 30f);
			SpriteRenderer[] iconImages = _iconImages;
			foreach (SpriteRenderer obj in iconImages)
			{
				Color color = obj.color;
				color.a = a;
				obj.color = color;
			}
		}
	}
}
