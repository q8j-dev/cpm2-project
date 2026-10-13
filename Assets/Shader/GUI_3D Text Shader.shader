Shader "GUI/3D Text Shader"
{
	Properties
	{
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _GlobalsPadding[2];
			float4 _Color;
			float4 _MainTex_ST;

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
				float3 pos : POSITION;
				float3 uv0 : TEXCOORD0;
			};

			struct v2f
			{
				float4 color : COLOR0;
				float2 uv0 : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.color = saturate(_Color);
				o.uv0 = v.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, float4(v.pos, 1.0)));
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 tex = tex2D(_MainTex, i.uv0);
				float4 col;
				col.rgb = i.color.rgb;
				col.a = tex.a * i.color.a;
				return col;
			}
			ENDHLSL
		}
	}
}
