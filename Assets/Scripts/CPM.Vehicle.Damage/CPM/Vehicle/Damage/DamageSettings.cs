using System;
using UnityEngine;

namespace CPM.Vehicle.Damage
{
	[Serializable]
	public struct DamageSettings
	{
		[Tooltip("Порог импульса, ниже которого удар игнорируется, Н·с")]
		public float MinImpulse;

		[Tooltip("Масштаб импульса в законе насыщения глубины, Н·с")]
		public float ImpulseScale;

		[Tooltip("Предельная глубина вмятины, м (поле maxDeform компонента)")]
		public float MaxDepth;

		[Tooltip("Базовый радиус зоны удара, м (поле deformRadius компонента)")]
		public float RadiusBase;

		[Tooltip("Гашение вертикальной составляющей удара, 0..1")]
		public float YForceDamp;

		[Tooltip("Амплитуда детерминированного шума складок, м")]
		public float NoiseAmplitude;

		[Tooltip("Высота валика по краю вмятины в долях глубины, 0..1")]
		public float RimRatio;

		[Tooltip("Число итераций релаксации")]
		public int SolverIterations;

		[Tooltip("Относительное удлинение ребра, за которым металл течёт, а не пружинит")]
		public float YieldStrain;

		[Tooltip("Доля надпорогового растяга, уходящая в новую ненапряжённую длину за итерацию, 0..1")]
		public float PlasticRate;

		[Tooltip("Двугранный угол, ниже которого поверхность считается плоской, град")]
		public float FlatAngleDeg;

		[Tooltip("Двугранный угол, выше которого ребро считается каркасом, град")]
		public float RidgeAngleDeg;

		[Tooltip("Доля направления скольжения в направлении вдавливания, 0..1")]
		public float SlideStretch;

		public static DamageSettings Default => new DamageSettings
		{
			MinImpulse = 800f,
			ImpulseScale = 6000f,
			MaxDepth = 0.12f,
			RadiusBase = 0.5f,
			YForceDamp = 1f,
			NoiseAmplitude = 0.003f,
			RimRatio = 0.05f,
			SolverIterations = 8,
			YieldStrain = 0.02f,
			PlasticRate = 0.2f,
			FlatAngleDeg = 10f,
			RidgeAngleDeg = 60f,
			SlideStretch = 2.5f
		};
	}
}
