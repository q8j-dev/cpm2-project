using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.PopUpMessage
{
	public class MessageWindow : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text textField;

		[SerializeField]
		private GameObject closeObj;

		[SerializeField]
		private GameObject windowObj;

		[SerializeField]
		private Image header;

		[SerializeField]
		[Header("Colors")]
		private Color infoColor;

		[SerializeField]
		private Color warningColor;

		[SerializeField]
		private Color errorColor;

		private readonly float _duration = 0.3f;

		private Transform _windowTr;

		private IDisposable _closeTimer;

		private static MessageWindow _instance;

		private static MessageWindow Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = UnityEngine.Object.Instantiate(Resources.Load<MessageWindow>("PopUpMessage"));
					UnityEngine.Object.DontDestroyOnLoad(_instance);
				}
				return _instance;
			}
		}

		private void Awake()
		{
			_windowTr = windowObj.transform;
		}

		public static void ShowMessage(string content, MessageType type = MessageType.Info, float duration = 3f, bool closable = true)
		{
			Instance.ShowMessageInternal(content, type, duration, closable);
		}

		private void ShowMessageInternal(string content, MessageType type, float duration, bool closable)
		{
			textField.text = content;
			closeObj.SetActive(closable && type != MessageType.Error);
			_closeTimer?.Dispose();
			if (duration > 0f)
			{
				_closeTimer = Observable.Timer(TimeSpan.FromSeconds(duration)).Subscribe((long _) =>
				{
					Deactivate();
				}).AddTo(this);
			}
			Image image = header;
			image.color = type switch
			{
				MessageType.Info => infoColor, 
				MessageType.Warning => warningColor, 
				MessageType.Error => errorColor, 
				_ => infoColor, 
			};
			Activate();
		}

		public void CloseWindow()
		{
			_closeTimer?.Dispose();
			Deactivate();
		}

		private void Activate()
		{
			windowObj.SetActive(value: true);
			_windowTr.localScale = new Vector3(0.1f, 0.1f, 1f);
			_windowTr.DOScale(Vector3.one, _duration);
		}

		private void Deactivate()
		{
			_windowTr.DOScale(new Vector3(0.1f, 0.1f, 1f), _duration).OnComplete(() =>
			{
				windowObj.SetActive(value: false);
			});
		}
	}
}
