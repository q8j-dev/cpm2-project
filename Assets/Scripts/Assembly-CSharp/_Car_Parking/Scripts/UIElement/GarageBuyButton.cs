using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class GarageBuyButton : MonoBehaviour
	{
		[field: SerializeField]
		public TextMeshProUGUI ButtonText { get; private set; }

		[field: SerializeField]
		public Button Button { get; private set; }
	}
}
