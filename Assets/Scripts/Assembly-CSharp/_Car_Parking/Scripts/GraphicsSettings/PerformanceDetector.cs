using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Device;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class PerformanceDetector
	{
		private int _lowGraphicsMemory = 2000;

		private int _highGraphicsMemory = 5000;

		private int _lowMemory = 4000;

		private int _highMemory = 6000;

		private const string CacheKey = "GfxTier_v3";

		private const int BenchmarkSize = 1024;

		private const int BlitsPerFrame = 8;

		private const int WarmupFrames = 30;

		private const int MeasureFrames = 60;

		private const float HighMsPerBlit = 4.5f;

		private const float MidMsPerBlit = 7f;

		public int DetectPerformanceTier()
		{
			int num = 0;
			if (UnityEngine.Device.SystemInfo.graphicsMemorySize > _highGraphicsMemory)
			{
				num += 2;
			}
			else if (UnityEngine.Device.SystemInfo.graphicsMemorySize > _lowGraphicsMemory)
			{
				num++;
			}
			if (UnityEngine.Device.SystemInfo.systemMemorySize > _highMemory && UnityEngine.Device.SystemInfo.graphicsMemorySize > _lowGraphicsMemory)
			{
				num += 2;
			}
			else if (UnityEngine.Device.SystemInfo.systemMemorySize > _lowMemory)
			{
				num++;
			}
			return num switch
			{
				4 => 4, 
				3 => 2, 
				2 => 1, 
				_ => 0, 
			};
		}

		public async UniTask<int> DetectPerformanceTierAsync(CancellationToken ct = default(CancellationToken))
		{
			return 2;
		}

		private async UniTask<int> BenchmarkAsync(CancellationToken ct)
		{
			Shader shader = Shader.Find("Hidden/GpuBenchmark");
			if (shader == null)
			{
				return -1;
			}
			int result = 0;
			Material mat = null;
			RenderTexture rt = null;
			try
			{
				mat = new Material(shader);
				rt = RenderTexture.GetTemporary(1024, 1024, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				for (int i = 0; i < 30; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						Graphics.Blit(Texture2D.whiteTexture, rt, mat);
					}
					await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, ct);
				}
				double totalGpuMs = 0.0;
				int validSamples = 0;
				FrameTiming[] timings = new FrameTiming[1];
				for (int i = 0; i < 60; i++)
				{
					for (int k = 0; k < 8; k++)
					{
						Graphics.Blit(Texture2D.whiteTexture, rt, mat);
					}
					await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, ct);
					FrameTimingManager.CaptureFrameTimings();
					if (FrameTimingManager.GetLatestTimings(1u, timings) != 0 && timings[0].gpuFrameTime > 0.0)
					{
						totalGpuMs += timings[0].gpuFrameTime;
						validSamples++;
					}
				}
				if (validSamples < 20)
				{
					return -1;
				}
				double num = totalGpuMs / (double)validSamples / 8.0;
				result = ((num <= 4.5) ? 2 : ((num <= 7.0) ? 1 : 0));
			}
			finally
			{
				if (rt != null)
				{
					RenderTexture.ReleaseTemporary(rt);
				}
				if (mat != null)
				{
					Object.Destroy(mat);
				}
			}
			return result;
		}

		private void StoreTier(int tier)
		{
			if (tier >= 0)
			{
				PlayerPrefs.SetInt("GfxTier_v3", tier);
				PlayerPrefs.Save();
			}
		}
	}
}
