using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Photon.Realtime;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	public class RpcRrRequestTest : RpcRrRequestObjectBase
	{
		private readonly ISlotService _slotService;

		public override CodeType Code => CodeType.Test;

		public static string GetRequestData(int dataInt)
		{
			return $"{dataInt}";
		}

		[Inject]
		public RpcRrRequestTest(ISlotService slotService)
		{
			_slotService = slotService;
		}

		public override async UniTask<string> ExecuteRequestAsync(Player sender, string requestData)
		{
			int num = int.Parse(requestData);
			foreach (CarSlot allSlotsInfo in _slotService.AllSlotsInfos)
			{
				Debug.LogError(JsonConvert.SerializeObject(allSlotsInfo));
			}
			return $"{num + 10}";
		}

		public override RpcRrResponseObjectBase CreateResponse(string executedRequestData)
		{
			return new RpcRrResponseTest(int.Parse(executedRequestData), RpcRrResponseObjectBase.CodeType.Success);
		}
	}
}
