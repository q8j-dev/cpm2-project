using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class PatchNotesItem : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text Text;

		public void Init(string data)
		{
			if ((bool)Text)
			{
				Text.text = data;
			}
		}
	}
}
