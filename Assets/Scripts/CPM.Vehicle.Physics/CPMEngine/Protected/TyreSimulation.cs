using System;
using UnityEngine;

namespace CPMEngine.Protected
{
	internal class TyreSimulation
	{
		private readonly float[] _surfParam = new float[Enum.GetValues(typeof(Surface)).Length];

		private float _fricMult = 1f;

		private float _fricTempTyre = 1f;

		private float _fricWearTyre;

		private int _width = 215;

		private int _prof = 45;

		private TyreType _type;

		private float _a0;

		private float _a1;

		private float _a2;

		private float _a3;

		private float _a4;

		private float _a5;

		private float _a6;

		private float _a7;

		private float _a8;

		private float _a9;

		private float _a10;

		private float _a11;

		private float _a12;

		private float _b0;

		private float _b1;

		private float _b2;

		private float _b3;

		private float _b4;

		private float _b5;

		private float _b6;

		private float _b7;

		private float _b8;

		private float _c0;

		private float _c1;

		private float _c2;

		private float _c3;

		private float _c4;

		private float _c5;

		private float _c6;

		private float _c7;

		private float _c8;

		private float _c9;

		private float _c10;

		private float _c11;

		private float _c12;

		private float _c13;

		private float _c14;

		private float _c15;

		private float _c16;

		private float _c17;

		private float _b0Front;

		private float _b2Front;

		private float _b0Side;

		private float _b2Side;

		private float _scale;

		private float _camber;

		private float _slipAngle;

		private float _slipRatio;

		private float _fz;

		private float _fx;

		private float _fy;

		private float _mz;

		private float _maxForceX;

		private float _maxForceY;

		private float _bestSlipRatio = 10f;

		private float _bestSlipAngle = 10f;

		private float _srVol;

		private float _saVol;

		private float _sa;

		private float _sr;

		private readonly Wheel _wheel;

		public float MaxForceZ { get; private set; }

		public float GetMaxForceY => _maxForceY;

		public float GetMaxForceX => _maxForceX;

		public TyreSimulation(Wheel wheel)
		{
			_wheel = wheel;
			SetType(TyreType.Touring, _width, _prof, 0.3f);
		}

		private static float LerpParam(float width, float prof, float k165MaxProf, float k345MaxProf, float k165MinProf, float k345MinProf)
		{
			float num = (width - 155f) / 300f;
			float num2 = (prof - 20f) / 60f;
			float num3 = (1f - num) * k165MaxProf + num * k345MaxProf;
			float num4 = (1f - num) * k165MinProf + num * k345MinProf;
			return (1f - num2) * num4 + num2 * num3;
		}

		private void SetSlick(int width, int prof, float r)
		{
			_width = width;
			_prof = prof;
			_surfParam[0] = 1f;
			_surfParam[1] = 1f;
			_surfParam[2] = 0.7f;
			_surfParam[3] = 0.6f;
			_surfParam[4] = 0.7f;
			_surfParam[5] = 0.5f;
			_surfParam[6] = 0.5f;
			_surfParam[7] = 0.5f;
			_surfParam[8] = 0.9f;
			_surfParam[9] = 0.4f;
			_surfParam[10] = 0.6f;
			_surfParam[11] = 1.2f;
			_surfParam[12] = 0.25f;
			_surfParam[13] = 1f;
			_surfParam[14] = 0.45f;
			_surfParam[15] = 1.05f;
			_surfParam[16] = 0.2f;
			_surfParam[17] = 0.55f;
			if (Mathf.Abs(r) <= 0.01f)
			{
				r = 0.27f;
			}
			float num = r / 0.27f;
			_scale = num * num * ((float)width / 225f);
			_a0 = 1.65f;
			_a1 = -72f / _scale;
			_a2 = 2050f;
			_a3 = 2000f * _scale;
			_a4 = 5.5f * _scale;
			_a5 = 0.011f;
			_a6 = -0.05f / _scale;
			_a7 = 0.5f;
			_a8 = 0.015f;
			_a9 = -0.019f / _scale;
			_a10 = -0.18f / _scale;
			_a11 = -10f / _scale;
			_a12 = -0.018f;
			_b1 = -72f / _scale;
			_b0Front = 1.55f;
			_b2Front = 1800f;
			_b0Side = _b0Front;
			_b2Side = _b2Front;
			_b3 = 23.3f / (_scale * _scale);
			_b4 = LerpParam(width, prof, 300f, 500f, 500f, 700f);
			_b5 = 0.08f;
			_b6 = 0.003f / (_scale * _scale);
			_b7 = -0.007f / _scale;
			_b8 = -0.03f;
			_c0 = 2.1f;
			_c1 = -3.9f;
			_c2 = -3.9f;
			_c3 = -1.26f;
			_c4 = -8.2f;
			_c5 = 0.025f;
			_c6 = 0f;
			_c7 = 0.044f;
			_c8 = -0.58f;
			_c9 = 0.18f;
			_c10 = 0.043f;
			_c11 = 0.048f;
			_c12 = -0.0035f;
			_c13 = -0.18f;
			_c14 = 0.14f;
			_c15 = -1.029f;
			_c16 = 0.27f;
			_c17 = -1.1f;
		}

