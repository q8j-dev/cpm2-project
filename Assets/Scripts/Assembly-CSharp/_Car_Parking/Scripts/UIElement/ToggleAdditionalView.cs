using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ToggleAdditionalView : MonoBehaviour
	{
		[SerializeField]
		private Toggle Toggle;

		[SerializeField]
		private GameObjectsActiveByUIState GameObjectsActiveByUIState;

		public void UpdateView()
		{
			if ((bool)Toggle && (bool)GameObjectsActiveByUIState)
			{
				GameObjectsActiveByUIState.UpdateState(Toggle.isOn ? GameObjectsActiveByUIState.StateType.Selected : GameObjectsActiveByUIState.StateType.Normal);
			}
		}

		private void OnEnable()
		{
			UpdateView();
			Toggle.onValueChanged.AddListener(ToggleValueChangedCallback);
		}

		private void OnDisable()
		{
			Toggle.onValueChanged.RemoveListener(ToggleValueChangedCallback);
		}

		private void ToggleValueChangedCallback(bool isOn)
		{
			UpdateView();
		}
	}
}
