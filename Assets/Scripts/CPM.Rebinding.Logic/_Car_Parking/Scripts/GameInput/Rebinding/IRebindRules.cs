using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public interface IRebindRules
	{
		bool IsReservedPath(string path);

		bool HasGlyph(string path);

		ConflictPolicy PolicyFor(InputAction action);

		bool IsLockedOn(InputAction action, RebindDeviceKind device);

		IReadOnlyList<InputAction> FollowersOf(InputAction leader);

		IReadOnlyList<InputAction> LeadersOf(InputAction follower);

		bool AreLinked(InputAction a, InputAction b);

		bool AllowsPressHold(InputAction action);

		bool CanFireTogether(BindingSlot a, BindingSlot b);
	}
}
