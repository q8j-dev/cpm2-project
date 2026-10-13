using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.LevelSnake
{
	public class BarrelObstacle : MonoBehaviour
	{
		private Vector3 _initialPosition;

		private Quaternion _initialRotation;

		private Rigidbody _rigidbody;

		private MenuLevelParent _menuLevelParent;

		[Inject]
		public void Construct(MenuLevelParent menuLevelParent)
		{
			_menuLevelParent = menuLevelParent;
		}

		private void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
			if ((bool)_menuLevelParent)
			{
				_menuLevelParent.StartedLevel += OnSetInitialPositionAndRotation;
				_menuLevelParent.RestartedLevel += ResetPositionAndRotation;
			}
		}

		private void OnSetInitialPositionAndRotation()
		{
			_initialPosition = base.transform.position;
			_initialRotation = base.transform.rotation;
		}

		private void ResetPositionAndRotation()
		{
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;
			base.transform.position = _initialPosition;
			base.transform.rotation = _initialRotation;
		}

		private void OnDestroy()
		{
			if ((bool)_menuLevelParent)
			{
				_menuLevelParent.StartedLevel -= OnSetInitialPositionAndRotation;
				_menuLevelParent.RestartedLevel -= ResetPositionAndRotation;
			}
		}
	}
}
