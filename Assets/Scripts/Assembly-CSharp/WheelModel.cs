using System;
using System.Collections.Generic;
using System.Linq;
using CPMEngine;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.Drag;

public class WheelModel : MonoBehaviour
{
	public GameObject Rim;

	public GameObject Rim2;

	public GameObject Tire;

	public GameObject Tire2;

	public Material WheelMat;

	public Material TireMat;

	public GameObject Spacer;

	private float _spacerWidth;

	public GameObject LodModel;

	public DynamicBrakeDisk BrakeDisk;

	public GameObject BrakeDiskGm;

	public GameObject SpinRimEffect;

	public Material BrakeDiskMaterial;

	[NonSerialized]
	public List<Material> RuntimeMaterials;

	[Header("To show")]
	public int RimModelId;

	public float DefWidth = 0.27f;

	public float DefDiam = 0.48f;

	public bool DefWidthCalibrated;

	private const float DefLodWidth = 0.231f;

	private const float DefLodDiam = 0.65f;

	private const float DefSpacer = 0.02f;

	public bool DualTyre;

	public UnityEngine.PhysicMaterial RimColliderPhysMat;

	private GameObject _rimCollider;

	private MeshCollider _rimColliderMesh;

	public Vector3 CurrentPosition;

	private float _currentRimWidth;

	private DynamicTire _dynamicTire1;

	private DynamicTire _dynamicTire2;

	public float RimRadius;

	[Header("Mount face calibration")]
	public float MountFaceLocalX;

	public bool MountFaceCalibrated;

	private float _rimWidthScale = 1f;

	private float _currentRimWidthRaw;

	private const float DefaultContactX = -0.08f;

	private static readonly Dictionary<Mesh, float> BarrelCenterCache = new Dictionary<Mesh, float>();

	public Transform RimColliderTransform
	{
		get
		{
			if (!_rimCollider)
			{
				return null;
			}
			return _rimCollider.transform;
		}
	}

	public Collider RimCollider => _rimColliderMesh;

	public float SpacerWidth
	{
		set
		{
			_spacerWidth = Utils.Mm2Meter(value);
			if (_currentRimWidth > 0f)
			{
				UpdatePosition(_currentRimWidth);
			}
		}
	}

	private void ResetRimsReferences()
	{
		Rim = null;
		Rim2 = null;
		OnValidate();
	}

	public void UseTyreDeformation(bool val)
	{
		if ((bool)_dynamicTire1)
		{
			_dynamicTire1.UseDeformation = val;
		}
		if ((bool)_dynamicTire2)
		{
			_dynamicTire2.UseDeformation = val;
		}
	}

	public void SetOpenWheelsEnabled(bool openWheelsEnabled)
	{
		if (!openWheelsEnabled)
		{
			if ((bool)_rimCollider)
			{
				_rimCollider.SetActive(value: false);
			}
			return;
		}
		if (!_rimCollider)
		{
			_rimCollider = new GameObject("RimCollider");
			_rimCollider.layer = LayerViaName.WheelLayer;
			_rimCollider.transform.SetParent(base.transform, worldPositionStays: false);
			_rimColliderMesh = _rimCollider.AddComponent<MeshCollider>();
			_rimColliderMesh.convex = true;
			_rimColliderMesh.sharedMesh = RimColliderCylinderMesh.Get();
			if ((bool)RimColliderPhysMat)
			{
				_rimColliderMesh.sharedMaterial = RimColliderPhysMat;
			}
			if (_currentRimWidth > 0f)
			{
				UpdatePosition(_currentRimWidth);
			}
			ApplyRimColliderScale();
		}
		_rimCollider.SetActive(value: true);
	}

	private void ApplyRimColliderScale()
	{
		if ((bool)_rimCollider && !(_currentRimWidthRaw <= 0f) && !(RimRadius <= 0f))
		{
			_rimCollider.transform.localScale = new Vector3(_currentRimWidthRaw, RimRadius * 2f, RimRadius * 2f);
		}
	}

