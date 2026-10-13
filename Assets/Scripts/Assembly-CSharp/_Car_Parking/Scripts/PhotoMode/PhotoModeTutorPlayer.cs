using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace _Car_Parking.Scripts.PhotoMode
{
	public class PhotoModeTutorPlayer : MonoBehaviour
	{
		[SerializeField]
		private VideoPlayerUI VideoPlayerUI;

		private bool _stopPlaying;

		private CancellationTokenSource _miniTutorsCancellation;

		public bool IsPlaying { get; private set; }

		private void OnDestroy()
		{
			_stopPlaying = true;
		}

		public async UniTask StartPlayingMainTutors(List<VideoClip> clips)
		{
			_stopPlaying = false;
			_miniTutorsCancellation = new CancellationTokenSource();
			await UniTask.Yield();
			if (_stopPlaying)
			{
				return;
			}
			for (int i = 0; i < clips.Count; i++)
			{
				if (_stopPlaying)
				{
					break;
				}
				if (_miniTutorsCancellation.IsCancellationRequested)
				{
					break;
				}
				IsPlaying = true;
				VideoPlayerUI.PrepareAndPlay(clips[i]);
				await UniTask.Delay(TimeSpan.FromSeconds(clips[i].length), ignoreTimeScale: true, PlayerLoopTiming.Update, _miniTutorsCancellation.Token).SuppressCancellationThrow();
				if (i == clips.Count - 1)
				{
					i = -1;
				}
			}
		}

		public void StopVideo()
		{
			CommonExtensions.CancelAndDisposeToken(ref _miniTutorsCancellation);
			VideoPlayerUI.Stop();
			IsPlaying = false;
			_stopPlaying = true;
		}
	}
}
