using System.Collections.Generic;
using _Car_Parking.Scripts.CommonUtility;

namespace CPM.Account
{
	public interface IPrefsSaversGetter
	{
		List<IAllPrefsSaver> GetPrefsSavers();
	}
}
