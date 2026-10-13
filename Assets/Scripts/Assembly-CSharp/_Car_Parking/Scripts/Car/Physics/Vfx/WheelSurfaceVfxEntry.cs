using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.Physics.Vfx
{
	[Serializable]
	public class WheelSurfaceVfxEntry
	{
		public string ID;

		public ParticleSystem Prefab;

		public SurfaceTint[] Surfaces;

		[Header("Activation")]
		public float MinWheelLinearSpeed;

		public float MinSlipRatio;

		public float MinSlipAngle;

		[Tooltip("Если включено — эмит только при нажатом газе (отсекает торможение/юз).")]
		public bool RequireThrottle;

		[Header("Drift gating")]
		public bool EmitOnRearDuringDrift = true;

		public bool EmitOnFrontDuringDrift = true;

		public bool SkipEmitOnDriftWithoutWheelSpin = true;

		[Header("Quality gating")]
		public bool EmitForOnlineCarsAtLowQuality = true;

		[Header("Emission")]
		[Min(1f)]
		public int FrameSkip = 2;

		[Min(1f)]
		public int EmitCount = 1;

		public bool UseWheelSmokeVelocityForParticles = true;

		[Tooltip("Угол подъёма стартовой скорости частиц над горизонтом, в градусах. 0 = вектор параллелен земле (как раньше). 20–30 рекомендовано для брызг грязи, чтобы частицы летели чуть вверх и не падали под землю. Применяется только когда UseWheelSmokeVelocityForParticles=true.")]
		[Range(0f, 60f)]
		public float EmitUpwardAngleDegrees;

		public WheelEmitOrigin EmitOrigin;

		[Header("Trajectory shaping")]
		[Tooltip("Применяется только когда UseWheelSmokeVelocityForParticles=true и машина локальная (не online). Гасит эффект «чем быстрее крутится колесо — тем дальше летит частица»: горизонтальная стартовая скорость плавно выходит на плато у этого значения (м/с). На малой скорости почти без изменений, на большой не растёт бесконечно. 0 = без ограничения (старое поведение).")]
		[Min(0f)]
		public float MaxLaunchSpeed;

		[Tooltip("Множитель горизонтальной стартовой скорости частиц. <1 держит частицы ближе к поверхности.")]
		[Min(0f)]
		public float LaunchSpeedMultiplier = 1f;

		[Tooltip("Случайный разброс направления вылета (полуугол конуса вокруг вертикали, в градусах), свой для каждой частицы. Чем больше — тем шире рассеивание грязи/пыли. 0 = все частицы летят в одну сторону.")]
		[Range(0f, 90f)]
		public float SpreadAngleDegrees;

		[Range(0f, 1f)]
		[Tooltip("Случайный разброс скорости частиц (±доля). 0.3 = ±30% к стартовой скорости у каждой частицы. 0 = одинаковая скорость.")]
		public float SpeedRandomness;

		[Header("Wheel hub mode (used only when EmitOrigin = WheelHub)")]
		[Tooltip("Угловая скорость частиц вокруг оси колеса (рад/с). Линейная тангенциальная скорость = это значение × radius колеса. Не зависит от реальной AngularVelocity колеса; 0 = частицы стоят на месте.")]
		[Min(0f)]
		public float ParticleAngularSpeed = 1f;

		[Min(0f)]
		[Tooltip("Смещение круга эмиссии вдоль оси колеса наружу (м). 0 = центральная плоскость колеса.")]
		public float OuterSideOffset;
	}
}
