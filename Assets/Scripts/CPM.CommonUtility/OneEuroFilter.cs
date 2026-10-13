using System;
using UnityEngine;

public sealed class OneEuroFilter
{
	private readonly float _minCutoff;

	private readonly float _beta;

	private readonly float _derivativeCutoff;

	private float _previousValue;

	private float _filteredValue;

	private float _filteredDerivative;

	private bool _initialized;

	public OneEuroFilter(float minCutoff = 1.2f, float beta = 0.02f, float derivativeCutoff = 1f)
	{
		_minCutoff = Mathf.Max(0.0001f, minCutoff);
		_beta = Mathf.Max(0f, beta);
		_derivativeCutoff = Mathf.Max(0.0001f, derivativeCutoff);
	}

	public void Reset()
	{
		_initialized = false;
	}

	public float Filter(float value, float deltaTime)
	{
		if (deltaTime <= 0f)
		{
			if (!_initialized)
			{
				return value;
			}
			return _filteredValue;
		}
		if (!_initialized)
		{
			_initialized = true;
			_previousValue = value;
			_filteredValue = value;
			_filteredDerivative = 0f;
			return value;
		}
		float value2 = (value - _previousValue) / deltaTime;
		_filteredDerivative = LowPass(_filteredDerivative, value2, Alpha(_derivativeCutoff, deltaTime));
		float cutoff = _minCutoff + _beta * Mathf.Abs(_filteredDerivative);
		_filteredValue = LowPass(_filteredValue, value, Alpha(cutoff, deltaTime));
		_previousValue = value;
		return _filteredValue;
	}

	private static float Alpha(float cutoff, float deltaTime)
	{
		float num = 1f / ((float)Math.PI * 2f * cutoff);
		return 1f / (1f + num / deltaTime);
	}

	private static float LowPass(float previous, float value, float alpha)
	{
		return alpha * value + (1f - alpha) * previous;
	}
}
