using System;
using System.Threading;
using CPM.Scripts.VinylDrawer;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class WindowVinylsTuningScope : VisualTuningScope
	{
		private VinylDrawer _vinylDrawer;

		[Inject]
		public void Construct(VinylDrawer vinylDrawer)
		{
			_vinylDrawer = vinylDrawer;
		}

		public override void Init()
		{
			Update();
		}

		private void Update()
		{
			_vinylDrawer.SetCamState(isActive: false);
			_vinylDrawer.DeleteVinylContext(VinylsType.WindowVinyls);
			SwitchToCarVinyls(_vinylDrawer, _vinylDrawer.GetCancellationTokenOnDestroy()).Forget();
		}

		public static async UniTask SwitchToCarVinyls(VinylDrawer vinylDrawer, CancellationToken ct = default(CancellationToken))
		{
			_ = 3;
			try
			{
				await UniTask.WaitForEndOfFrame(vinylDrawer, ct);
				await UniTask.WaitForEndOfFrame(vinylDrawer, ct);
				await UniTask.WaitForEndOfFrame(vinylDrawer, ct);
				await UniTask.WaitForEndOfFrame(vinylDrawer, ct);
				vinylDrawer.SetCamState(isActive: false);
				vinylDrawer.FocusOnVinyls(VinylsType.CarVinyls);
				vinylDrawer.SetCamState(isActive: true);
			}
			catch (OperationCanceledException)
			{
			}
		}

		public override void Apply()
		{
			throw new NotImplementedException();
		}

		public override void Revert()
		{
			throw new NotImplementedException();
		}

		public override void Select(int index)
		{
			throw new NotImplementedException();
		}
	}
}
