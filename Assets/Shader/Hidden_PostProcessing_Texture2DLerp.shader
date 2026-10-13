Shader "Hidden/PostProcessing/Texture2DLerp"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[26];
			float _RenderViewportScaleFactor;

			void vert(float3 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				o_SV_POSITION0.xy = i_POSITION0.xy;
				o_SV_POSITION0.zw = float2(0.0, 1.0);
				r0.xy = asuint(float2(1.0, 1.0) + i_POSITION0.xy);
				r0.xy = asuint(asfloat(r0.xy) * float2(0.5, -0.5) + float2(0.0, 1.0));
				o_TEXCOORD1 = float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0.xy);
				o_TEXCOORD0 = i_POSITION0.xy * float2(0.5, -0.5) + float2(0.5, 0.5);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[28];
			float _Interp;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _To : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_To : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_To.Sample(sampler_To, i_TEXCOORD0.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0 = asuint(-asfloat(r1) + asfloat(r0));
				o_SV_Target0 = float4(_Interp, _Interp, _Interp, _Interp) * asfloat(r0) + asfloat(r1);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[26];
			float _RenderViewportScaleFactor;

			void vert(float3 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				o_SV_POSITION0.xy = i_POSITION0.xy;
				o_SV_POSITION0.zw = float2(0.0, 1.0);
				r0.xy = asuint(float2(1.0, 1.0) + i_POSITION0.xy);
				r0.xy = asuint(asfloat(r0.xy) * float2(0.5, -0.5) + float2(0.0, 1.0));
				o_TEXCOORD1 = float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0.xy);
				o_TEXCOORD0 = i_POSITION0.xy * float2(0.5, -0.5) + float2(0.5, 0.5);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[28];
			float _Interp;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _TargetColor;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1 = asuint(_TargetColor.xyzw + -asfloat(r0));
				o_SV_Target0 = float4(_Interp, _Interp, _Interp, _Interp) * asfloat(r1) + asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
}
