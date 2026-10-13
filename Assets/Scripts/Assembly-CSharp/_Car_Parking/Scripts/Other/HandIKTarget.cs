using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Other
{
	public class HandIKTarget : MonoBehaviour
	{
		private MoverAndRotatorByTween _moverAndRotatorByTween;

		public MoverAndRotatorByTween MoverAndRotatorByTween
		{
			get
			{
				if (!_moverAndRotatorByTween)
				{
					MoverByTween moverByTween = base.gameObject.AddComponent<MoverByTween>();
					RotatorByTween rotatorByTween = base.gameObject.AddComponent<RotatorByTween>();
					_moverAndRotatorByTween = base.gameObject.AddComponent<MoverAndRotatorByTween>();
					_moverAndRotatorByTween.MoverByTween = moverByTween;
					_moverAndRotatorByTween.RotatorByTween = rotatorByTween;
				}
				return _moverAndRotatorByTween;
			}
		}
	}
}
