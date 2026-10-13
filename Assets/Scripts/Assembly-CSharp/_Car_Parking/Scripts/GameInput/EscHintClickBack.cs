using UnityEngine;
using UnityEngine.EventSystems;

namespace _Car_Parking.Scripts.GameInput
{
	public class EscHintClickBack : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			if (V.CoreGameManager != null && V.CoreGameManager.ManagerUI != null)
			{
				V.CoreGameManager.ManagerUI.Back();
			}
		}
	}
}
