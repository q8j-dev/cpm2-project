using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods
{
	public class RpcCollection
	{
		private Dictionary<byte, RpcObjectBase> rpcObjects = new Dictionary<byte, RpcObjectBase>();

		private Dictionary<Type, byte> typeToCode = new Dictionary<Type, byte>();

		[Inject]
		public RpcCollection(IEnumerable<RpcObjectBase> rpcObjectBase)
		{
			rpcObjects.Clear();
			typeToCode.Clear();
			foreach (RpcObjectBase item in rpcObjectBase)
			{
				rpcObjects.Add(item.Code, item);
				typeToCode.Add(item.GetType(), item.Code);
			}
		}

		public byte GetRpcCode<T>() where T : RpcObjectBase
		{
			if (typeToCode.TryGetValue(typeof(T), out var value))
			{
				return value;
			}
			Debug.LogError("No RPC object of type " + typeof(T).Name + " was found.");
			return 0;
		}

		public RpcObjectBase GetRpcObject(byte a)
		{
			if (rpcObjects.TryGetValue(a, out var value))
			{
				return value;
			}
			return null;
		}
	}
}
