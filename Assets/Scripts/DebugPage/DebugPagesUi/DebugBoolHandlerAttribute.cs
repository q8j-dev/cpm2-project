using System;
using JetBrains.Annotations;

namespace DebugPagesUi
{
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class DebugBoolHandlerAttribute : Attribute
	{
		public string Path { get; }

		public bool DefaultValue { get; }

		public DebugBoolHandlerAttribute(string path, bool defaultValue = false)
		{
			Path = path;
			DefaultValue = defaultValue;
		}
	}
}
