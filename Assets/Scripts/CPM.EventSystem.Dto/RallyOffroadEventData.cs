using System;
using MemoryPack.Internal;
using Newtonsoft.Json;
using _Car_Parking.Scripts.EventSystem.Data.Response;

[Serializable]
[Preserve]
public class RallyOffroadEventData : EvenDataBase
{
	[JsonProperty("Cups")]
	public int[] Cups;

	[Preserve]
	public RallyOffroadEventData()
	{
	}
}
