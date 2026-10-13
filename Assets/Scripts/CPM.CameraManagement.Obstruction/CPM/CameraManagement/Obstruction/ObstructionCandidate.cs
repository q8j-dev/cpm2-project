namespace CPM.CameraManagement.Obstruction
{
	public readonly struct ObstructionCandidate
	{
		public readonly float Distance;

		public readonly bool IsIgnored;

		public ObstructionCandidate(float distance, bool isIgnored)
		{
			Distance = distance;
			IsIgnored = isIgnored;
		}
	}
}
