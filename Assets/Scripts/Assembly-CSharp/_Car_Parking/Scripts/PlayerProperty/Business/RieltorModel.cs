using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	public class RieltorModel : MonoBehaviour
	{
		[SerializeField]
		private bool IsNeedUpdatePositionOnSceneLoaded = default;

		[SerializeField]
		private Transform TargetObject = default;

		[SerializeField]
		private float RaycastHeight = 2f;

		[SerializeField]
		private LayerMask LayerMask = -1;

		[SerializeField]
		private float YOffset = default;

		[Header("Icon on head")]
		[SerializeField]
		private Image Image = default;

		private Vector3 _lastOrigin;

		private Vector3 _lastHitPoint;

		private bool _hasHit;

		private void Start()
		{
			if ((bool)V.SceneControl && IsNeedUpdatePositionOnSceneLoaded)
			{
				V.SceneControl.OnSceneLoadedEvent += AdjustTargetObjectPosition;
			}
		}

		private void AdjustTargetObjectPosition(int sceneIndex, bool isLoaded)
		{
			if (TargetObject == null)
			{
				return;
			}
			_lastOrigin = base.transform.position + Vector3.up * RaycastHeight;
			_hasHit = false;
			RaycastHit[] array = Physics.RaycastAll(_lastOrigin, Vector3.down, 10f, LayerMask);
			float num = float.MinValue;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (!raycastHit.transform.IsChildOf(TargetObject) && !(raycastHit.transform == TargetObject) && !(raycastHit.transform.GetComponentInParent<Canvas>() != null) && raycastHit.point.y > num)
				{
					num = raycastHit.point.y;
					_lastHitPoint = raycastHit.point;
					_hasHit = true;
				}
			}
			if (_hasHit)
			{
				Vector3 position = TargetObject.position;
				position.y = num + YOffset;
				TargetObject.position = position;
			}
		}

		public void SetIcon(Sprite sprite)
		{
			if (!(sprite == null) && !(Image == null))
			{
				Image.sprite = sprite;
			}
		}

		private void OnDestroy()
		{
			if ((bool)V.SceneControl)
			{
				V.SceneControl.OnSceneLoadedEvent -= AdjustTargetObjectPosition;
			}
		}
	}
}
