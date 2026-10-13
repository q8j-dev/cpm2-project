using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using CPM.Rating.Entities;
using MemoryPack;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Other
{
	public class MemoryPackHelper
	{
		private const int XorSwitchKey1 = 6;

		private const int XorSwitchKey2 = 4;

		private const int XorRemoveKey2 = 8;

		private const int XorAddKey1 = 0;

		public static IDictionary<string, object> DeserializeDictionary(string packed, Dictionary<string, Type> types, IDictionary<ushort, string> indexKeyMap, string accountID)
		{
			InitProvidersIfNeeds();
			Dictionary<ushort, byte[]>? dictionary = MemoryPackSerializer.Deserialize<Dictionary<ushort, byte[]>>(BrotliDecompress(Xor(Convert.FromBase64String(packed), GetXorKey(accountID))));
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			foreach (KeyValuePair<ushort, byte[]> item in dictionary)
			{
				if (indexKeyMap.TryGetValue(item.Key, out var value) && types.TryGetValue(value, out var value2))
				{
					dictionary2[value] = MemoryPackSerializer.Deserialize(value2, item.Value);
				}
			}
			return dictionary2;
		}

		public static string SerializeDictionary(IDictionary<string, object> changedData, IDictionary<string, ushort> keyIndexMap, string accountID)
		{
			InitProvidersIfNeeds();
			Dictionary<ushort, byte[]> value = new Dictionary<ushort, byte[]>();
			try
			{
				if (changedData == null)
				{
					return null;
				}
				ICollection<string> keys = changedData.Keys;
				List<string> list = new List<string>();
				foreach (string item in keys)
				{
					list.Add(item);
				}
				while (list.Count > 0)
				{
					int index = UnityEngine.Random.Range(0, list.Count);
					string text = list[index];
					list.RemoveAt(index);
					if (!string.IsNullOrEmpty(text) && changedData[text] != null && keyIndexMap.TryGetValue(text, out var value2))
					{
						value[value2] = MemoryPackSerializer.Serialize(changedData[text].GetType(), changedData[text]);
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return "";
			}
			return Convert.ToBase64String(Xor(BrotliCompress(MemoryPackSerializer.Serialize(in value)), GetXorKey(accountID)));
		}

		public static string SerializeJson<T>(string json, string accountID, bool useBrotli = false)
		{
			InitProvidersIfNeeds();
			string result = "";
			try
			{
				return SerializeObject(JsonConvert.DeserializeObject<T>(json), accountID, useBrotli);
			}
			catch (Exception)
			{
				return result;
			}
		}

		public static string SerializeObject<T>(T obj, string accountID, bool useBrotli = false)
		{
			InitProvidersIfNeeds();
			string result = "";
			try
			{
				byte[] bin = MemoryPackSerializer.Serialize(in obj);
				if (useBrotli)
				{
					bin = BrotliCompress(bin);
				}
				bin = Xor(bin, GetXorKey(accountID));
				result = Convert.ToBase64String(bin);
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static Result<T, string> TryDeserialize<T>(string stringBase64, string accountID, bool useBrotli = false)
		{
			InitProvidersIfNeeds();
			try
			{
				byte[] bin = Convert.FromBase64String(stringBase64);
				bin = Xor(bin, GetXorKey(accountID));
				if (useBrotli)
				{
					bin = BrotliDecompress(bin);
				}
				return Result<T, string>.Success(MemoryPackSerializer.Deserialize<T>(bin));
			}
			catch (Exception ex)
			{
				return Result<T, string>.Fail(ex.Message);
			}
		}

		private static void InitProvidersIfNeeds()
		{
			MemoryPackFormatterProvider.Register(new RatingDataFormatter());
			MemoryPackFormatterProvider.Register(new ColorFormatter());
			MemoryPackFormatterProvider.Register(new CarPaintColorFormatter());
			MemoryPackFormatterProvider.Register(new InstalledBodyKitsFormatter());
			MemoryPackFormatterProvider.Register(new ColorSerializableWithAlphaFormatter());
			MemoryPackFormatterProvider.Register(new PartDataFormatter());
			MemoryPackFormatterProvider.Register(new MaintenanceInfoFormatter());
			MemoryPackFormatterProvider.Register(new BodyKitColorFormatter());
			MemoryPackFormatterProvider.Register(new Vector3Formatter());
		}

		private static byte[] Xor(byte[] bin, byte[] xorKey)
		{
			for (int i = 0; i < bin.Length; i++)
			{
				bin[i] ^= xorKey[i % xorKey.Length];
			}
			return bin;
		}

		private static byte[] GetXorKey(string accountID)
		{
			if (string.IsNullOrEmpty(accountID))
			{
				accountID = "1";
			}
			List<char> list = new List<char>(accountID.ToCharArray());
			if (6 < list.Count && 4 < list.Count)
			{
				char value = list[6];
				char value2 = list[4];
				list[6] = value2;
				list[4] = value;
			}
			if (8 < list.Count)
			{
				list.RemoveAt(8);
			}
			if (0 < list.Count)
			{
				list.Add(list[0]);
			}
			accountID = new string(list.ToArray());
			return Encoding.UTF8.GetBytes(accountID);
		}

		private static byte[] BrotliDecompress(byte[] bin)
		{
			using MemoryStream stream = new MemoryStream(bin);
			using BrotliStream brotliStream = new BrotliStream(stream, CompressionMode.Decompress);
			using MemoryStream memoryStream = new MemoryStream();
			brotliStream.CopyTo(memoryStream);
			return memoryStream.ToArray();
		}

		private static byte[] BrotliCompress(byte[] bin)
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BrotliStream brotliStream = new BrotliStream(memoryStream, CompressionLevel.Optimal);
			brotliStream.Write(bin, 0, bin.Length);
			brotliStream.Close();
			return memoryStream.ToArray();
		}
	}
}
