Shader "Skybox/Atmosphere"
{
	Properties
	{
		[KeywordEnum(None, Simple, High Quality)] _SunDisk ("Sun", Float) = 2
		[NoScaleOffset] _Tex ("NightStars   (HDR)", Cube) = "black" {}
		[Gamma] _Exposure ("Stars Exposure", Range(0, 8)) = 1
		_Rotation ("Rotation", Float) = 0
		_CloudsDistMul ("Clouds Distance", Float) = 1000
		_CloudsFade ("CloudsFade", Range(0, 1)) = 1
		_GroundReflection ("Ground Reflection", Range(0, 1)) = 0.2
		_StarsPitch ("Stars Pitch", Float) = 0
		_StarsRotation ("Stars Rotation", Float) = 0
		[NoScaleOffset] _Clouds2D_A ("Clouds A", Cube) = "black" {}
		[NoScaleOffset] _Clouds2D_B ("Clouds B", Cube) = "black" {}
		_CloudsMorph ("Clouds Morph", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags { "PreviewType" = "Skybox" "QUEUE" = "Background" "RenderType" = "Background" }
		Pass
		{
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ FOG_EXP
			#pragma multi_compile __ _QUALITY_POTATO

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[4];
			float _GlobalsPad1;
			float _Rotation;
			float _StarsPitch;
			float _StarsRotation;
			cbuffer UnityLighting : register(b1)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float3 i_TEXCOORD5 : TEXCOORD5, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_TEXCOORD0 : TEXCOORD0, out float3 o_TEXCOORD1 : TEXCOORD1, out float3 o_TEXCOORD2 : TEXCOORD2, out float3 o_TEXCOORD3 : TEXCOORD3, out float4 o_TEXCOORD4 : TEXCOORD4, out float3 o_TEXCOORD5 : TEXCOORD5)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r1 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r2 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r1.yyyy));
				r2 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r1.xxxx) + asfloat(r2));
				r2 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r1.zzzz) + asfloat(r2));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r1.wwww) + asfloat(r2);
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				o_TEXCOORD1 = unity_ObjectToWorld._m03_m13_m23 * i_POSITION0.www + asfloat(r0.xyz);
				o_TEXCOORD0 = asfloat(r1.xyz);
				o_TEXCOORD2.y = asfloat(r1.y);
				r0.xyz = asuint(float3(0.0174532942, 0.0174532942, 0.0174532942) * float3(_Rotation, _StarsPitch, _StarsRotation));
				float sin0;
				float cos0;
				sincos(asfloat(r0.x), sin0, cos0);
				r0.x = asuint(sin0);
				r2.x = asuint(cos0);
				r3.x = asuint(-asfloat(r0.x));
				r3.y = r2.x;
				r3.z = r0.x;
				o_TEXCOORD2.x = dot(asfloat(r3.yx), asfloat(r1.xz));
				o_TEXCOORD2.z = dot(asfloat(r3.zy), asfloat(r1.xz));
				r0.x = asuint(dot(_WorldSpaceLightPos0.xyzw, _WorldSpaceLightPos0.xyzw));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r2.xyz = asuint(_WorldSpaceLightPos0.xyz * asfloat(r0.xxx));
				o_TEXCOORD3.x = dot(asfloat(r3.yx), asfloat(r2.xz));
				o_TEXCOORD3.z = dot(asfloat(r3.zy), asfloat(r2.xz));
				o_TEXCOORD3.y = asfloat(r2.y);
				float sin1;
				float cos1;
				sincos(asfloat(r0.y), sin1, cos1);
				r0.x = asuint(sin1);
				r2.x = asuint(cos1);
				float sin2;
				float cos2;
				sincos(asfloat(r0.z), sin2, cos2);
				r3.x = asuint(sin2);
				r4.x = asuint(cos2);
				r5.z = r0.x;
				r5.y = r2.x;
				r5.x = asuint(-asfloat(r0.x));
				o_TEXCOORD5.y = dot(asfloat(r5.xy), asfloat(r1.yz));
				r1.w = asuint(dot(asfloat(r5.yz), asfloat(r1.yz)));
				r0.x = asuint(-asfloat(r3.x));
				r0.y = r4.x;
				r0.z = r3.x;
				o_TEXCOORD5.z = dot(asfloat(r0.zy), asfloat(r1.xw));
				o_TEXCOORD5.x = dot(asfloat(r0.yx), asfloat(r1.xw));
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if (defined(FOG_EXP) && defined(_QUALITY_POTATO)) || defined(_QUALITY_POTATO)
			float4 _GlobalsPad0[2];
			float4 _Tex_HDR;
			float _Exposure;
			float _CloudsDistMul;
			float _SunIntensity;
			float _CloudsFade;
			float _GroundReflection;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[1];
			float _CloudsMorph;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			float4 _GlobalsPad8[1];
			float _CloudsClearLerp01;
			cbuffer UnityLighting : register(b1)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
			}
			cbuffer _AtmosphereGlobals : register(b2)
			{
				float _GlobalOvercast : packoffset(c0);
				float3 _GlobalOvercastTint : packoffset(c0.y);
			}
			Texture2D<float4> _skyViewLut : register(t0);
			Texture2D<float4> _transmittanceLut : register(t1);
			TextureCube<float4> _Tex : register(t2);
			TextureCube<float4> _Clouds2D_A : register(t3);
			TextureCube<float4> _Clouds2D_B : register(t4);
			SamplerState sampler_transmittanceLut : register(s0);
			SamplerState sampler_skyViewLut : register(s1);
			SamplerState sampler_Tex : register(s2);
			SamplerState sampler_Clouds2D_A : register(s3);
			SamplerState sampler_Clouds2D_B : register(s4);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float4 i_TEXCOORD4 : TEXCOORD4, float3 i_TEXCOORD5 : TEXCOORD5, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				uint4 r7;
				uint4 r8;
				uint4 r9;
				bool4 b0;
				bool4 b2;
				bool4 b3;
				bool4 b9;
				r0.y = asuint(abs(i_TEXCOORD2.y));
				r0.xz = asuint(i_TEXCOORD2.xz);
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				r0.w = asuint(saturate(ceil(-i_TEXCOORD0.y)));
				r1 = asuint(_Clouds2D_B.SampleLevel(sampler_Clouds2D_B, asfloat(r0.xyz), asfloat(r0.w)));
				r2 = asuint(_Clouds2D_A.SampleLevel(sampler_Clouds2D_A, asfloat(r0.xyz), asfloat(r0.w)));
				r0.xyz = asuint(float3(1.0, -1.0, 1.0) * asfloat(r0.xyz));
				r1 = asuint(-asfloat(r2) + asfloat(r1));
				r3.x = asuint(saturate(_CloudsMorph));
				r1 = asuint(asfloat(r3.xxxx) * asfloat(r1) + asfloat(r2));
				b2.xyz = (float3(0.0, 0.0, 0.0) < i_TEXCOORD3.zxy);
				r2.x = b2.x ? r1.y : r1.x;
				r4 = asuint(_Clouds2D_B.SampleLevel(sampler_Clouds2D_B, asfloat(r0.xyz), asfloat(r0.w)));
				r0 = asuint(_Clouds2D_A.SampleLevel(sampler_Clouds2D_A, asfloat(r0.xyz), asfloat(r0.w)));
				r3.yzw = asuint(asfloat(r4.xyw) + -asfloat(r0.xyw));
				r0.xyz = asuint(asfloat(r3.xxx) * asfloat(r3.yzw) + asfloat(r0.xyw));
				r0.w = b2.y ? r0.x : r1.z;
				r2.y = b2.z ? r0.y : 0u;
				r3.xyz = asuint(i_TEXCOORD3.zxy * i_TEXCOORD3.zxy);
				r0.w = asuint(asfloat(r3.y) * asfloat(r0.w));
				r0.w = asuint(asfloat(r3.x) * asfloat(r2.x) + asfloat(r0.w));
				r0.w = asuint(asfloat(r3.z) * asfloat(r2.y) + asfloat(r0.w));
				r2.y = asuint(max(0.00200000009, abs(i_TEXCOORD0.y)));
				r2.xz = asuint(i_TEXCOORD0.xz);
				r2.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r2.w = asuint(rsqrt(asfloat(r2.w)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r2.www));
				r4.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.zzz));
				r4.xyz = asuint(asfloat(r4.xyz) * float3(_CloudsDistMul, _CloudsDistMul, _CloudsDistMul) + float3(0.0, 6.36000013, 0.0));
				r2.w = asuint(dot(asfloat(r4.xyz), asfloat(r4.xyz)));
				r2.w = asuint(sqrt(asfloat(r2.w)));
				r3.w = asuint(-6.36000013 + asfloat(r2.w));
				r4.xyz = asuint(asfloat(r4.xyz) / asfloat(r2.www));
				r5.z = asuint(saturate(10.0 * asfloat(r3.w)));
				r2.w = asuint(dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz));
				r2.w = asuint(rsqrt(asfloat(r2.w)));
				r6.xyz = asuint(_WorldSpaceLightPos0.xyz * asfloat(r2.www));
				r2.w = asuint(dot(asfloat(r6.xyz), asfloat(r4.xyz)));
				r3.w = asuint(dot(-asfloat(r6.xyz), asfloat(r4.xyz)));
				r5.x = asuint(saturate(asfloat(r3.w) * 0.5 + 0.5));
				r4 = asuint(_transmittanceLut.Sample(sampler_transmittanceLut, asfloat(r5.xz)));
				r5.y = asuint(saturate(asfloat(r2.w) * 0.5 + 0.5));
				r5 = asuint(_transmittanceLut.Sample(sampler_transmittanceLut, asfloat(r5.yz)));
				r5.xyz = asuint(asfloat(r5.xyz) * asfloat(r0.www));
				r7.xyz = asuint(float3(-1.0, -1.0, -1.0) + _GlobalOvercastTint.xyz);
				r0.w = asuint(saturate(_GlobalOvercast));
				r7.xyz = asuint(asfloat(r0.www) * asfloat(r7.xyz) + float3(1.0, 1.0, 1.0));
				r5.xyz = asuint(asfloat(r7.xyz) * asfloat(r5.xyz));
				r1.w = asuint(log2(asfloat(r1.w)));
				r1.w = asuint(2.20000005 * asfloat(r1.w));
				r1.w = asuint(exp2(asfloat(r1.w)));
				r2.w = asuint(saturate(_CloudsClearLerp01));
				r2.w = asuint(1.0 + -asfloat(r2.w));
				r3.w = asuint(asfloat(r2.w) * asfloat(r1.w));
				r1.w = asuint(1.0 - asfloat(r1.w) * asfloat(r2.w));
				r2.w = asuint(dot(asfloat(r6.xzy), float3(2.46523496e-10, 2.46523496e-10, 1.0)));
				r8.z = asuint(saturate(asfloat(r2.w) * 0.5 + 0.5));
				r8.yw = uint2(0u, 0u);
				r9 = asuint(_transmittanceLut.Sample(sampler_transmittanceLut, asfloat(r8.zw)));
				r9.xyz = asuint(float3(_GroundReflection, _GroundReflection, _GroundReflection) * asfloat(r9.xyz));
				r9.xyz = asuint(asfloat(r9.xyz) * asfloat(r0.yyy));
				r5.xyz = asuint(asfloat(r5.xyz) * asfloat(r3.www) + asfloat(r9.xyz));
				b9.xyz = (float3(0.0, 0.0, 0.0) < -i_TEXCOORD3.zxy);
				r1.x = b9.x ? r1.y : r1.x;
				r0.x = b9.y ? r0.x : r1.z;
				r0.y = b9.z ? r0.y : 0u;
				r0.z = asuint(1.0 - asfloat(r0.z) * _CloudsFade);
				r0.xz = asuint(asfloat(r3.yw) * asfloat(r0.xz));
				r0.x = asuint(asfloat(r3.x) * asfloat(r1.x) + asfloat(r0.x));
				r0.x = asuint(asfloat(r3.z) * asfloat(r0.y) + asfloat(r0.x));
				r1.xyz = asuint(asfloat(r0.xxx) * asfloat(r4.xyz));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r7.xyz));
				r0.x = asuint(asfloat(r0.w) * -0.149999976 + 1.0);
				r0.y = asuint(0.200000003 * asfloat(r0.w));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r3.www));
				r1.xyz = asuint(asfloat(r1.xyz) * float3(0.00499999989, 0.00499999989, 0.00499999989) + asfloat(r5.xyz));
				r3.xyz = asuint(float3(1.0, 2.46523496e-10, 2.46523496e-10) * asfloat(r6.zxy));
				r3.xyz = asuint(asfloat(r6.yzx) * float3(2.46523496e-10, 2.46523496e-10, 1.0) + -asfloat(r3.xyz));
				r4.xyz = asuint(float3(2.46523496e-10, 2.46523496e-10, 1.0) * asfloat(r3.yzx));
				r4.xyz = asuint(asfloat(r3.zxy) * float3(1.0, 2.46523496e-10, 2.46523496e-10) + -asfloat(r4.xyz));
				r0.x = asuint(dot(asfloat(r2.xzy), float3(2.46523496e-10, 2.46523496e-10, 1.0)));
				r2.xyz = asuint(asfloat(r2.xyz) - asfloat(r0.xxx) * float3(2.46523496e-10, 1.0, 2.46523496e-10));
				r0.x = asuint(max(-1.0, asfloat(r0.x)));
				r0.x = asuint(min(1.0, asfloat(r0.x)));
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.www));
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r4.xyz)));
				r2.x = asuint(dot(asfloat(r2.xyz), asfloat(r3.xyz)));
				r2.y = asuint(max(abs(asfloat(r2.x)), abs(asfloat(r0.w))));
				r2.y = asuint(1.0 / asfloat(r2.y));
				r2.z = asuint(min(abs(asfloat(r2.x)), abs(asfloat(r0.w))));
				r2.y = asuint(asfloat(r2.z) * asfloat(r2.y));
				r2.z = asuint(asfloat(r2.y) * asfloat(r2.y));
				r2.w = asuint(asfloat(r2.z) * 0.0208350997 + -0.0851330012);
				r2.w = asuint(asfloat(r2.z) * asfloat(r2.w) + 0.180141002);
				r2.w = asuint(asfloat(r2.z) * asfloat(r2.w) + -0.330299497);
				r2.z = asuint(asfloat(r2.z) * asfloat(r2.w) + 0.999866009);
				r2.w = asuint(asfloat(r2.y) * asfloat(r2.z));
				r2.w = asuint(asfloat(r2.w) * -2.0 + 1.57079637);
				b3.x = (abs(asfloat(r0.w)) < abs(asfloat(r2.x)));
				r2.w = b3.x ? r2.w : 0u;
				r2.y = asuint(asfloat(r2.y) * asfloat(r2.z) + asfloat(r2.w));
				b2.z = (asfloat(r0.w) < -asfloat(r0.w));
				r2.z = b2.z ? 0xc0490fdbu : 0u;
				r2.y = asuint(asfloat(r2.y) + asfloat(r2.z));
				r2.z = asuint(min(asfloat(r2.x), asfloat(r0.w)));
				r0.w = asuint(max(asfloat(r2.x), asfloat(r0.w)));
				b0.w = (asfloat(r0.w) >= -asfloat(r0.w));
				b2.x = (asfloat(r2.z) < -asfloat(r2.z));
				b0.w = b2.x && b0.w;
				r0.w = asuint(b0.w ? -asfloat(r2.y) : asfloat(r2.y));
				r0.w = asuint(3.14159274 + asfloat(r0.w));
				r0.w = asuint(0.159154937 * asfloat(r0.w));
				r2.x = asuint(abs(asfloat(r0.x)) * -0.0187292993 + 0.0742610022);
				r2.x = asuint(asfloat(r2.x) * abs(asfloat(r0.x)) + -0.212114394);
				r2.x = asuint(asfloat(r2.x) * abs(asfloat(r0.x)) + 1.57072878);
				r2.y = asuint(1.0 + -abs(asfloat(r0.x)));
				b0.x = (asfloat(r0.x) < -asfloat(r0.x));
				r2.y = asuint(sqrt(asfloat(r2.y)));
				r2.z = asuint(asfloat(r2.x) * asfloat(r2.y));
				r2.z = asuint(asfloat(r2.z) * -2.0 + 3.14159274);
				r0.x = b0.x ? r2.z : 0u;
				r0.x = asuint(asfloat(r2.x) * asfloat(r2.y) + asfloat(r0.x));
				r0.x = asuint(1.57072878 + -asfloat(r0.x));
				b2.x = (1.57069635 < abs(asfloat(r0.x)));
				r2.x = b2.x ? 0u : r0.w;
				b0.w = (0.0 < asfloat(r0.x));
				b2.z = (asfloat(r0.x) < 0.0);
				r0.x = asuint(0.636619747 * abs(asfloat(r0.x)));
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r2.z = b2.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				r0.w = asuint(asint(r2.z) + -asint(r0.w));
				r0.w = asuint(float(asint(r0.w)));
				r0.w = asuint(0.5 * asfloat(r0.w));
				r2.y = asuint(asfloat(r0.w) * asfloat(r0.x) + 0.5);
				r2 = asuint(_skyViewLut.Sample(sampler_skyViewLut, asfloat(r2.xy)));
				r3.xyz = asuint(asfloat(r2.xyz) * asfloat(r2.www));
				r0.x = asuint(dot(asfloat(r3.xyz), float3(0.75, 0.75, 0.75)));
				r2.xyz = asuint(asfloat(r0.xxx) - asfloat(r2.xyz) * asfloat(r2.www));
				r0.xyw = asuint(asfloat(r0.yyy) * asfloat(r2.xyz) + asfloat(r3.xyz));
				r2.x = asuint(dot(i_TEXCOORD0.xyz, i_TEXCOORD0.xyz));
				r2.x = asuint(rsqrt(asfloat(r2.x)));
				r2.xyz = asuint(i_TEXCOORD0.xyz * asfloat(r2.xxx));
				r2.x = asuint(dot(asfloat(r2.xzy), float3(2.46523496e-10, 2.46523496e-10, 1.0)));
				r8.x = asuint(saturate(asfloat(r2.x) * 0.5 + 0.5));
				r2 = asuint(_transmittanceLut.Sample(sampler_transmittanceLut, asfloat(r8.xy)));
				r2.xyz = asuint(float3(_Exposure, _Exposure, _Exposure) * asfloat(r2.xyz));
				r3 = asuint(_Tex.Sample(sampler_Tex, i_TEXCOORD5.xyz));
				r2.w = asuint(-1.0 + asfloat(r3.w));
				r2.w = asuint(_Tex_HDR.w * asfloat(r2.w) + 1.0);
				r2.w = asuint(log2(asfloat(r2.w)));
				r2.w = asuint(_Tex_HDR.y * asfloat(r2.w));
				r2.w = asuint(exp2(asfloat(r2.w)));
				r2.w = asuint(_Tex_HDR.x * asfloat(r2.w));
				r3.xyz = asuint(asfloat(r2.www) * asfloat(r3.xyz));
				r2.xyz = asuint(asfloat(r3.xyz) * asfloat(r2.xyz));
				r0.xyw = asuint(asfloat(r2.xyz) * asfloat(r1.www) + asfloat(r0.xyw));
				r1.xyz = asuint(asfloat(r1.xyz) + -asfloat(r0.xyw));
				r0.xyz = asuint(asfloat(r0.zzz) * asfloat(r1.xyz) + asfloat(r0.xyw));
				r0.xyz = asuint(float3(_SunIntensity, _SunIntensity, _SunIntensity) * asfloat(r0.xyz));
				r0.w = asuint(saturate(1.0 + i_TEXCOORD0.y));
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				o_SV_Target0.xyz = asfloat(r0.xyz) * asfloat(r0.www);
				o_SV_Target0.w = 1.0;
			}
			#elif defined(FOG_EXP) || 1
			float4 _GlobalsPad0[2];
			float4 _Tex_HDR;
			float _Exposure;
			float _CloudsDistMul;
			float _SunIntensity;
			float _CloudsFade;
			float _GroundReflection;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[1];
			float _CloudsMorph;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			float4 _GlobalsPad8[1];
			float _CloudsClearLerp01;
			cbuffer UnityLighting : register(b1)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
			}
			cbuffer _AtmosphereGlobals : register(b2)
			{
				float _GlobalOvercast : packoffset(c0);
				float3 _GlobalOvercastTint : packoffset(c0.y);
			}
			Texture2D<float4> _skyViewLut : register(t0);
			Texture2D<float4> _transmittanceLut : register(t1);
			TextureCube<float4> _Tex : register(t2);
			TextureCube<float4> _Clouds2D_A : register(t3);
			TextureCube<float4> _Clouds2D_B : register(t4);
			SamplerState sampler_transmittanceLut : register(s0);
			SamplerState sampler_skyViewLut : register(s1);
			SamplerState sampler_Tex : register(s2);
			SamplerState sampler_Clouds2D_A : register(s3);
			SamplerState sampler_Clouds2D_B : register(s4);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float4 i_TEXCOORD4 : TEXCOORD4, float3 i_TEXCOORD5 : TEXCOORD5, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				uint4 r7;
				uint4 r8;
				uint4 r9;
				bool4 b0;
				bool4 b1;
				bool4 b2;
				bool4 b3;
				bool4 b9;
				r0.y = asuint(abs(i_TEXCOORD2.y));
				r0.xz = asuint(i_TEXCOORD2.xz);
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				r0.w = asuint(saturate(ceil(-i_TEXCOORD0.y)));
				r1 = asuint(_Clouds2D_B.SampleLevel(sampler_Clouds2D_B, asfloat(r0.xyz), asfloat(r0.w)));
				r2 = asuint(_Clouds2D_A.SampleLevel(sampler_Clouds2D_A, asfloat(r0.xyz), asfloat(r0.w)));
				r0.xyz = asuint(float3(1.0, -1.0, 1.0) * asfloat(r0.xyz));
				r1 = asuint(-asfloat(r2) + asfloat(r1));
				r3.x = asuint(saturate(_CloudsMorph));
				r1 = asuint(asfloat(r3.xxxx) * asfloat(r1) + asfloat(r2));
				b2.xyz = (float3(0.0, 0.0, 0.0) < i_TEXCOORD3.zxy);
				r2.x = b2.x ? r1.y : r1.x;
				r4 = asuint(_Clouds2D_B.SampleLevel(sampler_Clouds2D_B, asfloat(r0.xyz), asfloat(r0.w)));
				r0 = asuint(_Clouds2D_A.SampleLevel(sampler_Clouds2D_A, asfloat(r0.xyz), asfloat(r0.w)));
				r3.yzw = asuint(asfloat(r4.xyw) + -asfloat(r0.xyw));
				r0.xyz = asuint(asfloat(r3.xxx) * asfloat(r3.yzw) + asfloat(r0.xyw));
				r0.w = b2.y ? r0.x : r1.z;
				r2.y = b2.z ? r0.y : 0u;
				r3.xyz = asuint(i_TEXCOORD3.zxy * i_TEXCOORD3.zxy);
				r0.w = asuint(asfloat(r3.y) * asfloat(r0.w));
				r0.w = asuint(asfloat(r3.x) * asfloat(r2.x) + asfloat(r0.w));
				r0.w = asuint(asfloat(r3.z) * asfloat(r2.y) + asfloat(r0.w));
				r2.y = asuint(max(0.00200000009, abs(i_TEXCOORD0.y)));
				r2.xz = asuint(i_TEXCOORD0.xz);
				r2.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r2.w = asuint(rsqrt(asfloat(r2.w)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r2.www));
				r4.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.zzz));
				r4.xyz = asuint(asfloat(r4.xyz) * float3(_CloudsDistMul, _CloudsDistMul, _CloudsDistMul) + float3(0.0, 6.36000013, 0.0));
				r2.w = asuint(dot(asfloat(r4.xyz), asfloat(r4.xyz)));
				r2.w = asuint(sqrt(asfloat(r2.w)));
				r3.w = asuint(-6.36000013 + asfloat(r2.w));
				r4.xyz = asuint(asfloat(r4.xyz) / asfloat(r2.www));
				r5.z = asuint(saturate(10.0 * asfloat(r3.w)));
				r2.w = asuint(dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz));
				r2.w = asuint(rsqrt(asfloat(r2.w)));
				r6.xyz = asuint(_WorldSpaceLightPos0.xyz * asfloat(r2.www));
				r2.w = asuint(dot(asfloat(r6.xyz), asfloat(r4.xyz)));
				r3.w = asuint(dot(-asfloat(r6.xyz), asfloat(r4.xyz)));
				r5.x = asuint(saturate(asfloat(r3.w) * 0.5 + 0.5));
				r4 = asuint(_transmittanceLut.Sample(sampler_transmittanceLut, asfloat(r5.xz)));
				r5.y = asuint(saturate(asfloat(r2.w) * 0.5 + 0.5));
				r5 = asuint(_transmittanceLut.Sample(sampler_transmittanceLut, asfloat(r5.yz)));
				r5.xyz = asuint(asfloat(r5.xyz) * asfloat(r0.www));
				r7.xyz = asuint(float3(-1.0, -1.0, -1.0) + _GlobalOvercastTint.xyz);
				r0.w = asuint(saturate(_GlobalOvercast));
				r7.xyz = asuint(asfloat(r0.www) * asfloat(r7.xyz) + float3(1.0, 1.0, 1.0));
				r5.xyz = asuint(asfloat(r7.xyz) * asfloat(r5.xyz));
				r1.w = asuint(log2(asfloat(r1.w)));
				r1.w = asuint(2.20000005 * asfloat(r1.w));
				r1.w = asuint(exp2(asfloat(r1.w)));
				r2.w = asuint(saturate(_CloudsClearLerp01));
				r2.w = asuint(1.0 + -asfloat(r2.w));
				r3.w = asuint(asfloat(r2.w) * asfloat(r1.w));
				r1.w = asuint(1.0 - asfloat(r1.w) * asfloat(r2.w));
				r2.w = asuint(dot(asfloat(r6.xzy), float3(2.46523496e-10, 2.46523496e-10, 1.0)));
				r8.z = asuint(saturate(asfloat(r2.w) * 0.5 + 0.5));
				r8.yw = uint2(0u, 0u);
				r9 = asuint(_transmittanceLut.Sample(sampler_transmittanceLut, asfloat(r8.zw)));
				r9.xyz = asuint(float3(_GroundReflection, _GroundReflection, _GroundReflection) * asfloat(r9.xyz));
				r9.xyz = asuint(asfloat(r9.xyz) * asfloat(r0.yyy));
				r5.xyz = asuint(asfloat(r5.xyz) * asfloat(r3.www) + asfloat(r9.xyz));
				b9.xyz = (float3(0.0, 0.0, 0.0) < -i_TEXCOORD3.zxy);
				r1.x = b9.x ? r1.y : r1.x;
				r0.x = b9.y ? r0.x : r1.z;
				r0.y = b9.z ? r0.y : 0u;
				r0.z = asuint(1.0 - asfloat(r0.z) * _CloudsFade);
				r0.xz = asuint(asfloat(r3.yw) * asfloat(r0.xz));
				r0.x = asuint(asfloat(r3.x) * asfloat(r1.x) + asfloat(r0.x));
				r0.x = asuint(asfloat(r3.z) * asfloat(r0.y) + asfloat(r0.x));
				r1.xyz = asuint(asfloat(r0.xxx) * asfloat(r4.xyz));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r7.xyz));
				r0.x = asuint(asfloat(r0.w) * -0.149999976 + 1.0);
				r0.y = asuint(0.200000003 * asfloat(r0.w));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r3.www));
				r1.xyz = asuint(asfloat(r1.xyz) * float3(0.00499999989, 0.00499999989, 0.00499999989) + asfloat(r5.xyz));
				r0.x = asuint(dot(asfloat(r2.xzy), float3(2.46523496e-10, 2.46523496e-10, 1.0)));
				r2.xyz = asuint(asfloat(r2.xyz) - asfloat(r0.xxx) * float3(2.46523496e-10, 1.0, 2.46523496e-10));
				r0.x = asuint(max(-1.0, asfloat(r0.x)));
				r0.x = asuint(min(1.0, asfloat(r0.x)));
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.www));
				r3.xyz = asuint(float3(1.0, 2.46523496e-10, 2.46523496e-10) * asfloat(r6.zxy));
				r3.xyz = asuint(asfloat(r6.yzx) * float3(2.46523496e-10, 2.46523496e-10, 1.0) + -asfloat(r3.xyz));
				r4.xyz = asuint(float3(2.46523496e-10, 2.46523496e-10, 1.0) * asfloat(r3.yzx));
				r4.xyz = asuint(asfloat(r3.zxy) * float3(1.0, 2.46523496e-10, 2.46523496e-10) + -asfloat(r4.xyz));
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r3.xyz)));
				r2.x = asuint(dot(asfloat(r2.xyz), asfloat(r4.xyz)));
				r2.y = asuint(max(abs(asfloat(r2.x)), abs(asfloat(r0.w))));
				r2.y = asuint(1.0 / asfloat(r2.y));
				r2.z = asuint(min(abs(asfloat(r2.x)), abs(asfloat(r0.w))));
				r2.y = asuint(asfloat(r2.z) * asfloat(r2.y));
				r2.z = asuint(asfloat(r2.y) * asfloat(r2.y));
				r2.w = asuint(asfloat(r2.z) * 0.0208350997 + -0.0851330012);
				r2.w = asuint(asfloat(r2.z) * asfloat(r2.w) + 0.180141002);
				r2.w = asuint(asfloat(r2.z) * asfloat(r2.w) + -0.330299497);
				r2.z = asuint(asfloat(r2.z) * asfloat(r2.w) + 0.999866009);
				r2.w = asuint(asfloat(r2.y) * asfloat(r2.z));
				r2.w = asuint(asfloat(r2.w) * -2.0 + 1.57079637);
				b3.x = (abs(asfloat(r2.x)) < abs(asfloat(r0.w)));
				r2.w = b3.x ? r2.w : 0u;
				r2.y = asuint(asfloat(r2.y) * asfloat(r2.z) + asfloat(r2.w));
				b2.z = (asfloat(r2.x) < -asfloat(r2.x));
				r2.z = b2.z ? 0xc0490fdbu : 0u;
				r2.y = asuint(asfloat(r2.y) + asfloat(r2.z));
				r2.z = asuint(min(asfloat(r2.x), asfloat(r0.w)));
				r0.w = asuint(max(asfloat(r2.x), asfloat(r0.w)));
				b0.w = (asfloat(r0.w) >= -asfloat(r0.w));
				b2.x = (asfloat(r2.z) < -asfloat(r2.z));
				b0.w = b2.x && b0.w;
				r0.w = asuint(b0.w ? -asfloat(r2.y) : asfloat(r2.y));
				r0.w = asuint(3.14159274 + asfloat(r0.w));
				r0.w = asuint(0.159154937 * asfloat(r0.w));
				r2.x = asuint(abs(asfloat(r0.x)) * -0.0187292993 + 0.0742610022);
				r2.x = asuint(asfloat(r2.x) * abs(asfloat(r0.x)) + -0.212114394);
				r2.x = asuint(asfloat(r2.x) * abs(asfloat(r0.x)) + 1.57072878);
				r2.y = asuint(1.0 + -abs(asfloat(r0.x)));
				b0.x = (asfloat(r0.x) < -asfloat(r0.x));
				r2.y = asuint(sqrt(asfloat(r2.y)));
				r2.z = asuint(asfloat(r2.x) * asfloat(r2.y));
				r2.z = asuint(asfloat(r2.z) * -2.0 + 3.14159274);
				r0.x = b0.x ? r2.z : 0u;
				r0.x = asuint(asfloat(r2.x) * asfloat(r2.y) + asfloat(r0.x));
				r0.x = asuint(1.57072878 + -asfloat(r0.x));
				b2.x = (1.57069635 < abs(asfloat(r0.x)));
				r2.x = b2.x ? 0u : r0.w;
				b0.w = (0.0 < asfloat(r0.x));
				b2.z = (asfloat(r0.x) < 0.0);
				r0.x = asuint(0.636619747 * abs(asfloat(r0.x)));
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r2.z = b2.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				r0.w = asuint(asint(r2.z) + -asint(r0.w));
				r0.w = asuint(float(asint(r0.w)));
				r0.w = asuint(0.5 * asfloat(r0.w));
				r2.y = asuint(asfloat(r0.w) * asfloat(r0.x) + 0.5);
				r2 = asuint(_skyViewLut.Sample(sampler_skyViewLut, asfloat(r2.xy)));
				r3.xyz = asuint(asfloat(r2.xyz) * asfloat(r2.www));
				r0.x = asuint(dot(asfloat(r3.xyz), float3(0.75, 0.75, 0.75)));
				r2.xyz = asuint(asfloat(r0.xxx) - asfloat(r2.xyz) * asfloat(r2.www));
				r0.xyw = asuint(asfloat(r0.yyy) * asfloat(r2.xyz) + asfloat(r3.xyz));
				r2 = asuint(_Tex.Sample(sampler_Tex, i_TEXCOORD5.xyz));
				r2.w = asuint(-1.0 + asfloat(r2.w));
				r2.w = asuint(_Tex_HDR.w * asfloat(r2.w) + 1.0);
				r2.w = asuint(log2(asfloat(r2.w)));
				r2.w = asuint(_Tex_HDR.y * asfloat(r2.w));
				r2.w = asuint(exp2(asfloat(r2.w)));
				r2.w = asuint(_Tex_HDR.x * asfloat(r2.w));
				r2.xyz = asuint(asfloat(r2.www) * asfloat(r2.xyz));
				r2.w = asuint(dot(i_TEXCOORD0.xyz, i_TEXCOORD0.xyz));
				r2.w = asuint(rsqrt(asfloat(r2.w)));
				r3.xyz = asuint(i_TEXCOORD0.xyz * asfloat(r2.www));
				r2.w = asuint(dot(asfloat(r3.xzy), float3(2.46523496e-10, 2.46523496e-10, 1.0)));
				r3.x = asuint(dot(asfloat(r3.xyz), asfloat(r6.xyz)));
				r8.x = asuint(saturate(asfloat(r2.w) * 0.5 + 0.5));
				r4 = asuint(_transmittanceLut.Sample(sampler_transmittanceLut, asfloat(r8.xy)));
				r3.yzw = asuint(float3(_Exposure, _Exposure, _Exposure) * asfloat(r4.xyz));
				r2.xyz = asuint(asfloat(r3.yzw) * asfloat(r2.xyz));
				r0.xyw = asuint(asfloat(r2.xyz) * asfloat(r1.www) + asfloat(r0.xyw));
				r1.xyz = asuint(asfloat(r1.xyz) + -asfloat(r0.xyw));
				r0.xyz = asuint(asfloat(r0.zzz) * asfloat(r1.xyz) + asfloat(r0.xyw));
				r0.w = asuint(0.999957204 + -asfloat(r3.x));
				b1.x = (asfloat(r3.x) < 0.999957204);
				r1.y = asuint(asfloat(r0.w) * 300.0 + 0.0199999996);
				r0.w = asuint(-72134.75 * asfloat(r0.w));
				r0.w = asuint(exp2(asfloat(r0.w)));
				r1.y = asuint(1.0 / asfloat(r1.y));
				r1.y = asuint(0.00999999978 * asfloat(r1.y));
				r0.w = asuint(asfloat(r0.w) * 0.5 + asfloat(r1.y));
				r0.w = asuint(-0.999000013 + asfloat(r0.w));
				r0.w = asuint(saturate(1000.01288 * asfloat(r0.w)));
				r0.w = b1.x ? r0.w : 0x3f800000u;
				r1.x = asuint(asfloat(r0.w) * -2.0 + 3.0);
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				r0.w = asuint(asfloat(r1.x) * asfloat(r0.w));
				r0.w = asuint(min(1.0, asfloat(r0.w)));
				r0.w = asuint(1000.0 * asfloat(r0.w));
				r1.xyz = asuint(asfloat(r0.www) * asfloat(r4.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r1.www) + asfloat(r0.xyz));
				r0.xyz = asuint(float3(_SunIntensity, _SunIntensity, _SunIntensity) * asfloat(r0.xyz));
				r0.w = asuint(saturate(1.0 + i_TEXCOORD0.y));
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				o_SV_Target0.xyz = asfloat(r0.xyz) * asfloat(r0.www);
				o_SV_Target0.w = 1.0;
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
