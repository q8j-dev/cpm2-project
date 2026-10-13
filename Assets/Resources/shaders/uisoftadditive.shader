Shader "UI Extensions/UISoftAdditive"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
	}
	SubShader
	{
		Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass
		{
			Blend OneMinusDstColor One, OneMinusDstColor One
			ColorMask [_ColorMask]
			Cull Off
			ZTest [unity_GUIZTestMode]
			ZWrite Off
			Stencil { Ref [_Stencil] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] Comp [_StencilComp] Pass [_StencilOp] Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _GlobalsPadding[2];
			float4 _Color;

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
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(unity_MatrixVP, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)));
				o.color = v.color * _Color;
				o.texcoord = v.texcoord;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.texcoord) * i.color;
				clip(color.a - 0.01);
				color.rgb *= color.a;
				return color;
			}
			ENDHLSL
		}
	}
}
