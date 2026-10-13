using System;

namespace CPM.Balance
{
	public interface IBalanceDocumentStore
	{
		BalanceDocument Current { get; }

		event Action Changed;
	}
}