		private void SetSemiSlick(int width, int prof, float r)
		{
			_width = width;
			_prof = prof;
			_surfParam[0] = 1f;
			_surfParam[1] = 1f;
			_surfParam[2] = 0.78f;
			_surfParam[3] = 0.8f;
			_surfParam[4] = 0.9f;
			_surfParam[5] = 0.6f;
			_surfParam[6] = 0.65f;
			_surfParam[7] = 0.5f;
			_surfParam[8] = 0.9f;
			_surfParam[9] = 0.4f;
			_surfParam[10] = 0.65f;
			_surfParam[11] = 1.25f;
			_surfParam[12] = 0.2f;
			_surfParam[13] = 1f;
			_surfParam[14] = 0.65f;
			_surfParam[15] = 1.05f;
			_surfParam[16] = 0.1f;
			_surfParam[17] = 0.7f;
			if (Mathf.Abs(r) < 0.01f)
			{
				r = 0.27f;
			}
			float num = r / 0.27f;
			_scale = num * num * ((float)width / 225f);
			_a0 = 1.6f;
			_a1 = -82f / _scale;
			_a2 = 1750f;
			_a3 = 1800f * _scale;
			_a4 = 8.5f * _scale;
			_a5 = 0.012f;
			_a6 = -0.07f / _scale;
			_a7 = 0.4f;
			_a8 = 0.017f;
			_a9 = -0.019f / _scale;
			_a10 = -0.18f / _scale;
			_a11 = -10f / _scale;
			_a12 = -0.019f;
			_b1 = -80f / _scale;
			_b0Front = 1.55f;
			_b2Front = 1500f;
			_b0Side = 1.55f;
			_b2Side = 1500f;
			_b3 = 23.3f / _scale / _scale;
			_b4 = LerpParam(width, prof, 280f, 450f, 450f, 650f);
			_b5 = 0.05f;
			_b6 = 0.0032f / (_scale * _scale);
			_b7 = -0.008f / _scale;
			_b8 = -0.025f;
			_c0 = 2.1f;
			_c1 = -3.9f;
			_c2 = -3.9f;
			_c3 = -1.26f;
			_c4 = -8.2f;
			_c5 = 0.025f;
			_c6 = 0f;
			_c7 = 0.044f;
			_c8 = -0.58f;
			_c9 = 0.18f;
			_c10 = 0.043f;
			_c11 = 0.048f;
			_c12 = -0.0035f;
			_c13 = -0.18f;
			_c14 = 0.14f;
			_c15 = -1.029f;
			_c16 = 0.27f;
			_c17 = -1.1f;
		}

		private void SetTouring(int width, int prof, float r)
		{
			_width = width;
			_prof = prof;
			_surfParam[0] = 1f;
			_surfParam[1] = 1f;
			_surfParam[2] = 0.82f;
			_surfParam[3] = 0.85f;
			_surfParam[4] = 0.9f;
			_surfParam[5] = 0.7f;
			_surfParam[6] = 0.85f;
			_surfParam[7] = 0.6f;
			_surfParam[8] = 0.9f;
			_surfParam[9] = 0.6f;
			_surfParam[10] = 0.7f;
			_surfParam[11] = 1.25f;
			_surfParam[12] = 0.35f;
			_surfParam[13] = 1f;
			_surfParam[14] = 0.55f;
			_surfParam[15] = 1.05f;
			_surfParam[16] = 0.2f;
			_surfParam[17] = 0.78f;
			if (Mathf.Abs(r) <= 0.01f)
			{
				r = 0.27f;
			}
			float num = r / 0.27f;
			_scale = num * num * ((float)width / 225f);
			_a0 = 1.45f;
			_a1 = -100f / _scale * 0.99f;
			_a2 = 1700f;
			_a3 = LerpParam(width, prof, 1300f, 1300f, 1300f, 1300f) * _scale;
			_a4 = LerpParam(width, prof, 11f, 11f, 11f, 11f) * _scale;
			_a5 = 0.013f;
			_a6 = -0.08f / _scale;
			_a7 = -0.4f;
			_a8 = 0.019f;
			_a9 = -0.019f / _scale;
			_a10 = -0.18f / _scale;
			_a11 = -11f / _scale;
			_a12 = -0.021f;
			_b1 = -100f / _scale * 0.99f;
			_b0Front = 1.45f;
			_b2Front = 1300f;
			_b0Side = _b0Front;
			_b2Side = _b2Front;
			_b3 = 23.3f / (_scale * _scale);
			_b4 = LerpParam(width, prof, 350f, 600f, 550f, 800f);
			_b5 = 0f;
			_b6 = 0.003f / (_scale * _scale);
			_b7 = -0.008f / _scale;
			_b8 = -0.03f;
			_c0 = 2.3f;
			_c1 = -3.8f;
			_c2 = -3.14f;
			_c3 = -1.16f;
			_c4 = -7.2f;
			_c5 = 0f;
			_c6 = 0f;
			_c7 = 0.044f;
			_c8 = -0.58f;
			_c9 = 0.18f;
			_c10 = 0.043f;
			_c11 = 0.048f;
			_c12 = -0.0035f;
			_c13 = -0.18f;
			_c14 = 0.14f;
			_c15 = -1.029f;
			_c16 = 0.27f;
			_c17 = -1.1f;
		}

