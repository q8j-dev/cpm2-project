using Photon.Pun;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcSpectator : RpcObjectBase
	{
		[Inject]
		public FreeDriveProvider _freeDriveProvider;

		public override byte Code => 69;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string data)
		{
			string[] array = b.Split(',');
			if (array.Length <= 1 || array[0] == null || !int.TryParse(array[0], out var result))
			{
				return;
			}
			string text = array[1];
			if (_freeDriveProvider == null || !_freeDriveProvider.FirstCarMultiPlayer)
			{
				return;
			}
			PhotonView component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>();
			if (!component || component.ViewID != result)
			{
				return;
			}
			bool flag = data == "1";
			if (_freeDriveProvider.FirstCarMultiPlayer.TryGetComponent<MultiPlayerPosControl>(out var component2) && !string.IsNullOrEmpty(text))
			{
				if (!component2.CachedSpectators.Contains(text))
				{
					component2.CachedSpectators.Add(text);
				}
				if (!flag)
				{
					component2.CachedSpectators.Remove(text);
				}
			}
		}
	}
}
