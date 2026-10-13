using Cysharp.Threading.Tasks;
using UnityEngine;

public class ScrollSnapSoundComponent : MonoBehaviour
{
	private float _maxValue;

	private float _minValue;

	private int _itemsCount;

	private int _lastCenteredPanel = -1;

	private float _previousScrollValue = -1f;

	private float _prevCenter;

	private float _lastSoundTime = -1f;

	private const float SoundCooldown = 0.25f;

	public bool IgnoreSound;

	public void Init(int values)
	{
		_itemsCount = values;
	}

	public void OnScrollValueChanged(Vector2 scrollValue)
	{
		CheckCenterCrossing(scrollValue.x);
		_previousScrollValue = scrollValue.x;
	}

	public void CheckCenterCrossing(float currentScrollValue)
	{
		if (_previousScrollValue < 0f)
		{
			return;
		}
		float num = currentScrollValue * (float)(_itemsCount - 1);
		float num2 = _previousScrollValue * (float)(_itemsCount - 1);
		for (int i = 0; i < _itemsCount; i++)
		{
			float num3 = i;
			bool num4 = (num2 < num3 && num >= num3) || (num2 > num3 && num <= num3);
			bool flag = (_prevCenter == 0f && Mathf.Approximately(_prevCenter, num3)) || (Mathf.Approximately(_prevCenter, _itemsCount - 1) && Mathf.Approximately(_prevCenter, num3));
			if (num4 && !flag)
			{
				PlaySound();
				_prevCenter = num3;
				break;
			}
		}
	}

	public void DelayedIgnoreSound()
	{
		DelayedIgnoreSoundAsync().Forget();
	}

	private async UniTaskVoid DelayedIgnoreSoundAsync()
	{
		IgnoreSound = true;
		await UniTask.Yield();
		IgnoreSound = false;
	}

	private void PlaySound()
	{
		if (!IgnoreSound && !(Time.unscaledTime - _lastSoundTime < 0.25f))
		{
			_lastSoundTime = Time.unscaledTime;
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ScrollSwitch, base.gameObject);
		}
	}
}
