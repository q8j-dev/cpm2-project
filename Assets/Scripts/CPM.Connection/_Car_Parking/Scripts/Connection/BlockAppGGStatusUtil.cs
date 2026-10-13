using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.Connection
{
	public class BlockAppGGStatusUtil
	{
		private ObscuredInt _blockGGStatus;

		public BlockAppGGStatusUtil(ObscuredInt blockAppStatus)
		{
			_blockGGStatus = blockAppStatus;
		}

		public bool BlockRoot()
		{
			if ((int)_blockGGStatus != 2)
			{
				return (int)_blockGGStatus == 3;
			}
			return true;
		}

		public bool BlockGG()
		{
			if ((int)_blockGGStatus != 1)
			{
				return (int)_blockGGStatus == 3;
			}
			return true;
		}
	}
}
