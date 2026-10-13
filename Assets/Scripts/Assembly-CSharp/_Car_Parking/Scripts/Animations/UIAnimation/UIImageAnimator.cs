using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Animations.UIAnimation
{
	public class UIImageAnimator : MonoBehaviour
	{
		[SerializeField]
		private Image ImageToAnimate;

		[SerializeField]
		private int FrameRate;

		private List<Sprite> _sprites = new List<Sprite>();

		private bool _isDestroyed;

		private bool _canPlay;

		private int _currentFrame;

		public bool IsPlaying { get; private set; }

		public int ClipDuration { get; private set; }

		[field: SerializeField]
		public bool Loop { get; set; }

		public event Action OnFinish;

		private void OnDestroy()
		{
			_isDestroyed = true;
			_sprites.Clear();
		}

		public async UniTask Play(List<Sprite> sprites, int frameRate = 0)
		{
			_sprites.Clear();
			_sprites = sprites;
			_canPlay = true;
			if (frameRate != 0)
			{
				FrameRate = frameRate;
			}
			await Animate();
		}

		public void Stop()
		{
			if (IsPlaying)
			{
				_canPlay = false;
				ImageToAnimate.sprite = _sprites[0];
			}
		}

		public void Pause()
		{
			_canPlay = false;
		}

		public void Resume()
		{
			_canPlay = true;
			Animate(_currentFrame).Forget();
		}

		private async UniTask Animate(int startFrame = 0)
		{
			if (_sprites.Count < 1)
			{
				return;
			}
			if (FrameRate <= 0)
			{
				FrameRate = 10;
			}
			int delay = 1000 / FrameRate;
			ClipDuration = delay * _sprites.Count;
			IsPlaying = true;
			for (int i = startFrame; i < _sprites.Count; i++)
			{
				if (!_canPlay)
				{
					IsPlaying = false;
					return;
				}
				if ((bool)_sprites[i])
				{
					ImageToAnimate.sprite = _sprites[i];
					_currentFrame = i;
					await UniTask.Delay(delay, ignoreTimeScale: true);
					if (_isDestroyed)
					{
						return;
					}
				}
			}
			if (Loop)
			{
				Animate().Forget();
				return;
			}
			IsPlaying = false;
			OnFinish?.Invoke();
		}
	}
}
