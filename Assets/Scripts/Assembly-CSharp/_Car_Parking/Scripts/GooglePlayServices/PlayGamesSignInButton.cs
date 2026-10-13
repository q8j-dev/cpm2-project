using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.GooglePlayServices
{
	[RequireComponent(typeof(Button))]
	public class PlayGamesSignInButton : MonoBehaviour
	{
		private void Awake()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
