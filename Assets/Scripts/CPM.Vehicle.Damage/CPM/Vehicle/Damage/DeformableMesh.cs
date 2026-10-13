using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace CPM.Vehicle.Damage
{
	public sealed class DeformableMesh : IDisposable
	{
		private readonly Transform _transform;

		private DamageTopology _topo;

		private NativeArray<float3> _current;

		private NativeArray<float> _invMass;

		private NativeArray<bool> _weldedDirty;

		private NativeArray<float3> _rawPositions;

		private NativeArray<bool> _rawDirty;

		private NativeArray<float3> _normals;

		private NativeArray<float3> _restNormals;

		private NativeArray<int> _anyDirty;

		private JobHandle _handle;

		private bool _pending;

		private bool _settling;

		private bool _disposed;

		private readonly Bounds _localBounds;

		public Mesh Mesh { get; }

		public bool Damaged { get; private set; }

		private bool Alive
		{
			get
			{
				if (Mesh != null)
				{
					return _transform != null;
				}
				return false;
			}
		}

		private static float UniformScale(Transform transform)
		{
			return math.max(0.0001f, math.abs(transform.lossyScale.x));
		}

		public DeformableMesh(Mesh mesh, Transform transform, in DamageSettings settings)
		{
			Mesh = mesh;
			_transform = transform;
			try
			{
				_topo = DamageTopologyFactory.FromMesh(mesh, in settings, Allocator.Persistent);
				int weldedCount = _topo.Weld.WeldedCount;
				int rawCount = _topo.RawCount;
				_current = new NativeArray<float3>(weldedCount, Allocator.Persistent);
				_current.CopyFrom(_topo.Weld.WeldedPositions);
				_invMass = new NativeArray<float>(weldedCount, Allocator.Persistent);
				_weldedDirty = new NativeArray<bool>(weldedCount, Allocator.Persistent);
				_rawPositions = new NativeArray<float3>(rawCount, Allocator.Persistent);
				_rawDirty = new NativeArray<bool>(rawCount, Allocator.Persistent);
				_normals = new NativeArray<float3>(rawCount, Allocator.Persistent);
				_restNormals = new NativeArray<float3>(rawCount, Allocator.Persistent);
				Vector3[] normals = mesh.normals;
				for (int i = 0; i < rawCount && i < normals.Length; i++)
				{
					_restNormals[i] = normals[i];
				}
				_normals.CopyFrom(_restNormals);
				_anyDirty = new NativeArray<int>(1, Allocator.Persistent);
				mesh.RecalculateBounds();
				_localBounds = mesh.bounds;
				Bounds localBounds = _localBounds;
				localBounds.Expand(settings.MaxDepth / UniformScale(transform) * 2f);
				mesh.bounds = localBounds;
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		public bool ScheduleImpact(in ImpactEvent impact, in DamageSettings settings)
		{
			if (_disposed || !Alive || _topo.Weld.WeldedCount == 0)
			{
				return false;
			}
			float num = UniformScale(_transform);
			Vector3 center = _transform.InverseTransformPoint(new Vector3(impact.WorldPoint.x, impact.WorldPoint.y, impact.WorldPoint.z));
			Vector3 vector = _transform.InverseTransformDirection(new Vector3(impact.WorldDir.x, impact.WorldDir.y, impact.WorldDir.z));
			float num2 = impact.Radius / num;
			float num3 = math.max(1f, impact.SlideStretch);
			float num4 = num2 * 1.5f * num3;
			if (!_localBounds.Intersects(new Bounds(center, Vector3.one * (num4 * 2f))))
			{
				return false;
			}
			float3 x = new float3(vector.x, vector.y, vector.z);
			if (math.lengthsq(x) < 1E-08f)
			{
				return false;
			}
			x = math.normalize(x);
			float3 slideAxis = new float3(0f, 0f, 1f);
			if (num3 > 1f && math.lengthsq(impact.SlideAxis) > 1E-08f)
			{
				Vector3 vector2 = _transform.InverseTransformDirection(new Vector3(impact.SlideAxis.x, impact.SlideAxis.y, impact.SlideAxis.z));
				if (vector2.sqrMagnitude > 1E-08f)
				{
					slideAxis = math.normalize(new float3(vector2.x, vector2.y, vector2.z));
				}
				else
				{
					num3 = 1f;
				}
			}
			else
			{
				num3 = 1f;
			}
			_handle.Complete();
			_settling = false;
			JobHandle dependsOn = IJobParallelForExtensions.Schedule(new SeedJob
			{
				Rest = _topo.Weld.WeldedPositions,
				Stiffness = _topo.VertexStiffness,
				Current = _current,
				InvMass = _invMass,
				ImpactPoint = new float3(center.x, center.y, center.z),
				ImpactDir = x,
				Depth = impact.Depth / num,
				Radius = num2,
				RimRatio = settings.RimRatio,
				SlideAxis = slideAxis,
				SlideStretch = num3
			}, _topo.Weld.WeldedCount, 64);
			JobHandle dependsOn2 = new RelaxJob
			{
				Current = _current,
				Edges = _topo.Edges.Edges,
				RestLength = _topo.Edges.RestLength,
				Stiffness = _topo.Edges.Stiffness01,
				InvMass = _invMass,
				EdgeCount = _topo.Edges.Count,
				Iterations = math.max(1, settings.SolverIterations),
				YieldStrain = settings.YieldStrain,
				PlasticRate = settings.PlasticRate
			}.Schedule(dependsOn);
			JobHandle deps = IJobParallelForExtensions.Schedule(new ClampAndNoiseJob
			{
				Rest = _topo.Weld.WeldedPositions,
				Stiffness = _topo.VertexStiffness,
				InvMass = _invMass,
				Current = _current,
				WeldedDirty = _weldedDirty,
				MaxDeform = settings.MaxDepth / num,
				NoiseAmplitude = settings.NoiseAmplitude / num,
				DirtyEpsilon = 0.0001f
			}, _topo.Weld.WeldedCount, 64, dependsOn2);
			_handle = ScheduleScatterAndNormals(deps);
			_pending = true;
			Damaged = true;
			return true;
		}

		private JobHandle ScheduleScatterAndNormals(JobHandle deps)
		{
			JobHandle dependsOn = IJobParallelForExtensions.Schedule(new ScatterJob
			{
				WeldedOf = _topo.Weld.WeldedOf,
				Welded = _current,
				WeldedDirty = _weldedDirty,
				RawPositions = _rawPositions,
				RawDirty = _rawDirty
			}, _topo.RawCount, 128, deps);
			return new NormalsJob
			{
				Triangles = _topo.Triangles,
				RawPositions = _rawPositions,
				RawDirty = _rawDirty,
				Normals = _normals
			}.Schedule(dependsOn);
		}

		public bool ScheduleRepair(float step, float sleepCap)
		{
			if (_disposed || !Alive || !Damaged)
			{
				return false;
			}
			_handle.Complete();
			new RepairJob
			{
				Rest = _topo.Weld.WeldedPositions,
				WeldedDirty = _weldedDirty,
				Current = _current,
				AnyDirty = _anyDirty,
				Step = math.saturate(step),
				SleepSqr = sleepCap * sleepCap
			}.Run();
			_settling = _anyDirty[0] == 0;
			_handle = ScheduleScatterAndNormals(default);
			_pending = true;
			return true;
		}

		public void CompleteAndApply()
		{
			if (_disposed || !_pending)
			{
				return;
			}
			_handle.Complete();
			_pending = false;
			if (!Alive)
			{
				return;
			}
			if (_settling)
			{
				for (int i = 0; i < _weldedDirty.Length; i++)
				{
					_weldedDirty[i] = false;
				}
				for (int j = 0; j < _rawDirty.Length; j++)
				{
					_rawDirty[j] = false;
				}
				_normals.CopyFrom(_restNormals);
				_settling = false;
				Damaged = false;
			}
			Mesh.SetVertices(_rawPositions.Reinterpret<Vector3>(), 0, _rawPositions.Length, MeshUpdateFlags.DontRecalculateBounds);
			Mesh.SetNormals(_normals.Reinterpret<Vector3>(), 0, _normals.Length, MeshUpdateFlags.DontRecalculateBounds);
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_handle.Complete();
				_topo.Dispose();
				if (_current.IsCreated)
				{
					_current.Dispose();
				}
				if (_invMass.IsCreated)
				{
					_invMass.Dispose();
				}
				if (_weldedDirty.IsCreated)
				{
					_weldedDirty.Dispose();
				}
				if (_rawPositions.IsCreated)
				{
					_rawPositions.Dispose();
				}
				if (_rawDirty.IsCreated)
				{
					_rawDirty.Dispose();
				}
				if (_normals.IsCreated)
				{
					_normals.Dispose();
				}
				if (_restNormals.IsCreated)
				{
					_restNormals.Dispose();
				}
				if (_anyDirty.IsCreated)
				{
					_anyDirty.Dispose();
				}
			}
		}
	}
}
