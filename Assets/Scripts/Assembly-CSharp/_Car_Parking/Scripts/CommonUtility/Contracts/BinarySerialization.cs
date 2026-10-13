using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility.Contracts
{
	public static class BinarySerialization
	{
		public static byte[] Serialize<T>(T myStruct)
		{
			try
			{
				using MemoryStream memoryStream = new MemoryStream();
				new BinaryFormatter().Serialize(memoryStream, myStruct);
				return memoryStream.ToArray();
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				return null;
			}
		}

		public static T Deserialize<T>(byte[] byteArray)
		{
			using MemoryStream serializationStream = new MemoryStream(byteArray);
			return (T)new BinaryFormatter().Deserialize(serializationStream);
		}
	}
}
