using System.Collections.Generic;
using CPM.Account;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	public class DeleterUserDataGetter : IDeleterUserDataGetter
	{
		private List<IDeleteUserData> _deleters = new List<IDeleteUserData>();

		public List<IDeleteUserData> GetDeleteUserDatas()
		{
			List<IDeleteUserData> list = new List<IDeleteUserData>();
			for (int i = 0; i < _deleters.Count; i++)
			{
				list.Add(_deleters[i]);
			}
			return list;
		}

		public void Add(IDeleteUserData deleteUserData)
		{
			_deleters.Add(deleteUserData);
		}

		public void Remove(IDeleteUserData deleteUserData)
		{
			if (deleteUserData != null)
			{
				_deleters.Remove(deleteUserData);
			}
		}
	}
}
