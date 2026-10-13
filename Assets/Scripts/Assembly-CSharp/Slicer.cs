using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Collider))]
public class Slicer : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Slicers have to work chained to properly cut the object. Put slider which should cut after this one")]
	private Slicer nextSlicer;

	[Tooltip("Need to add rigibodies to cut parts")]
	[SerializeField]
	private bool isAddRigidbody = true;

	[Tooltip("Mass of cut parts")]
	[SerializeField]
	private float massOfParts = 1f;

	[SerializeField]
	[Tooltip("Need to apply cut material on sliced place")]
	private bool isFillGaps = true;

	private bool _isCut;

	private Collider _collider;

	private Transform _subjectToCut;

	private Vector3 _localScale;

	private Vector3 _localRotation;

	private Vector3 _localPosition;

	private Vector3 _appliedForce;

	private GameObject[] _targets;

	private Material[] _materials;

	private Material[] _newMaterials;

	private Material _cutMaterial;

	private Mesh _mesh;

	private Vector3[] _baseVert;

	private Vector2[] _baseUv;

	private int[] _baseTri;

	private bool[] _side;

	private CutData _pos;

	private CutData _neg;

	private void SliceMesh()
	{
		Bounds bounds = _collider.bounds;
		Vector3 vector = _subjectToCut.InverseTransformPoint(bounds.center + base.transform.up * bounds.extents.y);
		Vector3 b = _subjectToCut.InverseTransformPoint(bounds.center + base.transform.up * bounds.extents.y + base.transform.right * bounds.extents.x);
		Vector3 c = _subjectToCut.InverseTransformPoint(bounds.center - base.transform.up * bounds.extents.y + base.transform.right * bounds.extents.x);
		Plane plane = new Plane(vector, b, c);
		for (int i = 0; i < _baseVert.Length; i++)
		{
			_side[i] = plane.GetSide(_baseVert[i]);
		}
		int num = 0;
		int num2 = 0;
		if (isFillGaps)
		{
			num = _pos.AddVertex(_pos.transform.localPosition);
			num2 = _neg.AddVertex(_neg.transform.localPosition);
		}
		bool flag = ((_baseVert.Length == _baseUv.Length) ? true : false);
		for (int j = 0; j < _baseTri.Length; j += 3)
		{
			int num3 = 0;
			int num4 = 0;
			int num5 = -1;
			if ((_side[_baseTri[j + 1]] != _side[_baseTri[j + 2]]) | (_side[_baseTri[j + 1]] != _side[_baseTri[j]]) | (_side[_baseTri[j + 2]] != _side[_baseTri[j]]))
			{
				if (_side[_baseTri[j]] == _side[_baseTri[j + 1]])
				{
					num3 = _baseTri[j];
					num4 = _baseTri[j + 1];
					num5 = _baseTri[j + 2];
				}
				else if (_side[_baseTri[j + 1]] == _side[_baseTri[j + 2]])
				{
					num3 = _baseTri[j + 1];
					num4 = _baseTri[j + 2];
					num5 = _baseTri[j];
				}
				else
				{
					num3 = _baseTri[j + 2];
					num4 = _baseTri[j];
					num5 = _baseTri[j + 1];
				}
			}
			if (num5 >= 0)
			{
				Vector3 normalized = new Vector3(_baseVert[num3].x - _baseVert[num5].x, _baseVert[num3].y - _baseVert[num5].y, _baseVert[num3].z - _baseVert[num5].z).normalized;
				Vector3 normalized2 = new Vector3(_baseVert[num4].x - _baseVert[num5].x, _baseVert[num4].y - _baseVert[num5].y, _baseVert[num4].z - _baseVert[num5].z).normalized;
				float num6 = Vector3.Dot(vector - _baseVert[num3], plane.normal) / Vector3.Dot(normalized, plane.normal);
				float num7 = Vector3.Dot(vector - _baseVert[num4], plane.normal) / Vector3.Dot(normalized2, plane.normal);
				Vector3 vector2 = num6 * normalized + _baseVert[num3];
				Vector3 vector3 = num7 * normalized2 + _baseVert[num4];
				Vector2 uv = _baseUv[num3];
				Vector2 uv2 = _baseUv[num4];
				Vector2 uv3 = _baseUv[num5];
				float t = Vector3.Distance(_baseVert[num3], vector2) / Vector3.Distance(_baseVert[num3], _baseVert[num5]);
				float t2 = Vector3.Distance(_baseVert[num4], vector3) / Vector3.Distance(_baseVert[num4], _baseVert[num5]);
				Vector2 uv4 = new Vector2(Mathf.Lerp(uv.x, uv3.x, t), Mathf.Lerp(uv.y, uv3.y, t));
				Vector2 uv5 = new Vector2(Mathf.Lerp(uv2.x, uv3.x, t2), Mathf.Lerp(uv2.y, uv3.y, t2));
				CutData cutData = (_side[num5] ? _pos : _neg);
				cutData.AddTriangle(cutData.AddVertex(_baseVert[num5], uv3), cutData.AddVertex(vector2, uv4), cutData.AddVertex(vector3, uv5));
				if (isFillGaps)
				{
					cutData.AddSubTriangle(cutData.AddVertex(vector3, uv5), cutData.AddVertex(vector2, uv4), _side[num5] ? num : num2);
				}
				cutData = (_side[num5] ? _neg : _pos);
				cutData.AddTriangle(cutData.AddVertex(_baseVert[num3], uv), cutData.AddVertex(_baseVert[num4], uv2), cutData.AddVertex(vector2, uv4));
				cutData.AddTriangle(cutData.AddVertex(_baseVert[num4], uv2), cutData.AddVertex(vector3, uv5), cutData.AddVertex(vector2, uv4));
				cutData.AddEdgeVertex(vector2);
				cutData.AddEdgeVertex(vector3);
				if (isFillGaps)
				{
					cutData.AddSubTriangle(cutData.AddVertex(vector2, uv4), cutData.AddVertex(vector3, uv5), _side[num5] ? num2 : num);
				}
			}
			else
			{
				Vector3 vertex = _baseVert[_baseTri[j]];
				Vector3 vertex2 = _baseVert[_baseTri[j + 1]];
				Vector3 vertex3 = _baseVert[_baseTri[j + 2]];
				Vector2 uv6 = Vector2.zero;
				Vector2 uv7 = Vector2.zero;
				Vector2 uv8 = Vector2.zero;
				if (flag)
				{
					uv6 = _baseUv[_baseTri[j]];
					uv7 = _baseUv[_baseTri[j + 1]];
					uv8 = _baseUv[_baseTri[j + 2]];
				}
				CutData cutData2 = (_side[_baseTri[j]] ? _pos : _neg);
				cutData2.AddTriangle(cutData2.AddVertex(vertex, uv6), cutData2.AddVertex(vertex2, uv7), cutData2.AddVertex(vertex3, uv8));
			}
		}
		if (isFillGaps)
		{
			Vector3 capCenter = _pos.GetCapCenter();
			_pos.MoveVertex(num, capCenter);
			Vector3 capCenter2 = _neg.GetCapCenter();
			_neg.MoveVertex(num2, capCenter2);
		}
	}

	private GameObject GenerateObject(CutData data, bool addRigidbody = true)
	{
		GameObject gameObject = data.gameObject;
		gameObject.transform.localScale = _localScale;
		gameObject.transform.localEulerAngles = _localRotation;
		gameObject.transform.localPosition = _localPosition;
		gameObject.AddComponent<MeshRenderer>().sharedMaterials = _newMaterials;
		gameObject.AddComponent<MeshFilter>().sharedMesh = data.mesh;
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = data.mesh;
		meshCollider.convex = addRigidbody;
		if (addRigidbody)
		{
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.mass = massOfParts;
			rigidbody.AddForce(_appliedForce, ForceMode.Impulse);
		}
		gameObject.AddComponent<DeactivatePhysics>().Mesh = data.mesh;
		return gameObject;
	}

	public void ActivateSlicer(Vector3 appliedForce, Material[] materials = null, params GameObject[] targets)
	{
		if (_collider == null)
		{
			_collider = GetComponent<Collider>();
		}
		_collider.enabled = true;
		_materials = materials;
		_appliedForce = appliedForce;
		_targets = targets;
	}

	private void Awake()
	{
		_collider = GetComponent<Collider>();
		_collider.enabled = false;
	}

	private void Start()
	{
		if ((bool)base.transform.parent && base.transform.parent.TryGetComponent<MeshRenderer>(out var component))
		{
			_cutMaterial = component.sharedMaterial;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		OnTriggerEnterAsync(other, this.GetCancellationTokenOnDestroy()).Forget();
	}

	private async UniTaskVoid OnTriggerEnterAsync(Collider other, CancellationToken ct = default(CancellationToken))
	{
		try
		{
			bool flag = false;
			GameObject[] targets = _targets;
			if (targets != null && targets.Length > 0)
			{
				targets = _targets;
				foreach (GameObject gameObject in targets)
				{
					if (other.gameObject == gameObject)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag || other.CompareTag("Car") || _isCut || other.gameObject.layer == LayerViaName.TransparentFX)
			{
				return;
			}
			_isCut = true;
			_subjectToCut = other.transform;
			_mesh = _subjectToCut.GetComponent<MeshFilter>()?.sharedMesh;
			if (_mesh == null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			_baseVert = _mesh.vertices;
			_baseUv = _mesh.uv;
			_baseTri = _mesh.triangles;
			_side = new bool[_baseVert.Length];
			_localScale = _subjectToCut.localScale;
			_localRotation = _subjectToCut.localEulerAngles;
			_localPosition = _subjectToCut.position;
			_pos = new CutData("Top Half");
			_neg = new CutData("Bottom Half");
			SliceMesh();
			Material[] array = _materials ?? _subjectToCut.GetComponent<MeshRenderer>().sharedMaterials;
			if (isFillGaps && _materials == null)
			{
				_newMaterials = new Material[array.Length + 1];
				for (int j = 0; j < array.Length; j++)
				{
					_newMaterials[j] = array[j];
				}
				_newMaterials[_newMaterials.Length - 1] = _cutMaterial;
			}
			else
			{
				_newMaterials = array;
			}
			_pos.BuildMesh("top side");
			GenerateObject(_pos, isAddRigidbody);
			_neg.BuildMesh("bottom side");
			GenerateObject(_neg, isAddRigidbody);
			UnityEngine.Object.Destroy(_subjectToCut.gameObject);
			if (nextSlicer != null)
			{
				await UniTask.WaitForEndOfFrame(this, ct);
				nextSlicer.ActivateSlicer(_appliedForce, _newMaterials, _pos.gameObject, _neg.gameObject);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		catch (OperationCanceledException)
		{
		}
	}
}
