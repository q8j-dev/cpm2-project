Shader "Hidden/PostProcessing/DiscardAlpha"
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

			float4 _GlobalsPadding[26];
			float _RenderViewportScaleFactor;

			Texture2D _MainTex;
			SamplerState sampler_MainTex;

			struct appdata
			{
				float3 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordStereo : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = (v.vertex.xy + 1.0) * 0.5;
				o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
				o.texcoordStereo = o.texcoord * _RenderViewportScaleFactor;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color = _MainTex.Sample(sampler_MainTex, i.texcoord);
				return float4(color.rgb, 1.0);
			}
			ENDHLSL
		}
	}
}
