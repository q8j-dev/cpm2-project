using Cysharp.Threading.Tasks;

namespace CPM.RemoteConfig
{
	public interface IRemoteConfigManager
	{
		UniTask<ConfigValuesStorage> GetConfigValuesStorage();

		ConfigValuesStorage GetDefaultConfigStorage();
	}
}