		private void SetOffroad(int width, int prof, float r)
		{
			_width = width;
			_prof = prof;
			_surfParam[0] = 1f;
			_surfParam[1] = 0.62f;
			_surfParam[2] = 0.62f;
			_surfParam[3] = 0.95f;
			_surfParam[4] = 0.95f;
			_surfParam[5] = 0.88f;
			_surfParam[6] = 0.93f;
			_surfParam[7] = 0.76f;
			_surfParam[8] = 0.54f;
			_surfParam[9] = 0.65f;
			_surfParam[10] = 0.58f;
			_surfParam[11] = 0.79f;
			_surfParam[12] = 0.55f;
			_surfParam[13] = 0.62f;
			_surfParam[14] = 0.83f;
			_surfParam[15] = 0.71f;
			_surfParam[16] = 0.25f;
			_surfParam[17] = 0.92f;
			r = Mathf.Max(Mathf.Abs(r), 0.27f);
			float num = r / 0.27f;
			_scale = num * num * ((float)width / 225f);
			_a0 = 1.5f;
			_a1 = -30f / _scale;
			_a2 = 1450f;
			_a3 = LerpParam(width, prof, 1200f, 1300f, 1300f, 1400f);
			_a4 = LerpParam(width, prof, 7.9f, 8f, 7.9f, 8f);
			_a5 = 0.012f;
			_a6 = -0.2f / _scale;
			_a7 = 0.2f;
			_a8 = -0.03f;
			_a9 = -0.001f / _scale;
			_a10 = -0.15f / _scale;
			_a11 = -8.5f / _scale;
			_a12 = -0.29f;
			_b1 = -60f / _scale;
			_b0Front = 1.4f;
			_b2Front = 1400f;
			_b0Side = _b0Front;
			_b2Side = _b2Front;
			_b3 = 20f / (_scale * _scale);
			_b4 = LerpParam(width, prof, 340f, 350f, 340f, 350f);
			_b5 = 0.04f;
			_b6 = 0f;
			_b7 = 0.06f / _scale;
			_b8 = -0.02f;
			_c0 = 2.3f;
			_c1 = -3.8f;
			_c2 = -3.14f;
			_c3 = -1.16f;
			_c4 = -7.2f;
			_c5 = 0f;
			_c6 = 0f;
			_c7 = 0.044f;
			_c8 = -0.58f;
			_c9 = 0.18f;
			_c10 = 0.043f;
			_c11 = 0.048f;
			_c12 = -0.0035f;
			_c13 = -0.18f;
			_c14 = 0.14f;
			_c15 = -1.029f;
			_c16 = 0.27f;
			_c17 = -1.1f;
		}

		private void SetSnow(int width, int prof, float r)
		{
			SetTouring(width, prof, r);
			_surfParam[0] = 1f;
			_surfParam[1] = 0.8f;
			_surfParam[2] = 0.75f;
			_surfParam[3] = 0.8f;
			_surfParam[4] = 0.8f;
			_surfParam[5] = 0.7f;
			_surfParam[6] = 0.85f;
			_surfParam[7] = 0.6f;
			_surfParam[8] = 0.8f;
			_surfParam[9] = 0.6f;
			_surfParam[10] = 0.7f;
			_surfParam[11] = 1.1f;
			_surfParam[12] = 0.5f;
			_surfParam[13] = 0.7f;
			_surfParam[14] = 0.7f;
			_surfParam[15] = 0.75f;
			_surfParam[16] = 0.32f;
			_surfParam[17] = 0.75f;
		}

