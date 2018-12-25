// 配合Grender一起使用的Shader
// https://github.com/garsonlab/RenderTuxture-AlphaBlend

Shader "UI/GRenderShader"
{
	Properties
	{
		[PerRendererData] _MainTex ("White Texture", 2D) = "white" {}
		_BlackTex ("Black Texture", 2D) = "black" {}
		_Color ("Color", Color) = (1,1,1,1)

		[Toggle]_Grey ("Grey", Float) = 0
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color = IN.color*_Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _BlackTex;
			half _Grey;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 white = tex2D(_MainTex, IN.texcoord);
				half4 black = tex2D(_BlackTex, IN.texcoord);

				half alpha = white.r - black.r;
				alpha = 1-alpha;

				fixed4 color;
				if(alpha == 0){
					color.rgb = fixed3(0,0,0);
				} else {
					color.rgb = black.rgb/alpha;
				}

				if(_Grey){
					color.rgb = dot(color.rgb, fixed3(0.22, 0.707, 0.071));
				}

				color.a = alpha;
				clip (color.a - 0.01);
				return color;
			}
		ENDCG
		}
	}
}
