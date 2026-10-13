using UnityEngine;

namespace CPM.Scripts.VinylDrawer
{
	public class PlateFreeLookWindow : MonoBehaviour, IBackableWindow
	{
		private VinylsEditor _vinylsEditor;

		public void Open(VinylsEditor vinylsEditor)
		{
			_vinylsEditor = vinylsEditor;
			InitBackButton();
		}

		public void InitBackButton()
		{
			V.CoreGameManager.ManagerUI.OnBackButtonActivated?.Invoke(obj: true);
			V.CoreGameManager.ManagerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_vinylsEditor.PlateFreeLookToggle();
		}
	}
}
