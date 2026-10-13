using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;

namespace CarInteractionAnimationsScripts
{
	public interface IAction : IDisposable
	{
		UniTask Run(CarAnimationsController car, CancellationToken cancellationToken);
	}
}
