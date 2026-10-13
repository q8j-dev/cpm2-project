using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Car
{
	public class BodyKitsTrim : IDisposable
	{
		private BodyKits _bodyKits;

		private List<int> _trims = new List<int>();

		private List<bodyKitValues> _bodyKitValues = new List<bodyKitValues>();

		public BodyKitsTrim(BodyKits bodyKits)
		{
			_bodyKits = bodyKits;
		}

		public void Dispose()
		{
		}

		public List<GameObject> GetAllTrims()
		{
			List<GameObject> list = new List<GameObject>();
			foreach (bodyKitValues bodyKitValue in _bodyKitValues)
			{
				if ((bool)bodyKitValue)
				{
					list.Add(bodyKitValue.gameObject);
				}
			}
			return list;
		}

		public void SetTrims(int[] trims)
		{
			for (int i = 0; i < trims.Length; i++)
			{
				if (!_trims.Contains(trims[i]))
				{
					LoadTrim(trims[i]);
				}
			}
			for (int j = 0; j < _trims.Count; j++)
			{
				if (!trims.Contains(_trims[j]))
				{
					RemoveTrim(_trims[j]);
				}
			}
			_trims.Clear();
			for (int k = 0; k < trims.Length; k++)
			{
				_trims.Add(trims[k]);
			}
		}

		private void LoadTrim(int index)
		{
			LoadTrimAsync(index, CancellationToken.None).Forget();
		}

		private void RemoveTrim(int index)
		{
			for (int i = 0; i < _bodyKitValues.Count; i++)
			{
				if (_bodyKitValues[i].Index == index)
				{
					_bodyKitValues[i].StartFade();
					_bodyKitValues.RemoveAt(i);
					break;
				}
			}
		}

		private async UniTaskVoid LoadTrimAsync(int index, CancellationToken cancellationToken)
		{
			if (index < 0)
			{
				return;
			}
			Transform transformTrim = _bodyKits.GetTransformByIDName(_bodyKits.trims, index);
			if ((bool)transformTrim)
			{
				GameObject gameObject = await _bodyKits.LoadAndInstantiateAsset($"bodyKits/trims/trim{index}", cancellationToken);
				if (!cancellationToken.IsCancellationRequested && (bool)gameObject)
				{
					gameObject.transform.parent = _bodyKits.parent;
					bodyKitValues item = _bodyKits.InitNewBodyKitValues(gameObject, BodyKitType.Trim, index);
					gameObject.transform.SetLocalData(transformTrim);
					_bodyKits.SetForVinylPlace(gameObject);
					_bodyKitValues.Add(item);
				}
			}
		}
	}
}
