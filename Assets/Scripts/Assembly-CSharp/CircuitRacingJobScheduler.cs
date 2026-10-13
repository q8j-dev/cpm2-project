using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class CircuitRacingJobScheduler : MonoBehaviour
{
	private NativeArray<float3> _positions;

	private NativeArray<float3> _forwards;

	private NativeArray<float3> _rights;

	private NativeArray<float3> _ups;

	private NativeArray<float3> _velocities;

	private NativeArray<float> _speeds;

	private NativeArray<NearbyCarResult> _results;

	private NativeArray<int> _resultCounts;

	private JobHandle _jobHandle;

	private bool _jobScheduled;

	private int _allocatedCount;

	private Rigidbody[] _rigidbodies;

	public List<CircuitCarStearing> Cars { get; private set; } = new List<CircuitCarStearing>();

	public void RegisterCars(List<CircuitCarStearing> cars)
	{
		Cars = cars;
		ReallocateIfNeeded(cars.Count);
		_rigidbodies = new Rigidbody[cars.Count];
		for (int i = 0; i < cars.Count; i++)
		{
			cars[i].TryGetComponent<Rigidbody>(out _rigidbodies[i]);
		}
	}

	private void ReallocateIfNeeded(int count)
	{
		if (count != _allocatedCount)
		{
			DisposeArrays();
			if (count != 0)
			{
				int length = count * 10;
				_positions = new NativeArray<float3>(count, Allocator.Persistent);
				_forwards = new NativeArray<float3>(count, Allocator.Persistent);
				_rights = new NativeArray<float3>(count, Allocator.Persistent);
				_ups = new NativeArray<float3>(count, Allocator.Persistent);
				_velocities = new NativeArray<float3>(count, Allocator.Persistent);
				_speeds = new NativeArray<float>(count, Allocator.Persistent);
				_results = new NativeArray<NearbyCarResult>(length, Allocator.Persistent);
				_resultCounts = new NativeArray<int>(count, Allocator.Persistent);
				_allocatedCount = count;
			}
		}
	}

	private void FixedUpdate()
	{
		int count = Cars.Count;
		if (count == 0)
		{
			CompleteJob();
			return;
		}
		if (_jobScheduled)
		{
			_jobHandle.Complete();
			_jobScheduled = false;
			_ = _allocatedCount;
		}
		ReallocateIfNeeded(count);
		for (int i = 0; i < count; i++)
		{
			CircuitCarStearing circuitCarStearing = Cars[i];
			Rigidbody rigidbody = _rigidbodies[i];
			Transform transform = circuitCarStearing.transform;
			float3 value = (rigidbody ? ((float3)rigidbody.position) : ((float3)transform.position));
			_positions[i] = value;
			_forwards[i] = transform.forward;
			_rights[i] = transform.right;
			_ups[i] = transform.up;
		}
		NearbyCarJob jobData = new NearbyCarJob
		{
			Positions = _positions,
			Forwards = _forwards,
			Rights = _rights,
			Ups = _ups,
			Velocities = _velocities,
			Speeds = _speeds,
			CarCount = count,
			Results = _results,
			ResultCounts = _resultCounts
		};
		_jobHandle = IJobParallelForExtensions.Schedule(jobData, count, 1);
		JobHandle.ScheduleBatchedJobs();
		_jobScheduled = true;
	}

	public void CompleteJob()
	{
		if (_jobScheduled)
		{
			_jobHandle.Complete();
			_jobScheduled = false;
		}
	}

	private void OnDestroy()
	{
		CompleteJob();
		DisposeArrays();
	}

	private void DisposeArrays()
	{
		if (_positions.IsCreated)
		{
			_positions.Dispose();
		}
		if (_forwards.IsCreated)
		{
			_forwards.Dispose();
		}
		if (_rights.IsCreated)
		{
			_rights.Dispose();
		}
		if (_ups.IsCreated)
		{
			_ups.Dispose();
		}
		if (_velocities.IsCreated)
		{
			_velocities.Dispose();
		}
		if (_speeds.IsCreated)
		{
			_speeds.Dispose();
		}
		if (_results.IsCreated)
		{
			_results.Dispose();
		}
		if (_resultCounts.IsCreated)
		{
			_resultCounts.Dispose();
		}
		_allocatedCount = 0;
	}
}
