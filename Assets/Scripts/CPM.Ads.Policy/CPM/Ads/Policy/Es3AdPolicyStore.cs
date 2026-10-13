using System;
using System.Collections.Generic;
using UnityEngine;

namespace CPM.Ads.Policy
{
	public sealed class Es3AdPolicyStore : IAdPolicyStore
	{
		private const string StateKey = "adPolicyState";

		private readonly string _filePath;

		public AdPolicyState State { get; private set; }

		public Es3AdPolicyStore(string filePath)
		{
			_filePath = filePath;
			State = Load();
		}

		public void Persist()
		{
			try
			{
				ES3.Save("adPolicyState", State, _filePath);
			}
			catch (Exception ex)
			{
				Debug.LogError("Es3AdPolicyStore: failed to persist ad policy state to '" + _filePath + "'. " + ex.Message);
			}
		}

		private AdPolicyState Load()
		{
			try
			{
				if (ES3.FileExists(_filePath) && ES3.KeyExists("adPolicyState", _filePath))
				{
					AdPolicyState adPolicyState = ES3.Load<AdPolicyState>("adPolicyState", _filePath);
					if (adPolicyState != null)
					{
						AdPolicyState adPolicyState2 = adPolicyState;
						if (adPolicyState2.Placements == null)
						{
							adPolicyState2.Placements = new List<PlacementEntry>();
						}
						foreach (PlacementEntry placement in adPolicyState.Placements)
						{
							if (placement.Contexts == null)
							{
								placement.Contexts = new List<ContextEntry>();
							}
						}
						return adPolicyState;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Es3AdPolicyStore: unreadable ad policy state at '" + _filePath + "', starting fresh. " + ex.Message);
			}
			return new AdPolicyState();
		}
	}
}
