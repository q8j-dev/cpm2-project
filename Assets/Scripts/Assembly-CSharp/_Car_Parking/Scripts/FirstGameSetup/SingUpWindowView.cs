using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	public class SingUpWindowView : MonoBehaviour
	{
		[field: SerializeField]
		public TMP_InputField EmailInputField { get; private set; }

		[field: SerializeField]
		public TMP_InputField PasswordInputField { get; private set; }

		[field: SerializeField]
		public TMP_InputField ConfirmPasswordInputField { get; private set; }

		[field: SerializeField]
		public Button CreateAccountButton { get; private set; }

		[field: SerializeField]
		public Button ContinueAsGuestButton { get; private set; }
	}
}
