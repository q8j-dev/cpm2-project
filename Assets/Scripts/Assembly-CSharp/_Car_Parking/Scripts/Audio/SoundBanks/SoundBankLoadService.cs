using System;
using System.Collections.Generic;
using AK.Wwise;
using AK.Wwise.Unity.WwiseAddressables;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Audio.SoundBanks
{
	public class SoundBankLoadService : IInitializable, IDisposable
	{
		private const int MaxFallShouts = 3;

		private const float FallShoutSeconds = 2f;

		private readonly List<IDisposable> _fallShoutTimers = new List<IDisposable>();

		private const string CatalogAddress = "SoundBankCatalog";

		private const float TimeoutWaitBank = 1f;

		private readonly Dictionary<SoundBankType, int> _refCounts = new Dictionary<SoundBankType, int>();

		private readonly HashSet<SoundBankType> _loading = new HashSet<SoundBankType>();

		private readonly HashSet<SoundBankType> _loaded = new HashSet<SoundBankType>();

		private Dictionary<SoundBankType, Bank> _banks;

		private bool _catalogLoading;

		public void Initialize()
		{
			LoadInitialBanks().Forget();
		}

		private async UniTaskVoid LoadInitialBanks()
		{
			await EnsureCatalogLoaded();
			TryLoadBank(SoundBankType.SfxMain).Forget();
			TryLoadBank(SoundBankType.AmbMain).Forget();
			TryLoadBank(SoundBankType.CpmUI).Forget();
			TryLoadBank(SoundBankType.CpmSoundBank).Forget();
			TryLoadBank(SoundBankType.CarSfxMain).Forget();
			TryLoadBank(SoundBankType.CarEngineMechSounds).Forget();
		}

		public bool TryTakeFallShoutSlot()
		{
			if (_fallShoutTimers.Count >= 3)
			{
				return false;
			}
			IDisposable timer = null;
			timer = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				_fallShoutTimers.Remove(timer);
			});
			_fallShoutTimers.Add(timer);
			return true;
		}

		public void Dispose()
		{
			foreach (IDisposable fallShoutTimer in _fallShoutTimers)
			{
				fallShoutTimer.Dispose();
			}
			_fallShoutTimers.Clear();
		}

		public bool IsLoaded(SoundBankType bankType)
		{
			return _loaded.Contains(bankType);
		}

		public bool IsLoading(SoundBankType bankType)
		{
			return _loading.Contains(bankType);
		}

		public async UniTask TryLoadBank(SoundBankType bankType)
		{
			if (bankType == SoundBankType.None)
			{
				return;
			}
			_refCounts.TryGetValue(bankType, out var value);
			_refCounts[bankType] = value + 1;
			await EnsureCatalogLoaded();
			if (!_refCounts.ContainsKey(bankType))
			{
				return;
			}
			if (!TryGetBank(bankType, out var bank))
			{
				_refCounts.Remove(bankType);
				return;
			}
			if (!IsLoading(bankType) && !IsLoaded(bankType))
			{
				_loading.Add(bankType);
				bank.Load();
				WaitBankRegistered(bankType, bank).Forget();
			}
			if (IsLoading(bankType))
			{
				await UniTask.WaitUntil(() => !IsLoading(bankType));
			}
		}

		public async UniTask TryUnloadBank(SoundBankType bankType)
		{
			if (bankType == SoundBankType.None || !_refCounts.TryGetValue(bankType, out var value) || value <= 0)
			{
				return;
			}
			value--;
			if (value > 0)
			{
				_refCounts[bankType] = value;
				return;
			}
			_refCounts.Remove(bankType);
			if (IsLoading(bankType))
			{
				await UniTask.WaitUntil(() => !IsLoading(bankType));
			}
			if (!_refCounts.ContainsKey(bankType))
			{
				await UniTask.NextFrame();
				if (!_refCounts.ContainsKey(bankType) && _loaded.Remove(bankType) && TryGetBank(bankType, out var bank))
				{
					bank.Unload();
				}
			}
		}

		private async UniTaskVoid WaitBankRegistered(SoundBankType bankType, Bank bank)
		{
			AsyncOperationHandle? bankLoadHandle = GetBankLoadHandle(bank);
			if (bankLoadHandle.HasValue)
			{
				AsyncOperationHandle loadHandle = bankLoadHandle.Value;
				await UniTask.WaitUntil(() => !loadHandle.IsValid());
			}
			if (_loading.Remove(bankType))
			{
				_loaded.Add(bankType);
			}
		}

		private AsyncOperationHandle? GetBankLoadHandle(Bank bank)
		{
			Dictionary<string, AssetReferenceWwiseBankData> dictionary = bank?.WwiseObjectReference?.AddressableBank?.Data;
			if (dictionary == null)
			{
				return null;
			}
			foreach (AssetReferenceWwiseBankData value in dictionary.Values)
			{
				if (value != null && value.OperationHandle.IsValid())
				{
					return value.OperationHandle;
				}
			}
			return null;
		}

		private bool TryGetBank(SoundBankType bankType, out Bank bank)
		{
			bank = null;
			if (_banks != null && _banks.TryGetValue(bankType, out bank))
			{
				return bank != null;
			}
			return false;
		}

		private async UniTask EnsureCatalogLoaded()
		{
			if (_banks != null)
			{
				return;
			}
			if (_catalogLoading)
			{
				await UniTask.WaitUntil(() => _banks != null);
				return;
			}
			_catalogLoading = true;
			Dictionary<SoundBankType, Bank> map = new Dictionary<SoundBankType, Bank>();
			try
			{
				SoundBankCatalog soundBankCatalog = await Addressables.LoadAssetAsync<SoundBankCatalog>("SoundBankCatalog");
				if (soundBankCatalog != null)
				{
					foreach (SoundBankCatalog.Entry entry in soundBankCatalog.Entries)
					{
						if (entry != null && entry.Bank != null)
						{
							map[entry.Type] = entry.Bank;
						}
					}
				}
			}
			catch (Exception)
			{
			}
			_banks = map;
			_catalogLoading = false;
		}

		public UniTask PostEvent(string soundEvent, SoundBankType bankType, GameObject target)
		{
			return PostInternal(bankType, target, () =>
			{
				AudioWrapper.PostEvent(soundEvent, target);
			});
		}

		public UniTask PostEvent(uint soundEvent, SoundBankType bankType, GameObject target)
		{
			return PostInternal(bankType, target, () =>
			{
				AudioWrapper.PostEvent(soundEvent, target);
			});
		}

		public async UniTask<uint> PostEventGetId(uint soundEvent, SoundBankType bankType, GameObject target)
		{
			bool flag = await CheckLoadedBank(bankType);
			if (target == null || !flag)
			{
				return 0u;
			}
			return AudioWrapper.PostEvent(soundEvent, target);
		}

		public UniTask PostEvent(uint soundEvent, SoundBankType bankType, GameObject target, uint flags, AkCallbackManager.EventCallback callback, object cookie)
		{
			return PostInternal(bankType, target, () =>
			{
				AudioWrapper.PostEvent(soundEvent, target, flags, callback, cookie);
			});
		}

		public UniTask PostEvent(AK.Wwise.Event soundEvent, SoundBankType bankType, GameObject target)
		{
			return PostInternal(bankType, target, () =>
			{
				soundEvent?.Post(target);
			});
		}

		private async UniTask PostInternal(SoundBankType bankType, GameObject target, Action post)
		{
			bool flag = await CheckLoadedBank(bankType);
			if (!(target == null) && flag)
			{
				post();
			}
		}

		private async UniTask<bool> CheckLoadedBank(SoundBankType bankType)
		{
			if (!IsLoaded(bankType) && IsLoading(bankType))
			{
				await UniTask.WaitUntil(() => IsLoaded(bankType)).TimeoutWithoutException(TimeSpan.FromSeconds(1.0));
			}
			return IsLoaded(bankType);
		}
	}
}
