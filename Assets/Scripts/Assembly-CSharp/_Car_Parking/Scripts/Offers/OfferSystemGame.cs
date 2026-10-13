using System;
using System.Collections.Generic;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.CDN;
using _Car_Parking.Scripts.DataEntities;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Offers
{
	public class OfferSystemGame : OfferSystemBase
	{
		private class GetOfferRequest
		{
			[JsonProperty("version")]
			public string Version;

			[JsonProperty("platform")]
			public string Platform;
		}

		public const string GetOfferFn = "GetOffersForPlayer24_1";

		private OfferModel[] _offers = Array.Empty<OfferModel>();

		private readonly PlayerDataStorage _playerDataStorage;

		private readonly ICDNDownloader _cdnDownloader;

		private bool _isCdnProcess;

		public override OfferModel[] Offers => _offers;

		public override bool IsDataReady
		{
			get
			{
				if (base.IsDataReady && Offers != null)
				{
					for (int i = 0; i < Offers.Length; i++)
					{
						if (!string.IsNullOrEmpty(Offers[i].Skin.ImgPath) && !_cdnDownloader.HaveLocalCache(Offers[i].Skin.ImgPath))
						{
							return false;
						}
						if (!string.IsNullOrEmpty(Offers[i].Skin.BigImgPath) && !_cdnDownloader.HaveLocalCache(Offers[i].Skin.BigImgPath))
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		[Preserve]
		public OfferSystemGame(ICDNDownloader cdnDownloader, PlayerDataStorage playerDataStorage)
		{
			_cdnDownloader = cdnDownloader;
			_playerDataStorage = playerDataStorage;
		}

		public override void Initialize()
		{
			base.Initialize();
			SceneManager.sceneLoaded += SceneLoadedCallback;
			SceneManager.sceneUnloaded += SceneUnloadedCallback;
			_playerDataStorage.OnAccountIdUpdatedEvent += OnAccountIDUpdated;
		}

		public override void Dispose()
		{
			base.Dispose();
			SceneManager.sceneLoaded -= SceneLoadedCallback;
			SceneManager.sceneUnloaded -= SceneUnloadedCallback;
			_playerDataStorage.OnAccountIdUpdatedEvent -= OnAccountIDUpdated;
		}

		public override async UniTask<bool> SyncOffers()
		{
			FirebaseWrappedFunction firebaseWrappedFunction = new FirebaseWrappedFunctionFactory().Create("GetOffersForPlayer24_1");
			GetOfferRequest value = new GetOfferRequest
			{
				Version = Application.version,
				Platform = ApplicationCpm.Platform
			};
			FunctionCallResult<OfferModel[]> functionCallResult = await firebaseWrappedFunction.CallAsyncResult<OfferModel[]>(JsonConvert.SerializeObject(value, JsonConvertCustomSettingsForDatabase.JsonSettingsForDataBase));
			if (functionCallResult.IsSuccess)
			{
				_offers = functionCallResult.Data;
			}
			else
			{
				_offers = Array.Empty<OfferModel>();
			}
			if (_offers == null)
			{
				_offers = Array.Empty<OfferModel>();
			}
			OnDataUpdated();
			return functionCallResult.IsSuccess;
		}

		protected override void OnDataUpdated()
		{
			base.OnDataUpdated();
			if (Offers != null)
			{
				UpdateCdn();
				if (IsDataReady)
				{
					OnDataUpdatedSendEvent();
				}
			}
		}

		private void SceneLoadedCallback(Scene scene, LoadSceneMode loadSceneMode)
		{
			UpdateCdn();
		}

		private void SceneUnloadedCallback(Scene scene)
		{
			UpdateCdn();
		}

		private void UpdateCdn()
		{
			if (Offers != null)
			{
				UpdateCdnAsync().Forget();
			}
		}

		private async UniTaskVoid UpdateCdnAsync()
		{
			while (_isCdnProcess)
			{
				await UniTask.Yield();
			}
			if (Offers == null)
			{
				return;
			}
			_isCdnProcess = true;
			if (ScenesHelper.IsMenuSceneActive())
			{
				if (IsDataReady)
				{
					_isCdnProcess = false;
					return;
				}
				List<UniTask> list = new List<UniTask>();
				if (Offers != null)
				{
					for (int i = 0; i < Offers.Length; i++)
					{
						list.Add(_cdnDownloader.LoadImageAsync(Offers[i].Skin.ImgPath));
						list.Add(_cdnDownloader.LoadImageAsync(Offers[i].Skin.BigImgPath));
					}
				}
				await UniTask.WhenAll(list);
				OnDataUpdatedSendEvent();
			}
			else
			{
				if (!IsDataReady)
				{
					_isCdnProcess = false;
					return;
				}
				if (Offers != null)
				{
					for (int j = 0; j < Offers.Length; j++)
					{
						_cdnDownloader.ClearLocalCache(Offers[j].Skin.ImgPath);
						_cdnDownloader.ClearLocalCache(Offers[j].Skin.BigImgPath);
					}
				}
				OnDataUpdatedSendEvent();
			}
			_isCdnProcess = false;
		}

		private void OnAccountIDUpdated(ObscuredString accountId)
		{
			_offers = Array.Empty<OfferModel>();
			IsDataReady = false;
			OnDataUpdatedSendEvent();
		}
	}
}
