using System;

namespace CPMEngine
{
	[Serializable]
	public class PhysicMaterial
	{
		public float RollingFriction = 0.012f;

		public float Friction = 1f;

		public float BumpMax;

		public float BumpMin;

		public float BumpScale = 30f;

		public float Smoothness;

		public bool SkidSmoke;

		public bool SandSmoke;

		private Surface _surfaceType;

		public Surface SurfaceType
		{
			get
			{
				return _surfaceType;
			}
			set
			{
				if (_surfaceType != value)
				{
					_surfaceType = value;
					SetMaterialProperties(value);
				}
			}
		}

		public void SetMaterialProperties(Surface surface)
		{
			switch (surface)
			{
			case Surface.Dryasphalt:
			case Surface.Indoor:
			case Surface.Wood:
				SetDryAsphalt();
				break;
			case Surface.Wetasphalt:
				SetWetAsphalt();
				break;
			case Surface.Grass:
				SetGrass();
				break;
			case Surface.Sand:
				SetSand();
				break;
			case Surface.Kerb:
				SetKerb();
				break;
			case Surface.Offroad:
				SetOffRoad();
				break;
			case Surface.Damp:
				SetDamp();
				break;
			case Surface.DirtRoad:
				SetDirt();
				break;
			case Surface.Gravel:
				SetGravel();
				break;
			case Surface.OutOfRoad:
				SetOutOfRoad();
				break;
			case Surface.Snow:
				SetSnow();
				break;
			case Surface.Winterroad:
				SetWinterRoad();
				break;
			case Surface.Ice:
				SetIce();
				break;
			case Surface.Glue:
				SetGlue();
				break;
			case Surface.Concrete:
				SetConcrete();
				break;
			default:
				SetNone();
				break;
			}
		}

		private void SetNone()
		{
			RollingFriction = 0.01f;
			BumpMin = 0f;
			BumpMax = 0f;
			BumpScale = 30f;
			SkidSmoke = false;
			SandSmoke = false;
			Smoothness = 0f;
		}

		private void SetDryAsphalt()
		{
			RollingFriction = 0.01f;
			BumpMin = 0f;
			BumpMax = 0f;
			BumpScale = 30f;
			SkidSmoke = true;
			SandSmoke = false;
			Smoothness = 0f;
		}

		private void SetWetAsphalt()
		{
			RollingFriction = 0.015f;
			BumpMin = 0f;
			BumpMax = 0f;
			BumpScale = 30f;
			SkidSmoke = true;
			SandSmoke = false;
			Smoothness = 0f;
		}

		private void SetGrass()
		{
			RollingFriction = 0.075f;
			BumpMin = -0.01f;
			BumpMax = 0.01f;
			BumpScale = 15f;
			SkidSmoke = false;
			SandSmoke = true;
			Smoothness = 0.5f;
		}

		private void SetSand()
		{
			RollingFriction = 0.15f;
			BumpMin = -0.01f;
			BumpMax = 0.01f;
			BumpScale = 10f;
			SkidSmoke = false;
			SandSmoke = true;
			Smoothness = 1f;
		}

		private void SetKerb()
		{
			RollingFriction = 0.015f;
			BumpMin = 0f;
			BumpMax = 0.002f;
			BumpScale = 20f;
			SkidSmoke = true;
			SandSmoke = false;
			Smoothness = 0f;
		}

		private void SetOutOfRoad()
		{
			RollingFriction = 0.025f;
			BumpMin = -0.01f;
			BumpMax = 0.01f;
			BumpScale = 16f;
			SkidSmoke = false;
			SandSmoke = true;
			Smoothness = 1f;
		}

		private void SetDamp()
		{
			RollingFriction = 0.06f;
			BumpMin = -0.01f;
			BumpMax = 0.01f;
			BumpScale = 8f;
			SkidSmoke = false;
			SandSmoke = true;
			Smoothness = 1f;
		}

		private void SetDirt()
		{
			RollingFriction = 0.04f;
			BumpMin = -0.01f;
			BumpMax = 0.01f;
			BumpScale = 8f;
			SkidSmoke = false;
			SandSmoke = true;
			Smoothness = 1f;
		}

		private void SetGravel()
		{
			RollingFriction = 0.035f;
			BumpMin = -0.005f;
			BumpMax = 0.008f;
			BumpScale = 12f;
			SkidSmoke = false;
			SandSmoke = true;
			Smoothness = 1f;
		}

		private void SetOffRoad()
		{
			RollingFriction = 0.07f;
			BumpMin = -0.01f;
			BumpMax = 0.01f;
			BumpScale = 8f;
			SkidSmoke = false;
			SandSmoke = true;
			Smoothness = 1f;
		}

		private void SetSnow()
		{
			RollingFriction = 0.04f;
			BumpMin = -0.005f;
			BumpMax = 0.005f;
			BumpScale = 8f;
			SkidSmoke = false;
			SandSmoke = true;
			Smoothness = 1f;
		}

		private void SetWinterRoad()
		{
			RollingFriction = 0.025f;
			BumpMin = -0.001f;
			BumpMax = 0.001f;
			BumpScale = 25f;
			SkidSmoke = false;
			SandSmoke = false;
			Smoothness = 0.5f;
		}

		private void SetIce()
		{
			RollingFriction = 0.015f;
			BumpMin = 0f;
			BumpMax = 0f;
			BumpScale = 30f;
			SkidSmoke = false;
			SandSmoke = false;
			Smoothness = 1f;
		}

		private void SetGlue()
		{
			RollingFriction = 0.04f;
			BumpMin = 0f;
			BumpMax = 0f;
			BumpScale = 30f;
			SkidSmoke = true;
			SandSmoke = false;
			Smoothness = 0f;
		}

		private void SetConcrete()
		{
			RollingFriction = 0.018f;
			BumpMin = 0f;
			BumpMax = 0f;
			BumpScale = 30f;
			SkidSmoke = true;
			SandSmoke = false;
			Smoothness = 0f;
		}
	}
}
