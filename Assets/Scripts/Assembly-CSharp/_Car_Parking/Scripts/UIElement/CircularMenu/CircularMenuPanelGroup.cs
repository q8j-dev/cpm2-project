using System;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.CircularMenu
{
	[Serializable]
	public struct CircularMenuPanelGroup
	{
		[HideInInspector]
		public string Name;

		public CircularMenuPanel[] Panels;

		public bool HasPanels
		{
			get
			{
				CircularMenuPanel[] panels = Panels;
				if (panels != null)
				{
					return panels.Length > 0;
				}
				return false;
			}
		}
	}
}
