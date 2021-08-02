Shader "Unlit/S_UIButton"
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

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _UseBorder ("Use Border", Int) = 0
        _UVScale ("UV Scale", Float) = 2
        [PerRendererData] _Button ("Button", Int) = 0
        [PerRendererData] _Selected ("Selected", Int) = 0
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

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "Yawnoc.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4    _Color;
            fixed4    _TextureSampleAdd;
            float4    _ClipRect;
            float4    _MainTex_ST;
            float     _Button;
            float     _Selected;
            int       _UseBorder;
            float     _UVScale;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frame(v2f IN)
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                // border
                fixed2 uv = IN.texcoord;
                float thck = fwidth(IN.texcoord);
                fixed2 stp = step(1.-thck, uv) + step(uv, thck);

                fixed4 bord_clr = lerp(
                    fixed4(1., 1., 1., 0.), 
                    fixed4(1., 0., 0., 0.), 
                    saturate(_Selected)
                );

                float bord = max(stp.x, stp.y) * _UseBorder;
                color += bord_clr * bord;

                fixed4 btn_color = fixed4(1., 1., 1., 0.);
                fixed2 uvo = (IN.texcoord - .5)*_UVScale;
                uvo *= 1.85;
                if (_Button < 1.)
                    color += btn_color * btn_play(uvo);
                else if (_Button < 2.0)
                    color += btn_color * btn_step(uvo);
                else if (_Button < 3.)
                    color += btn_color * btn_pause(uvo);
                else if (_Button < 4.)
                    color += btn_color * icon_exit(uvo);
                else if (_Button < 5.)
                    color += btn_color * icon_replay(uvo);
                else if (_Button < 6.)
                    color += btn_color * icon_win(uvo);

                // color.r += 1. - step(0.01, abs(frac(uv.x*3.-.5)));
                // color.g += 1. - step(0.01, abs(frac(uv.y*3.-.5)));

                return color;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // simple antialiasing of the frame
                fixed2 uv = IN.texcoord;
                float ddx = fwidth(uv.x)*0.5;
                float ddy = fwidth(uv.y)*0.5;
                fixed3 dd = fixed3(ddx, ddy, 0.);

                fixed4 a = frame(IN);
                IN.texcoord = uv + dd.xz;
                fixed4 b = frame(IN);
                IN.texcoord = uv - dd.xz;
                fixed4 c = frame(IN);
                IN.texcoord = uv + dd.zy;
                fixed4 d = frame(IN);
                IN.texcoord = uv - dd.zy;
                fixed4 e = frame(IN);
                return (b+c+d+e)/4.;
            }
        ENDCG
        }
    }
}
