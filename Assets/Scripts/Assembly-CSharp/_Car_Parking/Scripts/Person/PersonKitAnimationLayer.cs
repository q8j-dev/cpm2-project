using System;
using System.Threading;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Person
{
	[RequireComponent(typeof(MainPerson))]
	public class PersonKitAnimationLayer : MonoBehaviour
	{
		private const float WeightEpsilon = 0.001f;

		[SerializeField]
		private PersonKitAnimationConfig _config;

		private MainPerson _mainPerson;

		private Animator _animator;

		private RuntimeAnimatorController _resolvedFor;

		private int _layerIndex = -1;

		private GameObject _kitObject;

		private Equipment _kitEquipment;

		private int[] _kitIds;

		private bool _applied;

		private bool _worn;

		private float _weight;

		public bool IsWorn => _worn;

		private void Awake()
		{
			TryGetComponent<MainPerson>(out _mainPerson);
			if ((bool)_config)
			{
				UpdateWeightAsync(base.destroyCancellationToken).Forget();
				return;
			}
			Debug.LogError("PersonKitAnimationLayer has no config assigned on " + base.name, this);
			base.enabled = false;
		}

		private async UniTask UpdateWeightAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				bool flag = (_worn = IsKitWorn()) && !IsPoseSuppressed();
				if (!flag && !_applied)
				{
					await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
					continue;
				}
				if (!ResolveLayer())
				{
					if (!flag)
					{
						ClearWeight();
					}
					await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
					continue;
				}
				float num = (flag ? _config.Weight : 0f);
				float num2 = (flag ? _config.WeightLerpPowerUp : _config.WeightLerpPowerDown);
				_weight = Mathf.Lerp(_weight, num, num2 * Time.deltaTime);
				if (Mathf.Abs(num - _weight) <= 0.001f)
				{
					_weight = num;
				}
				_animator.SetLayerWeight(_layerIndex, _weight);
				_applied = flag || _weight > 0f;
				await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
			}
		}

		private bool IsKitWorn()
		{
			if (!_mainPerson)
			{
				return false;
			}
			GameObject[] allKits = _mainPerson.allKits;
			int slot = (int)_config.Slot;
			if (allKits == null || slot >= allKits.Length)
			{
				return false;
			}
			GameObject gameObject = allKits[slot];
			if ((object)gameObject != _kitObject)
			{
				_kitObject = gameObject;
				_kitEquipment = (gameObject ? gameObject.GetComponent<Equipment>() : null);
				_kitIds = _config.KitIdsFor(_mainPerson.Gender);
			}
			if ((bool)_kitEquipment && _kitObject.activeSelf)
			{
				return Array.IndexOf(_kitIds, _kitEquipment.id) >= 0;
			}
			return false;
		}

		private bool IsPoseSuppressed()
		{
			PersonAnimationsController personAnimationsController = (_mainPerson ? _mainPerson.PersonAnimationsController : null);
			if (!personAnimationsController)
			{
				return false;
			}
			if (personAnimationsController.IsEmoteAnimating)
			{
				return true;
			}
			return (personAnimationsController.SitGetOutCarSystem?.SitGetOutData)?.IsKitPoseSuppressed ?? false;
		}

		private void ClearWeight()
		{
			_applied = false;
			_weight = 0f;
		}

		private bool ResolveLayer()
		{
			Animator animator = ResolveAnimator();
			if (!animator)
			{
				return false;
			}
			RuntimeAnimatorController runtimeAnimatorController = SourceOf(animator);
			if (!runtimeAnimatorController)
			{
				return false;
			}
			if (_layerIndex < 0 || (object)animator != _animator || (object)runtimeAnimatorController != _resolvedFor)
			{
				_animator = animator;
				_resolvedFor = runtimeAnimatorController;
				_layerIndex = animator.GetLayerIndex(_config.LayerName);
			}
			return _layerIndex >= 0;
		}

		private Animator ResolveAnimator()
		{
			PersonAnimationsController personAnimationsController = _mainPerson.PersonAnimationsController;
			if (!personAnimationsController || !personAnimationsController.Animator)
			{
				return _mainPerson.Animator;
			}
			return personAnimationsController.Animator;
		}

		private static RuntimeAnimatorController SourceOf(Animator animator)
		{
			RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
			if (!(runtimeAnimatorController is AnimatorOverrideController animatorOverrideController))
			{
				return runtimeAnimatorController;
			}
			return animatorOverrideController.runtimeAnimatorController;
		}
	}
}
