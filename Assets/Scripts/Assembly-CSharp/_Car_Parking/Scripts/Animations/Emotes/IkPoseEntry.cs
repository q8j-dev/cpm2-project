using UnityEngine;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.PlayerProfile;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.Animations.Emotes
{
	public class IkPoseEntry : IEmoteEntry
	{
		private readonly PoseCatalogSO.PoseEntry _pose;

		public int Id => PoseCatalogSO.SlotOffsetFor(_pose.Audience) + _pose.IKPoseId;

		public int IkPoseId => _pose.IKPoseId;

		public Sprite Sprite => _pose.Sprite;

		public PoseAudience Audience => _pose.Audience;

		public AnimationShopTab ShopTab => AnimationShopTab.Poses;

		public GenderMismatchPolicy GenderMismatchPolicy => GenderMismatchPolicy.ShowDisabled;

		public EmoteBuyFlow BuyFlow => EmoteBuyFlow.Bundle;

		public int Price => 0;

		public int BundleId => _pose.BundleId;

		public bool IsOwned
		{
			get
			{
				if (!_pose.IsFree)
				{
					return PoseOwnership.Owns(IkPoseId);
				}
				return true;
			}
		}

		public bool CanAssignToWheel => IsOwned;

		public float? PreviewAutoStopSeconds => null;

		public bool KeepsRunningOnTurnOff => false;

		public IkPoseEntry(PoseCatalogSO.PoseEntry pose)
		{
			_pose = pose;
		}

		public void Preview(in EmotePreviewContext ctx)
		{
			ctx.TurnOff?.Invoke();
			if (!ctx.DriverRagdolled && (bool)ctx.Wic)
			{
				ctx.Wic.PreviewPose(IkPoseId);
			}
		}

		public void StopPreview(in EmotePreviewContext ctx)
		{
			if ((bool)ctx.Wic)
			{
				ctx.Wic.StopPreview();
			}
		}

		public EmoteWheelTapResult ActivateFromWheel(in EmoteWheelContext ctx)
		{
			if ((ctx.Gender < 0 || PoseAudienceRules.Matches(_pose.Audience, ctx.Gender)) && IsOwned)
			{
				WorldInteractionController component = ctx.Driver.GetComponent<WorldInteractionController>();
				if ((bool)component)
				{
					component.Toggle(IkPoseId);
				}
			}
			return EmoteWheelTapResult.Activated;
		}

		void IEmoteEntry.Preview(in EmotePreviewContext ctx)
		{
			Preview(in ctx);
		}

		void IEmoteEntry.StopPreview(in EmotePreviewContext ctx)
		{
			StopPreview(in ctx);
		}

		EmoteWheelTapResult IEmoteEntry.ActivateFromWheel(in EmoteWheelContext ctx)
		{
			return ActivateFromWheel(in ctx);
		}
	}
}
