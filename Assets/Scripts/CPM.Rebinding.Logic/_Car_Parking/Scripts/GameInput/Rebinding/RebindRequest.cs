namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct RebindRequest
	{
		public readonly BindingSlot Target;

		public readonly string CapturedPath;

		public readonly string NewInteractions;

		public readonly string NewProcessors;

		public readonly string CapturedModifierPath;

		public RebindRequest(BindingSlot target, string capturedPath, string newInteractions = null, string newProcessors = null, string capturedModifierPath = null)
		{
			Target = target;
			CapturedPath = capturedPath;
			NewInteractions = newInteractions;
			NewProcessors = newProcessors;
			CapturedModifierPath = capturedModifierPath;
		}

		public RebindRequest WithInteractions(string interactions)
		{
			return new RebindRequest(Target, CapturedPath, interactions, NewProcessors, CapturedModifierPath);
		}
	}
}
