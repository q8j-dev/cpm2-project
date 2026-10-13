using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class RebindPlan
	{
		private static readonly IReadOnlyList<BindingSlot> NoSlots = Array.Empty<BindingSlot>();

		private static readonly IReadOnlyList<BindingChange> NoChanges = Array.Empty<BindingChange>();

		public RebindVerdict Verdict { get; }

		public BindingSlot Target { get; }

		public string OldPath { get; }

		public string NewPath { get; }

		public string NewModifierPath { get; }

		public IReadOnlyList<BindingSlot> Conflicts { get; }

		public BindingSlot LockedBy { get; }

		public IReadOnlyList<BindingChange> Changes { get; }

		public bool NeedsConfirmation { get; }

		public InteractionClass TargetClass { get; }

		public bool IsAccepted
		{
			get
			{
				RebindVerdict verdict = Verdict;
				return verdict == RebindVerdict.NoChange || verdict == RebindVerdict.Free || verdict == RebindVerdict.Unbind;
			}
		}

		public bool LeavesUnbound(BindingSlot slot)
		{
			foreach (BindingChange change in Changes)
			{
				if (change.Action == slot.Action && change.Index == slot.Index)
				{
					return change.Changes(ChangeFields.Path) && string.IsNullOrEmpty(change.NewPath);
				}
			}
			return false;
		}

		private RebindPlan(RebindVerdict verdict, BindingSlot target, string oldPath, string newPath, IReadOnlyList<BindingSlot> conflicts, BindingSlot lockedBy, IReadOnlyList<BindingChange> changes, bool needsConfirmation, InteractionClass targetClass, string newModifierPath)
		{
			Verdict = verdict;
			Target = target;
			OldPath = oldPath;
			NewPath = newPath;
			NewModifierPath = newModifierPath;
			Conflicts = conflicts ?? NoSlots;
			LockedBy = lockedBy;
			Changes = changes ?? NoChanges;
			NeedsConfirmation = needsConfirmation;
			TargetClass = targetClass;
		}

		internal static RebindPlan Rejected(RebindVerdict verdict, BindingSlot target, string oldPath, string newPath, InteractionClass targetClass, BindingSlot lockedBy = default(BindingSlot), string newModifierPath = null)
		{
			return new RebindPlan(verdict, target, oldPath, newPath, null, lockedBy, null, needsConfirmation: false, targetClass, newModifierPath);
		}

		internal static RebindPlan Accepted(BindingSlot target, string oldPath, string newPath, IReadOnlyList<BindingSlot> conflicts, IReadOnlyList<BindingChange> changes, bool needsConfirmation, InteractionClass targetClass, string newModifierPath = null)
		{
			return new RebindPlan((conflicts.Count == 0) ? RebindVerdict.Free : RebindVerdict.Unbind, target, oldPath, newPath, conflicts, default, changes, needsConfirmation, targetClass, newModifierPath);
		}
	}
}
