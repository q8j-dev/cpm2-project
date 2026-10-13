Shader "Obi/Particles"
{
	Properties
	{
		_Color ("Particle color", Color) = (1,1,1,1)
		_RadiusScale ("Radius scale", Float) = 1
	}
	SubShader
	{
		Pass
		{
			Name "ParticleFwdBase"
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Geometry" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DIRECTIONAL
			#pragma multi_compile __ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile __ LIGHTPROBE_SH
			#pragma multi_compile __ SHADOWS_SCREEN
			#pragma multi_compile __ SHADOWS_SHADOWMASK
			#pragma multi_compile __ VERTEXLIGHT_ON

			#if defined(SHADER_STAGE_VERTEX)
			#if (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(SHADOWS_SCREEN) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN)) || (defined(DIRECTIONAL) && defined(SHADOWS_SCREEN) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(SHADOWS_SCREEN))
			float _RadiusScale;
			float _GlobalsPad0;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[6];
			float4 _Color;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ProjectionParams : packoffset(c5);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
			}
			cbuffer UnityPerDraw : register(b3)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
				float4x4 unity_MatrixInvV : packoffset(c13);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float4 o_TEXCOORD0 : TEXCOORD0, out float3 o_TEXCOORD1 : TEXCOORD1, out float3 o_TEXCOORD2 : TEXCOORD2, out float3 o_TEXCOORD3 : TEXCOORD3, out float3 o_TEXCOORD4 : TEXCOORD4, out float4 o_TEXCOORD6 : TEXCOORD6)
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
				uint4 r10;
				uint4 r11;
				bool4 b0;
				bool4 b1;
				r0.xy = asuint(unity_MatrixV._m01_m11 * i_POSITION0.yy);
				r0.xy = asuint(unity_MatrixV._m00_m10 * i_POSITION0.xx + asfloat(r0.xy));
				r0.xy = asuint(unity_MatrixV._m02_m12 * i_POSITION0.zz + asfloat(r0.xy));
				r0.xy = asuint(unity_MatrixV._m03_m13 + asfloat(r0.xy));
				r0.yzw = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyz = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r0.yzw));
				r0.xyz = asuint(unity_MatrixInvV._m03_m13_m23 + asfloat(r0.xyz));
				b0.w = (0.0 == glstate_matrix_projection._m30);
				b1.x = (0.0 == glstate_matrix_projection._m31);
				b0.w = b1.x && b0.w;
				b1.x = (0.0 == glstate_matrix_projection._m32);
				b0.w = b1.x && b0.w;
				r0.xyz = b0.www ? r0.xyz : asuint(unity_MatrixInvV._m03_m13_m23);
				r0.xyz = asuint(-i_POSITION0.xyz + asfloat(r0.xyz));
				r1.x = asuint(_RadiusScale * i_TEXCOORD1.w);
				r1.y = asuint(1.0 / asfloat(r1.x));
				r1.xzw = asuint(i_TEXCOORD1.xyz * asfloat(r1.xxx));
				r2.xyz = asuint(i_TEXCOORD1.xyz * asfloat(r1.yyy));
				r3.xyz = asuint(i_TEXCOORD1.yyy * asfloat(r2.xyz));
				r1.y = asuint(_RadiusScale * i_TEXCOORD0.w);
				r2.w = asuint(1.0 / asfloat(r1.y));
				r4.xyz = asuint(i_TEXCOORD0.xyz * asfloat(r1.yyy));
				r5.xyz = asuint(i_TEXCOORD0.xyz * asfloat(r2.www));
				r3.xyz = asuint(asfloat(r5.xyz) * i_TEXCOORD0.yyy + asfloat(r3.xyz));
				r1.y = asuint(_RadiusScale * i_TEXCOORD2.w);
				r2.w = asuint(1.0 / asfloat(r1.y));
				r6.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r1.yyy));
				r7.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r2.www));
				r3.xyz = asuint(asfloat(r7.xyz) * i_TEXCOORD2.yyy + asfloat(r3.xyz));
				r8.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.yyy));
				r9.xyz = asuint(i_TEXCOORD1.xxx * asfloat(r2.xyz));
				r2.xyz = asuint(i_TEXCOORD1.zzz * asfloat(r2.xyz));
				r2.xyz = asuint(asfloat(r5.xyz) * i_TEXCOORD0.zzz + asfloat(r2.xyz));
				r5.xyz = asuint(asfloat(r5.xyz) * i_TEXCOORD0.xxx + asfloat(r9.xyz));
				r5.xyz = asuint(asfloat(r7.xyz) * i_TEXCOORD2.xxx + asfloat(r5.xyz));
				r2.xyz = asuint(asfloat(r7.xyz) * i_TEXCOORD2.zzz + asfloat(r2.xyz));
				r7.xyz = asuint(asfloat(r5.xyz) * asfloat(r0.xxx) + asfloat(r8.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.zzz) + asfloat(r7.xyz));
				r7.x = asuint(unity_MatrixV._m12);
				r7.y = asuint(unity_MatrixV._m10);
				r7.z = asuint(unity_MatrixV._m11);
				r8.xyz = asuint(asfloat(r7.xyz) * -asfloat(r0.xyz));
				r7.xyz = asuint(-asfloat(r8.xyz) - asfloat(r0.zxy) * asfloat(r7.yzx));
				r1.y = asuint(dot(asfloat(r7.xyz), asfloat(r7.xyz)));
				r1.y = asuint(rsqrt(asfloat(r1.y)));
				r7.xyz = asuint(asfloat(r7.xyz) * asfloat(r1.yyy));
				r8.xyz = asuint(asfloat(r7.xyz) * -asfloat(r0.zxy));
				r8.xyz = asuint(-asfloat(r8.xyz) - asfloat(r0.yzx) * asfloat(r7.yzx));
				r1.y = asuint(dot(asfloat(r8.xyz), asfloat(r8.xyz)));
				r1.y = asuint(rsqrt(asfloat(r1.y)));
				r8.xyz = asuint(asfloat(r8.xyz) * asfloat(r1.yyy));
				r9.xyz = asuint(i_TEXCOORD1.yyy * asfloat(r1.xzw));
				r9.xyz = asuint(asfloat(r4.xyz) * i_TEXCOORD0.yyy + asfloat(r9.xyz));
				r9.xyz = asuint(asfloat(r6.xyz) * i_TEXCOORD2.yyy + asfloat(r9.xyz));
				r10.xyz = asuint(asfloat(r9.xyz) * asfloat(r8.yyy));
				r11.xyz = asuint(i_TEXCOORD1.xxx * asfloat(r1.xzw));
				r1.xyz = asuint(i_TEXCOORD1.zzz * asfloat(r1.xzw));
				r1.xyz = asuint(asfloat(r4.xyz) * i_TEXCOORD0.zzz + asfloat(r1.xyz));
				r4.xyz = asuint(asfloat(r4.xyz) * i_TEXCOORD0.xxx + asfloat(r11.xyz));
				r4.xyz = asuint(asfloat(r6.xyz) * i_TEXCOORD2.xxx + asfloat(r4.xyz));
				r1.xyz = asuint(asfloat(r6.xyz) * i_TEXCOORD2.zzz + asfloat(r1.xyz));
				r6.xyz = asuint(asfloat(r4.xyz) * asfloat(r8.xxx) + asfloat(r10.xyz));
				r6.xyz = asuint(asfloat(r1.xyz) * asfloat(r8.zzz) + asfloat(r6.xyz));
				r6.xyz = asuint(i_NORMAL0.yyy * asfloat(r6.xyz));
				r8.xyz = asuint(asfloat(r9.xyz) * asfloat(r7.xxx));
				r7.xzw = asuint(asfloat(r4.xyz) * asfloat(r7.zzz) + asfloat(r8.xyz));
				r7.xyz = asuint(asfloat(r1.xyz) * asfloat(r7.yyy) + asfloat(r7.xzw));
				r6.xyz = asuint(asfloat(r7.xyz) * i_NORMAL0.xxx + asfloat(r6.xyz));
				r1.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r2.w = asuint(1.0 / asfloat(r1.w));
				r1.w = asuint(sqrt(asfloat(r1.w)));
				o_TEXCOORD0.z = 1.0 / asfloat(r1.w);
				r7.xyz = asuint(asfloat(r2.www) * asfloat(r0.xyz));
				r0.x = asuint(1.0 + -asfloat(r2.w));
				r7.w = asuint(sqrt(asfloat(r0.x)));
				r7 = b0.wwww ? uint4(0u, 0u, 0u, 0x3f800000u) : r7;
				r0.xyz = asuint(asfloat(r9.xyz) * asfloat(r7.yyy));
				r0.xyz = asuint(asfloat(r4.xyz) * asfloat(r7.xxx) + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r7.zzz) + asfloat(r0.xyz));
				r0.xyz = asuint(i_POSITION0.xyz + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r7.www) * asfloat(r6.xyz) + asfloat(r0.xyz));
				o_TEXCOORD0.w = asfloat(r7.w);
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m03_m13_m23_m33 * i_POSITION0.wwww + asfloat(r1));
				o_SV_POSITION0 = asfloat(r1);
				o_COLOR0 = _Color.xyzw * i_COLOR0.xyzw;
				o_TEXCOORD0.xy = i_NORMAL0.xy;
				r4.xy = asuint(unity_MatrixV._m01_m11 * asfloat(r0.yy));
				r4.xy = asuint(unity_MatrixV._m00_m10 * asfloat(r0.xx) + asfloat(r4.xy));
				r4.xy = asuint(unity_MatrixV._m02_m12 * asfloat(r0.zz) + asfloat(r4.xy));
				r4.xy = asuint(unity_MatrixV._m03_m13 + asfloat(r4.xy));
				r4.yzw = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r4.yyy));
				r4.xyz = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r4.xxx) + asfloat(r4.yzw));
				r4.xyz = asuint(unity_MatrixInvV._m03_m13_m23 + asfloat(r4.xyz));
				r4.xyz = b0.www ? r4.xyz : asuint(unity_MatrixInvV._m03_m13_m23);
				r0.xyz = asuint(-asfloat(r4.xyz) + asfloat(r0.xyz));
				r4.xyz = asuint(-i_POSITION0.xyz + asfloat(r4.xyz));
				r6.xyz = asuint(unity_MatrixV._m01_m11_m21 * asfloat(r0.yyy));
				r6.xyz = asuint(unity_MatrixV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r6.xyz));
				o_TEXCOORD1 = unity_MatrixV._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r6.xyz);
				r6.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m10_m10_m10);
				r6.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m00_m00_m00 + asfloat(r6.xyz));
				r6.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m20_m20_m20 + asfloat(r6.xyz));
				r6.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m30_m30_m30 + asfloat(r6.xyz));
				r7.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m11_m11_m11);
				r7.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m01_m01_m01 + asfloat(r7.xyz));
				r7.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m21_m21_m21 + asfloat(r7.xyz));
				r7.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m31_m31_m31 + asfloat(r7.xyz));
				r8.xyz = asuint(unity_WorldToObject._m01_m11_m21 * _WorldSpaceLightPos0.yyy);
				r8.xyz = asuint(unity_WorldToObject._m00_m10_m20 * _WorldSpaceLightPos0.xxx + asfloat(r8.xyz));
				r8.xyz = asuint(unity_WorldToObject._m02_m12_m22 * _WorldSpaceLightPos0.zzz + asfloat(r8.xyz));
				r8.xyz = asuint(unity_WorldToObject._m03_m13_m23 * _WorldSpaceLightPos0.www + asfloat(r8.xyz));
				r7.xyz = asuint(asfloat(r8.yyy) * asfloat(r7.xyz));
				r6.xyz = asuint(asfloat(r6.xyz) * asfloat(r8.xxx) + asfloat(r7.xyz));
				r7.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m12_m12_m12);
				r7.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m02_m02_m02 + asfloat(r7.xyz));
				r7.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m22_m22_m22 + asfloat(r7.xyz));
				r7.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m32_m32_m32 + asfloat(r7.xyz));
				o_TEXCOORD2 = asfloat(r7.xyz) * asfloat(r8.zzz) + asfloat(r6.xyz);
				r6.xyz = asuint(asfloat(r4.yyy) * asfloat(r3.xyz));
				r4.xyw = asuint(asfloat(r5.xyz) * asfloat(r4.xxx) + asfloat(r6.xyz));
				r4.xyz = asuint(asfloat(r2.xyz) * asfloat(r4.zzz) + asfloat(r4.xyw));
				r6.xyz = asuint(asfloat(r4.yyy) * asfloat(r3.xyz));
				r4.xyw = asuint(asfloat(r5.xyz) * asfloat(r4.xxx) + asfloat(r6.xyz));
				r4.xyz = asuint(asfloat(r2.xyz) * asfloat(r4.zzz) + asfloat(r4.xyw));
				r6.xyz = asuint(unity_MatrixV._m01_m11_m21 * asfloat(r4.yyy));
				r4.xyw = asuint(unity_MatrixV._m00_m10_m20 * asfloat(r4.xxx) + asfloat(r6.xyz));
				o_TEXCOORD3 = unity_MatrixV._m02_m12_m22 * asfloat(r4.zzz) + asfloat(r4.xyw);
				r4.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.yyy));
				r0.xyw = asuint(asfloat(r5.xyz) * asfloat(r0.xxx) + asfloat(r4.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.zzz) + asfloat(r0.xyw));
				r3.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.yyy));
				r0.xyw = asuint(asfloat(r5.xyz) * asfloat(r0.xxx) + asfloat(r3.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.zzz) + asfloat(r0.xyw));
				r2.xyz = asuint(unity_MatrixV._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_MatrixV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r2.xyz));
				o_TEXCOORD4 = unity_MatrixV._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw);
				r0.x = asuint(_ProjectionParams.x * asfloat(r1.y));
				r0.w = asuint(0.5 * asfloat(r0.x));
				r0.xz = asuint(float2(0.5, 0.5) * asfloat(r1.xw));
				o_TEXCOORD6.zw = asfloat(r1.zw);
				o_TEXCOORD6.xy = asfloat(r0.xw) + asfloat(r0.zz);
			}
			#elif (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(VERTEXLIGHT_ON)) || (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH)) || (defined(DIRECTIONAL) && defined(VERTEXLIGHT_ON)) || defined(DIRECTIONAL)
			float _RadiusScale;
			float _GlobalsPad0;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[6];
			float4 _Color;
			cbuffer UnityLighting : register(b1)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
				float4x4 unity_MatrixInvV : packoffset(c13);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float4 o_TEXCOORD0 : TEXCOORD0, out float3 o_TEXCOORD1 : TEXCOORD1, out float3 o_TEXCOORD2 : TEXCOORD2, out float3 o_TEXCOORD3 : TEXCOORD3, out float3 o_TEXCOORD4 : TEXCOORD4)
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
				uint4 r10;
				uint4 r11;
				bool4 b0;
				bool4 b1;
				r0.xy = asuint(unity_MatrixV._m01_m11 * i_POSITION0.yy);
				r0.xy = asuint(unity_MatrixV._m00_m10 * i_POSITION0.xx + asfloat(r0.xy));
				r0.xy = asuint(unity_MatrixV._m02_m12 * i_POSITION0.zz + asfloat(r0.xy));
				r0.xy = asuint(unity_MatrixV._m03_m13 + asfloat(r0.xy));
				r0.yzw = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyz = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r0.yzw));
				r0.xyz = asuint(unity_MatrixInvV._m03_m13_m23 + asfloat(r0.xyz));
				b0.w = (0.0 == glstate_matrix_projection._m30);
				b1.x = (0.0 == glstate_matrix_projection._m31);
				b0.w = b1.x && b0.w;
				b1.x = (0.0 == glstate_matrix_projection._m32);
				b0.w = b1.x && b0.w;
				r0.xyz = b0.www ? r0.xyz : asuint(unity_MatrixInvV._m03_m13_m23);
				r0.xyz = asuint(-i_POSITION0.xyz + asfloat(r0.xyz));
				r1.x = asuint(_RadiusScale * i_TEXCOORD1.w);
				r1.y = asuint(1.0 / asfloat(r1.x));
				r1.xzw = asuint(i_TEXCOORD1.xyz * asfloat(r1.xxx));
				r2.xyz = asuint(i_TEXCOORD1.xyz * asfloat(r1.yyy));
				r3.xyz = asuint(i_TEXCOORD1.yyy * asfloat(r2.xyz));
				r1.y = asuint(_RadiusScale * i_TEXCOORD0.w);
				r2.w = asuint(1.0 / asfloat(r1.y));
				r4.xyz = asuint(i_TEXCOORD0.xyz * asfloat(r1.yyy));
				r5.xyz = asuint(i_TEXCOORD0.xyz * asfloat(r2.www));
				r3.xyz = asuint(asfloat(r5.xyz) * i_TEXCOORD0.yyy + asfloat(r3.xyz));
				r1.y = asuint(_RadiusScale * i_TEXCOORD2.w);
				r2.w = asuint(1.0 / asfloat(r1.y));
				r6.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r1.yyy));
				r7.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r2.www));
				r3.xyz = asuint(asfloat(r7.xyz) * i_TEXCOORD2.yyy + asfloat(r3.xyz));
				r8.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.yyy));
				r9.xyz = asuint(i_TEXCOORD1.xxx * asfloat(r2.xyz));
				r2.xyz = asuint(i_TEXCOORD1.zzz * asfloat(r2.xyz));
				r2.xyz = asuint(asfloat(r5.xyz) * i_TEXCOORD0.zzz + asfloat(r2.xyz));
				r5.xyz = asuint(asfloat(r5.xyz) * i_TEXCOORD0.xxx + asfloat(r9.xyz));
				r5.xyz = asuint(asfloat(r7.xyz) * i_TEXCOORD2.xxx + asfloat(r5.xyz));
				r2.xyz = asuint(asfloat(r7.xyz) * i_TEXCOORD2.zzz + asfloat(r2.xyz));
				r7.xyz = asuint(asfloat(r5.xyz) * asfloat(r0.xxx) + asfloat(r8.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.zzz) + asfloat(r7.xyz));
				r7.x = asuint(unity_MatrixV._m12);
				r7.y = asuint(unity_MatrixV._m10);
				r7.z = asuint(unity_MatrixV._m11);
				r8.xyz = asuint(asfloat(r7.xyz) * -asfloat(r0.xyz));
				r7.xyz = asuint(-asfloat(r8.xyz) - asfloat(r0.zxy) * asfloat(r7.yzx));
				r1.y = asuint(dot(asfloat(r7.xyz), asfloat(r7.xyz)));
				r1.y = asuint(rsqrt(asfloat(r1.y)));
				r7.xyz = asuint(asfloat(r7.xyz) * asfloat(r1.yyy));
				r8.xyz = asuint(asfloat(r7.xyz) * -asfloat(r0.zxy));
				r8.xyz = asuint(-asfloat(r8.xyz) - asfloat(r0.yzx) * asfloat(r7.yzx));
				r1.y = asuint(dot(asfloat(r8.xyz), asfloat(r8.xyz)));
				r1.y = asuint(rsqrt(asfloat(r1.y)));
				r8.xyz = asuint(asfloat(r8.xyz) * asfloat(r1.yyy));
				r9.xyz = asuint(i_TEXCOORD1.yyy * asfloat(r1.xzw));
				r9.xyz = asuint(asfloat(r4.xyz) * i_TEXCOORD0.yyy + asfloat(r9.xyz));
				r9.xyz = asuint(asfloat(r6.xyz) * i_TEXCOORD2.yyy + asfloat(r9.xyz));
				r10.xyz = asuint(asfloat(r9.xyz) * asfloat(r8.yyy));
				r11.xyz = asuint(i_TEXCOORD1.xxx * asfloat(r1.xzw));
				r1.xyz = asuint(i_TEXCOORD1.zzz * asfloat(r1.xzw));
				r1.xyz = asuint(asfloat(r4.xyz) * i_TEXCOORD0.zzz + asfloat(r1.xyz));
				r4.xyz = asuint(asfloat(r4.xyz) * i_TEXCOORD0.xxx + asfloat(r11.xyz));
				r4.xyz = asuint(asfloat(r6.xyz) * i_TEXCOORD2.xxx + asfloat(r4.xyz));
				r1.xyz = asuint(asfloat(r6.xyz) * i_TEXCOORD2.zzz + asfloat(r1.xyz));
				r6.xyz = asuint(asfloat(r4.xyz) * asfloat(r8.xxx) + asfloat(r10.xyz));
				r6.xyz = asuint(asfloat(r1.xyz) * asfloat(r8.zzz) + asfloat(r6.xyz));
				r6.xyz = asuint(i_NORMAL0.yyy * asfloat(r6.xyz));
				r8.xyz = asuint(asfloat(r9.xyz) * asfloat(r7.xxx));
				r7.xzw = asuint(asfloat(r4.xyz) * asfloat(r7.zzz) + asfloat(r8.xyz));
				r7.xyz = asuint(asfloat(r1.xyz) * asfloat(r7.yyy) + asfloat(r7.xzw));
				r6.xyz = asuint(asfloat(r7.xyz) * i_NORMAL0.xxx + asfloat(r6.xyz));
				r1.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r2.w = asuint(1.0 / asfloat(r1.w));
				r1.w = asuint(sqrt(asfloat(r1.w)));
				o_TEXCOORD0.z = 1.0 / asfloat(r1.w);
				r7.xyz = asuint(asfloat(r2.www) * asfloat(r0.xyz));
				r0.x = asuint(1.0 + -asfloat(r2.w));
				r7.w = asuint(sqrt(asfloat(r0.x)));
				r7 = b0.wwww ? uint4(0u, 0u, 0u, 0x3f800000u) : r7;
				r0.xyz = asuint(asfloat(r9.xyz) * asfloat(r7.yyy));
				r0.xyz = asuint(asfloat(r4.xyz) * asfloat(r7.xxx) + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r7.zzz) + asfloat(r0.xyz));
				r0.xyz = asuint(i_POSITION0.xyz + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r7.www) * asfloat(r6.xyz) + asfloat(r0.xyz));
				o_TEXCOORD0.w = asfloat(r7.w);
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * i_POSITION0.wwww + asfloat(r1);
				o_COLOR0 = _Color.xyzw * i_COLOR0.xyzw;
				o_TEXCOORD0.xy = i_NORMAL0.xy;
				r1.xy = asuint(unity_MatrixV._m01_m11 * asfloat(r0.yy));
				r1.xy = asuint(unity_MatrixV._m00_m10 * asfloat(r0.xx) + asfloat(r1.xy));
				r1.xy = asuint(unity_MatrixV._m02_m12 * asfloat(r0.zz) + asfloat(r1.xy));
				r1.xy = asuint(unity_MatrixV._m03_m13 + asfloat(r1.xy));
				r1.yzw = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r1.yyy));
				r1.xyz = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r1.xxx) + asfloat(r1.yzw));
				r1.xyz = asuint(unity_MatrixInvV._m03_m13_m23 + asfloat(r1.xyz));
				r1.xyz = b0.www ? r1.xyz : asuint(unity_MatrixInvV._m03_m13_m23);
				r0.xyz = asuint(-asfloat(r1.xyz) + asfloat(r0.xyz));
				r1.xyz = asuint(-i_POSITION0.xyz + asfloat(r1.xyz));
				r4.xyz = asuint(unity_MatrixV._m01_m11_m21 * asfloat(r0.yyy));
				r4.xyz = asuint(unity_MatrixV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r4.xyz));
				o_TEXCOORD1 = unity_MatrixV._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r4.xyz);
				r4.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m10_m10_m10);
				r4.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m00_m00_m00 + asfloat(r4.xyz));
				r4.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m20_m20_m20 + asfloat(r4.xyz));
				r4.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m30_m30_m30 + asfloat(r4.xyz));
				r6.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m11_m11_m11);
				r6.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m01_m01_m01 + asfloat(r6.xyz));
				r6.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m21_m21_m21 + asfloat(r6.xyz));
				r6.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m31_m31_m31 + asfloat(r6.xyz));
				r7.xyz = asuint(unity_WorldToObject._m01_m11_m21 * _WorldSpaceLightPos0.yyy);
				r7.xyz = asuint(unity_WorldToObject._m00_m10_m20 * _WorldSpaceLightPos0.xxx + asfloat(r7.xyz));
				r7.xyz = asuint(unity_WorldToObject._m02_m12_m22 * _WorldSpaceLightPos0.zzz + asfloat(r7.xyz));
				r7.xyz = asuint(unity_WorldToObject._m03_m13_m23 * _WorldSpaceLightPos0.www + asfloat(r7.xyz));
				r6.xyz = asuint(asfloat(r7.yyy) * asfloat(r6.xyz));
				r4.xyz = asuint(asfloat(r4.xyz) * asfloat(r7.xxx) + asfloat(r6.xyz));
				r6.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m12_m12_m12);
				r6.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m02_m02_m02 + asfloat(r6.xyz));
				r6.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m22_m22_m22 + asfloat(r6.xyz));
				r6.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m32_m32_m32 + asfloat(r6.xyz));
				o_TEXCOORD2 = asfloat(r6.xyz) * asfloat(r7.zzz) + asfloat(r4.xyz);
				r4.xyz = asuint(asfloat(r3.xyz) * asfloat(r1.yyy));
				r1.xyw = asuint(asfloat(r5.xyz) * asfloat(r1.xxx) + asfloat(r4.xyz));
				r1.xyz = asuint(asfloat(r2.xyz) * asfloat(r1.zzz) + asfloat(r1.xyw));
				r4.xyz = asuint(asfloat(r3.xyz) * asfloat(r1.yyy));
				r1.xyw = asuint(asfloat(r5.xyz) * asfloat(r1.xxx) + asfloat(r4.xyz));
				r1.xyz = asuint(asfloat(r2.xyz) * asfloat(r1.zzz) + asfloat(r1.xyw));
				r4.xyz = asuint(unity_MatrixV._m01_m11_m21 * asfloat(r1.yyy));
				r1.xyw = asuint(unity_MatrixV._m00_m10_m20 * asfloat(r1.xxx) + asfloat(r4.xyz));
				o_TEXCOORD3 = unity_MatrixV._m02_m12_m22 * asfloat(r1.zzz) + asfloat(r1.xyw);
				r1.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.yyy));
				r0.xyw = asuint(asfloat(r5.xyz) * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.zzz) + asfloat(r0.xyw));
				r1.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.yyy));
				r0.xyw = asuint(asfloat(r5.xyz) * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.zzz) + asfloat(r0.xyw));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_MatrixV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				o_TEXCOORD4 = unity_MatrixV._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw);
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK)
			float4 _GlobalsPad0[1];
			float4x4 _Camera_to_World;
			float4 _GlobalsPad1[3];
			float4 _LightColor0;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 unity_SHAr : packoffset(c39);
				float4 unity_SHAg : packoffset(c40);
				float4 unity_SHAb : packoffset(c41);
				float4 unity_OcclusionMaskSelector : packoffset(c46);
			}
			cbuffer UnityShadows : register(b3)
			{
				float4 _LightShadowData : packoffset(c24);
				float4 unity_ShadowFadeCenterAndType : packoffset(c25);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}
			cbuffer UnityProbeVolume : register(b5)
			{
				float4 unity_ProbeVolumeParams : packoffset(c0);
				float4x4 unity_ProbeVolumeWorldToObject : packoffset(c1);
				float3 unity_ProbeVolumeSizeInv : packoffset(c5);
				float3 unity_ProbeVolumeMin : packoffset(c6);
			}
			Texture2D<float4> _ShadowMapTexture : register(t0);
			Texture2D<float4> unity_ShadowMask : register(t1);
			Texture3D<float4> unity_ProbeVolumeSH : register(t2);
			SamplerState samplerunity_ShadowMask : register(s0);
			SamplerState samplerunity_ProbeVolumeSH : register(s1);
			SamplerState sampler_ShadowMapTexture : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float3 i_TEXCOORD4 : TEXCOORD4, float4 i_TEXCOORD6 : TEXCOORD6, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
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
				r0.x = asuint(dot(i_TEXCOORD0.xy, i_TEXCOORD0.xy));
				r0.x = asuint(asfloat(r0.x) / i_TEXCOORD0.w);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				if (b0.y) discard;
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(i_TEXCOORD0.z * asfloat(r0.x) + 1.0);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.yzw = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				r1.xyz = asuint(asfloat(r0.xxx) * i_TEXCOORD4.xyz + i_TEXCOORD3.xyz);
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r2.xy = asuint(glstate_matrix_projection._m21_m31 * asfloat(r0.zz));
				r2.xy = asuint(glstate_matrix_projection._m20_m30 * asfloat(r0.yy) + asfloat(r2.xy));
				r2.xy = asuint(glstate_matrix_projection._m22_m32 * asfloat(r0.ww) + asfloat(r2.xy));
				r2.xy = asuint(glstate_matrix_projection._m23_m33 + asfloat(r2.xy));
				r3.x = asuint(dot(unity_MatrixV._m00_m10_m20, asfloat(r1.xyz)));
				r3.y = asuint(dot(unity_MatrixV._m01_m11_m21, asfloat(r1.xyz)));
				r3.z = asuint(dot(unity_MatrixV._m02_m12_m22, asfloat(r1.xyz)));
				b0.x = (unity_ProbeVolumeParams.x == 1.0);
				r0.x = b0.x ? 0xffffffffu : 0u;
				[branch] if (b0.x)
				{
					r4.xyz = asuint(_Camera_to_World._m01_m11_m21 * asfloat(r0.zzz));
					r4.xyz = asuint(_Camera_to_World._m00_m10_m20 * asfloat(r0.yyy) + asfloat(r4.xyz));
					r0.yzw = asuint(_Camera_to_World._m02_m12_m22 * asfloat(r0.www) + asfloat(r4.xyz));
					r0.yzw = asuint(_Camera_to_World._m03_m13_m23 + asfloat(r0.yzw));
					b1.w = (unity_ProbeVolumeParams.y == 1.0);
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m01_m11_m21 * asfloat(r0.zzz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m00_m10_m20 * asfloat(r0.yyy) + asfloat(r4.xyz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m02_m12_m22 * asfloat(r0.www) + asfloat(r4.xyz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23 + asfloat(r4.xyz));
					r0.yzw = b1.www ? r4.xyz : r0.yzw;
					r0.yzw = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r0.yzw));
					r4.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r0.yzw));
					r0.y = asuint(0.25 * asfloat(r4.y));
					r0.z = asuint(unity_ProbeVolumeParams.z * 0.5);
					r0.w = asuint(0.25 - unity_ProbeVolumeParams.z * 0.5);
					r0.y = asuint(max(asfloat(r0.y), asfloat(r0.z)));
					r4.x = asuint(min(asfloat(r0.y), asfloat(r0.w)));
					r5 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r4.xzw)));
					r0.yzw = asuint(float3(0.25, 0.0, 0.0) + asfloat(r4.xzw));
					r6 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r0.yzw)));
					r0.yzw = asuint(float3(0.5, 0.0, 0.0) + asfloat(r4.xzw));
					r4 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r0.yzw)));
					r3.w = 0x3f800000u;
					r5.x = asuint(dot(asfloat(r5), asfloat(r3)));
					r5.y = asuint(dot(asfloat(r6), asfloat(r3)));
					r5.z = asuint(dot(asfloat(r4), asfloat(r3)));
				}
				else
				{
					r3.w = 0x3f800000u;
					r5.x = asuint(dot(unity_SHAr.xyzw, asfloat(r3)));
					r5.y = asuint(dot(unity_SHAg.xyzw, asfloat(r3)));
					r5.z = asuint(dot(unity_SHAb.xyzw, asfloat(r3)));
				}
				r0.yzw = asuint(max(float3(0.0, 0.0, 0.0), asfloat(r5.xyz)));
				r1.w = asuint(dot(i_TEXCOORD2.xyz, i_TEXCOORD2.xyz));
				r1.w = asuint(rsqrt(asfloat(r1.w)));
				r3.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r1.www));
				r1.x = asuint(saturate(dot(asfloat(r1.xyz), asfloat(r3.xyz))));
				r3.x = asuint(unity_MatrixV._m20);
				r3.y = asuint(unity_MatrixV._m21);
				r3.z = asuint(unity_MatrixV._m22);
				r1.y = asuint(dot(_WorldSpaceCameraPos.xyz, asfloat(r3.xyz)));
				r1.z = asuint(dot(-unity_ShadowFadeCenterAndType.xyz, -unity_ShadowFadeCenterAndType.xyz));
				r1.z = asuint(sqrt(asfloat(r1.z)));
				r1.z = asuint(asfloat(r1.z) + -asfloat(r1.y));
				r1.y = asuint(unity_ShadowFadeCenterAndType.w * asfloat(r1.z) + asfloat(r1.y));
				r1.y = asuint(saturate(asfloat(r1.y) * _LightShadowData.z + _LightShadowData.w));
				[branch] if (r0.x != 0)
				{
					b0.x = (unity_ProbeVolumeParams.y == 1.0);
					r3.xyz = b0.xxx ? asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23) : uint3(0u, 0u, 0u);
					r3.xyz = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r3.xyz));
					r3.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r3.xyz));
					r0.x = asuint(asfloat(r3.y) * 0.25 + 0.75);
					r1.z = asuint(unity_ProbeVolumeParams.z * 0.5 + 0.75);
					r3.x = asuint(max(asfloat(r1.z), asfloat(r0.x)));
					r3 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r3.xzw)));
				}
				else
				{
					r3 = asuint(unity_ShadowMask.Sample(samplerunity_ShadowMask, i_TEXCOORD6.xy));
				}
				r0.x = asuint(saturate(dot(asfloat(r3), unity_OcclusionMaskSelector.xyzw)));
				r1.zw = asuint(i_TEXCOORD6.zw / i_SV_POSITION0.ww);
				r3 = asuint(_ShadowMapTexture.Sample(sampler_ShadowMapTexture, asfloat(r1.zw)));
				r1.y = asuint(saturate(asfloat(r3.x) + asfloat(r1.y)));
				r0.x = asuint(min(asfloat(r1.y), asfloat(r0.x)));
				r1.xyz = asuint(_LightColor0.xyz * asfloat(r1.xxx));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx) + asfloat(r0.yzw));
				o_SV_Target0.xyz = i_COLOR0.xyz * asfloat(r0.xyz);
				o_SV_Depth0 = asfloat(r2.x) / asfloat(r2.y);
				o_SV_Target0.w = i_COLOR0.w;
			}
			#elif defined(DIRECTIONAL) && defined(LIGHTMAP_SHADOW_MIXING) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN)
			float4 _GlobalsPad0[1];
			float4x4 _Camera_to_World;
			float4 _GlobalsPad1[3];
			float4 _LightColor0;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 unity_SHAr : packoffset(c39);
				float4 unity_SHAg : packoffset(c40);
				float4 unity_SHAb : packoffset(c41);
				float4 unity_OcclusionMaskSelector : packoffset(c46);
			}
			cbuffer UnityShadows : register(b3)
			{
				float4 _LightShadowData : packoffset(c24);
				float4 unity_ShadowFadeCenterAndType : packoffset(c25);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}
			cbuffer UnityProbeVolume : register(b5)
			{
				float4 unity_ProbeVolumeParams : packoffset(c0);
				float4x4 unity_ProbeVolumeWorldToObject : packoffset(c1);
				float3 unity_ProbeVolumeSizeInv : packoffset(c5);
				float3 unity_ProbeVolumeMin : packoffset(c6);
			}
			Texture2D<float4> _ShadowMapTexture : register(t0);
			Texture3D<float4> unity_ProbeVolumeSH : register(t1);
			SamplerState samplerunity_ProbeVolumeSH : register(s0);
			SamplerState sampler_ShadowMapTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float3 i_TEXCOORD4 : TEXCOORD4, float4 i_TEXCOORD6 : TEXCOORD6, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
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
				r0.x = asuint(dot(i_TEXCOORD0.xy, i_TEXCOORD0.xy));
				r0.x = asuint(asfloat(r0.x) / i_TEXCOORD0.w);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				if (b0.y) discard;
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(i_TEXCOORD0.z * asfloat(r0.x) + 1.0);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.yzw = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				r1.xyz = asuint(asfloat(r0.xxx) * i_TEXCOORD4.xyz + i_TEXCOORD3.xyz);
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r2.xy = asuint(glstate_matrix_projection._m21_m31 * asfloat(r0.zz));
				r2.xy = asuint(glstate_matrix_projection._m20_m30 * asfloat(r0.yy) + asfloat(r2.xy));
				r2.xy = asuint(glstate_matrix_projection._m22_m32 * asfloat(r0.ww) + asfloat(r2.xy));
				r2.xy = asuint(glstate_matrix_projection._m23_m33 + asfloat(r2.xy));
				r3.x = asuint(dot(unity_MatrixV._m00_m10_m20, asfloat(r1.xyz)));
				r3.y = asuint(dot(unity_MatrixV._m01_m11_m21, asfloat(r1.xyz)));
				r3.z = asuint(dot(unity_MatrixV._m02_m12_m22, asfloat(r1.xyz)));
				b0.x = (unity_ProbeVolumeParams.x == 1.0);
				r0.x = b0.x ? 0xffffffffu : 0u;
				[branch] if (b0.x)
				{
					r4.xyz = asuint(_Camera_to_World._m01_m11_m21 * asfloat(r0.zzz));
					r4.xyz = asuint(_Camera_to_World._m00_m10_m20 * asfloat(r0.yyy) + asfloat(r4.xyz));
					r0.yzw = asuint(_Camera_to_World._m02_m12_m22 * asfloat(r0.www) + asfloat(r4.xyz));
					r0.yzw = asuint(_Camera_to_World._m03_m13_m23 + asfloat(r0.yzw));
					b1.w = (unity_ProbeVolumeParams.y == 1.0);
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m01_m11_m21 * asfloat(r0.zzz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m00_m10_m20 * asfloat(r0.yyy) + asfloat(r4.xyz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m02_m12_m22 * asfloat(r0.www) + asfloat(r4.xyz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23 + asfloat(r4.xyz));
					r0.yzw = b1.www ? r4.xyz : r0.yzw;
					r0.yzw = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r0.yzw));
					r4.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r0.yzw));
					r0.y = asuint(0.25 * asfloat(r4.y));
					r0.z = asuint(unity_ProbeVolumeParams.z * 0.5);
					r0.w = asuint(0.25 - unity_ProbeVolumeParams.z * 0.5);
					r0.y = asuint(max(asfloat(r0.y), asfloat(r0.z)));
					r4.x = asuint(min(asfloat(r0.y), asfloat(r0.w)));
					r5 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r4.xzw)));
					r0.yzw = asuint(float3(0.25, 0.0, 0.0) + asfloat(r4.xzw));
					r6 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r0.yzw)));
					r0.yzw = asuint(float3(0.5, 0.0, 0.0) + asfloat(r4.xzw));
					r4 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r0.yzw)));
					r3.w = 0x3f800000u;
					r5.x = asuint(dot(asfloat(r5), asfloat(r3)));
					r5.y = asuint(dot(asfloat(r6), asfloat(r3)));
					r5.z = asuint(dot(asfloat(r4), asfloat(r3)));
				}
				else
				{
					r3.w = 0x3f800000u;
					r5.x = asuint(dot(unity_SHAr.xyzw, asfloat(r3)));
					r5.y = asuint(dot(unity_SHAg.xyzw, asfloat(r3)));
					r5.z = asuint(dot(unity_SHAb.xyzw, asfloat(r3)));
				}
				r0.yzw = asuint(max(float3(0.0, 0.0, 0.0), asfloat(r5.xyz)));
				r1.w = asuint(dot(i_TEXCOORD2.xyz, i_TEXCOORD2.xyz));
				r1.w = asuint(rsqrt(asfloat(r1.w)));
				r3.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r1.www));
				r1.x = asuint(saturate(dot(asfloat(r1.xyz), asfloat(r3.xyz))));
				r3.x = asuint(unity_MatrixV._m20);
				r3.y = asuint(unity_MatrixV._m21);
				r3.z = asuint(unity_MatrixV._m22);
				r1.y = asuint(dot(_WorldSpaceCameraPos.xyz, asfloat(r3.xyz)));
				r1.z = asuint(dot(-unity_ShadowFadeCenterAndType.xyz, -unity_ShadowFadeCenterAndType.xyz));
				r1.z = asuint(sqrt(asfloat(r1.z)));
				r1.z = asuint(asfloat(r1.z) + -asfloat(r1.y));
				r1.y = asuint(unity_ShadowFadeCenterAndType.w * asfloat(r1.z) + asfloat(r1.y));
				r1.y = asuint(saturate(asfloat(r1.y) * _LightShadowData.z + _LightShadowData.w));
				[branch] if (r0.x != 0)
				{
					b0.x = (unity_ProbeVolumeParams.y == 1.0);
					r3.xyz = b0.xxx ? asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23) : uint3(0u, 0u, 0u);
					r3.xyz = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r3.xyz));
					r3.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r3.xyz));
					r0.x = asuint(asfloat(r3.y) * 0.25 + 0.75);
					r1.z = asuint(unity_ProbeVolumeParams.z * 0.5 + 0.75);
					r3.x = asuint(max(asfloat(r1.z), asfloat(r0.x)));
					r3 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r3.xzw)));
				}
				else
				{
					r3 = uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u);
				}
				r0.x = asuint(saturate(dot(asfloat(r3), unity_OcclusionMaskSelector.xyzw)));
				r1.zw = asuint(i_TEXCOORD6.xy / i_TEXCOORD6.ww);
				r3 = asuint(_ShadowMapTexture.Sample(sampler_ShadowMapTexture, asfloat(r1.zw)));
				r1.y = asuint(saturate(asfloat(r3.x) + asfloat(r1.y)));
				r0.x = asuint(min(asfloat(r1.y), asfloat(r0.x)));
				r1.xyz = asuint(_LightColor0.xyz * asfloat(r1.xxx));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx) + asfloat(r0.yzw));
				o_SV_Target0.xyz = i_COLOR0.xyz * asfloat(r0.xyz);
				o_SV_Depth0 = asfloat(r2.x) / asfloat(r2.y);
				o_SV_Target0.w = i_COLOR0.w;
			}
			#elif defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN) && defined(SHADOWS_SHADOWMASK)
			float4 _GlobalsPad0[1];
			float4x4 _Camera_to_World;
			float4 _GlobalsPad1[3];
			float4 _LightColor0;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 unity_SHAr : packoffset(c39);
				float4 unity_SHAg : packoffset(c40);
				float4 unity_SHAb : packoffset(c41);
				float4 unity_OcclusionMaskSelector : packoffset(c46);
			}
			cbuffer UnityShadows : register(b3)
			{
				float4 _LightShadowData : packoffset(c24);
				float4 unity_ShadowFadeCenterAndType : packoffset(c25);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}
			cbuffer UnityProbeVolume : register(b5)
			{
				float4 unity_ProbeVolumeParams : packoffset(c0);
				float4x4 unity_ProbeVolumeWorldToObject : packoffset(c1);
				float3 unity_ProbeVolumeSizeInv : packoffset(c5);
				float3 unity_ProbeVolumeMin : packoffset(c6);
			}
			Texture2D<float4> _ShadowMapTexture : register(t0);
			Texture2D<float4> unity_ShadowMask : register(t1);
			Texture3D<float4> unity_ProbeVolumeSH : register(t2);
			SamplerState samplerunity_ShadowMask : register(s0);
			SamplerState samplerunity_ProbeVolumeSH : register(s1);
			SamplerState sampler_ShadowMapTexture : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float3 i_TEXCOORD4 : TEXCOORD4, float4 i_TEXCOORD6 : TEXCOORD6, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
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
				r0.x = asuint(dot(i_TEXCOORD0.xy, i_TEXCOORD0.xy));
				r0.x = asuint(asfloat(r0.x) / i_TEXCOORD0.w);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				if (b0.y) discard;
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(i_TEXCOORD0.z * asfloat(r0.x) + 1.0);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.yzw = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				r1.xyz = asuint(asfloat(r0.xxx) * i_TEXCOORD4.xyz + i_TEXCOORD3.xyz);
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r2.xy = asuint(glstate_matrix_projection._m21_m31 * asfloat(r0.zz));
				r2.xy = asuint(glstate_matrix_projection._m20_m30 * asfloat(r0.yy) + asfloat(r2.xy));
				r2.xy = asuint(glstate_matrix_projection._m22_m32 * asfloat(r0.ww) + asfloat(r2.xy));
				r2.xy = asuint(glstate_matrix_projection._m23_m33 + asfloat(r2.xy));
				r3.x = asuint(dot(unity_MatrixV._m00_m10_m20, asfloat(r1.xyz)));
				r3.y = asuint(dot(unity_MatrixV._m01_m11_m21, asfloat(r1.xyz)));
				r3.z = asuint(dot(unity_MatrixV._m02_m12_m22, asfloat(r1.xyz)));
				b0.x = (unity_ProbeVolumeParams.x == 1.0);
				r0.x = b0.x ? 0xffffffffu : 0u;
				[branch] if (b0.x)
				{
					r4.xyz = asuint(_Camera_to_World._m01_m11_m21 * asfloat(r0.zzz));
					r4.xyz = asuint(_Camera_to_World._m00_m10_m20 * asfloat(r0.yyy) + asfloat(r4.xyz));
					r0.yzw = asuint(_Camera_to_World._m02_m12_m22 * asfloat(r0.www) + asfloat(r4.xyz));
					r0.yzw = asuint(_Camera_to_World._m03_m13_m23 + asfloat(r0.yzw));
					b1.w = (unity_ProbeVolumeParams.y == 1.0);
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m01_m11_m21 * asfloat(r0.zzz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m00_m10_m20 * asfloat(r0.yyy) + asfloat(r4.xyz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m02_m12_m22 * asfloat(r0.www) + asfloat(r4.xyz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23 + asfloat(r4.xyz));
					r0.yzw = b1.www ? r4.xyz : r0.yzw;
					r0.yzw = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r0.yzw));
					r4.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r0.yzw));
					r0.y = asuint(0.25 * asfloat(r4.y));
					r0.z = asuint(unity_ProbeVolumeParams.z * 0.5);
					r0.w = asuint(0.25 - unity_ProbeVolumeParams.z * 0.5);
					r0.y = asuint(max(asfloat(r0.y), asfloat(r0.z)));
					r4.x = asuint(min(asfloat(r0.y), asfloat(r0.w)));
					r5 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r4.xzw)));
					r0.yzw = asuint(float3(0.25, 0.0, 0.0) + asfloat(r4.xzw));
					r6 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r0.yzw)));
					r0.yzw = asuint(float3(0.5, 0.0, 0.0) + asfloat(r4.xzw));
					r4 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r0.yzw)));
					r3.w = 0x3f800000u;
					r5.x = asuint(dot(asfloat(r5), asfloat(r3)));
					r5.y = asuint(dot(asfloat(r6), asfloat(r3)));
					r5.z = asuint(dot(asfloat(r4), asfloat(r3)));
				}
				else
				{
					r3.w = 0x3f800000u;
					r5.x = asuint(dot(unity_SHAr.xyzw, asfloat(r3)));
					r5.y = asuint(dot(unity_SHAg.xyzw, asfloat(r3)));
					r5.z = asuint(dot(unity_SHAb.xyzw, asfloat(r3)));
				}
				r0.yzw = asuint(max(float3(0.0, 0.0, 0.0), asfloat(r5.xyz)));
				r1.w = asuint(dot(i_TEXCOORD2.xyz, i_TEXCOORD2.xyz));
				r1.w = asuint(rsqrt(asfloat(r1.w)));
				r3.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r1.www));
				r1.x = asuint(saturate(dot(asfloat(r1.xyz), asfloat(r3.xyz))));
				r3.x = asuint(unity_MatrixV._m20);
				r3.y = asuint(unity_MatrixV._m21);
				r3.z = asuint(unity_MatrixV._m22);
				r1.y = asuint(dot(_WorldSpaceCameraPos.xyz, asfloat(r3.xyz)));
				r1.z = asuint(dot(-unity_ShadowFadeCenterAndType.xyz, -unity_ShadowFadeCenterAndType.xyz));
				r1.z = asuint(sqrt(asfloat(r1.z)));
				r1.z = asuint(asfloat(r1.z) + -asfloat(r1.y));
				r1.y = asuint(unity_ShadowFadeCenterAndType.w * asfloat(r1.z) + asfloat(r1.y));
				r1.y = asuint(saturate(asfloat(r1.y) * _LightShadowData.z + _LightShadowData.w));
				[branch] if (r0.x != 0)
				{
					b0.x = (unity_ProbeVolumeParams.y == 1.0);
					r3.xyz = b0.xxx ? asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23) : uint3(0u, 0u, 0u);
					r3.xyz = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r3.xyz));
					r3.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r3.xyz));
					r0.x = asuint(asfloat(r3.y) * 0.25 + 0.75);
					r1.z = asuint(unity_ProbeVolumeParams.z * 0.5 + 0.75);
					r3.x = asuint(max(asfloat(r1.z), asfloat(r0.x)));
					r3 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r3.xzw)));
				}
				else
				{
					r3 = asuint(unity_ShadowMask.Sample(samplerunity_ShadowMask, i_TEXCOORD6.xy));
				}
				r0.x = asuint(saturate(dot(asfloat(r3), unity_OcclusionMaskSelector.xyzw)));
				r1.zw = asuint(i_TEXCOORD6.zw / i_SV_POSITION0.ww);
				r3 = asuint(_ShadowMapTexture.Sample(sampler_ShadowMapTexture, asfloat(r1.zw)));
				r0.x = asuint(-asfloat(r3.x) + asfloat(r0.x));
				r0.x = asuint(asfloat(r1.y) * asfloat(r0.x) + asfloat(r3.x));
				r1.xyz = asuint(_LightColor0.xyz * asfloat(r1.xxx));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx) + asfloat(r0.yzw));
				o_SV_Target0.xyz = i_COLOR0.xyz * asfloat(r0.xyz);
				o_SV_Depth0 = asfloat(r2.x) / asfloat(r2.y);
				o_SV_Target0.w = i_COLOR0.w;
			}
			#elif defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(SHADOWS_SCREEN)
			float4 _GlobalsPad0[1];
			float4x4 _Camera_to_World;
			float4 _GlobalsPad1[3];
			float4 _LightColor0;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 unity_SHAr : packoffset(c39);
				float4 unity_SHAg : packoffset(c40);
				float4 unity_SHAb : packoffset(c41);
				float4 unity_OcclusionMaskSelector : packoffset(c46);
			}
			cbuffer UnityShadows : register(b3)
			{
				float4 _LightShadowData : packoffset(c24);
				float4 unity_ShadowFadeCenterAndType : packoffset(c25);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}
			cbuffer UnityProbeVolume : register(b5)
			{
				float4 unity_ProbeVolumeParams : packoffset(c0);
				float4x4 unity_ProbeVolumeWorldToObject : packoffset(c1);
				float3 unity_ProbeVolumeSizeInv : packoffset(c5);
				float3 unity_ProbeVolumeMin : packoffset(c6);
			}
			Texture2D<float4> _ShadowMapTexture : register(t0);
			Texture3D<float4> unity_ProbeVolumeSH : register(t1);
			SamplerState samplerunity_ProbeVolumeSH : register(s0);
			SamplerState sampler_ShadowMapTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float3 i_TEXCOORD4 : TEXCOORD4, float4 i_TEXCOORD6 : TEXCOORD6, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
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
				r0.x = asuint(dot(i_TEXCOORD0.xy, i_TEXCOORD0.xy));
				r0.x = asuint(asfloat(r0.x) / i_TEXCOORD0.w);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				if (b0.y) discard;
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(i_TEXCOORD0.z * asfloat(r0.x) + 1.0);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.yzw = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				r1.xyz = asuint(asfloat(r0.xxx) * i_TEXCOORD4.xyz + i_TEXCOORD3.xyz);
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r2.xy = asuint(glstate_matrix_projection._m21_m31 * asfloat(r0.zz));
				r2.xy = asuint(glstate_matrix_projection._m20_m30 * asfloat(r0.yy) + asfloat(r2.xy));
				r2.xy = asuint(glstate_matrix_projection._m22_m32 * asfloat(r0.ww) + asfloat(r2.xy));
				r2.xy = asuint(glstate_matrix_projection._m23_m33 + asfloat(r2.xy));
				r3.x = asuint(dot(unity_MatrixV._m00_m10_m20, asfloat(r1.xyz)));
				r3.y = asuint(dot(unity_MatrixV._m01_m11_m21, asfloat(r1.xyz)));
				r3.z = asuint(dot(unity_MatrixV._m02_m12_m22, asfloat(r1.xyz)));
				b0.x = (unity_ProbeVolumeParams.x == 1.0);
				r0.x = b0.x ? 0xffffffffu : 0u;
				[branch] if (b0.x)
				{
					r4.xyz = asuint(_Camera_to_World._m01_m11_m21 * asfloat(r0.zzz));
					r4.xyz = asuint(_Camera_to_World._m00_m10_m20 * asfloat(r0.yyy) + asfloat(r4.xyz));
					r0.yzw = asuint(_Camera_to_World._m02_m12_m22 * asfloat(r0.www) + asfloat(r4.xyz));
					r0.yzw = asuint(_Camera_to_World._m03_m13_m23 + asfloat(r0.yzw));
					b1.w = (unity_ProbeVolumeParams.y == 1.0);
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m01_m11_m21 * asfloat(r0.zzz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m00_m10_m20 * asfloat(r0.yyy) + asfloat(r4.xyz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m02_m12_m22 * asfloat(r0.www) + asfloat(r4.xyz));
					r4.xyz = asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23 + asfloat(r4.xyz));
					r0.yzw = b1.www ? r4.xyz : r0.yzw;
					r0.yzw = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r0.yzw));
					r4.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r0.yzw));
					r0.y = asuint(0.25 * asfloat(r4.y));
					r0.z = asuint(unity_ProbeVolumeParams.z * 0.5);
					r0.w = asuint(0.25 - unity_ProbeVolumeParams.z * 0.5);
					r0.y = asuint(max(asfloat(r0.y), asfloat(r0.z)));
					r4.x = asuint(min(asfloat(r0.y), asfloat(r0.w)));
					r5 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r4.xzw)));
					r0.yzw = asuint(float3(0.25, 0.0, 0.0) + asfloat(r4.xzw));
					r6 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r0.yzw)));
					r0.yzw = asuint(float3(0.5, 0.0, 0.0) + asfloat(r4.xzw));
					r4 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r0.yzw)));
					r3.w = 0x3f800000u;
					r5.x = asuint(dot(asfloat(r5), asfloat(r3)));
					r5.y = asuint(dot(asfloat(r6), asfloat(r3)));
					r5.z = asuint(dot(asfloat(r4), asfloat(r3)));
				}
				else
				{
					r3.w = 0x3f800000u;
					r5.x = asuint(dot(unity_SHAr.xyzw, asfloat(r3)));
					r5.y = asuint(dot(unity_SHAg.xyzw, asfloat(r3)));
					r5.z = asuint(dot(unity_SHAb.xyzw, asfloat(r3)));
				}
				r0.yzw = asuint(max(float3(0.0, 0.0, 0.0), asfloat(r5.xyz)));
				r1.w = asuint(dot(i_TEXCOORD2.xyz, i_TEXCOORD2.xyz));
				r1.w = asuint(rsqrt(asfloat(r1.w)));
				r3.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r1.www));
				r1.x = asuint(saturate(dot(asfloat(r1.xyz), asfloat(r3.xyz))));
				r3.x = asuint(unity_MatrixV._m20);
				r3.y = asuint(unity_MatrixV._m21);
				r3.z = asuint(unity_MatrixV._m22);
				r1.y = asuint(dot(_WorldSpaceCameraPos.xyz, asfloat(r3.xyz)));
				r1.z = asuint(dot(-unity_ShadowFadeCenterAndType.xyz, -unity_ShadowFadeCenterAndType.xyz));
				r1.z = asuint(sqrt(asfloat(r1.z)));
				r1.z = asuint(asfloat(r1.z) + -asfloat(r1.y));
				r1.y = asuint(unity_ShadowFadeCenterAndType.w * asfloat(r1.z) + asfloat(r1.y));
				r1.y = asuint(saturate(asfloat(r1.y) * _LightShadowData.z + _LightShadowData.w));
				[branch] if (r0.x != 0)
				{
					b0.x = (unity_ProbeVolumeParams.y == 1.0);
					r3.xyz = b0.xxx ? asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23) : uint3(0u, 0u, 0u);
					r3.xyz = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r3.xyz));
					r3.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r3.xyz));
					r0.x = asuint(asfloat(r3.y) * 0.25 + 0.75);
					r1.z = asuint(unity_ProbeVolumeParams.z * 0.5 + 0.75);
					r3.x = asuint(max(asfloat(r1.z), asfloat(r0.x)));
					r3 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r3.xzw)));
				}
				else
				{
					r3 = uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u);
				}
				r0.x = asuint(saturate(dot(asfloat(r3), unity_OcclusionMaskSelector.xyzw)));
				r1.zw = asuint(i_TEXCOORD6.xy / i_TEXCOORD6.ww);
				r3 = asuint(_ShadowMapTexture.Sample(sampler_ShadowMapTexture, asfloat(r1.zw)));
				r0.x = asuint(-asfloat(r3.x) + asfloat(r0.x));
				r0.x = asuint(asfloat(r1.y) * asfloat(r0.x) + asfloat(r3.x));
				r1.xyz = asuint(_LightColor0.xyz * asfloat(r1.xxx));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx) + asfloat(r0.yzw));
				o_SV_Target0.xyz = i_COLOR0.xyz * asfloat(r0.xyz);
				o_SV_Depth0 = asfloat(r2.x) / asfloat(r2.y);
				o_SV_Target0.w = i_COLOR0.w;
			}
			#elif defined(DIRECTIONAL) && defined(LIGHTPROBE_SH)
			float4 _GlobalsPad0[8];
			float4 _LightColor0;
			cbuffer UnityLighting : register(b1)
			{
				float4 unity_OcclusionMaskSelector : packoffset(c46);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
			}
			cbuffer UnityProbeVolume : register(b3)
			{
				float4 unity_ProbeVolumeParams : packoffset(c0);
				float4x4 unity_ProbeVolumeWorldToObject : packoffset(c1);
				float3 unity_ProbeVolumeSizeInv : packoffset(c5);
				float3 unity_ProbeVolumeMin : packoffset(c6);
			}
			Texture3D<float4> unity_ProbeVolumeSH : register(t0);
			SamplerState samplerunity_ProbeVolumeSH : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float3 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0.x = asuint(dot(i_TEXCOORD0.xy, i_TEXCOORD0.xy));
				r0.x = asuint(asfloat(r0.x) / i_TEXCOORD0.w);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				if (b0.y) discard;
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(i_TEXCOORD0.z * asfloat(r0.x) + 1.0);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.yzw = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				r1.xyz = asuint(asfloat(r0.xxx) * i_TEXCOORD4.xyz + i_TEXCOORD3.xyz);
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r0.xz = asuint(glstate_matrix_projection._m21_m31 * asfloat(r0.zz));
				r0.xy = asuint(glstate_matrix_projection._m20_m30 * asfloat(r0.yy) + asfloat(r0.xz));
				r0.xy = asuint(glstate_matrix_projection._m22_m32 * asfloat(r0.ww) + asfloat(r0.xy));
				r0.xy = asuint(glstate_matrix_projection._m23_m33 + asfloat(r0.xy));
				r0.z = asuint(dot(i_TEXCOORD2.xyz, i_TEXCOORD2.xyz));
				r0.z = asuint(rsqrt(asfloat(r0.z)));
				r2.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r0.zzz));
				r0.z = asuint(saturate(dot(asfloat(r1.xyz), asfloat(r2.xyz))));
				b0.w = (unity_ProbeVolumeParams.x == 1.0);
				[branch] if (b0.w)
				{
					b0.w = (unity_ProbeVolumeParams.y == 1.0);
					r1.xyz = b0.www ? asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23) : uint3(0u, 0u, 0u);
					r1.xyz = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r1.xyz));
					r1.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r1.xyz));
					r0.w = asuint(asfloat(r1.y) * 0.25 + 0.75);
					r1.y = asuint(unity_ProbeVolumeParams.z * 0.5 + 0.75);
					r1.x = asuint(max(asfloat(r1.y), asfloat(r0.w)));
					r1 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r1.xzw)));
				}
				else
				{
					r1 = uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u);
				}
				r0.w = asuint(saturate(dot(asfloat(r1), unity_OcclusionMaskSelector.xyzw)));
				r1.xyz = asuint(_LightColor0.xyz * asfloat(r0.zzz));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www));
				o_SV_Target0.xyz = i_COLOR0.xyz * asfloat(r1.xyz);
				o_SV_Depth0 = asfloat(r0.x) / asfloat(r0.y);
				o_SV_Target0.w = i_COLOR0.w;
			}
			#elif defined(DIRECTIONAL) && defined(SHADOWS_SCREEN)
			float4 _GlobalsPad0[8];
			float4 _LightColor0;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 unity_OcclusionMaskSelector : packoffset(c46);
			}
			cbuffer UnityShadows : register(b3)
			{
				float4 _LightShadowData : packoffset(c24);
				float4 unity_ShadowFadeCenterAndType : packoffset(c25);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4 glstate_lightmodel_ambient : packoffset(c0);
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}
			cbuffer UnityProbeVolume : register(b5)
			{
				float4 unity_ProbeVolumeParams : packoffset(c0);
				float4x4 unity_ProbeVolumeWorldToObject : packoffset(c1);
				float3 unity_ProbeVolumeSizeInv : packoffset(c5);
				float3 unity_ProbeVolumeMin : packoffset(c6);
			}
			Texture2D<float4> _ShadowMapTexture : register(t0);
			Texture3D<float4> unity_ProbeVolumeSH : register(t1);
			SamplerState samplerunity_ProbeVolumeSH : register(s0);
			SamplerState sampler_ShadowMapTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float3 i_TEXCOORD4 : TEXCOORD4, float4 i_TEXCOORD6 : TEXCOORD6, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b1;
				r0.x = asuint(dot(i_TEXCOORD0.xy, i_TEXCOORD0.xy));
				r0.x = asuint(asfloat(r0.x) / i_TEXCOORD0.w);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				if (b0.y) discard;
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(i_TEXCOORD0.z * asfloat(r0.x) + 1.0);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.yzw = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				r1.xyz = asuint(asfloat(r0.xxx) * i_TEXCOORD4.xyz + i_TEXCOORD3.xyz);
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r0.xz = asuint(glstate_matrix_projection._m21_m31 * asfloat(r0.zz));
				r0.xy = asuint(glstate_matrix_projection._m20_m30 * asfloat(r0.yy) + asfloat(r0.xz));
				r0.xy = asuint(glstate_matrix_projection._m22_m32 * asfloat(r0.ww) + asfloat(r0.xy));
				r0.xy = asuint(glstate_matrix_projection._m23_m33 + asfloat(r0.xy));
				r2.xyz = asuint(glstate_lightmodel_ambient.xyz + glstate_lightmodel_ambient.xyz);
				r0.z = asuint(dot(i_TEXCOORD2.xyz, i_TEXCOORD2.xyz));
				r0.z = asuint(rsqrt(asfloat(r0.z)));
				r3.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r0.zzz));
				r0.z = asuint(saturate(dot(asfloat(r1.xyz), asfloat(r3.xyz))));
				r1.x = asuint(unity_MatrixV._m20);
				r1.y = asuint(unity_MatrixV._m21);
				r1.z = asuint(unity_MatrixV._m22);
				r0.w = asuint(dot(_WorldSpaceCameraPos.xyz, asfloat(r1.xyz)));
				r1.x = asuint(dot(-unity_ShadowFadeCenterAndType.xyz, -unity_ShadowFadeCenterAndType.xyz));
				r1.x = asuint(sqrt(asfloat(r1.x)));
				r1.x = asuint(asfloat(r1.x) + -asfloat(r0.w));
				r0.w = asuint(unity_ShadowFadeCenterAndType.w * asfloat(r1.x) + asfloat(r0.w));
				r0.w = asuint(saturate(asfloat(r0.w) * _LightShadowData.z + _LightShadowData.w));
				b1.x = (unity_ProbeVolumeParams.x == 1.0);
				[branch] if (b1.x)
				{
					b1.x = (unity_ProbeVolumeParams.y == 1.0);
					r1.xyz = b1.xxx ? asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23) : uint3(0u, 0u, 0u);
					r1.xyz = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r1.xyz));
					r1.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r1.xyz));
					r1.y = asuint(asfloat(r1.y) * 0.25 + 0.75);
					r2.w = asuint(unity_ProbeVolumeParams.z * 0.5 + 0.75);
					r1.x = asuint(max(asfloat(r2.w), asfloat(r1.y)));
					r1 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r1.xzw)));
				}
				else
				{
					r1 = uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u);
				}
				r1.x = asuint(saturate(dot(asfloat(r1), unity_OcclusionMaskSelector.xyzw)));
				r1.yz = asuint(i_TEXCOORD6.xy / i_TEXCOORD6.ww);
				r3 = asuint(_ShadowMapTexture.Sample(sampler_ShadowMapTexture, asfloat(r1.yz)));
				r1.x = asuint(-asfloat(r3.x) + asfloat(r1.x));
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.x) + asfloat(r3.x));
				r1.xyz = asuint(_LightColor0.xyz * asfloat(r0.zzz));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www) + asfloat(r2.xyz));
				o_SV_Target0.xyz = i_COLOR0.xyz * asfloat(r1.xyz);
				o_SV_Depth0 = asfloat(r0.x) / asfloat(r0.y);
				o_SV_Target0.w = i_COLOR0.w;
			}
			#elif defined(DIRECTIONAL)
			float4 _GlobalsPad0[8];
			float4 _LightColor0;
			cbuffer UnityLighting : register(b1)
			{
				float4 unity_OcclusionMaskSelector : packoffset(c46);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4 glstate_lightmodel_ambient : packoffset(c0);
				float4x4 glstate_matrix_projection : packoffset(c5);
			}
			cbuffer UnityProbeVolume : register(b3)
			{
				float4 unity_ProbeVolumeParams : packoffset(c0);
				float4x4 unity_ProbeVolumeWorldToObject : packoffset(c1);
				float3 unity_ProbeVolumeSizeInv : packoffset(c5);
				float3 unity_ProbeVolumeMin : packoffset(c6);
			}
			Texture3D<float4> unity_ProbeVolumeSH : register(t0);
			SamplerState samplerunity_ProbeVolumeSH : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float3 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				r0.x = asuint(dot(i_TEXCOORD0.xy, i_TEXCOORD0.xy));
				r0.x = asuint(asfloat(r0.x) / i_TEXCOORD0.w);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				if (b0.y) discard;
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(i_TEXCOORD0.z * asfloat(r0.x) + 1.0);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.yzw = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				r1.xyz = asuint(asfloat(r0.xxx) * i_TEXCOORD4.xyz + i_TEXCOORD3.xyz);
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r0.xz = asuint(glstate_matrix_projection._m21_m31 * asfloat(r0.zz));
				r0.xy = asuint(glstate_matrix_projection._m20_m30 * asfloat(r0.yy) + asfloat(r0.xz));
				r0.xy = asuint(glstate_matrix_projection._m22_m32 * asfloat(r0.ww) + asfloat(r0.xy));
				r0.xy = asuint(glstate_matrix_projection._m23_m33 + asfloat(r0.xy));
				r2.xyz = asuint(glstate_lightmodel_ambient.xyz + glstate_lightmodel_ambient.xyz);
				r0.z = asuint(dot(i_TEXCOORD2.xyz, i_TEXCOORD2.xyz));
				r0.z = asuint(rsqrt(asfloat(r0.z)));
				r3.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r0.zzz));
				r0.z = asuint(saturate(dot(asfloat(r1.xyz), asfloat(r3.xyz))));
				b0.w = (unity_ProbeVolumeParams.x == 1.0);
				[branch] if (b0.w)
				{
					b0.w = (unity_ProbeVolumeParams.y == 1.0);
					r1.xyz = b0.www ? asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23) : uint3(0u, 0u, 0u);
					r1.xyz = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r1.xyz));
					r1.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r1.xyz));
					r0.w = asuint(asfloat(r1.y) * 0.25 + 0.75);
					r1.y = asuint(unity_ProbeVolumeParams.z * 0.5 + 0.75);
					r1.x = asuint(max(asfloat(r1.y), asfloat(r0.w)));
					r1 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r1.xzw)));
				}
				else
				{
					r1 = uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u);
				}
				r0.w = asuint(saturate(dot(asfloat(r1), unity_OcclusionMaskSelector.xyzw)));
				r1.xyz = asuint(_LightColor0.xyz * asfloat(r0.zzz));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www) + asfloat(r2.xyz));
				o_SV_Target0.xyz = i_COLOR0.xyz * asfloat(r1.xyz);
				o_SV_Depth0 = asfloat(r0.x) / asfloat(r0.y);
				o_SV_Target0.w = i_COLOR0.w;
			}
			#endif
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "ShadowCaster"
			Tags { "LIGHTMODE" = "SHADOWCASTER" "SHADOWSUPPORT" = "true" }
			Cull Back
			ZTest LEqual
			ZWrite On
			Offset 1, 1
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ SHADOWS_CUBE
			#pragma multi_compile __ SHADOWS_DEPTH

			#if defined(SHADER_STAGE_VERTEX)
			float _RadiusScale;
			float _GlobalsPad0;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[6];
			float4 _Color;
			cbuffer UnityPerFrame : register(b1)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
				float4x4 unity_MatrixInvV : packoffset(c13);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_COLOR0 : COLOR0, out float4 o_TEXCOORD0 : TEXCOORD0, out float3 o_TEXCOORD1 : TEXCOORD1, out float4 o_SV_POSITION0 : SV_POSITION)
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
				bool4 b0;
				bool4 b1;
				o_COLOR0 = _Color.xyzw * i_COLOR0.xyzw;
				r0.xy = asuint(unity_MatrixV._m01_m11 * i_POSITION0.yy);
				r0.xy = asuint(unity_MatrixV._m00_m10 * i_POSITION0.xx + asfloat(r0.xy));
				r0.xy = asuint(unity_MatrixV._m02_m12 * i_POSITION0.zz + asfloat(r0.xy));
				r0.xy = asuint(unity_MatrixV._m03_m13 + asfloat(r0.xy));
				r0.yzw = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyz = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r0.yzw));
				r0.xyz = asuint(unity_MatrixInvV._m03_m13_m23 + asfloat(r0.xyz));
				b0.w = (0.0 == glstate_matrix_projection._m30);
				b1.x = (0.0 == glstate_matrix_projection._m31);
				b0.w = b1.x && b0.w;
				b1.x = (0.0 == glstate_matrix_projection._m32);
				b0.w = b1.x && b0.w;
				r0.xyz = b0.www ? r0.xyz : asuint(unity_MatrixInvV._m03_m13_m23);
				r0.xyz = asuint(-i_POSITION0.xyz + asfloat(r0.xyz));
				r1.x = asuint(_RadiusScale * i_TEXCOORD1.w);
				r1.y = asuint(1.0 / asfloat(r1.x));
				r1.xzw = asuint(i_TEXCOORD1.xyz * asfloat(r1.xxx));
				r2.xyz = asuint(i_TEXCOORD1.xyz * asfloat(r1.yyy));
				r3.xyz = asuint(i_TEXCOORD1.yyy * asfloat(r2.xyz));
				r1.y = asuint(_RadiusScale * i_TEXCOORD0.w);
				r2.w = asuint(1.0 / asfloat(r1.y));
				r4.xyz = asuint(i_TEXCOORD0.xyz * asfloat(r1.yyy));
				r5.xyz = asuint(i_TEXCOORD0.xyz * asfloat(r2.www));
				r3.xyz = asuint(asfloat(r5.xyz) * i_TEXCOORD0.yyy + asfloat(r3.xyz));
				r1.y = asuint(_RadiusScale * i_TEXCOORD2.w);
				r2.w = asuint(1.0 / asfloat(r1.y));
				r6.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r1.yyy));
				r7.xyz = asuint(i_TEXCOORD2.xyz * asfloat(r2.www));
				r3.xyz = asuint(asfloat(r7.xyz) * i_TEXCOORD2.yyy + asfloat(r3.xyz));
				r3.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.yyy));
				r8.xyz = asuint(i_TEXCOORD1.xxx * asfloat(r2.xyz));
				r2.xyz = asuint(i_TEXCOORD1.zzz * asfloat(r2.xyz));
				r2.xyz = asuint(asfloat(r5.xyz) * i_TEXCOORD0.zzz + asfloat(r2.xyz));
				r5.xyz = asuint(asfloat(r5.xyz) * i_TEXCOORD0.xxx + asfloat(r8.xyz));
				r5.xyz = asuint(asfloat(r7.xyz) * i_TEXCOORD2.xxx + asfloat(r5.xyz));
				r2.xyz = asuint(asfloat(r7.xyz) * i_TEXCOORD2.zzz + asfloat(r2.xyz));
				r3.xyz = asuint(asfloat(r5.xyz) * asfloat(r0.xxx) + asfloat(r3.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.zzz) + asfloat(r3.xyz));
				r1.y = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r2.x = asuint(sqrt(asfloat(r1.y)));
				r1.y = asuint(1.0 / asfloat(r1.y));
				o_TEXCOORD0.z = 1.0 / asfloat(r2.x);
				r2.xyz = asuint(asfloat(r1.yyy) * asfloat(r0.xyz));
				r1.y = asuint(1.0 + -asfloat(r1.y));
				r2.w = asuint(sqrt(asfloat(r1.y)));
				r2 = b0.wwww ? uint4(0u, 0u, 0u, 0x3f800000u) : r2;
				o_TEXCOORD0.w = asfloat(r2.w);
				o_TEXCOORD0.xy = i_NORMAL0.xy;
				r3.x = asuint(unity_MatrixV._m12);
				r3.y = asuint(unity_MatrixV._m10);
				r3.z = asuint(unity_MatrixV._m11);
				r5.xyz = asuint(asfloat(r3.xyz) * -asfloat(r0.xyz));
				r3.xyz = asuint(-asfloat(r5.xyz) - asfloat(r0.zxy) * asfloat(r3.yzx));
				r1.y = asuint(dot(asfloat(r3.xyz), asfloat(r3.xyz)));
				r1.y = asuint(rsqrt(asfloat(r1.y)));
				r3.xyz = asuint(asfloat(r3.xyz) * asfloat(r1.yyy));
				r5.xyz = asuint(asfloat(r3.xyz) * -asfloat(r0.zxy));
				r0.xyz = asuint(-asfloat(r5.xyz) - asfloat(r0.yzx) * asfloat(r3.yzx));
				r1.y = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r1.y = asuint(rsqrt(asfloat(r1.y)));
				r0.xyz = asuint(asfloat(r1.yyy) * asfloat(r0.xyz));
				r5.xyz = asuint(i_TEXCOORD1.yyy * asfloat(r1.xzw));
				r5.xyz = asuint(asfloat(r4.xyz) * i_TEXCOORD0.yyy + asfloat(r5.xyz));
				r5.xyz = asuint(asfloat(r6.xyz) * i_TEXCOORD2.yyy + asfloat(r5.xyz));
				r7.xyz = asuint(asfloat(r5.xyz) * asfloat(r0.yyy));
				r8.xyz = asuint(i_TEXCOORD1.xxx * asfloat(r1.xzw));
				r1.xyz = asuint(i_TEXCOORD1.zzz * asfloat(r1.xzw));
				r1.xyz = asuint(asfloat(r4.xyz) * i_TEXCOORD0.zzz + asfloat(r1.xyz));
				r4.xyz = asuint(asfloat(r4.xyz) * i_TEXCOORD0.xxx + asfloat(r8.xyz));
				r4.xyz = asuint(asfloat(r6.xyz) * i_TEXCOORD2.xxx + asfloat(r4.xyz));
				r1.xyz = asuint(asfloat(r6.xyz) * i_TEXCOORD2.zzz + asfloat(r1.xyz));
				r6.xyz = asuint(asfloat(r4.xyz) * asfloat(r0.xxx) + asfloat(r7.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.zzz) + asfloat(r6.xyz));
				r0.xyz = asuint(i_NORMAL0.yyy * asfloat(r0.xyz));
				r6.xyz = asuint(asfloat(r5.xyz) * asfloat(r3.xxx));
				r5.xyz = asuint(asfloat(r5.xyz) * asfloat(r2.yyy));
				r5.xyz = asuint(asfloat(r4.xyz) * asfloat(r2.xxx) + asfloat(r5.xyz));
				r3.xzw = asuint(asfloat(r4.xyz) * asfloat(r3.zzz) + asfloat(r6.xyz));
				r3.xyz = asuint(asfloat(r1.xyz) * asfloat(r3.yyy) + asfloat(r3.xzw));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r2.zzz) + asfloat(r5.xyz));
				r1.xyz = asuint(i_POSITION0.xyz + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r3.xyz) * i_NORMAL0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r2.www) * asfloat(r0.xyz) + asfloat(r1.xyz));
				r1.xy = asuint(unity_MatrixV._m01_m11 * asfloat(r0.yy));
				r1.xy = asuint(unity_MatrixV._m00_m10 * asfloat(r0.xx) + asfloat(r1.xy));
				r1.xy = asuint(unity_MatrixV._m02_m12 * asfloat(r0.zz) + asfloat(r1.xy));
				r1.xy = asuint(unity_MatrixV._m03_m13 + asfloat(r1.xy));
				r1.yzw = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r1.yyy));
				r1.xyz = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r1.xxx) + asfloat(r1.yzw));
				r1.xyz = asuint(unity_MatrixInvV._m03_m13_m23 + asfloat(r1.xyz));
				r1.xyz = b0.www ? r1.xyz : asuint(unity_MatrixInvV._m03_m13_m23);
				r1.xyz = asuint(-asfloat(r1.xyz) + asfloat(r0.xyz));
				r2.xyz = asuint(unity_MatrixV._m01_m11_m21 * asfloat(r1.yyy));
				r1.xyw = asuint(unity_MatrixV._m00_m10_m20 * asfloat(r1.xxx) + asfloat(r2.xyz));
				o_TEXCOORD1 = unity_MatrixV._m02_m12_m22 * asfloat(r1.zzz) + asfloat(r1.xyw);
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * i_POSITION0.wwww + asfloat(r0);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(SHADOWS_DEPTH)
			cbuffer UnityShadows : register(b0)
			{
				float4 unity_LightShadowBias : packoffset(c5);
			}
			cbuffer UnityPerFrame : register(b1)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
			}
			Texture3D<float4> _DitherMaskLOD : register(t0);
			SamplerState sampler_DitherMaskLOD : register(s0);

			void frag(float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.x = asuint(dot(i_TEXCOORD0.xy, i_TEXCOORD0.xy));
				r0.x = asuint(asfloat(r0.x) / i_TEXCOORD0.w);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(i_TEXCOORD0.z * asfloat(r0.x) + 1.0);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.xzw = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				if (b0.y) discard;
				r1.xy = asuint(float2(0.25, 0.25) * i_SV_Position0.xy);
				r1.z = asuint(0.9375 * i_COLOR0.w);
				r1 = asuint(_DitherMaskLOD.Sample(sampler_DitherMaskLOD, asfloat(r1.xyz)));
				r0.y = asuint(-0.00999999978 + asfloat(r1.w));
				b0.y = (asfloat(r0.y) < 0.0);
				if (b0.y) discard;
				r0.yz = asuint(glstate_matrix_projection._m21_m31 * asfloat(r0.zz));
				r0.xy = asuint(glstate_matrix_projection._m20_m30 * asfloat(r0.xx) + asfloat(r0.yz));
				r0.xy = asuint(glstate_matrix_projection._m22_m32 * asfloat(r0.ww) + asfloat(r0.xy));
				r0.xy = asuint(glstate_matrix_projection._m23_m33 + asfloat(r0.xy));
				r0.z = asuint(unity_LightShadowBias.x / asfloat(r0.y));
				r0.z = asuint(min(0.0, asfloat(r0.z)));
				r0.z = asuint(max(-1.0, asfloat(r0.z)));
				r0.x = asuint(asfloat(r0.x) + asfloat(r0.z));
				r0.z = asuint(min(asfloat(r0.x), asfloat(r0.y)));
				r0.z = asuint(asfloat(r0.z) + -asfloat(r0.x));
				r0.x = asuint(unity_LightShadowBias.y * asfloat(r0.z) + asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) / asfloat(r0.y));
				o_SV_Target0 = asfloat(r0.xxxx);
				o_SV_Depth0 = asfloat(r0.x);
			}
			#elif defined(SHADOWS_CUBE)
			cbuffer UnityShadows : register(b0)
			{
				float4 unity_LightShadowBias : packoffset(c5);
			}
			cbuffer UnityPerFrame : register(b1)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
			}
			Texture3D<float4> _DitherMaskLOD : register(t0);
			SamplerState sampler_DitherMaskLOD : register(s0);

			void frag(float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.x = asuint(dot(i_TEXCOORD0.xy, i_TEXCOORD0.xy));
				r0.x = asuint(asfloat(r0.x) / i_TEXCOORD0.w);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				b0.y = (asfloat(r0.x) < 0.0);
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(i_TEXCOORD0.z * asfloat(r0.x) + 1.0);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.xzw = asuint(i_TEXCOORD1.xyz * asfloat(r0.xxx));
				if (b0.y) discard;
				r1.xy = asuint(float2(0.25, 0.25) * i_SV_Position0.xy);
				r1.z = asuint(0.9375 * i_COLOR0.w);
				r1 = asuint(_DitherMaskLOD.Sample(sampler_DitherMaskLOD, asfloat(r1.xyz)));
				r0.y = asuint(-0.00999999978 + asfloat(r1.w));
				b0.y = (asfloat(r0.y) < 0.0);
				if (b0.y) discard;
				r0.yz = asuint(glstate_matrix_projection._m21_m31 * asfloat(r0.zz));
				r0.xy = asuint(glstate_matrix_projection._m20_m30 * asfloat(r0.xx) + asfloat(r0.yz));
				r0.xy = asuint(glstate_matrix_projection._m22_m32 * asfloat(r0.ww) + asfloat(r0.xy));
				r0.xy = asuint(glstate_matrix_projection._m23_m33 + asfloat(r0.xy));
				r0.z = asuint(min(asfloat(r0.x), asfloat(r0.y)));
				r0.z = asuint(asfloat(r0.z) + -asfloat(r0.x));
				r0.x = asuint(unity_LightShadowBias.y * asfloat(r0.z) + asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) / asfloat(r0.y));
				o_SV_Target0 = asfloat(r0.xxxx);
				o_SV_Depth0 = asfloat(r0.x);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
	Fallback "Diffuse"
}
