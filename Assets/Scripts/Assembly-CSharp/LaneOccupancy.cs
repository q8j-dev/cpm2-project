public struct LaneOccupancy
{
	public bool L_n2_FreeAhead;

	public bool L_n1_FreeAhead;

	public bool L_0_FreeAhead;

	public bool L_p1_FreeAhead;

	public bool L_p2_FreeAhead;

	public float L_n2_NearestAheadDist;

	public float L_n1_NearestAheadDist;

	public float L_0_NearestAheadDist;

	public float L_p1_NearestAheadDist;

	public float L_p2_NearestAheadDist;

	public bool L_n2_SafeBehind;

	public bool L_n1_SafeBehind;

	public bool L_0_SafeBehind;

	public bool L_p1_SafeBehind;

	public bool L_p2_SafeBehind;

	public bool IsLaneFreeAhead(int lane)
	{
		return lane switch
		{
			-2 => L_n2_FreeAhead, 
			-1 => L_n1_FreeAhead, 
			0 => L_0_FreeAhead, 
			1 => L_p1_FreeAhead, 
			2 => L_p2_FreeAhead, 
			_ => false, 
		};
	}

	public bool IsLaneSafeBehind(int lane)
	{
		return lane switch
		{
			-2 => L_n2_SafeBehind, 
			-1 => L_n1_SafeBehind, 
			0 => L_0_SafeBehind, 
			1 => L_p1_SafeBehind, 
			2 => L_p2_SafeBehind, 
			_ => false, 
		};
	}

	public float NearestAheadDist(int lane)
	{
		return lane switch
		{
			-2 => L_n2_NearestAheadDist, 
			-1 => L_n1_NearestAheadDist, 
			0 => L_0_NearestAheadDist, 
			1 => L_p1_NearestAheadDist, 
			2 => L_p2_NearestAheadDist, 
			_ => float.MaxValue, 
		};
	}
}
