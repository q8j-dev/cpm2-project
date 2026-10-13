namespace _Car_Parking.Scripts.Other.Cypher
{
	public class CypherMediator
	{
		public static ICypherCustom _cypherCustom;

		public static ICypherCustom CypherCustom
		{
			get
			{
				if (_cypherCustom == null)
				{
					_cypherCustom = new CypherStandalone();
				}
				return _cypherCustom;
			}
		}

		public static string Decode(string encryptedString, string getKey1, string getKey2)
		{
			return CypherCustom.Decode(encryptedString, getKey1, getKey2);
		}

		public static string Encode(string message, string key1, string key2)
		{
			return CypherCustom.Encode(message, key1, key2);
		}
	}
}
