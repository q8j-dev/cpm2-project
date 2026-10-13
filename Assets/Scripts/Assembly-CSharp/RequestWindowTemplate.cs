using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestWindowTemplate : MonoBehaviour
{
	[SerializeField]
	private Button _buttonOk;

	[SerializeField]
	private Button _buttonIgnore;

	[SerializeField]
	private Button _buttonDecline;

	[SerializeField]
	private TextMeshProUGUI _textMessage;

	[SerializeField]
	private Image _imageCar;

	public void Init(string text, int carId, Action<RequestAnswer> callback)
	{
		_textMessage.text = text;
		_imageCar.sprite = Resources.Load<Sprite>("UI/CarImages/b" + carId);
		_buttonOk.onClick.AddListener(() =>
		{
			callback(RequestAnswer.Confirm);
			Close();
		});
		_buttonIgnore.onClick.AddListener(() =>
		{
			callback(RequestAnswer.Ignore);
			Close();
		});
		_buttonDecline.onClick.AddListener(() =>
		{
			callback(RequestAnswer.Decline);
			Close();
		});
	}

	public void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
