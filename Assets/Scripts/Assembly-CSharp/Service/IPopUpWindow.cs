using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Service
{
	public interface IPopUpWindow
	{
		UniTaskVoid Create();

		void Destroy(GameObject gameObject);

		void Init();

		void Show();
	}
}
