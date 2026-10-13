using UnityEngine;

namespace _Car_Parking.Scripts.Localization
{
	[ExecuteInEditMode]
	public class LocalizationSync : MonoBehaviour
	{
		public string TableId;

		public Sheet[] Sheets;

		public Object SaveFolder;

		private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";
	}
}
