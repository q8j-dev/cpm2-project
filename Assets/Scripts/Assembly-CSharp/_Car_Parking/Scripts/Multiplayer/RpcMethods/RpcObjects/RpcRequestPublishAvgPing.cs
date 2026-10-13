using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	public class RpcRequestPublishAvgPing : RpcObjectBase
	{
		public override byte Code => 150;

		public static event Action OnReceived;

		[Preserve]
		public RpcRequestPublishAvgPing()
		{
		}

		public override void Execute(MultiPlayerPosControl _, string __, string ___)
		{
			OnReceived?.Invoke();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			OnReceived = null;
		}
	}
}
