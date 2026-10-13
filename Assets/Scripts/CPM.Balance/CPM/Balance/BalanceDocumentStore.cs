using System;

namespace CPM.Balance
{
	public sealed class BalanceDocumentStore : IBalanceDocumentStore
	{
		private BalanceDocument _current = new BalanceDocument();

		public BalanceDocument Current => _current;

		public event Action Changed;

		public void Set(BalanceDocument document)
		{
			_current = document ?? new BalanceDocument();
			Changed?.Invoke();
		}
	}
}
