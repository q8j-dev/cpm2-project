using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.MenuUI
{
	[RequireComponent(typeof(ContentSizeFitter))]
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LayoutTMPUpdater : MonoBehaviour
	{
		private TextMeshProUGUI _text;

		private ContentSizeFitter _sizeFitter;

		private void Awake()
		{
			_text = GetComponent<TextMeshProUGUI>();
			_sizeFitter = GetComponent<ContentSizeFitter>();
			_text.OnPreRenderText += OnTextUpdated;
		}

		private void OnTextUpdated(TMP_TextInfo textInfo)
		{
			OnTextUpdatedAsync(base.destroyCancellationToken).Forget();
		}

		private async UniTask OnTextUpdatedAsync(CancellationToken cancellationToken)
		{
			if ((bool)_sizeFitter)
			{
				_sizeFitter.enabled = false;
			}
			await UniTask.NextFrame(cancellationToken);
			if (!cancellationToken.IsCancellationRequested && (bool)_sizeFitter)
			{
				_sizeFitter.enabled = true;
			}
		}

		private void OnDestroy()
		{
			_text.OnPreRenderText -= OnTextUpdated;
		}
	}
}
