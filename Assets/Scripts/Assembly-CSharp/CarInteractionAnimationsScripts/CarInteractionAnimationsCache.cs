using System.Collections.Generic;
using Addler.Runtime.Core.LifetimeBinding;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CarInteractionAnimationsScripts
{
	public class CarInteractionAnimationsCache
	{
		private readonly Dictionary<int, Dictionary<string, AnimationClip>> _cachesCars = new Dictionary<int, Dictionary<string, AnimationClip>>();

		private readonly Dictionary<int, int> _carsInGame = new Dictionary<int, int>();

		public async UniTask<AnimationClip> GetAnimation(string path, GameObject car)
		{
			VehicleData carData = car.GetComponent<VehicleData>();
			if (!_cachesCars.ContainsKey(carData.CarId))
			{
				AnimationClip animationClip = await Addressables.LoadAssetAsync<AnimationClip>(path).BindTo(car);
				Dictionary<string, AnimationClip> dictionary = new Dictionary<string, AnimationClip>();
				dictionary.TryAdd(path, animationClip);
				_cachesCars.TryAdd(carData.CarId, dictionary);
				return animationClip;
			}
			if (!_cachesCars[carData.CarId].ContainsKey(path))
			{
				AnimationClip animationClip2 = await Addressables.LoadAssetAsync<AnimationClip>(path).BindTo(car);
				_cachesCars[carData.CarId].TryAdd(path, animationClip2);
				return animationClip2;
			}
			AnimationClip animationClip3 = _cachesCars[carData.CarId][path];
			if ((bool)animationClip3)
			{
				return animationClip3;
			}
			AnimationClip animationClip4 = await Addressables.LoadAssetAsync<AnimationClip>(path).BindTo(car);
			_cachesCars[carData.CarId][path] = animationClip4;
			return animationClip4;
		}

		public void AddCar(int carId)
		{
			if (_carsInGame.ContainsKey(carId))
			{
				_carsInGame[carId]++;
			}
			else
			{
				_carsInGame.Add(carId, 1);
			}
		}

		public void RemoveCar(int carId)
		{
			if (!_carsInGame.ContainsKey(carId))
			{
				return;
			}
			_carsInGame[carId]--;
			if (_carsInGame[carId] == 0)
			{
				_carsInGame.Remove(carId);
				if (_cachesCars.ContainsKey(carId))
				{
					_cachesCars[carId].Clear();
					_cachesCars.Remove(carId);
				}
			}
		}
	}
}
