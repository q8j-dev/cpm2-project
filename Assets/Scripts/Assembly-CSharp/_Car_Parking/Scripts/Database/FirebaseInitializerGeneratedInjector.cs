using System.Collections.Generic;
using CPM.Account;
using VContainer;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Database
{
	[Preserve]
	internal class FirebaseInitializerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FirebaseInitializer firebaseInitializer = new FirebaseInitializer((IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters));
			Inject(firebaseInitializer, resolver, parameters);
			return firebaseInitializer;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FirebaseInitializer obj = (FirebaseInitializer)instance;
			CPMAnalytics analytics = (CPMAnalytics)resolver.ResolveOrParameter(typeof(CPMAnalytics), "analytics", parameters);
			obj.Construct(analytics);
		}
	}
}
