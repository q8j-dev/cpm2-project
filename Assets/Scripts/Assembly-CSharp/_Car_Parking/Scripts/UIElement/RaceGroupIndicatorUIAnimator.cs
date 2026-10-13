using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class RaceGroupIndicatorUIAnimator : MonoBehaviour
	{
		[SerializeField]
		private float Offset = 5f;

		[SerializeField]
		private float MoveDuration = 0.3f;

		[SerializeField]
		private float WaitDelay = 0.2f;

		[SerializeField]
		private float BackDelay = 0.5f;

		[SerializeField]
		private Ease MoveEase = Ease.InBack;

		[SerializeField]
		private Transform[] Arrows;

		private float _firstDefaultPos;

		private float _secondDefaultPos;

		private float _thirdDefaultPos;

		private bool _isDisabled;

		private void OnEnable()
		{
			_isDisabled = false;
			_firstDefaultPos = Arrows[0].position.x;
			_secondDefaultPos = Arrows[1].position.x;
			_thirdDefaultPos = Arrows[2].position.x;
			AnimateToForward().Forget();
		}

		private void OnDisable()
		{
			_isDisabled = true;
			Transform[] arrows = Arrows;
			for (int i = 0; i < arrows.Length; i++)
			{
				arrows[i].DOKill();
			}
			SetDefaultPositions();
		}

		private async UniTask AnimateToForward()
		{
			SetDefaultPositions();
			Arrows[0].DOMoveX(_firstDefaultPos + Offset, MoveDuration).SetEase(MoveEase);
			await UniTask.Delay(TimeSpan.FromSeconds(WaitDelay));
			if (_isDisabled)
			{
				return;
			}
			Arrows[1].DOMoveX(_secondDefaultPos + Offset, MoveDuration).SetEase(MoveEase);
			await UniTask.Delay(TimeSpan.FromSeconds(WaitDelay));
			if (!_isDisabled)
			{
				Arrows[2].DOMoveX(_thirdDefaultPos + Offset, MoveDuration).SetEase(MoveEase);
				await UniTask.Delay(TimeSpan.FromSeconds(BackDelay));
				if (!_isDisabled)
				{
					AnimateToForward().Forget();
				}
			}
		}

		private void SetDefaultPositions()
		{
			Vector3 position = Arrows[0].position;
			Arrows[0].position = new Vector3(_firstDefaultPos, position.y, position.z);
			Vector3 position2 = Arrows[1].position;
			Arrows[1].position = new Vector3(_secondDefaultPos, position2.y, position2.z);
			Vector3 position3 = Arrows[2].position;
			Arrows[2].position = new Vector3(_thirdDefaultPos, position3.y, position3.z);
		}
	}
}
