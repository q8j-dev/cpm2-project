using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	internal class RpcPolicePenaltyAnswerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcPolicePenaltyAnswer rpcPolicePenaltyAnswer = new RpcPolicePenaltyAnswer();
			Inject(rpcPolicePenaltyAnswer, resolver, parameters);
			return rpcPolicePenaltyAnswer;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcPolicePenaltyAnswer obj = (RpcPolicePenaltyAnswer)instance;
			ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
			obj.Construct(steamAchievementService);
		}
	}
}
