Shader "CPM/URP/FX/VerticalBillboardAdditive"
{
	Properties
	{
		[MainTexture] _BaseMap ("BaseMap", 2D) = "white" {}
		[MainColor] _Color ("Color", Color) = (1,1,1,0)
		_FadeDistance ("FadeDistance", Float) = 1
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
			Cull Back
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DIRLIGHTMAP_COMBINED
			#pragma multi_compile __ FOG_EXP
			#pragma multi_compile __ LIGHTMAP_ON

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[21];
			float4 _ProjectionParams;
			float4 _GlobalsPad1[42];
			float4x4 unity_MatrixV;
			float4x4 unity_MatrixInvV;
			float4 _GlobalsPad2[4];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _FadeDistance : packoffset(c3);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1, out float3 o_INTERP2 : INTERP2)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b1;
				r0.xyz = asuint(unity_WorldToObject._m01_m11_m21 * unity_MatrixInvV._m12_m12_m12);
				r0.xyz = asuint(unity_WorldToObject._m00_m10_m20 * unity_MatrixInvV._m02_m02_m02 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * unity_MatrixInvV._m22_m22_m22 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 * unity_MatrixInvV._m32_m32_m32 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r1.xyz = asuint(unity_WorldToObject._m01_m11_m21 * -asfloat(r0.yyy));
				r0.xyw = asuint(asfloat(r1.xyz) - unity_WorldToObject._m00_m10_m20 * asfloat(r0.xxx));
				r0.xyz = asuint(asfloat(r0.xyw) - unity_WorldToObject._m02_m12_m22 * asfloat(r0.zzz));
				r0.y = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.y = asuint(rsqrt(asfloat(r0.y)));
				r0.xy = asuint(asfloat(r0.xz) * asfloat(r0.yy));
				r0.z = asuint(max(abs(asfloat(r0.x)), abs(asfloat(r0.y))));
				r0.z = asuint(1.0 / asfloat(r0.z));
				r0.w = asuint(min(abs(asfloat(r0.x)), abs(asfloat(r0.y))));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.w = asuint(asfloat(r0.z) * asfloat(r0.z));
				r1.x = asuint(asfloat(r0.w) * 0.0208350997 + -0.0851330012);
				r1.x = asuint(asfloat(r0.w) * asfloat(r1.x) + 0.180141002);
				r1.x = asuint(asfloat(r0.w) * asfloat(r1.x) + -0.330299497);
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.x) + 0.999866009);
				r1.x = asuint(asfloat(r0.z) * asfloat(r0.w));
				r1.x = asuint(asfloat(r1.x) * -2.0 + 1.57079637);
				b1.y = (abs(asfloat(r0.y)) < abs(asfloat(r0.x)));
				r1.x = b1.y ? r1.x : 0u;
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.w) + asfloat(r1.x));
				b0.w = (asfloat(r0.y) < -asfloat(r0.y));
				r0.w = b0.w ? 0xc0490fdbu : 0u;
				r0.z = asuint(asfloat(r0.z) + asfloat(r0.w));
				r0.w = asuint(min(asfloat(r0.x), asfloat(r0.y)));
				r0.x = asuint(max(asfloat(r0.x), asfloat(r0.y)));
				b0.x = (asfloat(r0.x) >= -asfloat(r0.x));
				b0.y = (asfloat(r0.w) < -asfloat(r0.w));
				b0.x = b0.y && b0.x;
				r0.x = asuint(b0.x ? -asfloat(r0.z) : asfloat(r0.z));
				float sin0;
				float cos0;
				sincos(asfloat(r0.x), sin0, cos0);
				r0.x = asuint(sin0);
				r1.x = asuint(cos0);
				r2.z = r0.x;
				r2.y = r1.x;
				r2.x = asuint(-asfloat(r0.x));
				r0.x = asuint(dot(asfloat(r2.yz), i_POSITION0.xz));
				r1.x = asuint(dot(asfloat(r2.yz), i_NORMAL0.xz));
				r0.yzw = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r0.yzw));
				r0.w = asuint(dot(asfloat(r2.xy), i_POSITION0.xz));
				r1.z = asuint(dot(asfloat(r2.xy), i_NORMAL0.xz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.www) + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r2 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r2 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r2));
				r2 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r2));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r2);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				r0.w = asuint(unity_MatrixV._m21 * asfloat(r0.y));
				r0.w = asuint(unity_MatrixV._m20 * asfloat(r0.x) + asfloat(r0.w));
				r0.w = asuint(unity_MatrixV._m22 * asfloat(r0.z) + asfloat(r0.w));
				o_INTERP1.xyz = asfloat(r0.xyz);
				r0.x = asuint(unity_MatrixV._m23 + asfloat(r0.w));
				r0.x = asuint(-_ProjectionParams.y + -asfloat(r0.x));
				r0.y = asuint(max(0.00100000005, _FadeDistance));
				o_INTERP1.w = saturate(asfloat(r0.x) / asfloat(r0.y));
				r1.y = asuint(i_NORMAL0.y);
				r0.x = asuint(dot(asfloat(r1.xyz), unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(asfloat(r1.xyz), unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(asfloat(r1.xyz), unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP2 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[14];
			float3 _WorldSpaceCameraPos;
			float _GlobalsPad4;
			float4 _GlobalsPad5[3];
			float4 unity_OrthoParams;
			float4 _GlobalsPad6[39];
			float4x4 unity_MatrixV;
			float4 _GlobalsPad7[60];
			float _SA_APDistanceScale;
			float _SA_APSliceCount;
			float _SA_APKmPerSlice;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _BaseMap_ST : packoffset(c1);
				float4 _Color : packoffset(c2);
			}
			Texture3D<float4> _SA_AerialPerspectiveLut : register(t0);
			Texture2D<float4> _BaseMap : register(t1);
			SamplerState sampler_BaseMap : register(s0);
			SamplerState sampler_SA_AerialPerspectiveLut : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b1;
				r0.xyz = asuint(_WorldSpaceCameraPos.xzy + -i_INTERP1.xzy);
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				b0.w = (0.0 == unity_OrthoParams.w);
				r1.x = asuint(unity_MatrixV._m20);
				r1.z = asuint(unity_MatrixV._m21);
				r1.y = asuint(unity_MatrixV._m22);
				r0.xyz = b0.www ? r0.xyz : r1.xyz;
				r0.w = asuint(dot(-asfloat(r0.xyz), -asfloat(r0.xyz)));
				r0.w = asuint(sqrt(asfloat(r0.w)));
				r0.w = asuint(max(9.99999997e-07, asfloat(r0.w)));
				r0.xyz = asuint(-asfloat(r0.xyz) / asfloat(r0.www));
				r0.w = asuint(max(abs(asfloat(r0.y)), abs(asfloat(r0.x))));
				r0.w = asuint(1.0 / asfloat(r0.w));
				r1.x = asuint(min(abs(asfloat(r0.y)), abs(asfloat(r0.x))));
				r0.w = asuint(asfloat(r1.x) * asfloat(r0.w));
				r1.x = asuint(asfloat(r0.w) * asfloat(r0.w));
				r1.y = asuint(asfloat(r1.x) * 0.0208350997 + -0.0851330012);
				r1.y = asuint(asfloat(r1.x) * asfloat(r1.y) + 0.180141002);
				r1.y = asuint(asfloat(r1.x) * asfloat(r1.y) + -0.330299497);
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y) + 0.999866009);
				r1.y = asuint(asfloat(r1.x) * asfloat(r0.w));
				r1.y = asuint(asfloat(r1.y) * -2.0 + 1.57079637);
				b1.z = (abs(asfloat(r0.x)) < abs(asfloat(r0.y)));
				r1.y = b1.z ? r1.y : 0u;
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.x) + asfloat(r1.y));
				b1.x = (asfloat(r0.x) < -asfloat(r0.x));
				r1.x = b1.x ? 0xc0490fdbu : 0u;
				r0.w = asuint(asfloat(r1.x) + asfloat(r0.w));
				r1.x = asuint(min(asfloat(r0.y), asfloat(r0.x)));
				b1.x = (asfloat(r1.x) < -asfloat(r1.x));
				r0.x = asuint(max(asfloat(r0.y), asfloat(r0.x)));
				r0.y = asuint(max(-1.0, asfloat(r0.z)));
				r0.y = asuint(min(1.0, asfloat(r0.y)));
				b0.x = (asfloat(r0.x) >= -asfloat(r0.x));
				b0.x = b1.x && b0.x;
				r0.x = asuint(b0.x ? -asfloat(r0.w) : asfloat(r0.w));
				r1.x = asuint(asfloat(r0.x) * 0.159154937 + 0.5);
				r0.xzw = asuint(unity_MatrixV._m01_m11_m21 * i_INTERP1.yyy);
				r0.xzw = asuint(unity_MatrixV._m00_m10_m20 * i_INTERP1.xxx + asfloat(r0.xzw));
				r0.xzw = asuint(unity_MatrixV._m02_m12_m22 * i_INTERP1.zzz + asfloat(r0.xzw));
				r0.xzw = asuint(unity_MatrixV._m03_m13_m23 + asfloat(r0.xzw));
				r0.x = asuint(dot(asfloat(r0.xzw), asfloat(r0.xzw)));
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(0.00100000005 * asfloat(r0.x));
				b0.z = (0.0 < _SA_APDistanceScale);
				r0.z = b0.z ? asuint(_SA_APDistanceScale) : 0x3f800000u;
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				r0.z = asuint(1.0 / _SA_APKmPerSlice);
				r2.z = asuint(asfloat(r0.x) * asfloat(r0.z));
				b0.x = (asfloat(r2.z) < 0.5);
				r2.y = asuint(saturate(asfloat(r2.z) + asfloat(r2.z)));
				r2.xw = uint2(0x3f000000u, 0x3f800000u);
				r0.xz = b0.xx ? r2.xy : r2.zw;
				r0.x = asuint(asfloat(r0.x) / _SA_APSliceCount);
				r1.z = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(abs(asfloat(r0.y)) * -0.0187292993 + 0.0742610022);
				r0.x = asuint(asfloat(r0.x) * abs(asfloat(r0.y)) + -0.212114394);
				r0.x = asuint(asfloat(r0.x) * abs(asfloat(r0.y)) + 1.57072878);
				r0.w = asuint(1.0 + -abs(asfloat(r0.y)));
				b0.y = (asfloat(r0.y) < -asfloat(r0.y));
				r0.w = asuint(sqrt(asfloat(r0.w)));
				r1.w = asuint(asfloat(r0.x) * asfloat(r0.w));
				r1.w = asuint(asfloat(r1.w) * -2.0 + 3.14159274);
				r0.y = b0.y ? r1.w : 0u;
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.w) + asfloat(r0.y));
				r0.x = asuint(1.57079637 + -asfloat(r0.x));
				r1.y = asuint(asfloat(r0.x) * 0.318309873 + 0.5);
				r1 = asuint(_SA_AerialPerspectiveLut.SampleLevel(sampler_SA_AerialPerspectiveLut, asfloat(r1.xyz), 0.0));
				r0.x = asuint(1.0 - asfloat(r0.z) * asfloat(r1.w));
				r0.yz = asuint(i_INTERP0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw);
				r1 = asuint(_BaseMap.SampleBias(sampler_BaseMap, asfloat(r0.yz), _GlobalMipBias.x));
				r1 = asuint(_Color.xyzw * asfloat(r1));
				o_SV_Target0.xyz = asfloat(r1.xyz) * asfloat(r0.xxx);
				o_SV_Target0.w = i_INTERP1.w * asfloat(r1.w);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "DepthNormalsOnly"
			Tags { "LIGHTMODE" = "DepthNormalsOnly" }
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[21];
			float4 _ProjectionParams;
			float4 _GlobalsPad1[42];
			float4x4 unity_MatrixV;
			float4x4 unity_MatrixInvV;
			float4 _GlobalsPad2[4];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _FadeDistance : packoffset(c3);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b1;
				r0.xyz = asuint(unity_WorldToObject._m01_m11_m21 * unity_MatrixInvV._m12_m12_m12);
				r0.xyz = asuint(unity_WorldToObject._m00_m10_m20 * unity_MatrixInvV._m02_m02_m02 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * unity_MatrixInvV._m22_m22_m22 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 * unity_MatrixInvV._m32_m32_m32 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r1.xyz = asuint(unity_WorldToObject._m01_m11_m21 * -asfloat(r0.yyy));
				r0.xyw = asuint(asfloat(r1.xyz) - unity_WorldToObject._m00_m10_m20 * asfloat(r0.xxx));
				r0.xyz = asuint(asfloat(r0.xyw) - unity_WorldToObject._m02_m12_m22 * asfloat(r0.zzz));
				r0.y = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.y = asuint(rsqrt(asfloat(r0.y)));
				r0.xy = asuint(asfloat(r0.xz) * asfloat(r0.yy));
				r0.z = asuint(max(abs(asfloat(r0.x)), abs(asfloat(r0.y))));
				r0.z = asuint(1.0 / asfloat(r0.z));
				r0.w = asuint(min(abs(asfloat(r0.x)), abs(asfloat(r0.y))));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.w = asuint(asfloat(r0.z) * asfloat(r0.z));
				r1.x = asuint(asfloat(r0.w) * 0.0208350997 + -0.0851330012);
				r1.x = asuint(asfloat(r0.w) * asfloat(r1.x) + 0.180141002);
				r1.x = asuint(asfloat(r0.w) * asfloat(r1.x) + -0.330299497);
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.x) + 0.999866009);
				r1.x = asuint(asfloat(r0.z) * asfloat(r0.w));
				r1.x = asuint(asfloat(r1.x) * -2.0 + 1.57079637);
				b1.y = (abs(asfloat(r0.y)) < abs(asfloat(r0.x)));
				r1.x = b1.y ? r1.x : 0u;
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.w) + asfloat(r1.x));
				b0.w = (asfloat(r0.y) < -asfloat(r0.y));
				r0.w = b0.w ? 0xc0490fdbu : 0u;
				r0.z = asuint(asfloat(r0.z) + asfloat(r0.w));
				r0.w = asuint(min(asfloat(r0.x), asfloat(r0.y)));
				r0.x = asuint(max(asfloat(r0.x), asfloat(r0.y)));
				b0.x = (asfloat(r0.x) >= -asfloat(r0.x));
				b0.y = (asfloat(r0.w) < -asfloat(r0.w));
				b0.x = b0.y && b0.x;
				r0.x = asuint(b0.x ? -asfloat(r0.z) : asfloat(r0.z));
				float sin0;
				float cos0;
				sincos(asfloat(r0.x), sin0, cos0);
				r0.x = asuint(sin0);
				r1.x = asuint(cos0);
				r2.z = r0.x;
				r2.y = r1.x;
				r2.x = asuint(-asfloat(r0.x));
				r0.x = asuint(dot(asfloat(r2.yz), i_POSITION0.xz));
				r1.x = asuint(dot(asfloat(r2.yz), i_NORMAL0.xz));
				r0.yzw = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r0.yzw));
				r0.w = asuint(dot(asfloat(r2.xy), i_POSITION0.xz));
				r1.z = asuint(dot(asfloat(r2.xy), i_NORMAL0.xz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.www) + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r2 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r2 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r2));
				r2 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r2));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r2);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				r1.y = asuint(i_NORMAL0.y);
				r2.x = asuint(dot(asfloat(r1.xyz), unity_WorldToObject._m00_m10_m20));
				r2.y = asuint(dot(asfloat(r1.xyz), unity_WorldToObject._m01_m11_m21));
				r2.z = asuint(dot(asfloat(r1.xyz), unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP1.xyz = asfloat(r2.xyz) * asfloat(r0.www);
				r0.y = asuint(unity_MatrixV._m21 * asfloat(r0.y));
				r0.x = asuint(unity_MatrixV._m20 * asfloat(r0.x) + asfloat(r0.y));
				r0.x = asuint(unity_MatrixV._m22 * asfloat(r0.z) + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 + asfloat(r0.x));
				r0.x = asuint(-_ProjectionParams.y + -asfloat(r0.x));
				r0.y = asuint(max(0.00100000005, _FadeDistance));
				o_INTERP1.w = saturate(asfloat(r0.x) / asfloat(r0.y));
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.x = asuint(dot(i_INTERP1.xyz, i_INTERP1.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				o_SV_Target0.xyz = i_INTERP1.xyz * asfloat(r0.xxx);
				o_SV_Target0.w = 0.0;
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "ShadowCaster"
			Tags { "LIGHTMODE" = "SHADOWCASTER" }
			ColorMask 0
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[21];
			float4 _ProjectionParams;
			float4 _GlobalsPad1[42];
			float4x4 unity_MatrixV;
			float4x4 unity_MatrixInvV;
			float4 _GlobalsPad2[4];
			float4x4 unity_MatrixVP;
			float4 _GlobalsPad3[47];
			float4 _ShadowBias;
			float4 _GlobalsPad4[2];
			float3 _LightDirection;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _FadeDistance : packoffset(c3);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b1;
				r0.xyz = asuint(unity_WorldToObject._m01_m11_m21 * unity_MatrixInvV._m12_m12_m12);
				r0.xyz = asuint(unity_WorldToObject._m00_m10_m20 * unity_MatrixInvV._m02_m02_m02 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * unity_MatrixInvV._m22_m22_m22 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 * unity_MatrixInvV._m32_m32_m32 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r1.xyz = asuint(unity_WorldToObject._m01_m11_m21 * -asfloat(r0.yyy));
				r0.xyw = asuint(asfloat(r1.xyz) - unity_WorldToObject._m00_m10_m20 * asfloat(r0.xxx));
				r0.xyz = asuint(asfloat(r0.xyw) - unity_WorldToObject._m02_m12_m22 * asfloat(r0.zzz));
				r0.y = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.y = asuint(rsqrt(asfloat(r0.y)));
				r0.xy = asuint(asfloat(r0.xz) * asfloat(r0.yy));
				r0.z = asuint(max(abs(asfloat(r0.x)), abs(asfloat(r0.y))));
				r0.z = asuint(1.0 / asfloat(r0.z));
				r0.w = asuint(min(abs(asfloat(r0.x)), abs(asfloat(r0.y))));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.w = asuint(asfloat(r0.z) * asfloat(r0.z));
				r1.x = asuint(asfloat(r0.w) * 0.0208350997 + -0.0851330012);
				r1.x = asuint(asfloat(r0.w) * asfloat(r1.x) + 0.180141002);
				r1.x = asuint(asfloat(r0.w) * asfloat(r1.x) + -0.330299497);
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.x) + 0.999866009);
				r1.x = asuint(asfloat(r0.z) * asfloat(r0.w));
				r1.x = asuint(asfloat(r1.x) * -2.0 + 1.57079637);
				b1.y = (abs(asfloat(r0.y)) < abs(asfloat(r0.x)));
				r1.x = b1.y ? r1.x : 0u;
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.w) + asfloat(r1.x));
				b0.w = (asfloat(r0.y) < -asfloat(r0.y));
				r0.w = b0.w ? 0xc0490fdbu : 0u;
				r0.z = asuint(asfloat(r0.z) + asfloat(r0.w));
				r0.w = asuint(min(asfloat(r0.x), asfloat(r0.y)));
				r0.x = asuint(max(asfloat(r0.x), asfloat(r0.y)));
				b0.x = (asfloat(r0.x) >= -asfloat(r0.x));
				b0.y = (asfloat(r0.w) < -asfloat(r0.w));
				b0.x = b0.y && b0.x;
				r0.x = asuint(b0.x ? -asfloat(r0.z) : asfloat(r0.z));
				float sin0;
				float cos0;
				sincos(asfloat(r0.x), sin0, cos0);
				r0.x = asuint(sin0);
				r1.x = asuint(cos0);
				r2.z = r0.x;
				r2.y = r1.x;
				r2.x = asuint(-asfloat(r0.x));
				r0.x = asuint(dot(asfloat(r2.yz), i_NORMAL0.xz));
				r0.w = asuint(dot(asfloat(r2.yz), i_POSITION0.xz));
				r0.z = asuint(dot(asfloat(r2.xy), i_NORMAL0.xz));
				r1.x = asuint(dot(asfloat(r2.xy), i_POSITION0.xz));
				r0.y = asuint(i_NORMAL0.y);
				r2.x = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m00_m10_m20));
				r2.y = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m01_m11_m21));
				r2.z = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m02_m12_m22));
				r0.x = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r0.x = asuint(max(1.17549435e-38, asfloat(r0.x)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r0.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.xxx));
				r1.y = asuint(saturate(dot(_LightDirection.xyz, asfloat(r0.xyz))));
				r1.y = asuint(1.0 + -asfloat(r1.y));
				r1.y = asuint(_ShadowBias.y * asfloat(r1.y));
				r2.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r2.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.www) + asfloat(r2.xyz));
				r1.xzw = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r1.xxx) + asfloat(r2.xyz));
				r1.xzw = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r1.xzw));
				r2.xyz = asuint(_LightDirection.xyz * _ShadowBias.xxx + asfloat(r1.xzw));
				r2.xyz = asuint(asfloat(r0.xyz) * asfloat(r1.yyy) + asfloat(r2.xyz));
				o_INTERP1.xyz = asfloat(r0.xyz);
				r0 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r2.yyyy));
				r0 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r2.xxxx) + asfloat(r0));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r2.zzzz) + asfloat(r0));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0));
				o_SV_POSITION0.z = min(1.0, asfloat(r0.z));
				o_SV_POSITION0.xyw = asfloat(r0.xyw);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				r0.x = asuint(unity_MatrixV._m21 * asfloat(r1.z));
				r0.x = asuint(unity_MatrixV._m20 * asfloat(r1.x) + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m22 * asfloat(r1.w) + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 + asfloat(r0.x));
				r0.x = asuint(-_ProjectionParams.y + -asfloat(r0.x));
				r0.y = asuint(max(0.00100000005, _FadeDistance));
				o_INTERP1.w = saturate(asfloat(r0.x) / asfloat(r0.y));
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, out float4 o_SV_TARGET0 : SV_TARGET0)
			{
				o_SV_TARGET0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}