		private void SetVintage(int width, int prof, float r)
		{
			_width = width;
			_prof = prof;
			_surfParam[0] = 1f;
			_surfParam[1] = 0.78f;
			_surfParam[2] = 0.55f;
			_surfParam[3] = 0.78f;
			_surfParam[4] = 0.65f;
			_surfParam[5] = 0.78f;
			_surfParam[6] = 0.82f;
			_surfParam[7] = 0.55f;
			_surfParam[8] = 0.82f;
			_surfParam[9] = 0.5f;
			_surfParam[10] = 0.65f;
			_surfParam[11] = 1.1f;
			_surfParam[12] = 0.22f;
			_surfParam[13] = 0.75f;
			_surfParam[14] = 0.35f;
			_surfParam[15] = 0.95f;
			_surfParam[16] = 0.12f;
			_surfParam[17] = 0.78f;
			if (Mathf.Abs(r) <= 0.01f)
			{
				r = 0.27f;
			}
			float num = r / 0.27f;
			_scale = num * num * ((float)width / 225f);
			_a0 = 1.4f;
			_a1 = -120f / _scale;
			_a2 = 1100f;
			_a3 = 700f * _scale;
			_a4 = 11f * _scale;
			_a5 = 0.013f;
			_a6 = -0.08f / _scale;
			_a7 = -0.6f;
			_a8 = 0.019f;
			_a9 = -0.019f / _scale;
			_a10 = -0.18f / _scale;
			_a11 = -11f / _scale;
			_a12 = -0.021f;
			_b1 = -120f / _scale;
			_b0Front = 1.4f;
			_b2Front = 1000f;
			_b0Side = _b0Front;
			_b2Side = _b2Front;
			_b3 = 23.3f / (_scale * _scale);
			_b4 = LerpParam(width, prof, 250f, 400f, 400f, 550f);
			_b5 = 0f;
			_b6 = 0.003f / (_scale * _scale);
			_b7 = -0.008f / _scale;
			_b8 = -0.03f;
			_c0 = 2.3f;
			_c1 = -3.8f;
			_c2 = -3.14f;
			_c3 = -1.16f;
			_c4 = -7.2f;
			_c5 = 0f;
			_c6 = 0f;
			_c7 = 0.044f;
			_c8 = -0.58f;
			_c9 = 0.18f;
			_c10 = 0.043f;
			_c11 = 0.048f;
			_c12 = -0.0035f;
			_c13 = -0.18f;
			_c14 = 0.14f;
			_c15 = -1.029f;
			_c16 = 0.27f;
			_c17 = -1.1f;
		}

		private void SetSoftSlick(int width, int prof, float r)
		{
			_width = width;
			_prof = prof;
			_surfParam[0] = 1f;
			_surfParam[1] = 1f;
			_surfParam[2] = 0.7f;
			_surfParam[3] = 0.6f;
			_surfParam[4] = 0.7f;
			_surfParam[5] = 0.5f;
			_surfParam[6] = 0.5f;
			_surfParam[7] = 0.5f;
			_surfParam[8] = 0.9f;
			_surfParam[9] = 0.4f;
			_surfParam[10] = 0.6f;
			_surfParam[11] = 1.25f;
			_surfParam[12] = 0.2f;
			_surfParam[13] = 1f;
			_surfParam[14] = 0.4f;
			_surfParam[15] = 1.05f;
			_surfParam[16] = 0.15f;
			_surfParam[17] = 0.55f;
			if (Mathf.Abs(r) <= 0.01f)
			{
				r = 0.27f;
			}
			float num = r / 0.27f;
			_scale = num * num * ((float)width / 225f);
			_a0 = 1.65f;
			_a1 = -72f / _scale;
			_a2 = 2200f;
			_a3 = 2200f * _scale;
			_a4 = 5.5f * _scale;
			_a5 = 0.011f;
			_a6 = -0.05f / _scale;
			_a7 = 0.5f;
			_a8 = 0.015f;
			_a9 = -0.019f / _scale;
			_a10 = -0.18f / _scale;
			_a11 = -10f / _scale;
			_a12 = -0.018f;
			_b1 = -72f / _scale;
			_b0Front = 1.55f;
			_b2Front = 1950f;
			_b0Side = _b0Front;
			_b2Side = _b2Front;
			_b3 = 23.3f / (_scale * _scale);
			_b4 = LerpParam(width, prof, 320f, 530f, 530f, 730f);
			_b5 = 0.08f;
			_b6 = 0.003f / (_scale * _scale);
			_b7 = -0.007f / _scale;
			_b8 = -0.03f;
			_c0 = 2.1f;
			_c1 = -3.9f;
			_c2 = -3.9f;
			_c3 = -1.26f;
			_c4 = -8.2f;
			_c5 = 0.025f;
			_c6 = 0f;
			_c7 = 0.044f;
			_c8 = -0.58f;
			_c9 = 0.18f;
			_c10 = 0.043f;
			_c11 = 0.048f;
			_c12 = -0.0035f;
			_c13 = -0.18f;
			_c14 = 0.14f;
			_c15 = -1.029f;
			_c16 = 0.27f;
			_c17 = -1.1f;
		}

