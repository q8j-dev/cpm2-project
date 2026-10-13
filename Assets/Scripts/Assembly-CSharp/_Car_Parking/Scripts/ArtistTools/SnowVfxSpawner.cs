using AK.Wwise;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.ArtistTools.Attributes;
using _Car_Parking.Scripts.ArtistTools.Trackables;

namespace _Car_Parking.Scripts.ArtistTools
{
	public class SnowVfxSpawner : TrackableWithAspects
	{
		private const float MinSpeedToVfx = 10f;

		[SerializeField]
		[DropDownByVfxId]
		public string VfxID;

		[SerializeField]
		public Transform VfxPlace;

		public AK.Wwise.Event PlaySnowbankSound;

		private bool _isDestroyed;

		private VfxPoolProvider _vfxPoolProvider;

		private ParticleSystem _vfxEffect;

		[Inject]
		public void Construct(VfxPoolProvider vfxPoolProvider)
		{
			_vfxPoolProvider = vfxPoolProvider;
		}

		private void Start()
		{
			base.OnDestroyEvent += OnDestroyImpl;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.transform.TryGetComponent<Powertrain>(out var component) && component.SpeedKmh > 10f)
			{
				ContactPoint contactPoint = collision.contacts[0];
				TryActivateVfxEffect(contactPoint.point, component.Rigidbody);
				PlaySnowbankSound.Post(base.gameObject);
			}
		}

		public void OnDestroyImpl(TrackableWithAspects _)
		{
			base.OnDestroyEvent -= OnDestroyImpl;
			ReturnVfx(_vfxEffect);
			_isDestroyed = true;
		}

		private void ReturnVfx(ParticleSystem vfx)
		{
			if ((bool)vfx)
			{
				_vfxPoolProvider.ReturnToPool(vfx);
			}
		}

		public void TryActivateVfxEffect(Vector3 position, Rigidbody rb)
		{
			if (VfxID.IsNullOrWhitespace() || VfxID == "<None>" || _vfxPoolProvider == null)
			{
				return;
			}
			_vfxEffect = _vfxPoolProvider.GetFromPool(VfxID);
			if ((bool)_vfxEffect)
			{
				Vector3 localPosition = Vector3.zero;
				Quaternion localRotation = Quaternion.identity;
				Vector3 localScale = Vector3.one;
				if ((bool)VfxPlace)
				{
					localPosition = VfxPlace.localPosition;
					localRotation = VfxPlace.localRotation;
					localScale = VfxPlace.localScale;
				}
				_vfxEffect.transform.SetLocalPositionAndRotation(localPosition, localRotation);
				_vfxEffect.transform.localScale = localScale;
				_vfxEffect.transform.position = position;
				Vector3 upwards = -rb.velocity;
				if (upwards.sqrMagnitude > 0.001f)
				{
					Quaternion rotation = Quaternion.LookRotation(Vector3.forward, upwards);
					_vfxEffect.transform.rotation = rotation;
				}
				_vfxEffect.Play();
				OnStopReturnVfx(_vfxEffect).Forget();
			}
		}

		private async UniTask OnStopReturnVfx(ParticleSystem vfx)
		{
			if (!vfx)
			{
				return;
			}
			while (vfx.isPlaying)
			{
				await UniTask.Yield();
				if (_isDestroyed)
				{
					ReturnVfx(vfx);
					return;
				}
			}
			ReturnVfx(vfx);
		}
	}
}
