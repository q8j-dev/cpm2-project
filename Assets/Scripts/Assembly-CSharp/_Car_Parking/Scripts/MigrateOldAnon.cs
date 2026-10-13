using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts
{
	public class MigrateOldAnon
	{
		public const string MigrateAnonymousFn = "MigrateAnonymous24_1";

		private const int MigrateTimeoutSeconds = 15;

		private const int MaxRetries = 2;

		private readonly DataBase _dataBase;

		public MigrateOldAnon(DataBase dataBase)
		{
			_dataBase = dataBase;
		}

		public async UniTask<MigrateResult> Migrate()
		{
			MigrateResult migrateResult = null;
			for (int attempt = 0; attempt < 2; attempt++)
			{
				migrateResult = await MigrateInternal();
				if (migrateResult.MigrationMessageType != MigrationMessageType.NetworkError)
				{
					break;
				}
				if (attempt < 1)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(2.0));
				}
			}
			return migrateResult;
		}

		private async UniTask<MigrateResult> MigrateInternal()
		{
			_ = 2;
			try
			{
				string firebaseUid = ManagerKeysForPrefs.AccountId;
				if (string.IsNullOrEmpty(firebaseUid))
				{
					return new MigrateResult(MigrationMessageType.IdEmpty);
				}
				var (flag, cars) = await TryLoadCarsAsync(firebaseUid);
				if (!flag)
				{
					return new MigrateResult(MigrationMessageType.CarsNull);
				}
				var (flag2, encryptedRecords, encryptedWallet) = await TryLoadRecordsWalletAsync(firebaseUid);
				if (!flag2)
				{
					return new MigrateResult(MigrationMessageType.RecordsOrWalletNull);
				}
				string deviceID = AccountSystemV2Extension.GetDeviceID();
				string.IsNullOrEmpty(deviceID);
				string guestEmail = AccountSystemV2Extension.GuestEmail;
				string guestPassword = AccountSystemV2Extension.GuestPassword;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(BuildId1());
				stringBuilder.Append(BuildId2());
				ObscuredString obscuredString = stringBuilder.ToString();
				string md5Hash = AccountSystemV2Extension.GetMd5Hash($"{firebaseUid}:{deviceID}:{guestEmail}:{guestPassword}:{obscuredString}");
				string text = await MigrateOldAnonymous(firebaseUid, deviceID, md5Hash, cars, encryptedRecords, encryptedWallet);
				if (string.IsNullOrWhiteSpace(text))
				{
					return new MigrateResult(MigrationMessageType.NetworkError);
				}
				TryParseMigrationSignature(text, out var migrateResult);
				return migrateResult;
			}
			catch (Exception e)
			{
				if (IsNetworkError(e))
				{
					return new MigrateResult(MigrationMessageType.NetworkError);
				}
				return new MigrateResult(MigrationMessageType.CallException);
			}
		}

		private async UniTask<string> MigrateOldAnonymous(string firebaseUid, string deviceId, string signature, List<string> encryptedCars, string encryptedRecords, string encryptedWallet)
		{
			FirebaseWrappedFunction firebaseWrappedFunction = new FirebaseWrappedFunctionFactory().Create("MigrateAnonymous24_1", 15);
			object value = new
			{
				uid = firebaseUid,
				deviceId = deviceId,
				md5Signature = signature,
				encryptedCars = encryptedCars,
				encryptedRecords = encryptedRecords,
				encryptedWallet = encryptedWallet
			};
			FunctionCallResult<string> functionCallResult = await firebaseWrappedFunction.CallAsyncResult<string>(JsonConvert.SerializeObject(value));
			if (functionCallResult.IsFail)
			{
				return "";
			}
			return functionCallResult.Data;
		}

		private bool TryParseMigrationSignature(string signature, out MigrateResult migrateResult)
		{
			if (string.IsNullOrWhiteSpace(signature))
			{
				migrateResult = new MigrateResult(MigrationMessageType.SignatureEmpty);
				return false;
			}
			string[] array = signature.Split(new char[1] { ';' }, 2, StringSplitOptions.None);
			if (!int.TryParse(array[0], out var result))
			{
				migrateResult = new MigrateResult(MigrationMessageType.InvalidSignature);
				return false;
			}
			if (result != 0 && result != 1)
			{
				migrateResult = new MigrateResult(MigrationMessageType.InvalidResultCode);
				return false;
			}
			bool isMigrated = result == 1;
			string message = ((array.Length > 1) ? array[1] : string.Empty);
			migrateResult = new MigrateResult(MigrationMessageType.Success, isMigrated, message);
			return true;
		}

		private ObscuredString BuildId1()
		{
			return string.Concat('K'.ToString(), 'L'.ToString(), char.ConvertFromUtf32(103), 'j'.ToString(), 'v'.ToString(), 'g'.ToString(), string.Concat('B', 'C'), 'a'.ToString(), 'A'.ToString(), 'e'.ToString(), new string('U', 2));
		}

		private ObscuredString BuildId2()
		{
			return '2'.ToString() + 'B' + 'm' + 'T' + 'y' + 'z' + 'd' + 'w' + 'u' + 'F' + 'Y';
		}

		private async UniTask<(bool success, List<string> cars)> TryLoadCarsAsync(string firebaseUid)
		{
			try
			{
				List<string> list = await _dataBase.GetAllCars(firebaseUid);
				if (list == null)
				{
					list = new List<string>();
				}
				return (true, list);
			}
			catch (Exception)
			{
				if (!InternetHelper.HasInternetConnection())
				{
					throw;
				}
				return (false, null);
			}
		}

		private async UniTask<(bool success, string records, string wallet)> TryLoadRecordsWalletAsync(string firebaseUid)
		{
			try
			{
				(string, string) tuple = await _dataBase.GetRecordsAndWalletAsync(firebaseUid);
				return (true, tuple.Item1, tuple.Item2);
			}
			catch (Exception)
			{
				if (!InternetHelper.HasInternetConnection())
				{
					throw;
				}
				return (false, null, null);
			}
		}

		private static bool IsNetworkError(Exception e)
		{
			for (Exception ex = e; ex != null; ex = ex.InnerException)
			{
				if (ex is TimeoutException)
				{
					return true;
				}
				if (ex is HttpRequestException)
				{
					return true;
				}
				if (ex is SocketException)
				{
					return true;
				}
				string message = ex.Message;
				if (message != null && (message.Contains("DEADLINE_EXCEEDED") || message.Contains("UNAVAILABLE") || message.Contains("timed out", StringComparison.OrdinalIgnoreCase)))
				{
					return true;
				}
			}
			if (!InternetHelper.HasInternetConnection())
			{
				return true;
			}
			return false;
		}
	}
}
