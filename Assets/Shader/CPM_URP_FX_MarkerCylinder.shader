Shader "CPM/URP/FX/MarkerCylinder"
{
	Properties
	{
		_Color ("Color", Color) = (0,0,0,0)
		_Fresnel ("Fresnel", Float) = 1.78
		[NoScaleOffset] _Icon ("Icon", 2D) = "black" {}
		_IconSize ("IconSize", Float) = 0.14
		_ColorPower ("ColorPower", Float) = 1
		_DistanceFade ("DistanceFade", Float) = -35
		_DistanceTransition ("DistanceTransition", Float) = 0.01
		_FarDistanceFade ("FarDistanceFade", Float) = -250
		_FarDistanceTransition ("FarDistanceTransition", Float) = -0.01
		_IconSizeDistance ("IconSizeDistance", Float) = -35
		_IconSizeFade ("IconSizeFade", Float) = 0.01
		_IconSizeFarTiling ("IconSizeFarTiling", Float) = 0.5
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	SubShader
	{
		Tags { "DisableBatching" = "False" "QUEUE" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "ShaderGraphShader" = "true" "ShaderGraphTargetId" = "UniversalUnlitSubTarget" "UniversalMaterialType" = "Unlit" }
		Pass
		{
			Name "Universal Forward"
			Blend SrcAlpha One, One One
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DIRLIGHTMAP_COMBINED
			#pragma multi_compile __ FOG_EXP
			#pragma multi_compile __ LIGHTMAP_ON

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[20];
			float3 _WorldSpaceCameraPos;
			float _GlobalsPad1;
			float4 _GlobalsPad2[55];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float3 o_INTERP1 : INTERP1, out float2 o_INTERP2 : INTERP2)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_INTERP0.xyz = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + -_WorldSpaceCameraPos.xyz);
				r0.x = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				o_INTERP0.w = sqrt(asfloat(r0.x));
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP1 = asfloat(r0.xyz) * asfloat(r0.www);
				r0.xyz = asuint(unity_ObjectToWorld._m13_m13_m13 * unity_MatrixVP._m01_m11_m31);
				r0.xyz = asuint(unity_MatrixVP._m00_m10_m30 * unity_ObjectToWorld._m03_m03_m03 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixVP._m02_m12_m32 * unity_ObjectToWorld._m23_m23_m23 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixVP._m03_m13_m33 + asfloat(r0.xyz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				o_INTERP2 = asfloat(r0.xy) * float2(0.5, -0.5) + float2(0.5, 0.5);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[4];
			float4 _ScaledScreenParams;
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[13];
			float4 _TimeParameters;
			float3 _WorldSpaceCameraPos;
			float _GlobalsPad4;
			float4 _ProjectionParams;
			float4 _ScreenParams;
			float4 _ZBufferParams;
			float4 unity_OrthoParams;
			float4 _GlobalsPad5[39];
			float4x4 unity_MatrixV;
			float4 _GlobalsPad6[12];
			float4x4 unity_MatrixInvVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _DistanceFade : packoffset(c0);
				float _DistanceTransition : packoffset(c0.y);
				float4 _Color : packoffset(c1);
				float _Fresnel : packoffset(c2);
				float _IconSize : packoffset(c4);
				float _IconSizeFade : packoffset(c4.y);
				float _IconSizeDistance : packoffset(c4.z);
				float _FarDistanceFade : packoffset(c4.w);
				float _FarDistanceTransition : packoffset(c5);
				float _IconSizeFarTiling : packoffset(c5.y);
				float _ColorPower : packoffset(c5.z);
			}
			Texture2D<float4> _CameraDepthTexture : register(t0);
			Texture2D<float4> _Icon : register(t1);
			SamplerState sampler_CameraDepthTexture : register(s0);
			SamplerState sampler_Icon : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float3 i_INTERP1 : INTERP1, float2 i_INTERP2 : INTERP2, nointerpolation bool i_SV_IsFrontFace0 : SV_IsFrontFace, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				bool4 b0;
				bool4 b1;
				bool4 b2;
				r0.x = asuint(dot(i_INTERP1.xyz, i_INTERP1.xyz));
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.xyz = asuint(i_INTERP1.xyz * asfloat(r0.xxx));
				b1.xy = (float2(0.0, 1.0) == unity_OrthoParams.ww);
				r2.xyz = asuint(_WorldSpaceCameraPos.xyz + -i_INTERP0.xyz);
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.www));
				r3.x = asuint(unity_MatrixV._m20);
				r3.y = asuint(unity_MatrixV._m21);
				r3.z = asuint(unity_MatrixV._m22);
				r1.xzw = b1.xxx ? r2.xyz : r3.xyz;
				r0.w = asuint(unity_WorldToObject._m11 * i_INTERP0.y);
				r0.w = asuint(unity_WorldToObject._m10 * i_INTERP0.x + asfloat(r0.w));
				r0.w = asuint(unity_WorldToObject._m12 * i_INTERP0.z + asfloat(r0.w));
				r0.w = asuint(unity_WorldToObject._m13 + asfloat(r0.w));
				r2.x = asuint(unity_MatrixV._m21 * i_INTERP0.y);
				r2.x = asuint(unity_MatrixV._m20 * i_INTERP0.x + asfloat(r2.x));
				r2.x = asuint(unity_MatrixV._m22 * i_INTERP0.z + asfloat(r2.x));
				r2.x = asuint(unity_MatrixV._m23 + asfloat(r2.x));
				b2.y = (_ProjectionParams.x < 0.0);
				r2.z = asuint(_ScaledScreenParams.y + -i_SV_POSITION0.y);
				r3.y = b2.y ? r2.z : asuint(i_SV_POSITION0.y);
				r3.x = asuint(i_SV_POSITION0.x);
				r3.xy = asuint(asfloat(r3.xy) / _ScaledScreenParams.xy);
				r2.yzw = asuint(float3(_ColorPower, _ColorPower, _ColorPower) * _Color.xyz);
				r4.x = asuint(_DistanceFade + i_INTERP0.w);
				r4.x = asuint(_DistanceTransition * asfloat(r4.x));
				r4.yz = asuint(float2(_FarDistanceFade, _IconSizeDistance) + i_INTERP0.ww);
				r4.y = asuint(_FarDistanceTransition * asfloat(r4.y));
				r4.x = asuint(saturate(asfloat(r4.x) * asfloat(r4.y)));
				r0.w = asuint(0.909999967 + -asfloat(r0.w));
				r0.w = asuint(saturate(0.806451619 * asfloat(r0.w)));
				r4.y = asuint(asfloat(r0.w) * -2.0 + 3.0);
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				r0.w = asuint(asfloat(r4.y) * asfloat(r0.w));
				[branch] if (b1.y)
				{
					r3.z = asuint(1.0 + -asfloat(r3.y));
					r5 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r3.xz), _GlobalMipBias.x));
					r4.yw = asuint(asfloat(r3.xz) * float2(2.0, 2.0) + float2(-1.0, -1.0));
					r6 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r4.wwww));
					r6 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r4.yyyy) + asfloat(r6));
					r5 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r5.xxxx) + asfloat(r6));
					r5 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r5));
					r5.xyz = asuint(asfloat(r5.xyz) / asfloat(r5.www));
					r1.y = asuint(unity_MatrixV._m21 * asfloat(r5.y));
					r1.y = asuint(unity_MatrixV._m20 * asfloat(r5.x) + asfloat(r1.y));
					r1.y = asuint(unity_MatrixV._m22 * asfloat(r5.z) + asfloat(r1.y));
					r1.y = asuint(unity_MatrixV._m23 + asfloat(r1.y));
					r1.y = asuint(abs(asfloat(r1.y)));
				}
				else
				{
					r3.w = asuint(1.0 + -asfloat(r3.y));
					r5 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r3.xw), _GlobalMipBias.x));
					r3.z = asuint(_ZBufferParams.z * asfloat(r5.x) + _ZBufferParams.w);
					r1.y = asuint(1.0 / asfloat(r3.z));
				}
				r1.y = asuint(asfloat(r2.x) + asfloat(r1.y));
				r1.y = asuint(0.5 * abs(asfloat(r1.y)));
				r1.y = asuint(min(1.0, asfloat(r1.y)));
				r0.xyz = asuint((asuint(float3(asfloat((i_SV_IsFrontFace0 ? 0xffffffffu : 0u)), asfloat((i_SV_IsFrontFace0 ? 0xffffffffu : 0u)), asfloat((i_SV_IsFrontFace0 ? 0xffffffffu : 0u)))) != 0) ? asfloat(r0.xyz) : -asfloat(r0.xyz));
				r2.x = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r2.x = asuint(rsqrt(asfloat(r2.x)));
				r0.xyz = asuint(asfloat(r2.xxx) * asfloat(r0.xyz));
				r2.x = asuint(dot(asfloat(r1.xzw), asfloat(r1.xzw)));
				r2.x = asuint(rsqrt(asfloat(r2.x)));
				r1.xzw = asuint(asfloat(r2.xxx) * asfloat(r1.xzw));
				r0.x = asuint(saturate(dot(asfloat(r0.xyz), asfloat(r1.xzw))));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0.x = asuint(log2(asfloat(r0.x)));
				r0.x = asuint(_Fresnel * asfloat(r0.x));
				r0.x = asuint(exp2(asfloat(r0.x)));
				r0.x = asuint(asfloat(r1.y) * asfloat(r0.x));
				r0.yz = asuint(asfloat(r3.xy) * float2(1.0, -1.0) + float2(0.0, 1.0));
				r1.xy = asuint(-i_INTERP2.xy + asfloat(r0.yz));
				r0.y = asuint(_ScreenParams.y / _ScreenParams.x);
				r1.z = asuint(asfloat(r1.y) * asfloat(r0.y));
				r0.yz = asuint(i_INTERP0.ww * asfloat(r1.xz));
				r1.x = asuint(saturate(_IconSizeFade * asfloat(r4.z)));
				r1.y = asuint(_IconSize * _IconSizeFarTiling + -_IconSize);
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y) + _IconSize);
				r1.xy = asuint(asfloat(r1.xx) * asfloat(r0.yz));
				r1.z = asuint(_TimeParameters.y * 0.0599999726 + asfloat(r1.y));
				r0.yz = asuint(float2(0.5, 0.469999999) + asfloat(r1.xz));
				r1 = asuint(_Icon.SampleBias(sampler_Icon, asfloat(r0.yz), _GlobalMipBias.x));
				b0.y = (asfloat(r1.x) >= 0.5);
				r0.y = b0.y ? 0x3f800000u : 0u;
				r0.x = asuint(asfloat(r0.w) * asfloat(r0.x) + asfloat(r0.y));
				r0.x = asuint(asfloat(r4.x) * asfloat(r0.x));
				o_SV_Target0.xyz = asfloat(r2.yzw) * asfloat(r0.xxx);
				o_SV_Target0.w = 1.0;
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "DepthNormalsOnly"
			Tags { "LIGHTMODE" = "DepthNormalsOnly" }
			Cull Off
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_INTERP0 : INTERP0)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0);
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP0 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_INTERP0 : INTERP0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.x = asuint(dot(i_INTERP0.xyz, i_INTERP0.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				o_SV_Target0.xyz = i_INTERP0.xyz * asfloat(r0.xxx);
				o_SV_Target0.w = 0.0;
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}
