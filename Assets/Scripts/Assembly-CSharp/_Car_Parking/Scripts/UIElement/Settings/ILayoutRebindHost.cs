using UnityEngine.InputSystem;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public interface ILayoutRebindHost
	{
		bool IsActionDisplayed(InputAction action);

		bool IsRowUnusedIn(InputAction action, string part, InputGroupsEnum group);

		bool IsHoldArrowForced(InputAction action, string part);

		LocalizedString ResolveRowLabel(InputAction action, string part);

		void ShowRowMessage(int rowIndex, LocalizedString message, bool? holdArrow = null);

		void HighlightRow(int rowIndex, RebindRowHighlight highlight);

		void PreviewRow(int rowIndex, string keyPath, string modifierPath);

		void RestoreRow(int rowIndex);

		void OnBindingsApplied();
	}
}