	public void UpdateTyreContactPlane(Vector3 localPlanePos, Vector3 localPlaneNormal, Vector3 latDeform, Vector3 longDeform, bool isTouching)
	{
		if ((bool)_dynamicTire1 && _dynamicTire1.gameObject.activeInHierarchy)
		{
			_dynamicTire1.SetContactPlane(localPlanePos, localPlaneNormal, latDeform, longDeform, isTouching);
		}
		if ((bool)_dynamicTire2 && _dynamicTire2.gameObject.activeInHierarchy)
		{
			_dynamicTire2.SetContactPlane(localPlanePos, localPlaneNormal, latDeform, longDeform, isTouching);
		}
	}

	public void HideWheel(bool set)
	{
		if (!set || FlagsStorage.IsCarInService)
		{
			if ((bool)Rim)
			{
				Rim.SetActive(!set);
			}
			if ((bool)Tire)
			{
				Tire.SetActive(!set);
			}
			if ((bool)Tire2)
			{
				Tire2.SetActive(!set);
			}
			if ((bool)Rim2)
			{
				Rim2.gameObject.SetActive(!set);
			}
		}
	}

	public void ShowLod(bool set)
	{
		if ((bool)Rim)
		{
			Rim.SetActive(!set);
		}
		if ((bool)Tire)
		{
			Tire.SetActive(!set);
		}
		if ((bool)Tire2)
		{
			Tire2.SetActive(!set);
		}
		if ((bool)Rim2)
		{
			Rim2.gameObject.SetActive(!set);
		}
		if ((bool)BrakeDiskGm)
		{
			BrakeDiskGm.SetActive(!set);
		}
		if ((bool)LodModel)
		{
			LodModel.SetActive(set);
		}
		UpdatePosition(_currentRimWidth);
	}

	private void OnDestroy()
	{
		CommonExtensions.DestroyMat(ref BrakeDiskMaterial);
		if (RuntimeMaterials == null)
		{
			return;
		}
		foreach (Material runtimeMaterial in RuntimeMaterials)
		{
			if ((bool)runtimeMaterial)
			{
				UnityEngine.Object.Destroy(runtimeMaterial);
			}
		}
		RuntimeMaterials = null;
	}

	private void OnValidate()
	{
		if (Rim == null && Rim2 == null)
		{
			List<MeshRenderer> list = (from x in GetComponentsInChildren<MeshRenderer>()
				where !x.name.ToUpper().Contains("BOLT")
				where !x.name.ToUpper().Contains("GRILLE")
				where !x.name.ToUpper().Contains("CHROME")
				select x).ToList();
			if (list.Count > 0)
			{
				if (list.Count == 1)
				{
					Rim = list[0].gameObject;
					Debug.Log("1 Rims was set into WheelModel");
				}
				if (list.Count == 2)
				{
					if (list.Exists((MeshRenderer x) => x.name.ToUpper().Contains("BLACK")))
					{
						int num = list.FindIndex((MeshRenderer x) => x.name.ToUpper().Contains("BLACK"));
						int index = ((num == 0) ? 1 : 0);
						Rim = list[index].gameObject;
						Rim2 = list[num].gameObject;
					}
					else
					{
						Rim = list[0].gameObject;
						Rim2 = list[1].gameObject;
					}
					Debug.Log("2 Rims was set into WheelModel");
				}
				if (list.Count == 3)
				{
					Debug.LogError("3 ERROR");
				}
			}
		}
		if (SpinRimEffect == null)
		{
			List<MeshRenderer> list2 = (from x in GetComponentsInChildren<MeshRenderer>()
				where x.name.ToUpper().Contains("RIMBLUR")
				select x).ToList();
			if (list2.Count > 0)
			{
				SpinRimEffect = list2[0].gameObject;
			}
		}
		if ((bool)SpinRimEffect && !SpinRimEffect.TryGetComponent<WheelSpinEffect>(out var _))
		{
			SpinRimEffect.AddComponent<WheelSpinEffect>();
		}
	}

