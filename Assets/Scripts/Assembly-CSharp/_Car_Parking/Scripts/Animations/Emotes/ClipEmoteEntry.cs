using UnityEngine;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.PlayerProfile;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.Animations.Emotes
{
	public class ClipEmoteEntry : IEmoteEntry
	{
		private readonly AnimationDataSO.AnimationDataItem _item;

		public int Id => _item.Y;

		public Sprite Sprite => _item.sprite;

		public PoseAudience Audience => _item.Audience;

		public AnimationShopTab ShopTab => AnimationShopTab.Animations;

		public GenderMismatchPolicy GenderMismatchPolicy => GenderMismatchPolicy.Hide;

		public EmoteBuyFlow BuyFlow
		{
			get
			{
				if (_item.BundleId < 0)
				{
					return EmoteBuyFlow.Coins;
				}
				return EmoteBuyFlow.Bundle;
			}
		}

		public int Price => _item.price;

		public int BundleId => _item.BundleId;

		public bool IsOwned
		{
			get
			{
				if (BuyFlow != EmoteBuyFlow.Bundle)
				{
					return AnimationShop.IsAnimationOwned(_item.Y);
				}
				return AnimationOwnership.Owns(_item.Y);
			}
		}

		public bool CanAssignToWheel
		{
			get
			{
				if (BuyFlow != EmoteBuyFlow.Coins)
				{
					return IsOwned;
				}
				return true;
			}
		}

		public float? PreviewAutoStopSeconds => _item.duration;

		public bool KeepsRunningOnTurnOff
		{
			get
			{
				if (_item.animationClip != null)
				{
					return _item.animationClip.isLooping;
				}
				return false;
			}
		}

		public ClipEmoteEntry(AnimationDataSO.AnimationDataItem item)
		{
			_item = item;
		}

		public void Preview(in EmotePreviewContext ctx)
		{
			if (ctx.IsMainMenu)
			{
				if ((bool)ctx.Animations)
				{
					ctx.Animations.PlaySpecialAnimation(_item.Y);
				}
			}
			else if (!ctx.IsMultiFree && !ctx.DriverRagdolled && (bool)ctx.Move)
			{
				if ((bool)ctx.Wic && ctx.Wic.IsPosed)
				{
					ctx.Wic.StopPreview();
				}
				ctx.Move.Input.Emotion = _item.Y;
			}
		}

		public void StopPreview(in EmotePreviewContext ctx)
		{
		}

		public EmoteWheelTapResult ActivateFromWheel(in EmoteWheelContext ctx)
		{
			if (BuyFlow == EmoteBuyFlow.Bundle && !IsOwned)
			{
				return EmoteWheelTapResult.Activated;
			}
			NewMoveController component = ctx.Driver.GetComponent<NewMoveController>();
			WorldInteractionController component2 = ctx.Driver.GetComponent<WorldInteractionController>();
			if (component2 != null)
			{
				if (component2.IsPosed)
				{
					ctx.SetOldSelected(0);
				}
				component2.ExitInteraction();
			}
			if (ctx.GetOldSelected() == _item.Y && component.IsEmotePlaying)
			{
				if (_item.animationClip != null && _item.animationClip.isLooping)
				{
					ctx.SetOldSelected(0);
					ctx.CancelReset();
					if ((bool)component.PersonAnimationsController)
					{
						component.PersonAnimationsController.StopSpecialAnimation();
					}
					else
					{
						component.Input.Emotion = 0;
					}
					return EmoteWheelTapResult.Stopped;
				}
				return EmoteWheelTapResult.Swallowed;
			}
			ctx.SetOldSelected(_item.Y);
			component.Input.Emotion = _item.Y;
			ctx.CancelReset();
			if (_item.animationClip != null && !_item.animationClip.isLooping)
			{
				ctx.ScheduleReset(_item.duration);
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
