using UnityEngine;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.Animations.Emotes
{
	public interface IEmoteEntry
	{
		int Id { get; }

		Sprite Sprite { get; }

		PoseAudience Audience { get; }

		AnimationShopTab ShopTab { get; }

		GenderMismatchPolicy GenderMismatchPolicy { get; }

		EmoteBuyFlow BuyFlow { get; }

		int Price { get; }

		int BundleId { get; }

		bool IsOwned { get; }

		bool CanAssignToWheel { get; }

		float? PreviewAutoStopSeconds { get; }

		bool KeepsRunningOnTurnOff { get; }

		void Preview(in EmotePreviewContext ctx);

		void StopPreview(in EmotePreviewContext ctx);

		EmoteWheelTapResult ActivateFromWheel(in EmoteWheelContext ctx);
	}
}
