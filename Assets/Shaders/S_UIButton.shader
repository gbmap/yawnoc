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

        [PerRendererData] _AnimDisappear ("Disappear Animation", Float) = 0.0
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
            float     _AnimDisappear;

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

                color += btn_color * draw_icon(_Button, uvo);
                /*
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
                */

                // color.r += 1. - step(0.01, abs(frac(uv.x*3.-.5)));
                // color.g += 1. - step(0.01, abs(frac(uv.y*3.-.5)));

                return color;
            }

            float4x4 abberration(v2f IN, float angle, fixed off)
            {
                fixed2 uv = IN.texcoord;
                fixed2 offset = fixed2(cos(angle), sin(angle)) * off;
                IN.texcoord = uv - offset.xy;
                fixed4 a = frame(IN);
                IN.texcoord = uv + offset.xy;
                fixed4 a2 = frame(IN);
                float4x4 clr = float4x4(
                    a.r, a.g, a.b, a.a, 
                    a2.r, a2.g, a2.b, a2.a, 
                    0., 0., 0., 0., 
                    0., 0., 0., 0.
                );
                return clr;
            }

            fixed4 abberration_clr1(v2f IN, float angle, fixed off)
            {
                float4x4 abberration_ = abberration(IN, angle, off);
                fixed4 a = abberration_[0];
                fixed4 b = abberration_[1];
                return fixed4(a.r, a.g*b.g, b.b, a.a);
            }
            
            fixed4 abberration_clr2(v2f IN, float angle, fixed off)
            {
                return fixed4(abberration_clr1(IN, angle, off).grb, 1.);
            }

            fixed4 abberration_clr3(v2f IN, float angle, fixed off)
            {
                return fixed4(abberration_clr1(IN, angle, off).rbg, 1.);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // simple antialiasing of the frame
                fixed2 uv = IN.texcoord;
                float ddx = fwidth(uv.x)*0.5;
                float ddy = fwidth(uv.y)*0.5;
                fixed3 dd = fixed3(ddx, ddy, 0.);

                // abberration factor
                float t = saturate(_AnimDisappear-0.001);///*_Time.y;
                float abb = frac(t);
                abb = abb*abb*abb*abb*2.;

                IN.texcoord = uv + dd.xz;
                fixed4 b = abberration_clr1(IN, 0., 0.075*abb);
                IN.texcoord = uv - dd.xz;
                fixed4 c = abberration_clr2(IN, radians(90.+t), 0.1*abb);
                IN.texcoord = uv + dd.zy;
                fixed4 d = abberration_clr1(IN, radians(45.+t), 0.05*abb);
                IN.texcoord = uv - dd.zy;
                fixed4 e = abberration_clr1(IN, radians(30.+t), 0.075*abb);

                fixed4 final = (b+c+d+e)/4.;
                final.a = 1. - abb;
                return final;
            }
        ENDCG
        }
    }
}
