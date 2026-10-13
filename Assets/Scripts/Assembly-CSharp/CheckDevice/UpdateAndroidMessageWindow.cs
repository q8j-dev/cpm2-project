using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

namespace CheckDevice
{
	public class UpdateAndroidMessageWindow : WindowScript
	{
		[SerializeField]
		private Button AcceptButton = default;

		protected override void Start()
		{
			AcceptButton.onClick.AddListener(Close);
			base.OnEnable();
		}

		protected override void OnDestroy()
		{
			AcceptButton.onClick.RemoveAllListeners();
			base.OnDestroy();
		}
	}
}
