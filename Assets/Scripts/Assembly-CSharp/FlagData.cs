using System;

[Serializable]
public class FlagData
{
	public int id;

	public int count;

	public FlagData(int id, int count = 0)
	{
		this.id = id;
		this.count = count;
	}
}
