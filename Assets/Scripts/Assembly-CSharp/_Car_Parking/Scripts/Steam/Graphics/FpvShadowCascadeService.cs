using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Steam.Graphics
{
	public class FpvShadowCascadeService : ITickable, IDisposable
	{
		private const float FpvShadowDistance = 50f;

		private const float FpvFirstCascadeMeters = 2.5f;

		private const int PersonFirstPersonCamIndex = 3;

		private const int BodyLookupRetryFrames = 30;

		private const float HeadroomMeters = 0.6f;

		private static readonly FieldInfo Cascade2SplitField = typeof(UniversalRenderPipelineAsset).GetField("m_Cascade2Split", BindingFlags.Instance | BindingFlags.NonPublic);

		private UniversalRenderPipelineAsset _asset;

		private bool _applied;

		private float _baseDistance;

		private float _baseSplit2;

		private float _appliedDistance;

		private float _appliedSplit2;

		private GameObject _cachedCar;

		private Collider _bodyCollider;

		private Transform _bodyTransform;

		private Bounds _bodyLocalBounds;

		private bool _hasLocalBounds;

		private int _nextBodyLookupFrame;

		public void Tick()
		{
			UniversalRenderPipelineAsset universalRenderPipelineAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
			if (_applied && universalRenderPipelineAsset != _asset)
			{
				Restore();
			}
			if (!universalRenderPipelineAsset || universalRenderPipelineAsset.shadowCascadeCount <= 1 || !IsFirstPersonView())
			{
				if (_applied)
				{
					Restore();
				}
			}
			else if (!_applied)
			{
				Apply(universalRenderPipelineAsset);
			}
			else if (!Mathf.Approximately(universalRenderPipelineAsset.shadowDistance, _appliedDistance) || !Mathf.Approximately(universalRenderPipelineAsset.cascade2Split, _appliedSplit2))
			{
				_applied = false;
				Apply(universalRenderPipelineAsset);
			}
		}

		public void Dispose()
		{
			if (_applied)
			{
				Restore();
			}
		}

		private void Apply(UniversalRenderPipelineAsset asset)
		{
			_asset = asset;
			_baseDistance = asset.shadowDistance;
			_baseSplit2 = asset.cascade2Split;
			_appliedDistance = Mathf.Min(_baseDistance, 50f);
			asset.shadowDistance = _appliedDistance;
			_appliedSplit2 = _baseSplit2;
			if (asset.shadowCascadeCount == 2 && _appliedDistance > 0f)
			{
				_appliedSplit2 = Mathf.Clamp01(2.5f / _appliedDistance);
				Cascade2SplitField?.SetValue(asset, _appliedSplit2);
			}
			_applied = true;
		}

		private void Restore()
		{
			if ((bool)_asset)
			{
				if (Mathf.Approximately(_asset.shadowDistance, _appliedDistance))
				{
					_asset.shadowDistance = _baseDistance;
				}
				if (Mathf.Approximately(_asset.cascade2Split, _appliedSplit2))
				{
					Cascade2SplitField?.SetValue(_asset, _baseSplit2);
				}
			}
			_asset = null;
			_applied = false;
		}

		private bool IsFirstPersonView()
		{
			if (FlagsStorage.OutOfCar)
			{
				GameObject driverPerson = V.DriverPerson;
				if (V.PersonCamIndex == 3 && (bool)driverPerson)
				{
					return driverPerson.activeInHierarchy;
				}
				return false;
			}
			GameObject car = V.Car;
			Camera main = Camera.main;
			if (!car || !main)
			{
				return false;
			}
			return IsInsideCarBody(car, main.transform.position);
		}

		private bool IsInsideCarBody(GameObject car, Vector3 point)
		{
			if (car != _cachedCar || (!_bodyTransform && Time.frameCount >= _nextBodyLookupFrame))
			{
				CacheCarBody(car);
			}
			if (!_bodyTransform)
			{
				return false;
			}
			if (_hasLocalBounds)
			{
				return _bodyLocalBounds.Contains(_bodyTransform.InverseTransformPoint(point));
			}
			if ((bool)_bodyCollider && _bodyCollider.enabled)
			{
				return _bodyCollider.bounds.Contains(point);
			}
			return false;
		}

		private void CacheCarBody(GameObject car)
		{
			_cachedCar = car;
			_bodyCollider = null;
			_bodyTransform = null;
			_hasLocalBounds = false;
			_nextBodyLookupFrame = Time.frameCount + 30;
			if (!car.TryGetComponent<VehicleData>(out var component))
			{
				return;
			}
			Collider collider = component.Collider;
			if (!collider)
			{
				return;
			}
			_bodyCollider = collider;
			_bodyTransform = collider.transform;
			Collider collider2 = collider;
			if (!(collider2 is BoxCollider boxCollider))
			{
				if (collider2 is MeshCollider meshCollider && (bool)meshCollider.sharedMesh)
				{
					_bodyLocalBounds = meshCollider.sharedMesh.bounds;
					_hasLocalBounds = true;
				}
			}
			else
			{
				_bodyLocalBounds = new Bounds(boxCollider.center, boxCollider.size);
				_hasLocalBounds = true;
			}
			if (_hasLocalBounds)
			{
				float num = Mathf.Max(0.0001f, Mathf.Abs(_bodyTransform.lossyScale.y));
				_bodyLocalBounds.SetMinMax(_bodyLocalBounds.min, _bodyLocalBounds.max + Vector3.up * (0.6f / num));
			}
		}
	}
}