	public void UpdateTyreDynamic(float tireHealth, float dirtAmount, bool forceUpdate = false)
	{
		if ((bool)_dynamicTire1 && (_dynamicTire1.gameObject.activeInHierarchy | forceUpdate))
		{
			_dynamicTire1.SetTireWearAndCollision(tireHealth, dirtAmount);
		}
		if ((bool)_dynamicTire2 && (_dynamicTire2.gameObject.activeInHierarchy | forceUpdate))
		{
			_dynamicTire2.SetTireWearAndCollision(tireHealth, dirtAmount);
		}
	}

	public void InstallSideDisk()
	{
		if ((bool)Rim)
		{
			Rim.transform.localRotation = Quaternion.identity;
			if ((bool)Rim2 && Rim2.transform.parent != Rim.transform)
			{
				Rim2.transform.localRotation = Quaternion.identity;
			}
		}
		if ((bool)Spacer)
		{
			Spacer.transform.localRotation = Quaternion.identity;
		}
		bool mirrored = base.transform.localScale.x < 0f;
		if ((bool)Tire)
		{
			ApplyTyreFacing(Tire.transform, mirrored);
		}
		if ((bool)Tire2)
		{
			ApplyTyreFacing(Tire2.transform, mirrored);
		}
		if ((bool)LodModel && (bool)Rim)
		{
			LodModel.transform.SetLocalPositionAndRotation(Rim.transform.localPosition, Quaternion.identity);
		}
	}

	private static void ApplyTyreFacing(Transform tyre, bool mirrored)
	{
		bool flag = tyre.TryGetComponent<DynamicTire>(out var component) && component.MirrorOnRightSide;
		bool flag2 = mirrored && !flag;
		tyre.localRotation = (flag2 ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity);
		Vector3 localScale = tyre.localScale;
		float num = Mathf.Abs(localScale.x);
		tyre.localScale = new Vector3(flag2 ? (0f - num) : num, localScale.y, localScale.z);
	}

	public void SetSize(float radius, float width, float rimRadius, float rimWidth, bool isOnline)
	{
		float num = ((DualTyre && (bool)Tire2) ? (rimWidth * 2f) : rimWidth);
		float num2 = (_rimWidthScale = num / Mathf.Max(DefWidth, 0.001f));
		_currentRimWidth = width;
		UpdatePosition(width);
		RimRadius = rimRadius;
		_currentRimWidthRaw = rimWidth;
		ApplyRimColliderScale();
		if ((bool)Tire && Tire.TryGetComponent<DynamicTire>(out _dynamicTire1))
		{
			_dynamicTire1.SetTireSize(radius, width, rimWidth, rimRadius, isOnline).Forget();
		}
		if ((bool)Tire2 && Tire2.TryGetComponent<DynamicTire>(out _dynamicTire2))
		{
			_dynamicTire2.SetTireSize(radius, width, rimWidth, rimRadius, isOnline).Forget();
		}
		if ((bool)Rim)
		{
			float num3 = rimRadius * 2f / Mathf.Max(DefDiam, 0.001f);
			Vector3 localScale = new Vector3(num2, num3, num3);
			Rim.transform.localScale = localScale;
			if ((bool)LodModel)
			{
				float num4 = radius * 2f / 0.65f;
				float x = num / 0.231f;
				Vector3 localScale2 = new Vector3(x, num4, num4);
				LodModel.transform.localScale = localScale2;
			}
			if ((bool)SpinRimEffect)
			{
				SpinRimEffect.transform.localScale = new Vector3(0f - num2, num3, num3);
			}
			if ((bool)Rim2 && !(Rim2.transform.parent != base.transform))
			{
				Rim2.transform.localScale = localScale;
			}
		}
	}

