using System;
using Cysharp.Threading.Tasks;
using Firebase.Functions;

namespace CPM.Database
{
	public interface IDatabaseInventorySaveLoadService<T>
	{
		event Action StartSaving;

		event Action EndSaving;

		UniTask<FunctionCallResult<FunctionResult>> Send(T data, bool fullData = false);

		void Init();
	}
}
