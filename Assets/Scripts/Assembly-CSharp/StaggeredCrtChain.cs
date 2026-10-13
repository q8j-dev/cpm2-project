using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteAlways]
public class StaggeredCrtChain : MonoBehaviour
{
	public enum ZoneOrder
	{
		RowMajor = 0,
		Checkerboard = 1,
		ShuffledStable = 2
	}

	[Header("Custom Render Textures")]
	[Tooltip("Heavy CRT (raymarching). Updated piecewise, one batch of zones per frame.")]
	public CustomRenderTexture simulationCRT;

	[Tooltip("Cheap CRT. Updated in a single full pass once per completed grid cycle. Sample THIS one in your final rendering.")]
	public CustomRenderTexture compositeCRT;

	[Header("Update Zone Grid (simulation CRT)")]
	[Min(1f)]
	public int gridColumns = 4;

	[Min(1f)]
	public int gridRows = 4;

	[Min(1f)]
	[Tooltip("How many grid cells are refreshed each frame.")]
	public int zonesPerFrame = 1;

	[Tooltip("Order in which cells are visited. Checkerboard/Shuffled make the progressive refresh less visible than a row sweep.")]
	public ZoneOrder zoneOrder;

	[Tooltip("Seed for ShuffledStable ordering.")]
	public int shuffleSeed = 1234;

	[Tooltip("Shader pass used when updating zones on the simulation CRT.")]
	[Min(0f)]
	public int simulationPassIndex;

	[Tooltip("OFF (safe): composite updates the frame AFTER the last zone, so it is guaranteed to see the finished simulation.\nON: composite is queued in the same frame as the last zone batch (one frame less latency) - but because the two CRTs depend on each other, Unity's update order between them within that frame is not guaranteed.")]
	[Header("Cycle")]
	public bool compositeOnSameFrameAsLastZone;

	[Header("Startup")]
	[Tooltip("Queue Initialize() (init color/texture) for both CRTs on enable.")]
	public bool initializeOnEnable = true;

	[Tooltip("Also do ONE full-texture simulation update on enable. Heavy single frame, but content is valid immediately instead of after the first full cycle.")]
	public bool fullPrimeOnEnable;

	[Tooltip("Force updateMode / initializationMode = OnDemand and zone space = Normalized on both CRT assets. Prevents Unity's Realtime mode from re-rendering / re-initializing the whole texture every frame behind this script's back.")]
	public bool enforceCrtSettings = true;

	[Header("Edit Mode")]
	[Tooltip("Keep the player loop ticking in Edit Mode so the chain runs without entering Play Mode.")]
	public new bool runInEditMode = true;

	[Tooltip("Repaint Scene views every editor tick so the result is visible while editing.")]
	public bool repaintSceneViews = true;

	private readonly List<CustomRenderTextureUpdateZone[]> _batches = new List<CustomRenderTextureUpdateZone[]>();

	private int _step;

	private bool _compositePending;

	private bool _dirty = true;

	private bool _primePending;

	public int StepsPerCycle => _batches.Count;

	public int CurrentStep => _step;

	public int CompletedCycles { get; private set; }

	public float CycleProgress01
	{
		get
		{
			if (_batches.Count != 0)
			{
				return Mathf.Clamp01((float)_step / (float)_batches.Count);
			}
			return 0f;
		}
	}

	public event Action CycleCompleted;

	private void OnEnable()
	{
		_step = 0;
		_compositePending = false;
		_dirty = true;
		Apply();
		if (initializeOnEnable)
		{
			if (simulationCRT != null)
			{
				simulationCRT.Initialize();
			}
			if (compositeCRT != null)
			{
				compositeCRT.Initialize();
			}
		}
		_primePending = fullPrimeOnEnable;
	}

	private void OnDisable()
	{
	}

	private void OnValidate()
	{
		gridColumns = Mathf.Max(1, gridColumns);
		gridRows = Mathf.Max(1, gridRows);
		zonesPerFrame = Mathf.Clamp(zonesPerFrame, 1, gridColumns * gridRows);
		_dirty = true;
	}

	private void LateUpdate()
	{
		if (simulationCRT == null)
		{
			return;
		}
		if (_dirty)
		{
			Apply();
		}
		if (_primePending)
		{
			_primePending = false;
			ForceFullUpdate();
		}
		else
		{
			if (_batches.Count == 0)
			{
				return;
			}
			if (_compositePending)
			{
				KickComposite();
				_compositePending = false;
				return;
			}
			simulationCRT.SetUpdateZones(_batches[_step]);
			simulationCRT.Update(1);
			_step++;
			if (_step >= _batches.Count)
			{
				_step = 0;
				if (compositeOnSameFrameAsLastZone)
				{
					KickComposite();
				}
				else
				{
					_compositePending = true;
				}
			}
		}
	}

