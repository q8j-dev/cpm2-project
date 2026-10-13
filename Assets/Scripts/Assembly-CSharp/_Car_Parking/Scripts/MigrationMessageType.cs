namespace _Car_Parking.Scripts
{
	public enum MigrationMessageType
	{
		Success = 0,
		IdEmpty = 1,
		CarsNull = 2,
		RecordsOrWalletNull = 3,
		CallException = 4,
		SignatureEmpty = 5,
		InvalidSignature = 6,
		InvalidResultCode = 7,
		NetworkError = 8
	}
}
