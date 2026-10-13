using System;
using System.Collections.Generic;
using System.Threading;
using AK.Wwise;
using CPM.Obstacles;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	public class ObstacleSoundPlayer : MonoBehaviour
	{
		[Serializable]
		public class Entry
		{
			[Tooltip("Any of these phases starts this sound. Listing several is how one sound covers both legs of a cycle: a slab sliding out and back is the same noise either way.")]
			public Obstacle.EventType[] Events = Array.Empty<Obstacle.EventType>();

			[Tooltip("The Wwise event to post.")]
			public AK.Wwise.Event Sound;

			[Tooltip("Seconds between the obstacle's event and the sound starting.")]
			public float DelayToPlay;

			[Tooltip("Seconds after which a looping sound is stopped. Below zero means the sound is not a loop and is left to finish on its own. Zero is not \"no limit\" - it posts the sound and stops it right away, which reads as a click.")]
			public float LoopDuration = -1f;
		}

		private const int StopFadeMs = 150;

		private const uint InvalidPlayingId = 0u;

		[Tooltip("The obstacle to listen to. Left empty, the nearest Obstacle on this object or its parents is used.")]
		[SerializeField]
		private Obstacle TargetObstacle;

		[Tooltip("What the sound is emitted from, which is where it is heard. Left empty, this object is used. Point it at the moving part when the component has to stay on the root - a RouteRunner switches its moving part off between runs, and a component on it dies with its pending timers.")]
		[SerializeField]
		private GameObject Emitter;

		[SerializeField]
		private List<Entry> Entries = new List<Entry>();

		private GameObject _emitter;

		private uint[] _playingIds;

		private CancellationTokenSource[] _timers;

		private CancellationTokenSource _disableCts;

		private void Awake()
		{
			if (!TargetObstacle)
			{
				TargetObstacle = GetComponentInParent<Obstacle>();
			}
			if (!TargetObstacle)
			{
				Debug.LogError("[ObstacleSoundPlayer] " + base.name + ": no Obstacle assigned and none found among the parents.", this);
				return;
			}
			_emitter = (Emitter ? Emitter : base.gameObject);
			_playingIds = new uint[Entries.Count];
			_timers = new CancellationTokenSource[Entries.Count];
		}

		private void OnEnable()
		{
			_disableCts = new CancellationTokenSource();
			if ((bool)TargetObstacle)
			{
				TargetObstacle.EventRaised += OnObstacleEvent;
			}
		}

		private void OnDisable()
		{
			if ((bool)TargetObstacle)
			{
				TargetObstacle.EventRaised -= OnObstacleEvent;
			}
			StopEveryEntry();
			CommonExtensions.CancelAndDisposeToken(ref _disableCts);
		}

		private void OnObstacleEvent(Obstacle.EventType type)
		{
			if (_timers == null || _disableCts == null)
			{
				return;
			}
			for (int i = 0; i < Entries.Count; i++)
			{
				Entry entry = Entries[i];
				if (entry != null && Matches(entry, type))
				{
					Play(i, entry);
				}
			}
		}

		private static bool Matches(Entry entry, Obstacle.EventType type)
		{
			if (entry.Events == null)
			{
				return false;
			}
			for (int i = 0; i < entry.Events.Length; i++)
			{
				if (entry.Events[i] == type)
				{
					return true;
				}
			}
			return false;
		}

		private void Play(int index, Entry entry)
		{
			if (entry.Sound == null || !entry.Sound.IsValid())
			{
				return;
			}
			if (entry.LoopDuration < 0f)
			{
				if (entry.DelayToPlay > 0f)
				{
					CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(), _disableCts.Token);
					PostAfterAsync(entry, linkedCts).Forget();
				}
				else
				{
					Post(entry);
				}
			}
			else
			{
				EnsureCapacity(index + 1);
				CancelEntry(index);
				_timers[index] = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
				RunLoopAsync(index, entry, _timers[index].Token).Forget();
			}
		}

		private async UniTaskVoid PostAfterAsync(Entry entry, CancellationTokenSource linkedCts)
		{
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(entry.DelayToPlay), ignoreTimeScale: false, PlayerLoopTiming.Update, linkedCts.Token);
				Post(entry);
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				linkedCts.Dispose();
			}
		}

		private async UniTaskVoid RunLoopAsync(int index, Entry entry, CancellationToken ct)
		{
			_ = 1;
			try
			{
				if (entry.DelayToPlay > 0f)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(entry.DelayToPlay), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
				uint playingId = Post(entry);
				if (playingId == 0)
				{
					_timers[index]?.Dispose();
					_timers[index] = null;
					return;
				}
				_playingIds[index] = playingId;
				await UniTask.Delay(TimeSpan.FromSeconds(entry.LoopDuration), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				AudioWrapper.StopPlayingId(playingId, 150);
				_playingIds[index] = 0u;
				_timers[index]?.Dispose();
				_timers[index] = null;
			}
			catch (OperationCanceledException)
			{
			}
		}

		private uint Post(Entry entry)
		{
			if (!_emitter)
			{
				return 0u;
			}
			return AudioWrapper.PostEvent(entry.Sound.Id, _emitter);
		}

		private void EnsureCapacity(int size)
		{
			if (size > _timers.Length)
			{
				Array.Resize(ref _playingIds, size);
				Array.Resize(ref _timers, size);
			}
		}

		private void StopEveryEntry()
		{
			if (_timers != null)
			{
				for (int i = 0; i < _timers.Length; i++)
				{
					CancelEntry(i);
				}
			}
		}

		private void CancelEntry(int index)
		{
			if (_timers[index] != null)
			{
				_timers[index].Cancel();
				_timers[index].Dispose();
				_timers[index] = null;
			}
			if (_playingIds[index] != 0)
			{
				AudioWrapper.StopPlayingId(_playingIds[index], 150);
				_playingIds[index] = 0u;
			}
		}
	}
}
