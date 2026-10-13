using Cysharp.Threading.Tasks;

namespace CPM.Scripts.Garage.FreeDriverService
{
	public interface IFreeDriverService
	{
		void PlaceDriverOnDefault();

		UniTask SetupFreeDriver(bool withDelay = false);

		void CreateDriver(bool withSetup = true);
	}
}
