Shader "Obi/DistanceFieldSlice"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			Cull Off
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _MainTex_ST;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION)
			{
				uint4 r0;
				uint4 r1;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float2 i_TEXCOORD0 : TEXCOORD0, float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(-0.5 + asfloat(r0.x));
				r0.y = asuint(ddx(asfloat(r0.x)));
				r0.z = asuint(ddy(asfloat(r0.x)));
				r0.y = asuint(abs(asfloat(r0.y)) + abs(asfloat(r0.z)));
				r0.z = asuint(asfloat(r0.x) + asfloat(r0.y));
				r0.y = asuint(asfloat(r0.y) + asfloat(r0.y));
				r0.y = asuint(1.0 / asfloat(r0.y));
				r0.y = asuint(saturate(asfloat(r0.z) * asfloat(r0.y)));
				r0.z = asuint(asfloat(r0.y) * -2.0 + 3.0);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.y));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y));
				r0.z = asuint(40.0 * abs(asfloat(r0.x)));
				r0.x = asuint(1.0 + -abs(asfloat(r0.x)));
				r0.z = asuint(sin(asfloat(r0.z)));
				r0.w = asuint(ddx(asfloat(r0.z)));
				r1.x = asuint(ddy(asfloat(r0.z)));
				r0.w = asuint(abs(asfloat(r1.x)) + abs(asfloat(r0.w)));
				r0.z = asuint(asfloat(r0.z) + asfloat(r0.w));
				r0.w = asuint(asfloat(r0.w) + asfloat(r0.w));
				r0.w = asuint(1.0 / asfloat(r0.w));
				r0.z = asuint(saturate(asfloat(r0.z) * asfloat(r0.w)));
				r0.w = asuint(asfloat(r0.z) * -2.0 + 3.0);
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.z));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.z = asuint(asfloat(r0.z) * 0.150000006 + 0.850000024);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				o_SV_Target0.yz = asfloat(r0.xx) * asfloat(r0.yy);
				o_SV_Target0.x = asfloat(r0.x);
				o_SV_Target0.w = 1.0;
			}
			#endif
			ENDHLSL
		}
	}
}
