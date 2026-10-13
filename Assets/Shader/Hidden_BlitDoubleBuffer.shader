Shader "Hidden/BlitDoubleBuffer"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BackgroundTex ("Texture", 2D) = "black" {}
		_Weight ("Weight", Float) = 1
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

			float4 _GlobalsPadding[2];
			float _Weight;

			cbuffer UnityPerDraw
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}

			cbuffer UnityPerFrame
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			sampler2D _MainTex;
			sampler2D _BackgroundTex;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.uv = v.uv;
				o.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)));
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 background = tex2D(_BackgroundTex, i.uv);
				float4 col = tex2D(_MainTex, i.uv);
				return float4(col.rgb * _Weight + background.rgb, background.a + _Weight);
			}
			ENDHLSL
		}
	}
}
