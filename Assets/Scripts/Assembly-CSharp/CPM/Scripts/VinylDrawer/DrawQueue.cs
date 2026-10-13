using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace CPM.Scripts.VinylDrawer
{
	public class DrawQueue
	{
		private readonly Queue<IVinylDrawPerformer> _queue;

		private UniTask? _drawJob;

		private VinylDrawer _vinylDrawer;

		public bool IsEmpty
		{
			get
			{
				if (_queue.Count == 0)
				{
					return IsJobFree;
				}
				return false;
			}
		}

		private bool IsJobFree
		{
			get
			{
				UniTask? drawJob = _drawJob;
				return !drawJob.HasValue || drawJob.GetValueOrDefault().Status != UniTaskStatus.Pending;
			}
		}

		[Inject]
		public DrawQueue(VinylDrawer vinylDrawer)
		{
			_vinylDrawer = vinylDrawer;
			_queue = new Queue<IVinylDrawPerformer>(20);
		}

		public void Enqueue(IVinylDrawPerformer performer)
		{
			_queue.Enqueue(performer);
			if (IsJobFree)
			{
				_drawJob = DrawJob();
			}
		}

		private async UniTask DrawJob()
		{
			while (_queue.Count > 0)
			{
				IVinylDrawPerformer performer = _queue.Dequeue();
				MonoBehaviour componentToCheck = performer.ComponentToCheck;
				if (!IsInGame())
				{
					continue;
				}
				if (performer.GetVinylsCount(VinylsType.CarVinyls, isFrontPlate: false) > 0)
				{
					if (!IsInGame())
					{
						continue;
					}
					await TryRenderVinyls(VinylsType.CarVinyls, isFrontPlate: false);
				}
				if (performer.GetVinylsCount(VinylsType.PlateVinyls, isFrontPlate: true) > 0)
				{
					if (!IsInGame())
					{
						continue;
					}
					await TryRenderVinyls(VinylsType.PlateVinyls, isFrontPlate: true);
				}
				if (performer.GetVinylsCount(VinylsType.PlateVinyls, isFrontPlate: false) > 0)
				{
					if (!IsInGame())
					{
						continue;
					}
					await TryRenderVinyls(VinylsType.PlateVinyls, isFrontPlate: false);
				}
				if (performer.GetVinylsCount(VinylsType.WindowVinyls, isFrontPlate: false) > 0 && IsInGame())
				{
					await TryRenderVinyls(VinylsType.WindowVinyls, isFrontPlate: false);
				}
				bool IsInGame()
				{
					if ((bool)componentToCheck && (bool)_vinylDrawer)
					{
						return true;
					}
					return false;
				}
				async UniTask TryRenderVinyls(VinylsType type, bool isFrontPlate)
				{
					performer.RenderVinyls(type, isFrontPlate);
					await UniTask.DelayFrame(2);
					float deadline = Time.realtimeSinceStartup + 10f;
					while ((bool)_vinylDrawer && _vinylDrawer.ContainerChildCount > 0)
					{
						if (Time.realtimeSinceStartup >= deadline)
						{
							_vinylDrawer.ForceCleanContainer();
							Debug.LogError($"ForceClean {type}");
							await UniTask.DelayFrame(2);
							break;
						}
						await UniTask.Yield();
					}
				}
			}
		}
	}
}
