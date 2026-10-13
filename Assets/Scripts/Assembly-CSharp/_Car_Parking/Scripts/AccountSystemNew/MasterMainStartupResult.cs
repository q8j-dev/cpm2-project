using CPM.Connection.Entities;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	[Preserve]
	public class MasterMainStartupResult
	{
		[JsonProperty("connectionData")]
		public UserConnectionData ConnectionData;

		[JsonProperty("allCars")]
		public string AllCars;

		[JsonProperty("playerRecords")]
		public string PlayerRecords;
	}
}
