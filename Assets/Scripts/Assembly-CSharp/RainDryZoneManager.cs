using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class RainDryZoneManager
{
	private struct WorldBox
	{
		public Vector3 center;

		public Vector3 halfExt;

		public float cos;

		public float sin;

		public float radius;

		public Vector3 feather;

		public Vector3 invFeather;

		public float wobble;

		public float reach;

		public float reachSq;
	}

	public const int MaxZones = 16;

	public static float SelectRadius = 150f;

	private const float CellSize = 8f;

	private static readonly int ID_A = Shader.PropertyToID("_RainDryZoneA");

	private static readonly int ID_B = Shader.PropertyToID("_RainDryZoneB");

	private static readonly int ID_Count = Shader.PropertyToID("_RainDryZoneCount");

	private static readonly int ID_C = Shader.PropertyToID("_RainDryZoneC");

	private static readonly int ID_D = Shader.PropertyToID("_RainDryZoneD");

	private static readonly int ID_Bounds = Shader.PropertyToID("_RainDryZoneBounds");

	private static readonly int ID_Gamma = Shader.PropertyToID("_RainDryZoneEdgeGamma");

	private static readonly int ID_WobFreq = Shader.PropertyToID("_RainDryZoneWobbleFreq");

	private static readonly List<RainDryZone> _zones = new List<RainDryZone>();

	private static readonly List<WorldBox> _boxes = new List<WorldBox>();

	private static bool _boxesDirty = true;

	private static int _boxesVersion;

	private static int _uploadedVersion = -1;

	private static readonly Vector4[] _a = new Vector4[16];

	private static readonly Vector4[] _b = new Vector4[16];

	private static readonly Vector4[] _c = new Vector4[16];

	private static readonly Vector4[] _d = new Vector4[16];

	private static readonly List<KeyValuePair<float, int>> _byDist = new List<KeyValuePair<float, int>>();

	private static readonly Comparison<KeyValuePair<float, int>> _cmpDist = (KeyValuePair<float, int> x, KeyValuePair<float, int> y) => x.Key.CompareTo(y.Key);

	private static readonly int[] _sel = new int[16];

	private static readonly int[] _lastSel = new int[16];

	private static int _lastCount = -1;

	private static Vector3 _feather = new Vector3(8f, 2f, 8f);

	private static float _wobble = 1.2f;

	private static float _edgeGamma = 4f;

	private static float _wobbleFreq = 0.5f;

	private static bool _hooked;

	private static Vector3 _lastCamCell = new Vector3(float.NaN, 0f, 0f);

	private static Vector3 _lastGlobalOffset = new Vector3(float.NaN, 0f, 0f);

	public static Vector3 EdgeFeather
	{
		get
		{
			return _feather;
		}
		set
		{
			Vector3 vector = Vector3.Max(value, new Vector3(0.001f, 0.001f, 0.001f));
			if (!(vector == _feather))
			{
				_feather = vector;
				ForceReupload();
			}
		}
	}

	public static float EdgeWobble
	{
		get
		{
			return _wobble;
		}
		set
		{
			float num = Mathf.Max(0f, value);
			if (num != _wobble)
			{
				_wobble = num;
				ForceReupload();
			}
		}
	}

	public static float WobbleFrequency
	{
		get
		{
			return _wobbleFreq;
		}
		set
		{
			float num = Mathf.Max(0f, value);
			if (num != _wobbleFreq)
			{
				_wobbleFreq = num;
				ForceReupload();
			}
		}
	}

	public static float EdgeGamma
	{
		get
		{
			return _edgeGamma;
		}
		set
		{
			float num = Mathf.Max(1f, value);
			if (num != _edgeGamma)
			{
				_edgeGamma = num;
				ForceReupload();
			}
		}
	}

	public static void SetEdgeFeather(float uniform)
	{
		EdgeFeather = new Vector3(uniform, uniform, uniform);
	}

	private static void ForceReupload()
	{
		_boxesDirty = true;
		_lastCount = -1;
		InvalidateSelection();
	}

	public static void Register(RainDryZone zone)
	{
		if (!(zone == null) && !_zones.Contains(zone))
		{
			_zones.Add(zone);
			_boxesDirty = true;
			EnsureHook();
			InvalidateSelection();
		}
	}

	public static void Unregister(RainDryZone zone)
	{
		if (_zones.Remove(zone))
		{
			_boxesDirty = true;
			InvalidateSelection();
		}
	}

	public static void MarkDirty()
	{
		_boxesDirty = true;
		InvalidateSelection();
	}

	private static void InvalidateSelection()
	{
		_lastCamCell = new Vector3(float.NaN, 0f, 0f);
	}

	private static void EnsureHook()
	{
		if (!_hooked)
		{
			RenderPipelineManager.beginCameraRendering += OnBeginCamera;
			_hooked = true;
		}
	}

	private static void CheckOriginShift()
	{
		if (!(V.GlobalOffset == _lastGlobalOffset))
		{
			_lastGlobalOffset = V.GlobalOffset;
			_boxesDirty = true;
			_lastCamCell = new Vector3(float.NaN, 0f, 0f);
		}
	}

	private static void OnBeginCamera(ScriptableRenderContext ctx, Camera cam)
	{
		if (cam.cameraType == CameraType.Game && cam.targetTexture == null)
		{
			CheckOriginShift();
			Vector3 position = cam.transform.position;
			Vector3 vector = new Vector3(Mathf.Floor(position.x / 8f), Mathf.Floor(position.y / 8f), Mathf.Floor(position.z / 8f));
			if (!(vector == _lastCamCell))
			{
				_lastCamCell = vector;
				SelectAndUpload(position);
			}
		}
	}

	private static void RebuildBoxes()
	{
		_boxes.Clear();
		for (int i = 0; i < _zones.Count; i++)
		{
			RainDryZone rainDryZone = _zones[i];
			if (!(rainDryZone == null) && rainDryZone.isActiveAndEnabled)
			{
				rainDryZone.GetWorldBox(out var worldCenter, out var halfExtents, out var yawRad);
				rainDryZone.GetEdgeFade(_feather, _wobble, out var feather, out var wobble);
				float magnitude = halfExtents.magnitude;
				float magnitude2 = new Vector3(halfExtents.x + 0.5f * feather.x + wobble, halfExtents.y + 0.5f * feather.y, halfExtents.z + 0.5f * feather.z + wobble).magnitude;
				_boxes.Add(new WorldBox
				{
					center = worldCenter,
					halfExt = halfExtents,
					cos = Mathf.Cos(yawRad),
					sin = Mathf.Sin(yawRad),
					radius = magnitude,
					feather = feather,
					invFeather = new Vector3(1f / feather.x, 1f / feather.y, 1f / feather.z),
					wobble = wobble,
					reach = magnitude2,
					reachSq = magnitude2 * magnitude2
				});
			}
		}
		_boxesDirty = false;
		_boxesVersion++;
	}

	private static void SelectAndUpload(Vector3 camPos)
	{
		if (_boxesDirty)
		{
			RebuildBoxes();
		}
		int count = _boxes.Count;
		bool flag = SelectRadius > 0f;
		int num;
		if (count <= 16 && !flag)
		{
			num = count;
			for (int i = 0; i < num; i++)
			{
				_sel[i] = i;
			}
		}
		else
		{
			float num2 = (flag ? SelectRadius : float.MaxValue);
			_byDist.Clear();
			for (int j = 0; j < count; j++)
			{
				float num3 = (_boxes[j].center - camPos).magnitude - _boxes[j].radius;
				if (num3 <= num2)
				{
					_byDist.Add(new KeyValuePair<float, int>(num3, j));
				}
			}
			_byDist.Sort(_cmpDist);
			num = Mathf.Min(_byDist.Count, 16);
			for (int k = 0; k < num; k++)
			{
				_sel[k] = _byDist[k].Value;
			}
		}
		Array.Sort(_sel, 0, num);
		if (_uploadedVersion == _boxesVersion && num == _lastCount)
		{
			bool flag2 = true;
			for (int l = 0; l < num; l++)
			{
				if (_sel[l] != _lastSel[l])
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				return;
			}
		}
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		for (int m = 0; m < num; m++)
		{
			WorldBox worldBox = _boxes[_sel[m]];
			_a[m] = new Vector4(worldBox.center.x, worldBox.center.y, worldBox.center.z, worldBox.cos);
			_b[m] = new Vector4(worldBox.halfExt.x, worldBox.halfExt.y, worldBox.halfExt.z, worldBox.sin);
			_c[m] = new Vector4(worldBox.invFeather.x, worldBox.invFeather.y, worldBox.invFeather.z, worldBox.wobble);
			_d[m] = new Vector4(worldBox.reachSq, 0f, 0f, 0f);
			Vector3 vector3 = new Vector3(worldBox.reach, worldBox.reach, worldBox.reach);
			if (m == 0)
			{
				vector = worldBox.center - vector3;
				vector2 = worldBox.center + vector3;
			}
			else
			{
				vector = Vector3.Min(vector, worldBox.center - vector3);
				vector2 = Vector3.Max(vector2, worldBox.center + vector3);
			}
		}
		for (int n = num; n < 16; n++)
		{
			_a[n] = Vector4.zero;
			_b[n] = Vector4.zero;
			_c[n] = Vector4.zero;
			_d[n] = Vector4.zero;
		}
		Vector3 vector4 = (vector + vector2) * 0.5f;
		float num4 = 0f;
		for (int num5 = 0; num5 < num; num5++)
		{
			WorldBox worldBox2 = _boxes[_sel[num5]];
			num4 = Mathf.Max(num4, (worldBox2.center - vector4).magnitude + worldBox2.reach);
		}
		Shader.SetGlobalVectorArray(ID_A, _a);
		Shader.SetGlobalVectorArray(ID_B, _b);
		Shader.SetGlobalVectorArray(ID_C, _c);
		Shader.SetGlobalVectorArray(ID_D, _d);
		Shader.SetGlobalVector(ID_Bounds, new Vector4(vector4.x, vector4.y, vector4.z, num4 * num4));
		Shader.SetGlobalInt(ID_Count, num);
		Shader.SetGlobalFloat(ID_Gamma, _edgeGamma);
		Shader.SetGlobalFloat(ID_WobFreq, _wobbleFreq);
		Array.Copy(_sel, _lastSel, num);
		_lastCount = num;
		_uploadedVersion = _boxesVersion;
	}

	public static float WetMaskAt(Vector3 wpos)
	{
		CheckOriginShift();
		if (_boxesDirty)
		{
			RebuildBoxes();
		}
		int count = _boxes.Count;
		if (count == 0)
		{
			return 1f;
		}
		float num = 0f;
		if (_wobbleFreq > 0f)
		{
			float num2 = wpos.x * _wobbleFreq;
			float num3 = wpos.z * _wobbleFreq;
			num = Mathf.Sin(num2 + 1.7f * num3) * 0.62f + Mathf.Sin(2.13f * num2 - 1.31f * num3 + 2.4f) * 0.38f;
		}
		float num4 = 0f;
		for (int i = 0; i < count; i++)
		{
			WorldBox worldBox = _boxes[i];
			float num5 = wpos.x - worldBox.center.x;
			float num6 = wpos.y - worldBox.center.y;
			float num7 = wpos.z - worldBox.center.z;
			if (!(num5 * num5 + num6 * num6 + num7 * num7 > worldBox.reach * worldBox.reach))
			{
				float f = num5 * worldBox.cos - num7 * worldBox.sin;
				float f2 = num5 * worldBox.sin + num7 * worldBox.cos;
				float num8 = Mathf.Abs(f) + worldBox.wobble * num;
				float num9 = Mathf.Abs(f2) + worldBox.wobble * num;
				float num10 = Smooth(Mathf.Clamp01((worldBox.halfExt.x - num8) * worldBox.invFeather.x + 0.5f));
				float num11 = Smooth(Mathf.Clamp01((worldBox.halfExt.y - Mathf.Abs(num6)) * worldBox.invFeather.y + 0.5f));
				float num12 = Smooth(Mathf.Clamp01((worldBox.halfExt.z - num9) * worldBox.invFeather.z + 0.5f));
				num4 = Mathf.Max(num4, num10 * num11 * num12);
				if (num4 >= 0.999f)
				{
					return 0f;
				}
			}
		}
		if (num4 <= 0f)
		{
			return 1f;
		}
		return Mathf.Pow(1f - num4, Mathf.Max(1f, _edgeGamma));
	}

	private static float Smooth(float t)
	{
		return t * t * (3f - 2f * t);
	}
}
