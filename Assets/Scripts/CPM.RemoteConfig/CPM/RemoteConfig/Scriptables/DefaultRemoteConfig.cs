using UnityEngine;

namespace CPM.RemoteConfig.Scriptables
{
	[CreateAssetMenu(menuName = "Scriptable/Config/DefaultRemote")]
	public class DefaultRemoteConfig : ScriptableObject
	{
		public ConfigValuesStorage configValuesStorage;
	}
}