		private void SetHardSlick(int width, int prof, float r)
		{
			_width = width;
			_prof = prof;
			_surfParam[0] = 1f;
			_surfParam[1] = 1f;
			_surfParam[2] = 0.72f;
			_surfParam[3] = 0.6f;
			_surfParam[4] = 0.7f;
			_surfParam[5] = 0.5f;
			_surfParam[6] = 0.5f;
			_surfParam[7] = 0.5f;
			_surfParam[8] = 0.9f;
			_surfParam[9] = 0.4f;
			_surfParam[10] = 0.6f;
			_surfParam[11] = 1.15f;
			_surfParam[12] = 0.25f;
			_surfParam[13] = 1f;
			_surfParam[14] = 0.45f;
			_surfParam[15] = 1.05f;
			_surfParam[16] = 0.2f;
			_surfParam[17] = 0.55f;
			if (Mathf.Abs(r) <= 0.01f)
			{
				r = 0.27f;
			}
			float num = r / 0.27f;
			_scale = num * num * ((float)width / 225f);
			_a0 = 1.55f;
			_a1 = -72f / _scale;
			_a2 = 1900f;
			_a3 = 1750f * _scale;
			_a4 = 5.5f * _scale;
			_a5 = 0.011f;
			_a6 = -0.05f / _scale;
			_a7 = 0.45f;
			_a8 = 0.015f;
			_a9 = -0.019f / _scale;
			_a10 = -0.18f / _scale;
			_a11 = -10f / _scale;
			_a12 = -0.018f;
			_b1 = -72f / _scale;
			_b0Front = 1.45f;
			_b2Front = 1650f;
			_b0Side = _b0Front;
			_b2Side = _b2Front;
			_b3 = 23.3f / (_scale * _scale);
			_b4 = LerpParam(width, prof, 280f, 470f, 470f, 670f);
			_b5 = 0.08f;
			_b6 = 0.003f / (_scale * _scale);
			_b7 = -0.007f / _scale;
			_b8 = -0.03f;
			_c0 = 2.1f;
			_c1 = -3.9f;
			_c2 = -3.9f;
			_c3 = -1.26f;
			_c4 = -8.2f;
			_c5 = 0.025f;
			_c6 = 0f;
			_c7 = 0.044f;
			_c8 = -0.58f;
			_c9 = 0.18f;
			_c10 = 0.043f;
			_c11 = 0.048f;
			_c12 = -0.0035f;
			_c13 = -0.18f;
			_c14 = 0.14f;
			_c15 = -1.029f;
			_c16 = 0.27f;
			_c17 = -1.1f;
		}

		private void SetDragSlick(int width, int prof, float r)
		{
			_width = width;
			_prof = prof;
			_surfParam[0] = 1f;
			_surfParam[1] = 1.05f;
			_surfParam[2] = 0.3f;
			_surfParam[3] = 0.4f;
			_surfParam[4] = 0.4f;
			_surfParam[5] = 0.3f;
			_surfParam[6] = 0.3f;
			_surfParam[7] = 0.3f;
			_surfParam[8] = 0.95f;
			_surfParam[9] = 0.2f;
			_surfParam[10] = 0.55f;
			_surfParam[11] = 1.5f;
			_surfParam[12] = 0.08f;
			_surfParam[13] = 0.95f;
			_surfParam[14] = 0.2f;
			_surfParam[15] = 1.3f;
			_surfParam[16] = 0.05f;
			_surfParam[17] = 0.3f;
			if (Mathf.Abs(r) <= 0.01f)
			{
				r = 0.27f;
			}
			float num = r / 0.27f;
			_scale = num * num * ((float)width / 225f);
			_a0 = 1.4f;
			_a1 = -50f / _scale;
			_a2 = 1400f;
			_a3 = 800f * _scale;
			_a4 = 7f * _scale;
			_a5 = 0.013f;
			_a6 = -0.06f / _scale;
			_a7 = -0.3f;
			_a8 = 0.017f;
			_a9 = -0.019f / _scale;
			_a10 = -0.18f / _scale;
			_a11 = -10f / _scale;
			_a12 = -0.02f;
			_b1 = -40f / _scale;
			_b0Front = 1.7f;
			_b2Front = 2600f;
			_b0Side = _b0Front;
			_b2Side = _b2Front;
			_b3 = 25f / (_scale * _scale);
			_b4 = LerpParam(width, prof, 450f, 650f, 650f, 850f);
			_b5 = 0.05f;
			_b6 = 0.003f / (_scale * _scale);
			_b7 = -0.006f / _scale;
			_b8 = -0.025f;
			_c0 = 2.1f;
			_c1 = -3.9f;
			_c2 = -3.9f;
			_c3 = -1.26f;
			_c4 = -8.2f;
			_c5 = 0.025f;
			_c6 = 0f;
			_c7 = 0.044f;
			_c8 = -0.58f;
			_c9 = 0.18f;
			_c10 = 0.043f;
			_c11 = 0.048f;
			_c12 = -0.0035f;
			_c13 = -0.18f;
			_c14 = 0.14f;
			_c15 = -1.029f;
			_c16 = 0.27f;
			_c17 = -1.1f;
		}

