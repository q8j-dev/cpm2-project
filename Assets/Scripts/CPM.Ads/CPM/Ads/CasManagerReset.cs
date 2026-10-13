using System;
using System.Reflection;
using CAS;
using UnityEngine;

namespace CPM.Ads
{
	internal static class CasManagerReset
	{
		private const string FactoryTypeName = "CAS.CASFactory";

		private const string ManagersFieldName = "managers";

		public static bool TryReleaseCachedManagers()
		{
			try
			{
				Type type = typeof(MobileAds).Assembly.GetType("CAS.CASFactory");
				if (type == null)
				{
					Debug.LogWarning("[Ads] CAS.CASFactory not found. The CAS plugin changed; the mediation manager cannot be rebuilt for a new player id.");
					return false;
				}
				FieldInfo field = type.GetField("managers", BindingFlags.Static | BindingFlags.NonPublic);
				if (field == null)
				{
					Debug.LogWarning("[Ads] CAS.CASFactory.managers not found. The CAS plugin changed; the mediation manager cannot be rebuilt.");
					return false;
				}
				field.SetValue(null, null);
				return true;
			}
			catch (Exception exception)
			{
				Debug.LogWarning("[Ads] Failed to release the cached CAS manager.");
				Debug.LogException(exception);
				return false;
			}
		}
	}
}
