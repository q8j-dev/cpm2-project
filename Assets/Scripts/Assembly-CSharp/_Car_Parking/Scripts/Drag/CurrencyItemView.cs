using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Drag
{
	public class CurrencyItemView : MonoBehaviour
	{
		public CurrencyItemType currencyItemType;

		[SerializeField]
		private TextMeshProUGUI BalanceField;

		private bool _isDestroyed;

		private bool _hasValue;

		private Tween _tween;

		private float _value;

		public float value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				UpdateComponents();
			}
		}

		private void Awake()
		{
			if (!BalanceField)
			{
				BalanceField = GetComponentInChildren<TextMeshProUGUI>();
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
		}

		private void OnDestroy()
		{
			_isDestroyed = true;
			_tween?.Kill();
		}

		public void UpdateComponents()
		{
			if ((bool)BalanceField)
			{
				BalanceField.text = $"{(int)value:N0}";
			}
		}

		public void SetBalance(float balance, bool ignoreAnimation = false)
		{
			if (!ignoreAnimation && _hasValue && !Mathf.Approximately(_value, balance))
			{
				TweenValueChange(balance).Forget();
			}
			else
			{
				SetBalanceNow(balance);
			}
		}

		public void SetBalanceNow(float balance)
		{
			_tween?.Kill();
			_hasValue = true;
			value = balance;
		}

		private async UniTask TweenValueChange(float changedValue)
		{
			if (!_isDestroyed)
			{
				_tween?.Kill();
				_tween = DOTween.To(() => value, (float x) =>
				{
					value = x;
				}, changedValue, 2f).SetUpdate(isIndependentUpdate: true);
				await UniTask.Yield();
			}
		}
	}
}
