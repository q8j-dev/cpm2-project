Shader "Hidden/PostProcessing/CopyStdFromTexArray"
{
	Properties
	{
		_MainTex ("", 2DArray) = "white" {}
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
			float _DepthSlice;

			void vert(float3 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_TEXCOORD0 : TEXCOORD0)
			{
				o_SV_POSITION0.xy = i_POSITION0.xy;
				o_SV_POSITION0.zw = float2(0.0, 1.0);
				o_TEXCOORD0.xy = i_POSITION0.xy * float2(0.5, -0.5) + float2(0.5, 0.5);
				o_TEXCOORD0.z = _DepthSlice;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			Texture2DArray<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				o_SV_Target0 = _MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xyz);
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
			float _DepthSlice;

			void vert(float3 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_TEXCOORD0 : TEXCOORD0)
			{
				o_SV_POSITION0.xy = i_POSITION0.xy;
				o_SV_POSITION0.zw = float2(0.0, 1.0);
				o_TEXCOORD0.xy = i_POSITION0.xy * float2(0.5, -0.5) + float2(0.5, 0.5);
				o_TEXCOORD0.z = _DepthSlice;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			Texture2DArray<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b1;
				bool4 b2;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xyz));
				b1 = (asfloat(r0) < float4(0.0, 0.0, 0.0, 0.0));
				b2 = (float4(0.0, 0.0, 0.0, 0.0) < asfloat(r0));
				b1 = b2 || b1;
				b2 = (float4(0.0, 0.0, 0.0, 0.0) == asfloat(r0));
				b1 = b2 || b1;
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				b1 = (int4(0, 0, 0, 0) == asint(r1));
				b1.x = b1.x || b1.y;
				b1.x = b1.x || b1.z;
				b1.x = b1.x || b1.w;
				o_SV_Target0 = b1.xxxx ? float4(0.0, 0.0, 0.0, 0.0) : asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
}
