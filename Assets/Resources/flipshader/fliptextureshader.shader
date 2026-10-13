Shader "Custom/FlipTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Darkness ("Darkness", Range(0, 1)) = 0.75
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _GlobalsPadding[2];
			float4 _MainTex_ST;
			float _Darkness;

			cbuffer UnityPerDraw
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}

			cbuffer UnityPerFrame
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			sampler2D _MainTex;

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
				o.uv = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)));
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = 1.0 - i.uv;
				float4 col = tex2D(_MainTex, uv);
				col.rgb *= _Darkness;
				return col;
			}
			ENDHLSL
		}
	}
}
