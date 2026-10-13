using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.CpmCustomFeatures.Benchmarking
{
	public sealed class MonoBenchmark : MonoBehaviour
	{
		private class ArrayFpsCounter
		{
			private float[] _deltaTimes;

			private int _capacity;

			private float _accumulatedTime;

			private int _head;

			private int _tail;

			public float FPS => (float)_capacity / _accumulatedTime;

			private void Init(int capacity)
			{
				_capacity = capacity;
				_deltaTimes = new float[_capacity];
				_tail = 0;
				_head = _capacity - 1;
			}

			public void Update(float dt)
			{
				_deltaTimes[_head] = dt;
				_head++;
				if (_head >= _capacity)
				{
					_head -= _capacity;
				}
				_accumulatedTime += dt;
				_accumulatedTime -= _deltaTimes[_tail];
				_tail++;
				if (_tail >= _capacity)
				{
					_tail -= _capacity;
				}
			}

			public ArrayFpsCounter(int capacity)
			{
				Init(capacity);
			}
		}

		private readonly ArrayFpsCounter _fpsCounter = new ArrayFpsCounter(75);

		private readonly List<float> _cachedFps = new List<float>();

		private readonly List<float> _cachedDt = new List<float>();

		private bool _recording;

		private Texture _texture;

		public Rect Position = new Rect(20f, 200f, 100f, 100f);

		public int FontSize = 36;

		public event Action OnUpdate;

		public void RecordFps()
		{
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(0, 0, Color.red);
			texture2D.Apply();
			_texture = texture2D;
			_cachedFps.Clear();
			_cachedDt.Clear();
			_recording = true;
		}

		public void StopRecordFps()
		{
			_recording = false;
		}

		public FpsData GetFpsData()
		{
			return new FpsData(_cachedFps, _cachedDt);
		}

		private void Update()
		{
			OnUpdate?.Invoke();
			if (!Mathf.Approximately(Time.timeScale, 0f))
			{
				float unscaledDeltaTime = Time.unscaledDeltaTime;
				_fpsCounter.Update(unscaledDeltaTime);
				if (_recording)
				{
					float fPS = _fpsCounter.FPS;
					_cachedFps.Add(fPS);
					_cachedDt.Add(unscaledDeltaTime);
				}
			}
		}

		private void OnGUI()
		{
			if (_recording)
			{
				GUI.DrawTexture(Position, _texture);
				GUI.Label(Position, "REC", new GUIStyle(GUI.skin.label)
				{
					alignment = TextAnchor.MiddleCenter,
					fontSize = FontSize
				});
			}
		}
	}
}
