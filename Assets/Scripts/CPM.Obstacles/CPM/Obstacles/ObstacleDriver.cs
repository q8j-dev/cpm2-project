using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CPM.Obstacles
{
	public sealed class ObstacleDriver : IDisposable
	{
		private const double ResyncSnapSeconds = 0.5;

		private const double ResyncDeadbandSeconds = 0.15;

		private const double ResyncSlewPerStep = 0.0005;

		private readonly IReadOnlyList<Obstacle> _obstacles;

		private readonly Func<double> _elapsedSeconds;

		private CancellationTokenSource _cts = new CancellationTokenSource();

		public ObstacleDriver(IReadOnlyList<Obstacle> obstacles, Func<double> elapsedSeconds)
		{
			_obstacles = obstacles ?? throw new ArgumentNullException("obstacles");
			_elapsedSeconds = elapsedSeconds ?? throw new ArgumentNullException("elapsedSeconds");
			RunAsync(_cts.Token).Forget();
		}

		public void Dispose()
		{
			BaseCommonExtensions.CancelAndDisposeToken(ref _cts);
		}

		private async UniTaskVoid RunAsync(CancellationToken ct)
		{
			double smoothElapsed = 0.0;
			bool running = false;
			try
			{
				while (!ct.IsCancellationRequested)
				{
					double num = Math.Max(0.0, _elapsedSeconds());
					if (num <= 0.0)
					{
						smoothElapsed = 0.0;
						running = false;
					}
					else if (!running)
					{
						smoothElapsed = num;
						running = true;
					}
					else
					{
						smoothElapsed += (double)Time.fixedDeltaTime;
						double value = num - smoothElapsed;
						double num2 = Math.Abs(value);
						if (num2 > 0.5)
						{
							smoothElapsed = num;
						}
						else if (num2 > 0.15)
						{
							smoothElapsed += (double)Math.Sign(value) * Math.Min(0.0005, num2 - 0.15);
						}
					}
					for (int i = 0; i < _obstacles.Count; i++)
					{
						Obstacle obstacle = _obstacles[i];
						if ((bool)obstacle)
						{
							obstacle.EvaluateAt(obstacle.UsesSmoothedClock ? smoothElapsed : num);
						}
					}
					await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
				}
			}
			catch (OperationCanceledException)
			{
			}
		}
	}
}