		private void SetBuggy(int width, int prof, float r)
		{
			_width = width;
			_prof = prof;
			_surfParam[0] = 1f;
			_surfParam[1] = 0.51f;
			_surfParam[2] = 0.48f;
			_surfParam[3] = 1f;
			_surfParam[4] = 0.95f;
			_surfParam[5] = 0.95f;
			_surfParam[6] = 1f;
			_surfParam[7] = 0.85f;
			_surfParam[8] = 0.41f;
			_surfParam[9] = 0.95f;
			_surfParam[10] = 0.58f;
			_surfParam[11] = 0.79f;
			_surfParam[12] = 0.6f;
			_surfParam[13] = 0.51f;
			_surfParam[14] = 0.83f;
			_surfParam[15] = 0.58f;
			_surfParam[16] = 0.25f;
			_surfParam[17] = 1f;
			r = Mathf.Max(Mathf.Abs(r), 0.27f);
			float num = r / 0.27f;
			_scale = num * num * ((float)width / 225f);
			_a0 = 1.5f;
			_a1 = -30f / _scale;
			_a2 = 1450f;
			_a3 = LerpParam(width, prof, 1080f, 1170f, 1170f, 1260f);
			_a4 = LerpParam(width, prof, 7.9f, 8f, 7.9f, 8f);
			_a5 = 0.012f;
			_a6 = -0.2f / _scale;
			_a7 = 0.2f;
			_a8 = -0.03f;
			_a9 = -0.001f / _scale;
			_a10 = -0.15f / _scale;
			_a11 = -8.5f / _scale;
			_a12 = -0.29f;
			_b1 = -60f / _scale;
			_b0Front = 1.4f;
			_b2Front = 1400f;
			_b0Side = _b0Front;
			_b2Side = _b2Front;
			_b3 = 20f / (_scale * _scale);
			_b4 = LerpParam(width, prof, 306f, 315f, 306f, 315f);
			_b5 = 0.04f;
			_b6 = 0f;
			_b7 = 0.06f / _scale;
			_b8 = -0.02f;
			_c0 = 2.3f;
			_c1 = -3.8f;
			_c2 = -3.14f;
			_c3 = -1.16f;
			_c4 = -7.2f;
			_c5 = 0f;
			_c6 = 0f;
			_c7 = 0.044f;
			_c8 = -0.58f;
			_c9 = 0.18f;
			_c10 = 0.043f;
			_c11 = 0.048f;
			_c12 = -0.0035f;
			_c13 = -0.18f;
			_c14 = 0.14f;
			_c15 = -1.029f;
			_c16 = 0.27f;
			_c17 = -1.1f;
		}

		public void SetType(TyreType type, int width, int prof, float r)
		{
			_type = type;
			switch (_type)
			{
			case TyreType.Touring:
				SetTouring(width, prof, r);
				break;
			case TyreType.SemiSlick:
				SetSemiSlick(width, prof, r);
				break;
			case TyreType.Slick:
				SetSlick(width, prof, r);
				break;
			case TyreType.Offroad:
				SetOffroad(width, prof, r);
				break;
			case TyreType.Winter:
				SetSnow(width, prof, r);
				break;
			case TyreType.Vintage:
				SetVintage(width, prof, r);
				break;
			case TyreType.SoftSlick:
				SetSoftSlick(width, prof, r);
				break;
			case TyreType.HardSlick:
				SetHardSlick(width, prof, r);
				break;
			case TyreType.DragSlick:
				SetDragSlick(width, prof, r);
				break;
			case TyreType.Buggy:
				SetBuggy(width, prof, r);
				break;
			default:
				SetTouring(width, prof, r);
				break;
			}
		}

		public float GetProf()
		{
			return _prof;
		}

		public float GetSaVolume()
		{
			return _saVol;
		}

		public float GetSrVolume()
		{
			return _srVol;
		}

		public float GetLateralSlip()
		{
			return _sa;
		}

		public float GetLongitudinalSlip()
		{
			return _sr;
		}

		public float GetFx()
		{
			return _fx;
		}

		public float GetFy()
		{
			return _fy;
		}

		public float GetMz()
		{
			return _mz;
		}

		public float GetFz()
		{
			return _fz;
		}

		public float GetBestSlipAngle()
		{
			return _bestSlipAngle;
		}

		public float GetBestSlipRatio()
		{
			return _bestSlipRatio;
		}

		private float GetSurfGrip(Surface surface)
		{
			float num = _surfParam[(int)surface];
			float wetness = WeatherPhysics.Wetness;
			if (wetness <= 0f)
			{
				return num;
			}
			float num2 = _surfParam[(int)WeatherPhysics.GetWetEquivalent(surface)] * WeatherPhysics.GetWetGripMul(surface);
			return num + (num2 - num) * wetness;
		}

		public float GetMuStatLong(float vel, float verticalLoad, Surface surfaceType)
		{
			float surfGrip = GetSurfGrip(surfaceType);
			float num = 1.5f * _scale;
			float num2 = Mathf.Max((verticalLoad >= num) ? (_b1 * (verticalLoad - num) + _b2Front) : _b2Front, _b2Front * 0.5f) * surfGrip;
			float num3 = Mathf.Max(1.02f - vel * 0.002f, 0.9f);
			return Mathf.Max(num2 * num3 * 0.001f, 0.1f);
		}

