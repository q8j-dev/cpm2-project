using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using _Car_Parking;
using _Car_Parking.Scripts.Car.Configs;

public class Skidmarks : MonoBehaviour
{
	public class MarkSection
	{
		public Vector3 Pos = Vector3.zero;

		public Vector3 Normal = Vector3.zero;

		public Vector4 Tangent = Vector4.zero;

		public Vector3 PosLeft = Vector3.zero;

		public Vector3 PosRight = Vector3.zero;

		public Color32 Colour;

		public int LastIndex;

		public Surface Surface;

		public float TimeCreated;
	}

	private struct SkidVertex
	{
		public Vector3 Pos;

		public Vector3 Normal;

		public Vector4 Tangent;

		public Color32 Colour;

		public Vector2 Uv;
	}

	[SerializeField]
	private Material skidmarksMaterial;

	public int maxMarks = 2048;

	public float groundOffset = 0.02f;

	private const float MinDistance = 0.1f;

	private const float MinSqrDistance = 0.010000001f;

	private const float MaxSqrDistance = 25f;

	private const float MaxOpacity = 1f;

	public bool Initialized;

	private int _markIndex;

	public MarkSection[] skidmarks;

	private Mesh _marksMesh;

	private MeshRenderer _mr;

	private MeshFilter _mf;

