namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct CaptureResult
	{
		public readonly CaptureStatus Status;

		public readonly string Path;

		public readonly string ModifierPath;

		public readonly int ModifierCount;

		public readonly bool Invert;

		public readonly bool NeedsRemap;

		public static readonly CaptureResult Cancelled = new CaptureResult(CaptureStatus.Cancelled, null);

		public static readonly CaptureResult Busy = new CaptureResult(CaptureStatus.Busy, null);

		private CaptureResult(CaptureStatus status, string path, bool invert = false, bool needsRemap = false, string modifierPath = null, int modifierCount = 0)
		{
			Status = status;
			Path = path;
			Invert = invert;
			NeedsRemap = needsRemap;
			ModifierPath = modifierPath;
			ModifierCount = modifierCount;
		}

		public static CaptureResult Captured(string path, string modifierPath = null)
		{
			return new CaptureResult(CaptureStatus.Captured, path, invert: false, needsRemap: false, modifierPath);
		}

		public static CaptureResult CapturedAxis(string path, bool invert, bool needsRemap, string modifierPath = null, int modifierCount = 0)
		{
			return new CaptureResult(CaptureStatus.Captured, path, invert, needsRemap, modifierPath, modifierCount);
		}
	}
}