	public void UpdatePosition(float width)
	{
		float num = (MountFaceCalibrated ? (GetBrakeDiscContactX() - MountFaceLocalX * _rimWidthScale) : (-0.08f));
		CurrentPosition = Vector3.right * num;
		Vector3 localPosition = CurrentPosition - Vector3.right * _spacerWidth;
		float num2 = localPosition.x + RimBarrelCenterX() * _rimWidthScale;
		if (DualTyre && (bool)Tire2)
		{
			float num3 = DefWidth * _rimWidthScale * 0.25f;
			if ((bool)Tire)
			{
				Tire.transform.localPosition = new Vector3(num2 - num3, 0f, 0f);
			}
			Tire2.transform.localPosition = new Vector3(num2 + num3, 0f, 0f);
		}
		else
		{
			if ((bool)Tire)
			{
				Tire.transform.localPosition = new Vector3(num2, 0f, 0f);
			}
			if ((bool)Tire2)
			{
				Tire2.transform.localPosition = CurrentPosition + Vector3.right * width;
			}
		}
		if ((bool)SpinRimEffect)
		{
			SpinRimEffect.transform.localPosition = localPosition;
		}
		if ((bool)_rimCollider)
		{
			_rimCollider.transform.localPosition = localPosition;
		}
		if (!Rim)
		{
			return;
		}
		Rim.transform.localPosition = localPosition;
		if ((bool)Rim2 && !(Rim2.transform.parent != base.transform))
		{
			Rim2.transform.localPosition = localPosition;
			if ((bool)LodModel)
			{
				LodModel.transform.localPosition = localPosition;
			}
		}
	}

	public float OuterTyreCenterLocalX()
	{
		float num = CurrentPosition.x - _spacerWidth + RimBarrelCenterX() * _rimWidthScale;
		if (DualTyre && (bool)Tire2)
		{
			num -= DefWidth * _rimWidthScale * 0.25f;
		}
		return num;
	}

	private float RimBarrelCenterX()
	{
		if (!Rim || !Rim.TryGetComponent<MeshFilter>(out var component) || !component.sharedMesh)
		{
			return 0f;
		}
		Mesh sharedMesh = component.sharedMesh;
		if (BarrelCenterCache.TryGetValue(sharedMesh, out var value))
		{
			return value;
		}
		if (!sharedMesh.isReadable)
		{
			float x = sharedMesh.bounds.center.x;
			BarrelCenterCache[sharedMesh] = x;
			return x;
		}
		Vector3[] vertices = sharedMesh.vertices;
		float num = 0f;
		Vector3[] array = vertices;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 vector = array[i];
			float num2 = vector.y * vector.y + vector.z * vector.z;
			if (num2 > num)
			{
				num = num2;
			}
		}
		float num3 = num * 0.7225f;
		float num4 = float.MaxValue;
		float num5 = float.MinValue;
		array = vertices;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 vector2 = array[i];
			if (!(vector2.y * vector2.y + vector2.z * vector2.z < num3))
			{
				if (vector2.x < num4)
				{
					num4 = vector2.x;
				}
				if (vector2.x > num5)
				{
					num5 = vector2.x;
				}
			}
		}
		float num6 = ((num4 <= num5) ? ((num4 + num5) * 0.5f) : 0f);
		BarrelCenterCache[sharedMesh] = num6;
		return num6;
	}

	private float GetBrakeDiscContactX()
	{
		if (!BrakeDisk || !BrakeDisk.DiskModel)
		{
			return -0.08f;
		}
		Transform transform = BrakeDisk.DiskModel.transform;
		Transform transform2 = BrakeDisk.transform;
		if (!transform.TryGetComponent<MeshFilter>(out var component) || !component.sharedMesh)
		{
			return -0.08f;
		}
		Bounds bounds = component.sharedMesh.bounds;
		float a = transform.localPosition.x + bounds.min.x * transform.localScale.x;
		float b = transform.localPosition.x + bounds.max.x * transform.localScale.x;
		float num = transform2.localPosition.x + Mathf.Min(a, b) * transform2.localScale.x;
		float num2 = transform2.localPosition.x + Mathf.Max(a, b) * transform2.localScale.x;
		if (!(Mathf.Abs(num - -0.08f) <= Mathf.Abs(num2 - -0.08f)))
		{
			return num2;
		}
		return num;
	}
}
