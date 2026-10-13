using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace _Car_Parking.Scripts.PhotoMode
{
	public class VideoPlayerUI : MonoBehaviour
	{
		[SerializeField]
		private RawImage VideoDisplay;

		[SerializeField]
		private VideoPlayer VideoPlayer;

		private double _pausedTime;

		public bool IsPlaying => VideoPlayer.isPlaying;

		private void Start()
		{
			VideoPlayer.renderMode = VideoRenderMode.APIOnly;
			VideoPlayer.prepareCompleted += OnVideoPrepared;
			VideoPlayer.started += OnVideoStarted;
			Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1.0), Scheduler.MainThreadIgnoreTimeScale).Subscribe((long _) =>
			{
				SetTextureToDisplay();
			}).AddTo(this);
		}

		private void OnDestroy()
		{
			VideoPlayer.prepareCompleted -= OnVideoPrepared;
			VideoPlayer.started -= OnVideoStarted;
		}

		public void PrepareAndPlay(VideoClip videoClip, bool loop = false)
		{
			if ((bool)videoClip)
			{
				VideoPlayer.clip = videoClip;
			}
			VideoPlayer.isLooping = loop;
			VideoPlayer.enabled = true;
			VideoPlayer.Prepare();
			_pausedTime = 0.0;
		}

		public void Stop()
		{
			VideoPlayer.Stop();
			VideoPlayer.enabled = false;
			VideoPlayer.clip = null;
			_pausedTime = 0.0;
		}

		public void Pause()
		{
			_pausedTime = VideoPlayer.time;
			VideoPlayer.Pause();
		}

		public void PauseAndDisable()
		{
			Pause();
			VideoPlayer.enabled = false;
		}

		public void Resume()
		{
			if (!VideoPlayer.enabled)
			{
				VideoPlayer.enabled = true;
			}
			if (!VideoPlayer.isPlaying)
			{
				VideoPlayer.time = _pausedTime;
				VideoPlayer.Play();
			}
		}

		public void ChangeVideoClip(VideoClip newClip)
		{
			VideoPlayer.Stop();
			VideoPlayer.clip = newClip;
			VideoPlayer.Prepare();
		}

		private void OnVideoPrepared(VideoPlayer videoPlayer)
		{
			videoPlayer.Play();
		}

		private void OnVideoStarted(VideoPlayer videoPlayer)
		{
			if ((bool)videoPlayer.texture)
			{
				VideoDisplay.texture = videoPlayer.texture;
			}
		}

		private void SetTextureToDisplay()
		{
			if (VideoPlayer.isPlaying && (bool)VideoPlayer.texture && VideoDisplay.texture != VideoPlayer.texture)
			{
				VideoDisplay.texture = VideoPlayer.texture;
			}
		}
	}
}
