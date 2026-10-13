using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;

namespace CPM.Connection
{
	public interface IConnectionManager : IConnectionManagerBase
	{
		ObscuredInt H { get; }

		event Action OnHashStatusChange;

		void SubscribeForConnectionDataHandle(IConnectionDataHandler connectionDataHandler);

		UniTaskVoid ReconnectPhoton(PhotonConnectionProfile profile);

		void SetNotLicensedApp();
	}
}
