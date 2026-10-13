using System;
using JetBrains.Annotations;

namespace DebugPagesUi
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[MeansImplicitUse]
	public class DebugIntHandlerAttribute : Attribute
	{
		public string Path { get; }

		public DebugIntHandlerAttribute(string path)
		{
			Path = path;
		}
	}
}
