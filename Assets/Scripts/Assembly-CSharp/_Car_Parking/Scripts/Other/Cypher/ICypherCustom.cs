namespace _Car_Parking.Scripts.Other.Cypher
{
	public interface ICypherCustom
	{
		string Decode(string encryptedString, string key1, string key2);

		string Encode(string message, string key1, string key2);
	}
}
