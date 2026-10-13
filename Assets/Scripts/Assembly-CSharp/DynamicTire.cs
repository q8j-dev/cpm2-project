using System;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class DynamicTire : MonoBehaviour
{
	[Serializable]
	private struct LodLevel
	{
		public MeshFilter Filter;

		public MeshRenderer Renderer;

		public Mesh OriginalMesh;

		public TireFitment.MeshReference Reference;
	}

	public bool UseDeformation;

	public bool MirrorOnRightSide;

	public TireSpinEffect SpinEffect;

	public Mesh OriginalMesh;

	[SerializeField]
	private TireFitment.MeshReference _meshReference = new TireFitment.MeshReference
	{
		InnerDiameter = 0.431f,
		OuterDiameter = 0.64f,
		InnerWidth = 0.22f,
		OuterWidth = 0.19f
	};

	[SerializeField]
	private float _beadSeatAllowance = 0.005f;

	[SerializeField]
	private float _beadWidthAllowance;

	[Range(0.203f, 0.7112f)]
	public float InnerDiameter = 0.46f;

	[Range(0.253f, 1.1f)]
	public float OuterDiameter = 1.63f;

	[Range(0.0762f, 0.381f)]
	public float InnerWidth = 0.26f;

	[Range(0.135f, 0.455f)]
	public float OuterWidth = 0.26f;

	[Range(0f, 1f)]
	public float Wear;

	[Range(0f, 1f)]
	public float DirtAmount;

	private static readonly Vector3 NoContactPlanePos = new Vector3(0f, -100f, 0f);

	[SerializeField]
	private Vector3 PlanePos = NoContactPlanePos;

	[SerializeField]
	private Vector3 PlaneNormal = Vector3.up;

	private MeshRenderer _meshRender;

	public MeshFilter F;

	[SerializeField]
	private LodLevel[] _lodLevels = Array.Empty<LodLevel>();

	private MeshRenderer[] _renderers;

	private Mesh[] _createdLodMeshes;

	private MaterialPropertyBlock _tyreBlock;

	private float _overUseChecker;

	private Material[] _tyreMats;

	private static readonly int PlanePosition = Shader.PropertyToID("_PlanePostition");

	private static readonly int InnerD = Shader.PropertyToID("_InnerD");

	private static readonly int OuterD = Shader.PropertyToID("_OuterD");

	private static readonly int Width = Shader.PropertyToID("_Width");

	private static readonly int Wear1 = Shader.PropertyToID("_Wear");

	private static readonly int Amount = Shader.PropertyToID("_DirtAmount");

	private static readonly int Normal = Shader.PropertyToID("_PlaneNormal");

	private static readonly int LatDeform = Shader.PropertyToID("_LatDeform");

	private static readonly int LongDeform = Shader.PropertyToID("_LongDeform");

	private static readonly int CarcassR = Shader.PropertyToID("_CarcassR");

	private static readonly int MaxDeflectId = Shader.PropertyToID("_MaxDeflect");

	private bool _isVisualInitialized;

	private bool _isPhysicInitialized;

	private int _fitGeneration;

	private Mesh _createdMesh;

	private WheelProvider _wheel;

	private MaterialPropertyBlock _mpb;

	private bool UsesMaterialInstance => _tyreMats != null;

	public void Init(WheelProvider wheel)
	{
		_wheel = wheel;
		UseDeformation = WheelProvider.TyreDeformationAllowed;
		InitMaterialPropertyBlock();
	}

	private bool IsVisualChanged(float wear, float dirt)
	{
		if (!_isVisualInitialized)
		{
			Wear = math.clamp(wear, 0f, 1f);
			DirtAmount = math.clamp(dirt, 0f, 1f);
			_isVisualInitialized = true;
			return _isVisualInitialized;
		}
		int num;
		if (Utils.IsApproximatelyEqual(wear, Wear))
		{
			num = ((!Utils.IsApproximatelyEqual(dirt, DirtAmount)) ? 1 : 0);
			if (num == 0)
			{
				goto IL_0091;
			}
		}
		else
		{
			num = 1;
		}
		Wear = math.clamp(wear, 0f, 1f);
		DirtAmount = math.clamp(dirt, 0f, 1f);
		goto IL_0091;
		IL_0091:
		return (byte)num != 0;
	}

	private bool IsPhysicalChanged(float innerDiameter, float outerDiameter, float innerWidth, float outerWidth)
	{
		if (!_isPhysicInitialized)
		{
			InnerDiameter = math.clamp(innerDiameter, 0.203f, 0.711f);
			OuterDiameter = outerDiameter;
			InnerWidth = math.clamp(innerWidth, 0.0762f, 0.381f);
			OuterWidth = outerWidth;
			_isPhysicInitialized = true;
			return _isPhysicInitialized;
		}
		int num;
		if (Utils.IsApproximatelyEqual(innerDiameter, InnerDiameter) && Utils.IsApproximatelyEqual(outerDiameter, OuterDiameter) && Utils.IsApproximatelyEqual(innerWidth, InnerWidth))
		{
			num = ((!Utils.IsApproximatelyEqual(outerWidth, OuterWidth)) ? 1 : 0);
			if (num == 0)
			{
				goto IL_00cc;
			}
		}
		else
		{
			num = 1;
		}
		InnerDiameter = math.clamp(innerDiameter, 0.203f, 0.711f);
		OuterDiameter = outerDiameter;
		InnerWidth = math.clamp(innerWidth, 0.0762f, 0.381f);
		OuterWidth = outerWidth;
		goto IL_00cc;
		IL_00cc:
		return (byte)num != 0;
	}

	private void OnDestroy()
	{
		TryDestroyMesh();
		if (_tyreMats == null)
		{
			return;
		}
		Material[] tyreMats = _tyreMats;
		foreach (Material material in tyreMats)
		{
			if ((bool)material)
			{
				UnityEngine.Object.Destroy(material);
			}
		}
	}

	private void UpdateDeformParams()
	{
		if (UseDeformation)
		{
			SetProp(PlanePosition, PlanePos);
			SetProp(Normal, PlaneNormal);
		}
	}

	private void SetProp(int id, float value)
	{
		if (_tyreMats != null)
		{
			Material[] tyreMats = _tyreMats;
			foreach (Material material in tyreMats)
			{
				if ((bool)material)
				{
					material.SetFloat(id, value);
				}
			}
		}
		else if (_mpb != null)
		{
			_mpb.SetFloat(id, value);
		}
	}

	private void SetProp(int id, Vector4 value)
	{
		if (_tyreMats != null)
		{
			Material[] tyreMats = _tyreMats;
			foreach (Material material in tyreMats)
			{
				if ((bool)material)
				{
					material.SetVector(id, value);
				}
			}
		}
		else if (_mpb != null)
		{
			_mpb.SetVector(id, value);
		}
	}

	private void ApplyProps()
	{
		if (UsesMaterialInstance || _mpb == null || _renderers == null)
		{
			return;
		}
		MeshRenderer[] renderers = _renderers;
		foreach (MeshRenderer meshRenderer in renderers)
		{
			if ((bool)meshRenderer)
			{
				meshRenderer.SetPropertyBlock(_mpb);
			}
		}
	}

	public void SetContactPlane(Vector3 localPlanePos, Vector3 localPlaneNormal, bool isTouching)
	{
		SetContactPlane(localPlanePos, localPlaneNormal, Vector3.zero, Vector3.zero, isTouching);
	}

	public void SetContactPlane(Vector3 worldPlanePos, Vector3 worldPlaneNormal, Vector3 latDeform, Vector3 longDeform, bool isTouching)
	{
		if (UseDeformation)
		{
			InitMaterialPropertyBlock();
			if (_tyreMats != null || _mpb != null)
			{
				PlanePos = (isTouching ? base.transform.InverseTransformPoint(worldPlanePos) : NoContactPlanePos);
				PlaneNormal = (isTouching ? base.transform.InverseTransformDirection(worldPlaneNormal) : Vector3.zero);
				UpdateDeformParams();
				SetProp(LatDeform, isTouching ? latDeform : Vector3.zero);
				SetProp(LongDeform, isTouching ? longDeform : Vector3.zero);
				ApplyProps();
			}
		}
	}

	private void UpdatePhysicParams()
	{
		InitMaterialPropertyBlock();
		if (_tyreMats != null || _mpb != null)
		{
			SetProp(InnerD, InnerDiameter);
			SetProp(OuterD, OuterDiameter);
			SetProp(Width, math.max(InnerWidth, OuterWidth));
			SetProp(MaxDeflectId, Mathf.Max((OuterDiameter - InnerDiameter) * 0.5f, 0.005f));
			float num = ((_meshReference.OuterDiameter > 0f) ? _meshReference.OuterDiameter : TireFitment.DefaultReference.OuterDiameter);
			float num2 = ((_meshReference.CarcassDiameter > 0f && num > 0f) ? (_meshReference.CarcassDiameter / num) : 1f);
			SetProp(CarcassR, OuterDiameter * 0.5f * num2);
			ApplyProps();
		}
	}

	private void CollectRenderers()
	{
		if (!_meshRender)
		{
			TryGetComponent<MeshRenderer>(out _meshRender);
		}
		int num = (_meshRender ? 1 : 0);
		LodLevel[] lodLevels = _lodLevels;
		for (int i = 0; i < lodLevels.Length; i++)
		{
			if ((bool)lodLevels[i].Renderer)
			{
				num++;
			}
		}
		_renderers = new MeshRenderer[num];
		int num2 = 0;
		if ((bool)_meshRender)
		{
			_renderers[num2++] = _meshRender;
		}
		lodLevels = _lodLevels;
		for (int i = 0; i < lodLevels.Length; i++)
		{
			LodLevel lodLevel = lodLevels[i];
			if ((bool)lodLevel.Renderer)
			{
				_renderers[num2++] = lodLevel.Renderer;
			}
		}
	}

	private void InitMaterialPropertyBlock()
	{
		if (_tyreMats != null || _mpb != null)
		{
			return;
		}
		if (_renderers == null)
		{
			CollectRenderers();
		}
		if (_renderers.Length == 0)
		{
			return;
		}
		if (Application.isPlaying)
		{
			_tyreMats = new Material[_renderers.Length];
			for (int i = 0; i < _renderers.Length; i++)
			{
				_tyreMats[i] = _renderers[i].material;
			}
		}
		else
		{
			_mpb = new MaterialPropertyBlock();
			_renderers[0].GetPropertyBlock(_mpb);
		}
	}

	private void UpdateVisualParams()
	{
		SetProp(Wear1, Wear);
		SetProp(Amount, DirtAmount);
	}

	public async UniTaskVoid SetTireSize(float tyreRadius, float tyreWidth, float rimWidth, float rimRadius, bool isOnline)
	{
		if (!IsPhysicalChanged(rimRadius * 2f, tyreRadius * 2f, rimWidth, tyreWidth))
		{
			return;
		}
		int generation = ++_fitGeneration;
		float outerD = OuterDiameter;
		float outerW = OuterWidth;
		float innerW = InnerWidth;
		float innerD = InnerDiameter;
		Mesh[] createdLods = new Mesh[_lodLevels.Length];
		Mesh created;
		if (isOnline)
		{
			created = await TireFitment.GetFittedTire(OriginalMesh, outerD, outerW, innerW, innerD, _meshReference);
			for (int i = 0; i < _lodLevels.Length; i++)
			{
				if (generation != _fitGeneration)
				{
					break;
				}
				if (IsLodFittable(_lodLevels[i]))
				{
					Mesh[] array = createdLods;
					int num = i;
					array[num] = await TireFitment.GetFittedTire(_lodLevels[i].OriginalMesh, outerD, outerW, innerW, innerD, _lodLevels[i].Reference);
				}
			}
		}
		else
		{
			created = TireFitment.GetRecalculateTire(OriginalMesh, outerD, outerW, innerW, innerD, _meshReference);
			for (int j = 0; j < _lodLevels.Length; j++)
			{
				if (IsLodFittable(_lodLevels[j]))
				{
					createdLods[j] = TireFitment.GetRecalculateTire(_lodLevels[j].OriginalMesh, outerD, outerW, innerW, innerD, _lodLevels[j].Reference);
				}
			}
		}
		if (generation != _fitGeneration || !F)
		{
			DestroyMeshes(created, createdLods);
			return;
		}
		TryDestroyMesh();
		_createdMesh = created;
		_createdLodMeshes = createdLods;
		F.sharedMesh = _createdMesh;
		SpinEffect.Init(_wheel, F.sharedMesh);
		for (int k = 0; k < _lodLevels.Length; k++)
		{
			if ((bool)_createdLodMeshes[k] && (bool)_lodLevels[k].Filter)
			{
				_lodLevels[k].Filter.sharedMesh = _createdLodMeshes[k];
			}
		}
		UpdatePhysicParams();
		_overUseChecker++;
	}

	private static void DestroyMeshes(Mesh mesh, Mesh[] meshes)
	{
		if ((bool)mesh)
		{
			UnityEngine.Object.Destroy(mesh);
		}
		if (meshes == null)
		{
			return;
		}
		foreach (Mesh mesh2 in meshes)
		{
			if ((bool)mesh2)
			{
				UnityEngine.Object.Destroy(mesh2);
			}
		}
	}

	private bool IsLodFittable(LodLevel lod)
	{
		if (!lod.Filter || !lod.OriginalMesh)
		{
			return false;
		}
		if (lod.OriginalMesh.isReadable)
		{
			return true;
		}
		Debug.LogError("[DynamicTire] Меш уровня '" + lod.OriginalMesh.name + "' не Read/Write — включите флаг в импортере модели, иначе уровень останется исходного размера", this);
		return false;
	}

	private void TryDestroyMesh()
	{
		if ((bool)_createdMesh)
		{
			UnityEngine.Object.Destroy(_createdMesh);
		}
		if (_createdLodMeshes == null)
		{
			return;
		}
		Mesh[] createdLodMeshes = _createdLodMeshes;
		foreach (Mesh mesh in createdLodMeshes)
		{
			if ((bool)mesh)
			{
				UnityEngine.Object.Destroy(mesh);
			}
		}
	}

	public void SetTireWearAndCollision(float wear, float dirt)
	{
		if (IsVisualChanged(1f - wear, dirt))
		{
			InitMaterialPropertyBlock();
			if (_tyreMats != null || _mpb != null)
			{
				UpdateVisualParams();
				ApplyProps();
			}
		}
	}
}
