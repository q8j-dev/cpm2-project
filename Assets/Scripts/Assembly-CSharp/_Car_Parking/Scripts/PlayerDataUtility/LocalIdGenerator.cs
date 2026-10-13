using UnityEngine;

namespace _Car_Parking.Scripts.PlayerDataUtility
{
	public class LocalIdGenerator
	{
		public static string CreateNewID()
		{
			string text = "ABCDEFGHIJKLMNPQRSTUVWXYZ";
			string text2 = "";
			for (int i = 0; i < 2; i++)
			{
				text2 += text[Random.Range(0, text.Length)];
			}
			for (int j = 0; j < 6; j++)
			{
				text2 += Random.Range(0, 9);
			}
			return text2;
		}
	}
}
