using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.StepProgress
{
	public abstract class ProgressCellView : MonoBehaviour
	{
		public abstract void SetFilled(bool filled);

		public abstract UniTask PlayFillAsync(CancellationToken token);

		public virtual void SetHidden(bool hidden)
		{
			base.gameObject.SetActive(!hidden);
		}
	}
}
