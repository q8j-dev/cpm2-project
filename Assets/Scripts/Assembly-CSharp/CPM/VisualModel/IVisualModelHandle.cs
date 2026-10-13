using System;
using UnityEngine;

namespace CPM.VisualModel
{
	public interface IVisualModelHandle : IDisposable
	{
		GameObject Root { get; }

		Bounds Bounds { get; }

		float FrontZSign { get; }
	}
}
