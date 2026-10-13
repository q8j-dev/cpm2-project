using Sirenix.Utilities;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.ArtistTools.Attributes;
using _Car_Parking.Scripts.ArtistTools.Trackables;

namespace _Car_Parking.Scripts.ArtistTools
{
	public class VfxSpawner : TrackableWithAspects
	{
		[SerializeField]
		[DropDownByVfxId]
		public string VfxID;

		[SerializeField]
		public Transform VfxPlace;

		[SerializeField]
		public bool ParentIsGlobalPoint;

		private VfxPoolProvider _vfxPoolProvider;

		private ParticleSystem _vfxEffect;

		[Inject]
		public void Construct(VfxPoolProvider vfxPoolProvider)
		{
			_vfxPoolProvider = vfxPoolProvider;
		}

		public void OnDestroyImpl(TrackableWithAspects _)
		{
			if ((bool)_vfxEffect)
			{
				_vfxPoolProvider.ReturnToPool(_vfxEffect);
			}
		}

		public void TryActivateVfxEffect()
		{
			if (VfxID.IsNullOrWhitespace() || VfxID == "<None>" || _vfxPoolProvider == null)
			{
				return;
			}
			_vfxEffect = _vfxPoolProvider.GetFromPool(VfxID);
			if ((bool)_vfxEffect)
			{
				_vfxEffect.transform.parent = base.transform;
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
				if (ParentIsGlobalPoint)
				{
					base.transform.parent = V.GlobalPoint;
				}
				_vfxEffect.Play();
			}
		}
	}
}
