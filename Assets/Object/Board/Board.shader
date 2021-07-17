Shader "Unlit/Board"
{
    Properties
    {
        _Board ("Texture", 2D) = "white" {}
        _CellSize("Cell Size", Vector) = (1, 1,1,1)
        _ClickTime("Click Time", Float) = 0.0
        _ClickPos("Click Position", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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

            sampler2D _Board;
            float4 _Board_ST;

            float4 _CellSize;

            float _ClickTime;
            float2 _ClickPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Board);
                return o;
            }

            float click(fixed2 uv, float2 click, float clickTime)
            {
                float t = saturate(_Time.y - clickTime);
                float sz = lerp(0.0, 0.2, pow(t, 0.15));

                float dst = length(click - uv);
                float thck = 0.01;
                float circle = step(dst, sz+thck)
                    - step(sz-thck, dst);
                    //- (1.-dst+dst*2.*t)
                return circle * (1.-t*t);

            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 a = step(0.05, frac(i.uv / _CellSize));
                float t = min(a.x, a.y);

                fixed4 col = tex2D(_Board, i.uv);
                fixed4 clr = max(col, fixed4(0.2, 0.2, 0.2, 0.2)*1.-t);

                float c = click(i.uv, _ClickPos, _ClickTime);
                clr = lerp(col, fixed4(1.,1.,1.,1.), c);
                return clr;

                return lerp(col, fixed4(0.2, 0.2, 0.2, 0.2), 1.-t);
            }
            ENDCG
        }
    }
}
