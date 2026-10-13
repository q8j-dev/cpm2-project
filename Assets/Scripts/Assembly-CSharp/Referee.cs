using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using VContainer;

public class Referee : MonoBehaviour
{
	private const float Speed = 0.1f;

	private const float AnimationStartDistance = 10f;

	private const float AnimationEndDistance = 0.05f;

	private const float DistanceCoefficient = 32f;

	private const float MinPosToAnimate = 2f;

	private const float IKTransitionDuration = 0.5f;

	[SerializeField]
	private Transform _leftTarget;

	[SerializeField]
	private Transform _rightTarget;

	private DragRacingTree _dragRacingTree;

	private TwoBoneIKConstraint _rightIK;

	private TwoBoneIKConstraint _leftIK;

	private float _leftCarDistance;

	private float _rightCarDistance;

	private float _targetZPosition;

	private float _initialRightZPos;

	private float _initialLeftZPos;

	private bool _movingTowardsTargetLeft = true;

	private bool _movingTowardsTargetRight = true;

	private bool _leftHandAnimStarted;

	private bool _rightHandAnimStarted;

	private bool _isLeftCarEntered;

	private bool _isRightCarEntered;

	private CancellationTokenSource _leftCarCts;

	private CancellationTokenSource _rightCarCts;

	private CancellationTokenSource _leftIKWeightCts;

	private CancellationTokenSource _rightIKWeightCts;

	[Inject]
	public void Construct(DragRacingTree dragRacingTree)
	{
		_dragRacingTree = dragRacingTree;
	}

	private void Start()
	{
		DragRacingTree dragRacingTree = _dragRacingTree;
		dragRacingTree.LeftCarDistance = (Action<float>)Delegate.Combine(dragRacingTree.LeftCarDistance, new Action<float>(SetLeftCarDistance));
		DragRacingTree dragRacingTree2 = _dragRacingTree;
		dragRacingTree2.RightCarDistance = (Action<float>)Delegate.Combine(dragRacingTree2.RightCarDistance, new Action<float>(SetRightCarDistance));
		_initialRightZPos = _rightTarget.position.z;
		_initialLeftZPos = _leftTarget.position.z;
		_rightIK = _rightTarget.parent.GetComponent<TwoBoneIKConstraint>();
		_leftIK = _leftTarget.parent.GetComponent<TwoBoneIKConstraint>();
	}

	public void OnCarEnterStartLine(bool isLeft)
	{
		if (isLeft)
		{
			if (!_isLeftCarEntered)
			{
				_leftCarCts?.Cancel();
				_leftCarCts?.Dispose();
				_leftCarCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
				RestartLeftIKWeight(starting: true);
				_isLeftCarEntered = true;
				AnimateHandAsync(isLeft, _leftCarCts.Token).Forget();
			}
		}
		else if (!_isRightCarEntered)
		{
			_rightCarCts?.Cancel();
			_rightCarCts?.Dispose();
			_rightCarCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			RestartRightIKWeight(starting: true);
			_isRightCarEntered = true;
			AnimateHandAsync(isLeft, _rightCarCts.Token).Forget();
		}
	}

	public void OnCarExitStartLine(bool isLeft)
	{
		if (isLeft)
		{
			_isLeftCarEntered = false;
			RestartLeftIKWeight(starting: false);
			_leftCarCts?.Cancel();
			_leftCarCts?.Dispose();
			_leftCarCts = null;
		}
		else
		{
			_isRightCarEntered = false;
			RestartRightIKWeight(starting: false);
			_rightCarCts?.Cancel();
			_rightCarCts?.Dispose();
			_rightCarCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			AnimateHandAsync(isLeft, _rightCarCts.Token).Forget();
		}
	}

	private void RestartLeftIKWeight(bool starting)
	{
		_leftIKWeightCts?.Cancel();
		_leftIKWeightCts?.Dispose();
		_leftIKWeightCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
		LerpIKWeightAsync(_leftIK, starting, _leftIKWeightCts.Token).Forget();
	}

	private void RestartRightIKWeight(bool starting)
	{
		_rightIKWeightCts?.Cancel();
		_rightIKWeightCts?.Dispose();
		_rightIKWeightCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
		LerpIKWeightAsync(_rightIK, starting, _rightIKWeightCts.Token).Forget();
	}

	private async UniTaskVoid AnimateHandAsync(bool isLeft, CancellationToken ct = default(CancellationToken))
	{
		try
		{
			while (isLeft ? _isLeftCarEntered : _isRightCarEntered)
			{
				await UniTask.Yield(ct);
				if (isLeft)
				{
					AnimateLeftHand(_leftCarDistance);
				}
				else
				{
					AnimateRightHand(_rightCarDistance);
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void SetLeftCarDistance(float distance)
	{
		_leftCarDistance = distance;
	}

	private void SetRightCarDistance(float distance)
	{
		_rightCarDistance = distance;
	}

	private async UniTaskVoid LerpIKWeightAsync(TwoBoneIKConstraint ik, bool starting, CancellationToken ct = default(CancellationToken))
	{
		try
		{
			float elapsed = 0f;
			float startWeight = ((!starting) ? 1 : 0);
			float endWeight = (starting ? 1 : 0);
			while (elapsed < 0.5f)
			{
				elapsed += Time.deltaTime;
				ik.weight = Mathf.Lerp(startWeight, endWeight, elapsed / 0.5f);
				await UniTask.Yield(ct);
			}
			ik.weight = endWeight;
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void AnimateRightHand(float distance)
	{
		_movingTowardsTargetRight = AnimateHand(distance, _rightTarget, _movingTowardsTargetRight, _initialLeftZPos);
	}

	private void AnimateLeftHand(float distance)
	{
		_movingTowardsTargetLeft = AnimateHand(distance, _leftTarget, _movingTowardsTargetLeft, _initialRightZPos);
	}

	private bool AnimateHand(float distance, Transform target, bool movingTowardsTarget, float initialZPos)
	{
		if (distance < 2f)
		{
			distance = 2f;
		}
		float num = initialZPos - distance / 32f;
		Vector3 position = target.position;
		float num2 = (movingTowardsTarget ? num : initialZPos);
		position.z = Mathf.MoveTowards(position.z, num2, 0.1f * Time.deltaTime * distance);
		target.position = position;
		if (position.z == num2)
		{
			return !movingTowardsTarget;
		}
		return movingTowardsTarget;
	}

	private void OnDestroy()
	{
		DragRacingTree dragRacingTree = _dragRacingTree;
		dragRacingTree.LeftCarDistance = (Action<float>)Delegate.Remove(dragRacingTree.LeftCarDistance, new Action<float>(SetLeftCarDistance));
		DragRacingTree dragRacingTree2 = _dragRacingTree;
		dragRacingTree2.RightCarDistance = (Action<float>)Delegate.Remove(dragRacingTree2.RightCarDistance, new Action<float>(SetRightCarDistance));
		_leftCarCts?.Cancel();
		_leftCarCts?.Dispose();
		_leftCarCts = null;
		_rightCarCts?.Cancel();
		_rightCarCts?.Dispose();
		_rightCarCts = null;
		_leftIKWeightCts?.Cancel();
		_leftIKWeightCts?.Dispose();
		_leftIKWeightCts = null;
		_rightIKWeightCts?.Cancel();
		_rightIKWeightCts?.Dispose();
		_rightIKWeightCts = null;
	}
}
