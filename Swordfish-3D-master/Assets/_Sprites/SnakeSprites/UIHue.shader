Shader "UI/UIHue" {
		Properties
		{
			[PerRendererData]
			_MainTex("Sprite Texture", 2D) = "white" {}
            _Color ("Tint", Color) = (1,1,1,1)

			_HueShift("HueShift", Range(0,1)) = 0
			_HueSaturationShift("HueSaturationShift", Range(0,1)) = 0
			_HueBrightnessShift("HueBrightness Shift", Range(0,1)) = 0

			_StencilComp ("Stencil Comparison", Float) = 8
            _Stencil ("Stencil ID", Float) = 0
            _StencilOp ("Stencil Operation", Float) = 0
            _StencilWriteMask ("Stencil Write Mask", Float) = 255
            _StencilReadMask ("Stencil Read Mask", Float) = 255
    
            _ColorMask ("Color Mask", Float) = 15

            [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

			Pass
		{
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "HSV.cginc"

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
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
		};

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
		    float _HueShift;
		    float _HueSaturationShift;
		    float _HueBrightnessShift;

		v2f vert(appdata_t v)
		{			
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
		}

		float4 frag(v2f IN) : SV_Target
		{
		    float4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
		    
		    if(_HueShift > 0.01f || _HueSaturationShift > 0.01f || _HueBrightnessShift > 0.01f)
		    {
			    float3 hsv = rgb2hsv(color.xyz);
			    hsv.x = frac(hsv.x + _HueShift);
			    hsv.y += _HueSaturationShift;
			    hsv.z += _HueBrightnessShift;
			    color.rgb = hsv2rgb(hsv);
			}

			//if (hsv.y > 1.0) { hsv.y -= 1.0; }
			//if (hsv.z > 1.0) { hsv.z -= 1.0; }
			//return float4(hsv.y,hsv.y,hsv.y,1);
						
			
            #ifdef UNITY_UI_CLIP_RECT
            color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
            #endif

            #ifdef UNITY_UI_ALPHACLIP
            clip (color.a - 0.001);
            #endif
                
			return color;
		}
			ENDCG
		}
		}
	}
