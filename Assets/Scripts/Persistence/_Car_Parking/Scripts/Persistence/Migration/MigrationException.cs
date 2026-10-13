using System;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	public class MigrationException : Exception
	{
		public ExceptionCode ExceptionCode { get; private set; }

		public MigrationException(string message, ExceptionCode exceptionCode)
			: base(message)
		{
			ExceptionCode = exceptionCode;
		}
	}
}
