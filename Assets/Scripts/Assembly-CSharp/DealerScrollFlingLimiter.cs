using UnityEngine;
using UnityEngine.UI;

public class DealerScrollFlingLimiter : MonoBehaviour
{
	private ScrollRect _scrollRect;

	private float _maxSpeed = 500f;

	private float _snapKickSpeed = 130f;

	public void Init(ScrollRect scrollRect, float maxSpeed, float snapKickSpeed)
	{
		_scrollRect = scrollRect;
		_maxSpeed = maxSpeed;
		_snapKickSpeed = snapKickSpeed;
	}

	private void Awake()
	{
		if (!_scrollRect)
		{
			TryGetComponent<ScrollRect>(out _scrollRect);
		}
	}

	private void LateUpdate()
	{
		if (!_scrollRect)
		{
			return;
		}
		Vector2 velocity = _scrollRect.velocity;
		float magnitude = velocity.magnitude;
		if (!(magnitude <= 0f))
		{
			if (magnitude > _maxSpeed)
			{
				_scrollRect.velocity = velocity * (_maxSpeed / magnitude);
			}
			else if (magnitude < _snapKickSpeed)
			{
				_scrollRect.velocity = Vector2.zero;
			}
		}
	}
}
