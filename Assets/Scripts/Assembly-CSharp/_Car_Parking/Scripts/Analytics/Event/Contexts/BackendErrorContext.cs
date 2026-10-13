using System;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	[Serializable]
	public class BackendErrorContext : IEventContext
	{
		public int FunctionId { get; set; }

		public int FunctionStatus { get; set; }

		public string WindowId { get; set; }
	}
}
