Shader "Skybox/SkyAtmosphere"
{
	Properties
	{
		_Exposure ("Exposure", Float) = 1
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

			#if defined(SHADER_STAGE_VERTEX)
			cbuffer UnityPerDraw : register(b0)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b1)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				o_TEXCOORD0 = i_POSITION0.xyz;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[9];
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float _SA_BottomRadius;
			float _SA_TopRadius;
			float3 _SA_SunDirection;
			float3 _SA_SunIlluminance;
			float _GlobalsPad4;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			float _SA_SunDiskAngularRadius;
			float _SA_SunDiskLuminanceScale;
			float _GlobalsPad8;
			float _GlobalsPad9;
			float _GlobalsPad10;
			float4 _GlobalsPad11[1];
			float4 _SA_SkyViewLutSize;
			float3 _SA_CameraPlanetPos;
			float _SA_CameraVhorizon;
			float _GlobalsPad12;
			float _GlobalsPad13;
			float _GlobalsPad14;
			float _Exposure;
			float _SA_HorizonFillEnabled;
			float _SA_HorizonFillStart;
			float _SA_HorizonFadeAngle;
			float _SA_HorizonFadeDarkening;
			float3 _SA_HorizonFillTint;
			float _SA_CloudsEnabled;
			float _SA_CloudSunIntensity;
			float _SA_CloudAmbientIntensity;
			Texture2D<float4> _SA_TransmittanceLut : register(t0);
			Texture2D<float4> _SA_SkyViewLut : register(t1);
			Texture2D<float4> _SA_CloudHemisphereLut : register(t2);
			SamplerState sampler_SA_TransmittanceLut : register(s0);
			SamplerState sampler_SA_SkyViewLut : register(s1);
			SamplerState sampler_SA_CloudHemisphereLut : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b0;
				bool4 b1;
				bool4 b2;
				bool4 b3;
				bool4 b4;
				r0.x = asuint(dot(i_TEXCOORD0.xyz, i_TEXCOORD0.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r0.xyz = asuint(i_TEXCOORD0.xzy * asfloat(r0.xxx));
				r0.w = asuint(dot(_SA_CameraPlanetPos.xyz, _SA_CameraPlanetPos.xyz));
				r1.x = asuint(sqrt(asfloat(r0.w)));
				b1.y = (asfloat(r1.x) >= _SA_TopRadius);
				[branch] if (b1.y)
				{
					r1.y = asuint(cos(_SA_SunDiskAngularRadius));
					r1.z = asuint(dot(asfloat(r0.xyz), _SA_SunDirection.xyz));
					b1.w = (0.0 >= _SA_SunDiskLuminanceScale);
					b2.x = (asfloat(r1.z) < asfloat(r1.y));
					b1.w = b2.x || b1.w;
					[branch] if (!b1.w)
					{
						r2.xyz = asuint(_SA_CameraPlanetPos.xyz / asfloat(r1.xxx));
						r1.w = asuint(dot(asfloat(r0.xyz), asfloat(r2.xyz)));
						r2.x = asuint(_SA_TopRadius * _SA_TopRadius);
						r2.y = asuint(_SA_BottomRadius * _SA_BottomRadius);
						r2.y = asuint(_SA_TopRadius * _SA_TopRadius + -asfloat(r2.y));
						r2.y = asuint(max(0.0, asfloat(r2.y)));
						r2.z = asuint(asfloat(r0.w) - _SA_BottomRadius * _SA_BottomRadius);
						r2.z = asuint(max(0.0, asfloat(r2.z)));
						r2.w = asuint(asfloat(r1.w) * asfloat(r1.w) + -1.0);
						r2.x = asuint(asfloat(r0.w) * asfloat(r2.w) + asfloat(r2.x));
						r2.xyz = asuint(sqrt(asfloat(r2.xyz)));
						r1.w = asuint(asfloat(r2.x) - asfloat(r1.x) * asfloat(r1.w));
						r1.w = asuint(max(0.0, asfloat(r1.w)));
						r2.x = asuint(_SA_TopRadius + -asfloat(r1.x));
						r2.w = asuint(asfloat(r2.z) + asfloat(r2.y));
						r1.w = asuint(-asfloat(r2.x) + asfloat(r1.w));
						r2.x = asuint(asfloat(r2.w) + -asfloat(r2.x));
						r3.x = asuint(asfloat(r1.w) / asfloat(r2.x));
						r3.y = asuint(asfloat(r2.z) / asfloat(r2.y));
						r2 = asuint(_SA_TransmittanceLut.SampleLevel(sampler_SA_TransmittanceLut, asfloat(r3.xy), 0.0));
						r1.w = asuint(1.0 - asfloat(r1.y) * asfloat(r1.y));
						r1.w = asuint(max(9.99999996e-13, asfloat(r1.w)));
						r2.w = asuint(1.0 + -asfloat(r1.y));
						r2.w = asuint(0.200000003 * asfloat(r2.w));
						r1.y = asuint(asfloat(r1.z) + -asfloat(r1.y));
						r1.z = asuint(1.0 / asfloat(r2.w));
						r1.y = asuint(saturate(asfloat(r1.y) * asfloat(r1.z)));
						r1.z = asuint(asfloat(r1.y) * -2.0 + 3.0);
						r1.y = asuint(asfloat(r1.y) * asfloat(r1.y));
						r1.y = asuint(asfloat(r1.z) * asfloat(r1.y));
						r2.xyz = asuint(_SA_SunIlluminance.xyz * asfloat(r2.xyz));
						r1.y = asuint(_SA_SunDiskLuminanceScale * asfloat(r1.y));
						r1.z = asuint(3.14159274 * asfloat(r1.w));
						r1.y = asuint(asfloat(r1.y) / asfloat(r1.z));
						r1.yzw = asuint(asfloat(r2.xyz) * asfloat(r1.yyy));
					}
					else
					{
						r1.yzw = uint3(0u, 0u, 0u);
					}
					o_SV_Target0.xyz = float3(_Exposure, _Exposure, _Exposure) * asfloat(r1.yzw);
					o_SV_Target0.w = 1.0;
					return;
				}
				r1.yzw = asuint(_SA_CameraPlanetPos.yzx / asfloat(r1.xxx));
				r2.x = asuint(dot(asfloat(r0.yzx), asfloat(r1.yzw)));
				r2.yzw = asuint(asfloat(r1.zwy) * asfloat(r0.yzx));
				r2.yzw = asuint(asfloat(r1.yzw) * asfloat(r0.zxy) + -asfloat(r2.yzw));
				r3.x = asuint(dot(asfloat(r2.yzw), asfloat(r2.yzw)));
				b3.y = (asfloat(r3.x) < 9.99999994e-09);
				r3.x = asuint(rsqrt(asfloat(r3.x)));
				r2.yzw = asuint(asfloat(r3.xxx) * asfloat(r2.yzw));
				r3.xzw = asuint(asfloat(r2.wyz) * asfloat(r1.yzw));
				r1.yzw = asuint(asfloat(r2.zwy) * asfloat(r1.zwy) + -asfloat(r3.xzw));
				r3.x = asuint(dot(asfloat(r1.yzw), asfloat(r1.yzw)));
				r3.x = asuint(rsqrt(asfloat(r3.x)));
				r1.yzw = asuint(asfloat(r3.xxx) * asfloat(r1.yzw));
				r4.x = asuint(dot(_SA_SunDirection.xyz, asfloat(r1.yzw)));
				r4.y = asuint(dot(_SA_SunDirection.xyz, asfloat(r2.yzw)));
				r1.y = asuint(dot(asfloat(r4.xy), asfloat(r4.xy)));
				r1.y = asuint(sqrt(asfloat(r1.y)));
				b1.z = (9.99999997e-07 < asfloat(r1.y));
				r1.y = asuint(asfloat(r4.x) / asfloat(r1.y));
				r1.y = b1.z ? r1.y : 0x3f800000u;
				r1.y = b3.y ? 0x3f800000u : r1.y;
				r1.z = asuint(-_SA_CameraVhorizon / asfloat(r1.x));
				b1.w = (asfloat(r2.x) < asfloat(r1.z));
				r2.y = asuint(max(-1.0, asfloat(r1.z)));
				r2.y = asuint(min(1.0, asfloat(r2.y)));
				r2.z = asuint(1.0 + -abs(asfloat(r2.y)));
				r2.z = asuint(sqrt(asfloat(r2.z)));
				r2.w = asuint(abs(asfloat(r2.y)) * -0.0187292993 + 0.0742610022);
				r2.w = asuint(asfloat(r2.w) * abs(asfloat(r2.y)) + -0.212114394);
				r2.w = asuint(asfloat(r2.w) * abs(asfloat(r2.y)) + 1.57072878);
				r3.x = asuint(asfloat(r2.w) * asfloat(r2.z));
				r3.x = asuint(asfloat(r3.x) * -2.0 + 3.14159274);
				b2.y = (asfloat(r2.y) < -asfloat(r2.y));
				r2.y = b2.y ? r3.x : 0u;
				r2.y = asuint(asfloat(r2.w) * asfloat(r2.z) + asfloat(r2.y));
				r2.y = asuint(-_SA_HorizonFillStart + asfloat(r2.y));
				r2.y = asuint(max(0.00100000005, asfloat(r2.y)));
				r2.z = asuint(cos(asfloat(r2.y)));
				b2.w = (0.5 < _SA_HorizonFillEnabled);
				b3.x = (asfloat(r2.x) < asfloat(r2.z));
				b2.w = b3.x && b2.w;
				r1.w = b1.w ? 0xffffffffu : 0u;
				[branch] if (b2.w)
				{
					b1.z = (asfloat(r2.z) < asfloat(r1.z));
					r2.w = asuint(_SA_CameraVhorizon / asfloat(r1.x));
					r3.x = asuint(1.0 + -abs(asfloat(r2.w)));
					r3.x = asuint(sqrt(asfloat(r3.x)));
					r3.y = asuint(abs(asfloat(r2.w)) * -0.0187292993 + 0.0742610022);
					r3.y = asuint(asfloat(r3.y) * abs(asfloat(r2.w)) + -0.212114394);
					r3.y = asuint(asfloat(r3.y) * abs(asfloat(r2.w)) + 1.57072878);
					r3.z = asuint(asfloat(r3.y) * asfloat(r3.x));
					r3.z = asuint(asfloat(r3.z) * -2.0 + 3.14159274);
					b2.w = (asfloat(r2.w) < -asfloat(r2.w));
					r2.w = b2.w ? r3.z : 0u;
					r2.w = asuint(asfloat(r3.y) * asfloat(r3.x) + asfloat(r2.w));
					r3.x = asuint(3.14159274 + -asfloat(r2.w));
					r3.y = asuint(1.0 + -abs(asfloat(r2.z)));
					r3.y = asuint(sqrt(asfloat(r3.y)));
					r3.z = asuint(abs(asfloat(r2.z)) * -0.0187292993 + 0.0742610022);
					r3.z = asuint(asfloat(r3.z) * abs(asfloat(r2.z)) + -0.212114394);
					r3.z = asuint(asfloat(r3.z) * abs(asfloat(r2.z)) + 1.57072878);
					r3.w = asuint(asfloat(r3.z) * asfloat(r3.y));
					r3.w = asuint(asfloat(r3.w) * -2.0 + 3.14159274);
					b2.z = (asfloat(r2.z) < -asfloat(r2.z));
					r2.z = b2.z ? r3.w : 0u;
					r2.z = asuint(asfloat(r3.z) * asfloat(r3.y) + asfloat(r2.z));
					r3.y = asuint(asfloat(r2.z) / asfloat(r3.x));
					r3.y = asuint(1.0 + -asfloat(r3.y));
					r3.y = asuint(sqrt(abs(asfloat(r3.y))));
					r3.y = asuint(1.0 + -asfloat(r3.y));
					r3.y = asuint(0.5 * asfloat(r3.y));
					r2.z = asuint(asfloat(r2.z) + -asfloat(r3.x));
					r2.z = asuint(asfloat(r2.z) / asfloat(r2.w));
					r2.z = asuint(sqrt(abs(asfloat(r2.z))));
					r2.z = asuint(asfloat(r2.z) * 0.5 + 0.5);
					r2.z = b1.z ? r2.z : r3.y;
					r2.w = asuint(asfloat(r1.y) * -0.5 + 0.5);
					r2.w = asuint(sqrt(asfloat(r2.w)));
					r3.xy = asuint(float2(0.5, 0.5) / _SA_SkyViewLutSize.xy);
					r2.zw = asuint(asfloat(r3.yx) + asfloat(r2.zw));
					r3.xz = asuint(float2(1.0, 1.0) + _SA_SkyViewLutSize.xy);
					r3.xz = asuint(_SA_SkyViewLutSize.xy / asfloat(r3.xz));
					r4.x = asuint(asfloat(r3.x) * asfloat(r2.w));
					r2.z = asuint(asfloat(r3.z) * asfloat(r2.z));
					r2.w = asuint(0.5 - _SA_SkyViewLutSize.w * 1.5);
					r2.w = asuint(min(asfloat(r2.z), asfloat(r2.w)));
					r4.y = b1.z ? r2.z : r2.w;
					r3 = asuint(_SA_SkyViewLut.SampleLevel(sampler_SA_SkyViewLut, asfloat(r4.xy), 0.0));
					r1.z = asuint(max(-1.0, asfloat(r2.x)));
					r1.z = asuint(min(1.0, asfloat(r1.z)));
					r2.z = asuint(1.0 + -abs(asfloat(r1.z)));
					r2.z = asuint(sqrt(asfloat(r2.z)));
					r2.w = asuint(abs(asfloat(r1.z)) * -0.0187292993 + 0.0742610022);
					r2.w = asuint(asfloat(r2.w) * abs(asfloat(r1.z)) + -0.212114394);
					r2.w = asuint(asfloat(r2.w) * abs(asfloat(r1.z)) + 1.57072878);
					r3.w = asuint(asfloat(r2.w) * asfloat(r2.z));
					r3.w = asuint(asfloat(r3.w) * -2.0 + 3.14159274);
					b1.z = (asfloat(r1.z) < -asfloat(r1.z));
					r1.z = b1.z ? r3.w : 0u;
					r1.z = asuint(asfloat(r2.w) * asfloat(r2.z) + asfloat(r1.z));
					r1.z = asuint(asfloat(r1.z) + -asfloat(r2.y));
					r2.y = asuint(max(9.99999975e-05, _SA_HorizonFadeAngle));
					r1.z = asuint(saturate(asfloat(r1.z) / asfloat(r2.y)));
					r2.y = asuint(asfloat(r1.z) * -2.0 + 3.0);
					r1.z = asuint(asfloat(r1.z) * asfloat(r1.z));
					r1.z = asuint(asfloat(r2.y) * asfloat(r1.z));
					r2.yzw = asuint(_SA_HorizonFillTint.xyz * asfloat(r3.xyz));
					r3.x = asuint(-1.0 + _SA_HorizonFadeDarkening);
					r1.z = asuint(asfloat(r1.z) * asfloat(r3.x) + 1.0);
					r3.xyz = asuint(asfloat(r2.yzw) * asfloat(r1.zzz));
				}
				else
				{
					r1.z = asuint(_SA_CameraVhorizon / asfloat(r1.x));
					r2.y = asuint(1.0 + -abs(asfloat(r1.z)));
					r2.y = asuint(sqrt(asfloat(r2.y)));
					r2.z = asuint(abs(asfloat(r1.z)) * -0.0187292993 + 0.0742610022);
					r2.z = asuint(asfloat(r2.z) * abs(asfloat(r1.z)) + -0.212114394);
					r2.z = asuint(asfloat(r2.z) * abs(asfloat(r1.z)) + 1.57072878);
					r2.w = asuint(asfloat(r2.z) * asfloat(r2.y));
					r2.w = asuint(asfloat(r2.w) * -2.0 + 3.14159274);
					b1.z = (asfloat(r1.z) < -asfloat(r1.z));
					r1.z = b1.z ? r2.w : 0u;
					r1.z = asuint(asfloat(r2.z) * asfloat(r2.y) + asfloat(r1.z));
					r2.y = asuint(3.14159274 + -asfloat(r1.z));
					r2.z = asuint(1.0 + -abs(asfloat(r2.x)));
					r2.z = asuint(sqrt(asfloat(r2.z)));
					r2.w = asuint(abs(asfloat(r2.x)) * -0.0187292993 + 0.0742610022);
					r2.w = asuint(asfloat(r2.w) * abs(asfloat(r2.x)) + -0.212114394);
					r2.w = asuint(asfloat(r2.w) * abs(asfloat(r2.x)) + 1.57072878);
					r4.x = asuint(asfloat(r2.w) * asfloat(r2.z));
					r4.x = asuint(asfloat(r4.x) * -2.0 + 3.14159274);
					b4.y = (asfloat(r2.x) < -asfloat(r2.x));
					r4.x = b4.y ? r4.x : 0u;
					r2.z = asuint(asfloat(r2.w) * asfloat(r2.z) + asfloat(r4.x));
					r2.w = asuint(asfloat(r2.z) / asfloat(r2.y));
					r2.w = asuint(1.0 + -asfloat(r2.w));
					r2.w = asuint(sqrt(abs(asfloat(r2.w))));
					r2.w = asuint(1.0 + -asfloat(r2.w));
					r2.w = asuint(0.5 * asfloat(r2.w));
					r2.y = asuint(asfloat(r2.z) + -asfloat(r2.y));
					r1.z = asuint(asfloat(r2.y) / asfloat(r1.z));
					r1.z = asuint(sqrt(abs(asfloat(r1.z))));
					r1.z = asuint(asfloat(r1.z) * 0.5 + 0.5);
					r1.z = (r1.w != 0) ? r1.z : r2.w;
					r1.y = asuint(asfloat(r1.y) * -0.5 + 0.5);
					r1.y = asuint(sqrt(asfloat(r1.y)));
					r2.yz = asuint(float2(0.5, 0.5) / _SA_SkyViewLutSize.xy);
					r1.y = asuint(asfloat(r2.y) + asfloat(r1.y));
					r2.yw = asuint(float2(1.0, 1.0) + _SA_SkyViewLutSize.xy);
					r2.yw = asuint(_SA_SkyViewLutSize.xy / asfloat(r2.yw));
					r4.x = asuint(asfloat(r2.y) * asfloat(r1.y));
					r1.y = asuint(asfloat(r2.z) + asfloat(r1.z));
					r4.y = asuint(asfloat(r1.y) * asfloat(r2.w));
					r3 = asuint(_SA_SkyViewLut.SampleLevel(sampler_SA_SkyViewLut, asfloat(r4.xy), 0.0));
				}
				b1.y = (0.5 < _SA_CloudsEnabled);
				b1.z = (asfloat(r0.z) >= 0.00499999989);
				b1.y = b1.y && b1.z;
				[branch] if (b1.y)
				{
					r1.y = asuint(saturate(asfloat(r0.z)));
					r1.z = asuint(1.0 + -asfloat(r1.y));
					r1.z = asuint(sqrt(asfloat(r1.z)));
					r2.y = asuint(asfloat(r1.y) * -0.0187292993 + 0.0742610022);
					r2.y = asuint(asfloat(r2.y) * asfloat(r1.y) + -0.212114394);
					r1.y = asuint(asfloat(r2.y) * asfloat(r1.y) + 1.57072878);
					r1.y = asuint(asfloat(r1.y) * asfloat(r1.z));
					r1.z = asuint(min(abs(asfloat(r0.y)), abs(asfloat(r0.x))));
					r2.y = asuint(max(abs(asfloat(r0.y)), abs(asfloat(r0.x))));
					r2.y = asuint(1.0 / asfloat(r2.y));
					r1.z = asuint(asfloat(r2.y) * asfloat(r1.z));
					r2.y = asuint(asfloat(r1.z) * asfloat(r1.z));
					r2.z = asuint(asfloat(r2.y) * 0.0208350997 + -0.0851330012);
					r2.z = asuint(asfloat(r2.y) * asfloat(r2.z) + 0.180141002);
					r2.z = asuint(asfloat(r2.y) * asfloat(r2.z) + -0.330299497);
					r2.y = asuint(asfloat(r2.y) * asfloat(r2.z) + 0.999866009);
					r2.z = asuint(asfloat(r2.y) * asfloat(r1.z));
					b2.w = (abs(asfloat(r0.x)) < abs(asfloat(r0.y)));
					r2.z = asuint(asfloat(r2.z) * -2.0 + 1.57079637);
					r2.z = b2.w ? r2.z : 0u;
					r1.z = asuint(asfloat(r1.z) * asfloat(r2.y) + asfloat(r2.z));
					b2.y = (asfloat(r0.x) < -asfloat(r0.x));
					r2.y = b2.y ? 0xc0490fdbu : 0u;
					r1.z = asuint(asfloat(r2.y) + asfloat(r1.z));
					r2.y = asuint(min(asfloat(r0.y), asfloat(r0.x)));
					r2.z = asuint(max(asfloat(r0.y), asfloat(r0.x)));
					b2.y = (asfloat(r2.y) < -asfloat(r2.y));
					b2.z = (asfloat(r2.z) >= -asfloat(r2.z));
					b2.y = b2.y && b2.z;
					r1.z = asuint(b2.y ? -asfloat(r1.z) : asfloat(r1.z));
					r2.y = asuint(7.06858349 + asfloat(r1.z));
					r2.y = asuint(0.636619747 * asfloat(r2.y));
					b2.z = (asfloat(r2.y) >= -asfloat(r2.y));
					r2.y = asuint(frac(asfloat(r2.y)));
					r2.y = asuint(b2.z ? asfloat(r2.y) : -asfloat(r2.y));
					r2.y = asuint(asfloat(r2.y) * 1.57079637 + -0.785398185);
					r2.y = asuint(cos(asfloat(r2.y)));
					r2.y = asuint(0.5 / asfloat(r2.y));
					r1.y = asuint(0.636619747 * asfloat(r1.y));
					r1.y = asuint(asfloat(r1.y) * asfloat(r1.y));
					r1.y = asuint(asfloat(r1.y) * asfloat(r2.y));
					float sin0;
					float cos0;
					sincos(asfloat(r1.z), sin0, cos0);
					r4.x = asuint(sin0);
					r5.x = asuint(cos0);
					r5.x = asuint(asfloat(r5.x) * asfloat(r1.y) + 0.5);
					r5.y = asuint(asfloat(r4.x) * asfloat(r1.y) + 0.5);
					r4 = asuint(_SA_CloudHemisphereLut.SampleLevel(sampler_SA_CloudHemisphereLut, asfloat(r5.xy), 0.0));
					r1.y = asuint(1.0 + -asfloat(r4.x));
					r1.z = asuint(1000.0 * asfloat(r4.y));
					r2.yzw = asuint(asfloat(r0.xyz) * asfloat(r1.zzz) + _SA_CameraPlanetPos.xyz);
					r1.z = asuint(dot(asfloat(r2.yzw), asfloat(r2.yzw)));
					r3.w = asuint(sqrt(asfloat(r1.z)));
					r2.yzw = asuint(asfloat(r2.yzw) / asfloat(r3.www));
					r2.y = asuint(dot(_SA_SunDirection.xyz, asfloat(r2.yzw)));
					r2.z = asuint(_SA_TopRadius * _SA_TopRadius);
					r2.w = asuint(_SA_BottomRadius * _SA_BottomRadius);
					r2.w = asuint(_SA_TopRadius * _SA_TopRadius + -asfloat(r2.w));
					r2.w = asuint(max(0.0, asfloat(r2.w)));
					r2.w = asuint(sqrt(asfloat(r2.w)));
					r4.x = asuint(asfloat(r1.z) - _SA_BottomRadius * _SA_BottomRadius);
					r4.x = asuint(max(0.0, asfloat(r4.x)));
					r4.x = asuint(sqrt(asfloat(r4.x)));
					r4.y = asuint(asfloat(r2.y) * asfloat(r2.y) + -1.0);
					r1.z = asuint(asfloat(r1.z) * asfloat(r4.y) + asfloat(r2.z));
					r1.z = asuint(sqrt(asfloat(r1.z)));
					r1.z = asuint(asfloat(r1.z) - asfloat(r3.w) * asfloat(r2.y));
					r1.z = asuint(max(0.0, asfloat(r1.z)));
					r2.z = asuint(_SA_TopRadius + -asfloat(r3.w));
					r4.y = asuint(asfloat(r4.x) + asfloat(r2.w));
					r1.z = asuint(-asfloat(r2.z) + asfloat(r1.z));
					r2.z = asuint(asfloat(r4.y) + -asfloat(r2.z));
					r5.x = asuint(asfloat(r1.z) / asfloat(r2.z));
					r5.y = asuint(asfloat(r4.x) / asfloat(r2.w));
					r5 = asuint(_SA_TransmittanceLut.SampleLevel(sampler_SA_TransmittanceLut, asfloat(r5.xy), 0.0));
					r1.z = asuint(-asfloat(r4.x) / asfloat(r3.w));
					r1.z = asuint(asfloat(r2.y) + -asfloat(r1.z));
					r2.y = asuint(max(9.99999975e-05, _SA_SunDiskAngularRadius));
					r2.y = asuint(asfloat(r2.y) + asfloat(r2.y));
					r1.z = asuint(asfloat(r1.z) / asfloat(r2.y));
					r1.z = asuint(saturate(0.5 + asfloat(r1.z)));
					r2.yzw = asuint(asfloat(r5.xyz) * asfloat(r1.zzz));
					r2.yzw = asuint(_SA_SunIlluminance.xyz * asfloat(r2.yzw));
					r2.yzw = asuint(float3(_SA_CloudSunIntensity, _SA_CloudSunIntensity, _SA_CloudSunIntensity) * asfloat(r2.yzw));
					r5.xyz = asuint(float3(_SA_CloudAmbientIntensity, _SA_CloudAmbientIntensity, _SA_CloudAmbientIntensity) * asfloat(r3.xyz));
					r2.yzw = asuint(asfloat(r2.yzw) * asfloat(r4.zzz));
					r2.yzw = asuint(asfloat(r3.xyz) * asfloat(r1.yyy) + asfloat(r2.yzw));
					r3.xyz = asuint(asfloat(r5.xyz) * asfloat(r4.www) + asfloat(r2.yzw));
				}
				else
				{
					r1.y = 0x3f800000u;
				}
				[branch] if (r1.w == 0)
				{
					r1.z = asuint(cos(_SA_SunDiskAngularRadius));
					r0.x = asuint(dot(asfloat(r0.xyz), _SA_SunDirection.xyz));
					b0.y = (0.0 >= _SA_SunDiskLuminanceScale);
					b0.z = (asfloat(r0.x) < asfloat(r1.z));
					b0.y = b0.y || b0.z;
					[branch] if (!b0.y)
					{
						r0.y = asuint(_SA_TopRadius * _SA_TopRadius);
						r0.z = asuint(_SA_BottomRadius * _SA_BottomRadius);
						r0.z = asuint(_SA_TopRadius * _SA_TopRadius + -asfloat(r0.z));
						r0.z = asuint(max(0.0, asfloat(r0.z)));
						r1.w = asuint(asfloat(r0.w) - _SA_BottomRadius * _SA_BottomRadius);
						r1.w = asuint(max(0.0, asfloat(r1.w)));
						r1.w = asuint(sqrt(asfloat(r1.w)));
						r2.y = asuint(asfloat(r2.x) * asfloat(r2.x) + -1.0);
						r0.y = asuint(asfloat(r0.w) * asfloat(r2.y) + asfloat(r0.y));
						r0.yz = asuint(sqrt(asfloat(r0.yz)));
						r0.y = asuint(asfloat(r0.y) - asfloat(r1.x) * asfloat(r2.x));
						r0.y = asuint(max(0.0, asfloat(r0.y)));
						r0.w = asuint(_SA_TopRadius + -asfloat(r1.x));
						r1.x = asuint(asfloat(r1.w) + asfloat(r0.z));
						r0.y = asuint(asfloat(r0.y) + -asfloat(r0.w));
						r0.w = asuint(asfloat(r1.x) + -asfloat(r0.w));
						r2.x = asuint(asfloat(r0.y) / asfloat(r0.w));
						r2.y = asuint(asfloat(r1.w) / asfloat(r0.z));
						r2 = asuint(_SA_TransmittanceLut.SampleLevel(sampler_SA_TransmittanceLut, asfloat(r2.xy), 0.0));
						r0.y = asuint(1.0 - asfloat(r1.z) * asfloat(r1.z));
						r0.y = asuint(max(9.99999996e-13, asfloat(r0.y)));
						r0.z = asuint(1.0 + -asfloat(r1.z));
						r0.yz = asuint(float2(3.14159274, 0.200000003) * asfloat(r0.yz));
						r0.x = asuint(asfloat(r0.x) + -asfloat(r1.z));
						r0.z = asuint(1.0 / asfloat(r0.z));
						r0.x = asuint(saturate(asfloat(r0.x) * asfloat(r0.z)));
						r0.z = asuint(asfloat(r0.x) * -2.0 + 3.0);
						r0.x = asuint(asfloat(r0.x) * asfloat(r0.x));
						r0.x = asuint(asfloat(r0.z) * asfloat(r0.x));
						r1.xzw = asuint(_SA_SunIlluminance.xyz * asfloat(r2.xyz));
						r0.x = asuint(_SA_SunDiskLuminanceScale * asfloat(r0.x));
						r0.x = asuint(asfloat(r0.x) / asfloat(r0.y));
						r0.xyz = asuint(asfloat(r1.xzw) * asfloat(r0.xxx));
					}
					else
					{
						r0.xyz = uint3(0u, 0u, 0u);
					}
					r3.xyz = asuint(asfloat(r0.xyz) * asfloat(r1.yyy) + asfloat(r3.xyz));
				}
				o_SV_Target0.xyz = float3(_Exposure, _Exposure, _Exposure) * asfloat(r3.xyz);
				o_SV_Target0.w = 1.0;
			}
			#endif
			ENDHLSL
		}
	}
}
