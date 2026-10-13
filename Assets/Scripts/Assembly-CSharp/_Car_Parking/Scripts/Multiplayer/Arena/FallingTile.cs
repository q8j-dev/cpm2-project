using System;
using UnityEngine;
using _Car_Parking.Scripts.Base;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public class FallingTile : MonoBehaviour
	{
		[SerializeField]
		private TriggerRelay TriggerRelay = default;

		[SerializeField]
		private MeshRenderer MeshRenderer = default;

		[SerializeField]
		private GameObject Root = default;

		private int _id;

		private bool _isFalling;

		private bool _isDestroyed;

		private Material _material;

		private double _startTime;

		private float _delaySeconds;

		private float _durationSeconds;

		private Color _startColor;

		private Color _endColor;

		public bool IsFalling => _isFalling;

		public bool IsDestroyed => _isDestroyed;

		public int Id => _id;

		public event Action<FallingTile> OnDriverPersonTriggered;

		public void Init(int id)
		{
			_id = id;
		}

		private void OnEnable()
		{
			if ((bool)TriggerRelay)
			{
				TriggerRelay.OnTriggerEntered += OnTriggerEntered;
			}
		}

		private void OnDisable()
		{
			if ((bool)TriggerRelay)
			{
				TriggerRelay.OnTriggerEntered -= OnTriggerEntered;
			}
		}

		private void OnTriggerEntered(Collider collider, TriggerRelay triggerRelay)
		{
			if (!_isFalling && !_isDestroyed && (bool)V.DriverPerson && (bool)collider.attachedRigidbody && collider.attachedRigidbody.gameObject == V.DriverPerson)
			{
				OnDriverPersonTriggered?.Invoke(this);
			}
		}

		public void StartFalling(double startTime, FallingZoneConfig.TileDestroyData destroyData)
		{
			if (!_isFalling && destroyData != null)
			{
				_isFalling = true;
				_startTime = startTime;
				_delaySeconds = Mathf.Max(0f, destroyData.Delay);
				_durationSeconds = Mathf.Max(0f, destroyData.Duration);
				_endColor = destroyData.Color;
				if ((bool)MeshRenderer)
				{
					_material = MeshRenderer.material;
					_startColor = _material.color;
				}
			}
		}

		public void EvaluateAt(double time)
		{
			if (!_isFalling || _isDestroyed)
			{
				return;
			}
			double num = time - _startTime;
			if (num < (double)_delaySeconds)
			{
				return;
			}
			if (num >= (double)(_delaySeconds + _durationSeconds))
			{
				_isDestroyed = true;
				if ((bool)Root)
				{
					Root.SetActive(value: false);
				}
			}
			else if ((bool)_material)
			{
				_material.color = Color.Lerp(_startColor, _endColor, (float)((num - (double)_delaySeconds) / (double)_durationSeconds));
			}
		}

		private void OnDestroy()
		{
			BaseCommonExtensions.DestroyMat(ref _material);
		}
	}
}
