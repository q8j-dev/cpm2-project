using System.Collections.Generic;
using UnityEngine;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class ExhaustComposition
	{
		public List<GameObject> exhaust1 = new List<GameObject>();

		private List<ParticleSystem> _exhaustsStrongShotParticles = new List<ParticleSystem>();

		private List<ParticleSystem> _exhaustsWeakShotParticles = new List<ParticleSystem>();

		private List<ParticleSystem> _burbleExhaustParticle = new List<ParticleSystem>();

		private readonly VehicleData _vehicle;

		public ExhaustComposition(VehicleData vehicle)
		{
			_vehicle = vehicle;
		}

		public void Install()
		{
			CreateStrongShotParticles();
			CreateWeakShotParticles();
			CreateBurbleParticles();
		}

		public void OnQualityChanged(int obj)
		{
			if (_vehicle.IsRacingCar)
			{
				RespawnParticles();
			}
		}

		public void RespawnParticles()
		{
			CreateStrongShotParticles();
			CreateWeakShotParticles();
			CreateBurbleParticles();
		}

		public void CreateBurbleParticles()
		{
			ClearBurbleParticles();
			if (IsDisableRacingCarParticles() || !V.BurbleExhaustParticle)
			{
				return;
			}
			for (int i = 0; i < _vehicle.CarChildsReferences.Exhaust.Length; i++)
			{
				Transform transform = _vehicle.CarChildsReferences.Exhaust[i];
				if ((bool)transform)
				{
					GameObject gameObject = Object.Instantiate(V.BurbleExhaustParticle, transform);
					gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, -180f, 0f));
					gameObject.transform.localScale = Vector3.one;
					_burbleExhaustParticle.Add(gameObject.GetComponent<ParticleSystem>());
					ParticleSystem.MainModule main = _burbleExhaustParticle[i].main;
					main.startDelay = Random.Range(0f, 0.5f);
				}
			}
		}

		public void CreateStrongShotParticles()
		{
			ClearStrongShotParticles();
			if (IsDisableRacingCarParticles() || !V.ShootStrongExhaustParticle)
			{
				return;
			}
			for (int i = 0; i < _vehicle.CarChildsReferences.Exhaust.Length; i++)
			{
				Transform transform = _vehicle.CarChildsReferences.Exhaust[i];
				if ((bool)transform)
				{
					GameObject gameObject = Object.Instantiate(V.ShootStrongExhaustParticle, transform);
					gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, -180f, 0f));
					gameObject.transform.localScale = Vector3.one;
					_exhaustsStrongShotParticles.Add(gameObject.GetComponent<ParticleSystem>());
					ParticleSystem.MainModule main = _exhaustsStrongShotParticles[i].main;
					main.startDelay = Random.Range(0f, 0.5f);
				}
			}
		}

		public void CreateWeakShotParticles()
		{
			ClearWeakShotParticles();
			if (IsDisableRacingCarParticles() || !V.ShootWeakExhaustParticle)
			{
				return;
			}
			for (int i = 0; i < _vehicle.CarChildsReferences.Exhaust.Length; i++)
			{
				Transform transform = _vehicle.CarChildsReferences.Exhaust[i];
				if ((bool)transform)
				{
					GameObject gameObject = Object.Instantiate(V.ShootWeakExhaustParticle, transform);
					gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, -180f, 0f));
					gameObject.transform.localScale = Vector3.one;
					_exhaustsWeakShotParticles.Add(gameObject.GetComponent<ParticleSystem>());
					ParticleSystem.MainModule main = _exhaustsWeakShotParticles[i].main;
					main.startDelay = Random.Range(0f, 0.5f);
				}
			}
		}

		public void PlayStrongShootParticle()
		{
			if (_vehicle.IsRacingCar && IsLodActive())
			{
				return;
			}
			List<ParticleSystem> exhaustsStrongShotParticles = _exhaustsStrongShotParticles;
			if (exhaustsStrongShotParticles == null || exhaustsStrongShotParticles.Count <= 0)
			{
				return;
			}
			foreach (ParticleSystem exhaustsStrongShotParticle in _exhaustsStrongShotParticles)
			{
				if ((bool)exhaustsStrongShotParticle)
				{
					exhaustsStrongShotParticle.Play();
					exhaustsStrongShotParticle.Stop(withChildren: false);
				}
			}
		}

		public void PlayWeakShootParticle()
		{
			if (_vehicle.IsRacingCar && IsLodActive())
			{
				return;
			}
			List<ParticleSystem> exhaustsWeakShotParticles = _exhaustsWeakShotParticles;
			if (exhaustsWeakShotParticles == null || exhaustsWeakShotParticles.Count <= 0)
			{
				return;
			}
			foreach (ParticleSystem exhaustsWeakShotParticle in _exhaustsWeakShotParticles)
			{
				if ((bool)exhaustsWeakShotParticle)
				{
					exhaustsWeakShotParticle.Play();
				}
			}
		}

		public void PlayBurbleParticle()
		{
			if (_vehicle.IsRacingCar && IsLodActive())
			{
				return;
			}
			List<ParticleSystem> burbleExhaustParticle = _burbleExhaustParticle;
			if (burbleExhaustParticle == null || burbleExhaustParticle.Count <= 0)
			{
				return;
			}
			foreach (ParticleSystem item in _burbleExhaustParticle)
			{
				if ((bool)item)
				{
					item.Play();
				}
			}
		}

		public void UpdateDamagedExhaust(bool play)
		{
			if (!_vehicle.CarHealthComposition.Damaged)
			{
				return;
			}
			foreach (GameObject item in exhaust1)
			{
				if ((bool)item && item.TryGetComponent<ParticleSystem>(out var component))
				{
					if (play)
					{
						component.Play();
					}
					else
					{
						component.Stop();
					}
				}
			}
		}

		public void DamageEngine(bool state)
		{
			if (state)
			{
				if (exhaust1.Count != 0 || !V.DamagedExhaustParticle || _vehicle.CarChildsReferences.Exhaust.Length == 0)
				{
					return;
				}
				int num = 0;
				Transform[] exhaust = _vehicle.CarChildsReferences.Exhaust;
				foreach (Transform transform in exhaust)
				{
					if ((bool)transform)
					{
						GameObject gameObject = Object.Instantiate(V.DamagedExhaustParticle, transform, worldPositionStays: true);
						gameObject.transform.position = transform.position;
						gameObject.transform.localScale = Vector3.one;
						Vector3 position = gameObject.transform.position;
						if (transform.transform.localScale.y == -1f)
						{
							gameObject.transform.localEulerAngles = transform.localEulerAngles;
						}
						else
						{
							gameObject.transform.LookAt(position - _vehicle.CarChildsReferences.Exhaust[num].transform.forward * 2f);
						}
						ParticleSystem.MainModule main = gameObject.GetComponent<ParticleSystem>().main;
						main.startDelay = Random.Range(0f, 0.5f);
						exhaust1.Add(gameObject);
						num++;
					}
				}
				return;
			}
			foreach (GameObject item in exhaust1)
			{
				if ((bool)item)
				{
					Object.Destroy(item);
				}
			}
			exhaust1 = new List<GameObject>();
		}

		private void ClearStrongShotParticles()
		{
			if (_exhaustsStrongShotParticles.Count == 0)
			{
				return;
			}
			foreach (ParticleSystem exhaustsStrongShotParticle in _exhaustsStrongShotParticles)
			{
				if ((bool)exhaustsStrongShotParticle)
				{
					Object.Destroy(exhaustsStrongShotParticle.gameObject);
				}
			}
			_exhaustsStrongShotParticles = new List<ParticleSystem>();
		}

		private void ClearWeakShotParticles()
		{
			if (_exhaustsWeakShotParticles.Count == 0)
			{
				return;
			}
			foreach (ParticleSystem exhaustsWeakShotParticle in _exhaustsWeakShotParticles)
			{
				if ((bool)exhaustsWeakShotParticle)
				{
					Object.Destroy(exhaustsWeakShotParticle.gameObject);
				}
			}
			_exhaustsWeakShotParticles = new List<ParticleSystem>();
		}

		private void ClearBurbleParticles()
		{
			if (_burbleExhaustParticle.Count == 0)
			{
				return;
			}
			foreach (ParticleSystem item in _burbleExhaustParticle)
			{
				if ((bool)item)
				{
					Object.Destroy(item.gameObject);
				}
			}
			_burbleExhaustParticle = new List<ParticleSystem>();
		}

		private bool IsDisableRacingCarParticles()
		{
			if (!CPMGraphicsSettingsManager.Instance)
			{
				return false;
			}
			if (_vehicle.IsRacingCar)
			{
				return CPMGraphicsSettingsManager.Instance.AdditionalEffects == CPMGraphicsSettings.AdditionalEffects.Low;
			}
			return false;
		}

		private bool IsLodActive()
		{
			if ((bool)_vehicle.BodyLoadingComposition.LODModel)
			{
				return _vehicle.BodyLoadingComposition.LODModel.activeInHierarchy;
			}
			return false;
		}
	}
}