		public float GetMuStatLat(float velocity, float verticalLoad, Surface surfaceType)
		{
			float surfGrip = GetSurfGrip(surfaceType);
			float num = 1.5f * _scale;
			float num2 = Mathf.Max((verticalLoad >= num) ? (_a1 * (verticalLoad - num) + _a2) : _a2, _a2 * 0.5f) * surfGrip;
			float num3 = Mathf.Max(1.02f - velocity * 0.002f, 0.9f);
			return Mathf.Max(num2 * num3 * 0.001f, 0.1f);
		}

		public float GetFricMult()
		{
			return _fricMult;
		}

		public float GetFricScale(PhysicMaterial mat)
		{
			return Mathf.Max(mat.Friction * _fricMult * (_fricTempTyre - _fricWearTyre), 0f);
		}

		public void SetFricMult(float mult)
		{
			_fricMult = mult;
		}

		public void SetFricTempTyre(float mult)
		{
			_fricTempTyre = mult;
		}

		public void SetFricWearTyre(float mult)
		{
			_fricWearTyre = mult;
		}

		public void SetCamber(float camber)
		{
			_camber = camber * 57.29578f;
		}

		public void SetSlipAngle(float sa)
		{
			_slipAngle = sa * 57.29578f;
		}

		public void SetSlipRatio(float sr)
		{
			_slipRatio = sr * 100f;
		}

		public void SetNormalForce(float force)
		{
			_fz = force * 0.001f;
		}

		public void CalcForces(PhysicMaterial mat)
		{
			Surface surfaceType = mat.SurfaceType;
			float friction = GetSurfGrip(surfaceType) * mat.Friction * _fricMult * (_fricTempTyre - _fricWearTyre);
			float k = Mathf.Max(1.02f - Mathf.Abs(_wheel.AngularVelocity * _wheel.Radius) * 0.002f, 0.9f);
			bool dig = surfaceType == Surface.Sand || surfaceType == Surface.Grass || surfaceType == Surface.DirtRoad || surfaceType == Surface.Snow;
			float num = _slipAngle / _bestSlipAngle;
			float num2 = _slipRatio / _bestSlipRatio;
			float num3 = Mathf.Sqrt(PhysMath.Sqr(num2) + PhysMath.Sqr(num));
			_saVol = (_srVol = ((num3 > 0.9f) ? Mathf.Min((num3 - 0.9f) * 0.7f, 1.5f) : 0f));
			if (num3 > 1f)
			{
				_slipAngle = _bestSlipAngle * num3 * Mathf.Sign(num);
				_slipRatio = _bestSlipRatio * num3 * Mathf.Sign(num2);
			}
			_sa = num;
			_sr = num2;
			float num4 = Mathf.Abs(_sa);
			if (num4 > 2f)
			{
				num4 = 2f;
			}
			float num5 = Mathf.Max(num4 - 1f, 0f);
			_b0 = _b0Front * (1f - num5) + _b0Side * num5;
			_b2 = _b2Front * (1f - num5) + _b2Side * num5;
			_fx = CalcFx(friction, k, dig);
			_fy = CalcFy(friction, k, dig);
			if (num3 > 1f)
			{
				_fx *= Mathf.Abs(num2) / num3;
				_fy *= Mathf.Abs(num) / num3;
			}
			_mz = CalcMz();
		}

		public void CombineForces(Vector3 tarVel, Vector3 fwdDir, Vector3 rightDir, float wr)
		{
			float fx = _fx;
			float fy = _fy;
			float f = Vector3.Dot(fwdDir, tarVel) - wr;
			float f2 = Vector3.Dot(rightDir, tarVel);
			float num = Mathf.Abs(f);
			float num2 = Mathf.Abs(f2);
			float i = Mathf.Abs(_sr);
			float i2 = Mathf.Abs(_sa);
			float num3 = Mathf.Sqrt(PhysMath.Sqr(i) + PhysMath.Sqr(i2));
			if (num2 > 20f)
			{
				num2 = 20f;
			}
			float num4 = Mathf.Sqrt(PhysMath.Sqr(num) + PhysMath.Sqr(num2));
			if (!(num4 < 0.001f) && !(num3 <= 1f))
			{
				num /= num4;
				num2 /= num4;
				if (_wheel.IsFront && _wheel.IsPowered)
				{
					float t = Mathf.Clamp01((num3 - 1f) / 1f);
					float num5 = Mathf.Lerp(1f, 1.3f, t);
					num2 = Mathf.Min(num2 * num5, 1f);
					num = Mathf.Sqrt(Mathf.Max(1f - num2 * num2, 0f));
				}
				float num6 = Mathf.Sign(fx);
				float num7 = Mathf.Sign(fy);
				float num8 = Mathf.Sqrt(PhysMath.Sqr(fx) + PhysMath.Sqr(fy));
				_fx = num8 * num6 * num;
				_fy = num8 * num7 * num2;
			}
		}

