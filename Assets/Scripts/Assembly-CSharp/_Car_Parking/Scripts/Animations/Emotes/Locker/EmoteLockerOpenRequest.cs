namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	public sealed class EmoteLockerOpenRequest
	{
		private EmoteLockerContext _context;

		public void Set(EmoteLockerContext context)
		{
			_context = context;
		}

		public bool TryConsume(out EmoteLockerContext context)
		{
			context = _context;
			_context = null;
			return context != null;
		}
	}
}
