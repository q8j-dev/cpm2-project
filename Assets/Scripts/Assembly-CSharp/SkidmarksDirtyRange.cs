public struct SkidmarksDirtyRange
{
	private int _min;

	private int _max;

	private bool _any;

	private bool _wrapped;

	public void Touch(int index)
	{
		if (!_any)
		{
			_min = index;
			_max = index;
			_any = true;
			return;
		}
		if (index < _max)
		{
			_wrapped = true;
		}
		if (index < _min)
		{
			_min = index;
		}
		if (index > _max)
		{
			_max = index;
		}
	}

	public bool TryGetRange(int capacity, out int start, out int count)
	{
		if (!_any)
		{
			start = 0;
			count = 0;
			return false;
		}
		if (_wrapped)
		{
			start = 0;
			count = capacity;
			return true;
		}
		start = _min;
		count = _max - _min + 1;
		return true;
	}

	public void Clear()
	{
		_any = false;
		_wrapped = false;
		_min = 0;
		_max = 0;
	}
}
