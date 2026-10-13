using System;
using JetBrains.Annotations;

namespace DebugPagesUi
{
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class DebugStringHandlerAttribute : Attribute
	{
		public string Path { get; }

		public DebugStringHandlerAttribute(string path)
		{
			Path = path;
		}
	}
}
