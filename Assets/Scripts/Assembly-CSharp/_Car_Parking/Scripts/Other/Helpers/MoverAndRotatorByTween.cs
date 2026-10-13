using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class MoverAndRotatorByTween : MonoBehaviour
	{
		public MoverByTween MoverByTween;

		public RotatorByTween RotatorByTween;

		public void ApplyToZero(float duration, Ease ease, bool isLocal)
		{
			if ((bool)MoverByTween)
			{
				MoverByTween.MoveToZero(duration, ease, isLocal);
			}
			if ((bool)RotatorByTween)
			{
				RotatorByTween.RotateToZero(duration, ease, isLocal);
			}
		}
	}
}