		private float CalcFy(float friction, float k, bool dig)
		{
			float num;
			if (dig)
			{
				num = (0f - _a1) * _fz + _a2;
			}
			else
			{
				float num2 = _scale * 1.5f;
				num = ((_fz >= num2) ? (_a1 * (_fz - num2) + _a2) : _a2);
				num = Mathf.Max(num, _a2 * 0.5f);
			}
			num *= friction * k;
			float a = _a0;
			float num3 = num * _fz;
			float num4 = a * num3;
			float num5 = _a6 * _fz + _a7;
			float num6 = 100000f;
			if (Mathf.Abs(num4) > 0.001f && Mathf.Abs(_a4) > 0.001f)
			{
				float num7 = _fz / _a4;
				num6 = _a3 * (2f * num7 / (1f + num7 * num7)) * (1f - _a5 * Mathf.Abs(_camber)) / num4;
			}
			float num8 = _camber;
			float num9 = _a8 * num8;
			if (num8 > 0f && _slipAngle > 0f)
			{
				num8 *= 0.5f;
			}
			if (num8 < 0f && _slipAngle < 0f)
			{
				num8 *= 0.5f;
			}
			if (num8 < -2f && _slipAngle > 0f)
			{
				num8 *= 0.5f;
			}
			if (num8 > 2f && _slipAngle < 0f)
			{
				num8 *= 0.5f;
			}
			float num10 = (_a11 * _fz + _a12) * num8 * _fz;
			float num11 = _slipAngle + num9;
			float num12 = num6 * num11;
			float num13 = Mathf.Sin(a * Mathf.Atan(num12 * (1f - num5) + num5 * Mathf.Atan(num12)));
			_fy = num3 * num13 + num10;
			_maxForceY = ((num13 >= 0f) ? (num3 + num10) : (num3 - num10));
			_bestSlipAngle = CalcIdealSlipRatioAndAngle(num6, a, num5, num9);
			return _fy;
		}

		private float CalcFx(float friction, float k, bool dig)
		{
			float num;
			if (dig)
			{
				num = _b2;
			}
			else
			{
				float num2 = Mathf.Min(Mathf.Abs(_camber - 2f * Mathf.Clamp(0f - _sa, -1f, 1f)), 10f);
				float num3 = _scale * 1.5f;
				num = ((_fz >= num3) ? (_b1 * (1f + num2 * 0.025f) * (_fz - num3) + _b2) : _b2);
				if (num < _b2 * 0.5f)
				{
					num = _b2 * 0.5f;
				}
			}
			num *= friction;
			num *= k;
			float b = _b0;
			float num4 = num * _fz;
			float num5 = b * num4;
			float num6 = _fz * _fz;
			float num7 = ((Mathf.Abs(num5) <= 0.001f) ? 100000f : ((_b3 * num6 + _b4 * _fz) * Mathf.Exp((0f - _b5) * _fz) / num5));
			float num8 = Mathf.Clamp(_slipRatio, -100f, 100f);
			float num9 = _b6 * num6 + _b7 * _fz + _b8;
			float num10 = num7 * num8;
			float num11 = Mathf.Sin(b * Mathf.Atan(num10 - num9 * (num10 - Mathf.Atan(num10))));
			_fx = num4 * num11;
			_bestSlipRatio = CalcIdealSlipRatioAndAngle(num7, b, num9, 0f);
			_maxForceX = num4;
			return _fx;
		}

		private float CalcMz()
		{
			float num = Mathf.Abs(_camber);
			float num2 = _fz * _fz;
			float c = _c0;
			float num3 = _c1 * num2 + _c2 * _fz;
			float num4 = c * num3;
			float num5 = (_c7 * num2 + _c8 * _fz + _c9) * (1f - _c10 * num);
			float num6 = ((Mathf.Abs(num4) <= 0.001f) ? 100000f : ((_c3 * num2 + _c4 * _fz) * (1f - _c6 * num) * Mathf.Exp((0f - _c5) * _fz) / num4));
			float num7 = _c11 * _camber + _c12 * _fz + _c13;
			float num8 = (_c14 * num2 + _c15 * _fz) * _camber + _c16 * _fz + _c17;
			float num9 = _slipAngle + num7;
			float num10 = Mathf.Sin(c * Mathf.Atan(num6 * (1f - num5) * num9 + num5 * Mathf.Atan(num6 * num9)));
			_mz = num3 * num10 + num8;
			MaxForceZ = ((num10 >= 0f) ? (num3 + num8) : (num3 - num8));
			return _mz;
		}

		private float CalcIdealSlipRatioAndAngle(float b, float c, float e, float sh)
		{
			float num = 10f;
			return Mathf.Clamp((Mathf.Tan(1.570795f / (c + 1E-06f)) - e * Mathf.Atan(b * (num + sh)) - sh * b * (1f - e)) / (b * (1f - e)), 0.001f, 20f);
		}
	}
}