	private void KickComposite()
	{
		if (compositeCRT != null)
		{
			compositeCRT.Update(1);
		}
		CompletedCycles++;
		if (CycleCompleted != null)
		{
			CycleCompleted();
		}
	}

	private void Apply()
	{
		_dirty = false;
		EnforceSettings();
		RebuildBatches();
		if (_step >= _batches.Count)
		{
			_step = 0;
		}
	}

	private void EnforceSettings()
	{
		if (simulationCRT != null && compositeCRT != null && simulationCRT == compositeCRT)
		{
			Debug.LogError("[StaggeredCrtChain] simulationCRT and compositeCRT must be two different assets.", this);
		}
		if (simulationCRT != null)
		{
			if (enforceCrtSettings)
			{
				simulationCRT.updateMode = CustomRenderTextureUpdateMode.OnDemand;
				simulationCRT.initializationMode = CustomRenderTextureUpdateMode.OnDemand;
				simulationCRT.updateZoneSpace = CustomRenderTextureUpdateZoneSpace.Normalized;
			}
			if (simulationCRT.doubleBuffered)
			{
				Debug.LogWarning("[StaggeredCrtChain] '" + simulationCRT.name + "' has Double Buffered enabled. This script implements double buffering manually with two CRTs - disable it on the asset.", this);
			}
		}
		if (compositeCRT != null)
		{
			if (enforceCrtSettings)
			{
				compositeCRT.updateMode = CustomRenderTextureUpdateMode.OnDemand;
				compositeCRT.initializationMode = CustomRenderTextureUpdateMode.OnDemand;
			}
			compositeCRT.ClearUpdateZones();
			if (compositeCRT.doubleBuffered)
			{
				Debug.LogWarning("[StaggeredCrtChain] '" + compositeCRT.name + "' has Double Buffered enabled. Disable it on the asset.", this);
			}
		}
	}

	private void RebuildBatches()
	{
		_batches.Clear();
		if (simulationCRT == null)
		{
			return;
		}
		int num = Mathf.Max(1, gridColumns);
		int num2 = Mathf.Max(1, gridRows);
		int num3 = num * num2;
		int num4 = Mathf.Clamp(zonesPerFrame, 1, num3);
		List<int> list = new List<int>(num3);
		switch (zoneOrder)
		{
		case ZoneOrder.Checkerboard:
		{
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < num2; k++)
				{
					for (int l = 0; l < num; l++)
					{
						if (((k + l) & 1) == j)
						{
							list.Add(k * num + l);
						}
					}
				}
			}
			break;
		}
		case ZoneOrder.ShuffledStable:
		{
			for (int m = 0; m < num3; m++)
			{
				list.Add(m);
			}
			System.Random random = new System.Random(shuffleSeed);
			for (int num5 = num3 - 1; num5 > 0; num5--)
			{
				int index = random.Next(num5 + 1);
				int value = list[num5];
				list[num5] = list[index];
				list[index] = value;
			}
			break;
		}
		default:
		{
			for (int i = 0; i < num3; i++)
			{
				list.Add(i);
			}
			break;
		}
		}
		float num6 = 1f / (float)num;
		float num7 = 1f / (float)num2;
		for (int n = 0; n < num3; n += num4)
		{
			int num8 = Mathf.Min(num4, num3 - n);
			CustomRenderTextureUpdateZone[] array = new CustomRenderTextureUpdateZone[num8];
			for (int num9 = 0; num9 < num8; num9++)
			{
				int num10 = list[n + num9];
				int num11 = num10 / num;
				int num12 = num10 % num;
				array[num9] = new CustomRenderTextureUpdateZone
				{
					updateZoneCenter = new Vector3(((float)num12 + 0.5f) * num6, ((float)num11 + 0.5f) * num7, 0f),
					updateZoneSize = new Vector3(num6, num7, 1f),
					rotation = 0f,
					passIndex = simulationPassIndex,
					needSwap = false
				};
			}
			_batches.Add(array);
		}
	}

	[ContextMenu("Restart Cycle")]
	public void RestartCycle()
	{
		_step = 0;
		_compositePending = false;
		_dirty = true;
	}

	[ContextMenu("Force Full Update Now (heavy)")]
	public void ForceFullUpdate()
	{
		if (!(simulationCRT == null))
		{
			simulationCRT.ClearUpdateZones();
			simulationCRT.Update(1);
			_step = 0;
			_compositePending = true;
		}
	}
}
