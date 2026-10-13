using System.Collections.Generic;
using _Car_Parking.Scripts.CommonUtility;

namespace CPM.Account
{
	public interface IDeleterUserDataGetter
	{
		List<IDeleteUserData> GetDeleteUserDatas();

		void Add(IDeleteUserData deleteUserData);

		void Remove(IDeleteUserData deleteUserData);
	}
}
