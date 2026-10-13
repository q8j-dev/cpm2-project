using System.Collections.Generic;
using VContainer;

namespace CPM.Common
{
	[Preserve]
	internal class ChatFilterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ChatFilter chatFilter = new ChatFilter();
			Inject(chatFilter, resolver, parameters);
			return chatFilter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
