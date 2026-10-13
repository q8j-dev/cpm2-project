using System;
using JetBrains.Annotations;

namespace DebugPagesUi
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[MeansImplicitUse]
	public class DebugButtonAttribute : Attribute
	{
		public string Path { get; }

		public DebugButtonAttribute(string path)
		{
			Path = path;
		}
	}
}
