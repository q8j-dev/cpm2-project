using System;
using UnityEngine;

namespace Service.Abstract
{
	[Serializable]
	public struct Client
	{
		[SerializeField]
		private ClientController client;

		[field: SerializeField]
		public int id { get; private set; }

		public Client(int id, ClientController client)
		{
			this.id = id;
			this.client = client;
		}
	}
}