	private static readonly VertexAttributeDescriptor[] VertexLayout = new VertexAttributeDescriptor[5]
	{
		new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
		new VertexAttributeDescriptor(VertexAttribute.Normal),
		new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4),
		new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4),
		new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
	};

	private const MeshUpdateFlags SilentUpdate = MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds;

	private SkidVertex[] _verts;

	private SkidmarksDirtyRange _dirty;

	public static List<Skidmarks> allSkidmarks = new List<Skidmarks>();

	private Color GetColorForSurface(Surface surface)
	{
		return SkidmarkColorContainer.GetColor(surface);
	}

	protected void Awake()
	{
		allSkidmarks.Add(this);
		if (base.transform.position != Vector3.zero)
		{
			base.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		}
	}

	protected void Start()
	{
		Initialize().Forget();
	}

	private async UniTaskVoid Initialize()
	{
		skidmarks = new MarkSection[maxMarks];
		for (int i = 0; i < maxMarks; i++)
		{
			if (i % 4096 == 0)
			{
				await UniTask.Yield();
				if (this == null)
				{
					skidmarks = null;
					return;
				}
			}
			skidmarks[i] = new MarkSection();
		}
		_mf = GetComponent<MeshFilter>();
		_mr = GetComponent<MeshRenderer>();
		if (_mr == null)
		{
			_mr = base.gameObject.AddComponent<MeshRenderer>();
		}
		_marksMesh = new Mesh();
		_marksMesh.MarkDynamic();
		if (_mf == null)
		{
			_mf = base.gameObject.AddComponent<MeshFilter>();
		}
		_mf.sharedMesh = _marksMesh;
		_verts = new SkidVertex[maxMarks * 4];
		for (int j = 0; j < maxMarks; j++)
		{
			_verts[j * 4].Uv = new Vector2(0f, 0f);
			_verts[j * 4 + 1].Uv = new Vector2(1f, 0f);
			_verts[j * 4 + 2].Uv = new Vector2(0f, 1f);
			_verts[j * 4 + 3].Uv = new Vector2(1f, 1f);
		}
		_marksMesh.indexFormat = IndexFormat.UInt32;
		_marksMesh.SetVertexBufferParams(maxMarks * 4, VertexLayout);
		_marksMesh.SetVertexBufferData(_verts, 0, 0, _verts.Length, 0, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
		int[] array = new int[maxMarks * 6];
		for (int k = 0; k < maxMarks; k++)
		{
			array[k * 6] = k * 4;
			array[k * 6 + 1] = k * 4 + 2;
			array[k * 6 + 2] = k * 4 + 1;
			array[k * 6 + 3] = k * 4 + 2;
			array[k * 6 + 4] = k * 4 + 3;
			array[k * 6 + 5] = k * 4 + 1;
		}
		_marksMesh.SetIndexBufferParams(array.Length, IndexFormat.UInt32);
		_marksMesh.SetIndexBufferData(array, 0, 0, array.Length, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
		_marksMesh.subMeshCount = 1;
		_marksMesh.SetSubMesh(0, new SubMeshDescriptor(0, array.Length), MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
		_marksMesh.bounds = new Bounds(Vector3.zero, new Vector3(50000f, 50000f, 50000f));
		_mr.shadowCastingMode = ShadowCastingMode.Off;
		_mr.receiveShadows = false;
		_mr.material = skidmarksMaterial;
		_mr.lightProbeUsage = LightProbeUsage.Off;
		Initialized = true;
	}

	protected void LateUpdate()
	{
		if (Initialized && !FlagsStorage.HideSkidMarks && _dirty.TryGetRange(maxMarks, out var start, out var count))
		{
			_dirty.Clear();
			_marksMesh.SetVertexBufferData(_verts, start * 4, start * 4, count * 4, 0, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
		}
	}

	public static void AllSkidMarksSetActive(bool value)
	{
		foreach (Skidmarks allSkidmark in allSkidmarks)
		{
			if (!(allSkidmark == null))
			{
				allSkidmark.enabled = value;
				if (allSkidmark.TryGetComponent<MeshRenderer>(out var component))
				{
					component.enabled = value;
				}
			}
		}
	}

	public static float ResolveMarkQuadWidth(float markWidth, Surface surface)
	{
		return markWidth * 2f;
	}

	public int AddSkidMark(Vector3 pos, Vector3 normal, float opacity, int lastIndex, float markWidth, Surface surface)
	{
		if (opacity > 1f)
		{
			opacity = 1f;
		}
		else if (opacity < 0f)
		{
			return -1;
		}
		markWidth = ResolveMarkQuadWidth(markWidth, surface);
		Color32 colour = GetColorForSurface(surface);
		colour.a = (byte)(opacity * 255f);
		if (!FlagsStorage.HideSkidMarks)
		{
			return AddSkidMarkInternal(pos, normal, colour, lastIndex, markWidth, surface);
		}
		return _markIndex;
	}

	private int AddSkidMarkInternal(Vector3 pos, Vector3 normal, Color32 colour, int lastIndex, float markWidth, Surface surface)
	{
		if (!Initialized || colour.a == 0)
		{
			return -1;
		}
		MarkSection markSection = null;
		Vector3 lhs = Vector3.zero;
		Vector3 vector = pos - V.GlobalOffset + normal * groundOffset;
		if (lastIndex != -1)
		{
			markSection = skidmarks[lastIndex];
			lhs = vector - markSection.Pos;
			if (lhs.sqrMagnitude < 0.010000001f)
			{
				return lastIndex;
			}
			if (lhs.sqrMagnitude > 25f)
			{
				lastIndex = -1;
				markSection = null;
				lhs = Vector3.zero;
			}
		}
		colour.a = (byte)((float)(int)colour.a * 1f);
		MarkSection markSection2 = skidmarks[_markIndex];
		if (markSection2 != null)
		{
			markSection2.Pos = vector;
			markSection2.Normal = normal;
			markSection2.Colour = colour;
			markSection2.LastIndex = lastIndex;
			markSection2.TimeCreated = Time.time;
			markSection2.Surface = surface;
			if (markSection != null)
			{
				Vector3 normalized = Vector3.Cross(lhs, normal).normalized;
				markSection2.PosLeft = markSection2.Pos + normalized * (markWidth * 0.5f);
				markSection2.PosRight = markSection2.Pos - normalized * (markWidth * 0.5f);
				markSection2.Tangent = new Vector4(normalized.x, normalized.y, normalized.z, 1f);
				if (markSection.LastIndex == -1)
				{
					markSection.Tangent = markSection2.Tangent;
					markSection.PosLeft = markSection2.Pos + normalized * (markWidth * 0.5f);
					markSection.PosRight = markSection2.Pos - normalized * (markWidth * 0.5f);
				}
			}
		}
		UpdateSkidmarksMesh();
		int markIndex = _markIndex;
		_markIndex = ++_markIndex % maxMarks;
		return markIndex;
	}

	private void UpdateSkidmarksMesh()
	{
		if (Initialized)
		{
			MarkSection markSection = skidmarks[_markIndex];
			if (markSection != null && markSection.LastIndex != -1)
			{
				MarkSection markSection2 = skidmarks[markSection.LastIndex];
				int num = _markIndex * 4;
				_verts[num].Pos = markSection2.PosLeft;
				_verts[num + 1].Pos = markSection2.PosRight;
				_verts[num + 2].Pos = markSection.PosLeft;
				_verts[num + 3].Pos = markSection.PosRight;
				_verts[num].Normal = markSection2.Normal;
				_verts[num + 1].Normal = markSection2.Normal;
				_verts[num + 2].Normal = markSection.Normal;
				_verts[num + 3].Normal = markSection.Normal;
				_verts[num].Tangent = markSection2.Tangent;
				_verts[num + 1].Tangent = markSection2.Tangent;
				_verts[num + 2].Tangent = markSection.Tangent;
				_verts[num + 3].Tangent = markSection.Tangent;
				_verts[num].Colour = markSection2.Colour;
				_verts[num + 1].Colour = markSection2.Colour;
				_verts[num + 2].Colour = markSection.Colour;
				_verts[num + 3].Colour = markSection.Colour;
				_dirty.Touch(_markIndex);
			}
		}
	}

	private void OnDestroy()
	{
		allSkidmarks.Remove(this);
		skidmarks = null;
		_verts = null;
		if ((bool)_marksMesh)
		{
			_marksMesh.Clear();
			Object.Destroy(_marksMesh);
			_marksMesh = null;
		}
		if ((bool)_mr)
		{
			Object.Destroy(_mr);
			_mr = null;
		}
		if ((bool)_mf)
		{
			Object.Destroy(_mf);
			_mf = null;
		}
	}
}
