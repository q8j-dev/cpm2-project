using System;
using CarInteractionAnimationsScripts;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.Animations.Emotes
{
	public readonly struct EmotePreviewContext
	{
		public readonly PersonAnimationsController Animations;

		public readonly WorldInteractionController Wic;

		public readonly NewMoveController Move;

		public readonly bool IsMainMenu;

		public readonly bool IsMultiFree;

		public readonly bool DriverRagdolled;

		public readonly Action TurnOff;

		public EmotePreviewContext(PersonAnimationsController animations, WorldInteractionController wic, NewMoveController move, bool isMainMenu, bool isMultiFree, bool driverRagdolled, Action turnOff)
		{
			Animations = animations;
			Wic = wic;
			Move = move;
			IsMainMenu = isMainMenu;
			IsMultiFree = isMultiFree;
			DriverRagdolled = driverRagdolled;
			TurnOff = turnOff;
		}
	}
}
