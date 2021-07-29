Shader "Unlit/Board"
{
    Properties
    {
        _Board ("Texture", 2D) = "white" {}
        _CameraPos ("Camera Position", Vector) = (0, 0, 0, 0)
        _CameraSize("Camera Size", Vector) = (0, 0, 0, 0)
        _BoardSize("Board Size", Vector) = (1,1,1,1)
        _CellSize("Cell Size", Vector) = (1, 1, 1, 1)
        _ClickTime("Click Time", Float) = 0.0
        _ClickPos("Click Position", Vector) = (1, 1, 1, 1)
        _GridColor("Grid Color", Color) = (0.1,0.1,0.1,1.0)
        _BorderColor("Border Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MousePos("Mouse Position", Vector) = (0.0, 0.0, 0.0, 0.0)
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
            float4    _Board_ST;

            float4 _CellSize;
            float2 _BoardSize;

            float  _ClickTime;
            float2 _ClickPos;

            float2 _CameraPos;
            float2 _CameraSize;

            fixed4 _GridColor;
            fixed4 _BorderColor;

            float2 _SelectedCell; 

            float3 _MousePos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Board);
                return o;
            }

            #define N_RINGS 1
            float click(fixed2 uv, float2 click, float clickTime)
            {
                float invfac = 9.;
                float factor = 1./invfac;
                float dst = length(click - uv);

                float d = 0.;
                for (int i = 0; i <= N_RINGS; i++)
                {
                    float tt    = float(i)/N_RINGS;
                    float t = saturate((_Time.y-tt*0.10 - clickTime) * 2.5);
                    t = pow(t, 0.5)*factor;

                    float dst0  = dst;
                    float thck  = .01-t*.01;
                    float outer = 1.-step(t,dst0);
                    float inner = 1.-step(t-thck,dst0);
                    d     = max(d, (outer-inner) * (1.-t*invfac)); 
                }
                return d;
            }

            float hover(fixed2 board_uv, float2 selected_cell)
            {
                fixed2 n_sel = selected_cell/_BoardSize;
                fixed2 n_cellsz = _CellSize / _BoardSize;

                fixed2 sqr = step(n_sel, board_uv) - step(n_sel, board_uv-n_cellsz);
                return min(sqr.x, sqr.y) * (sin(_Time.y*5.)+1.)*.5;
            }

            float dst_grid(fixed2 uv)
            {
                fixed2 gridUv = uv*_BoardSize*_CellSize; 
                //gridUv.y -= _CellSize.y*50.;
                //gridUv *= _BoardSize * _CellSize;
                //gridUv = frac(gridUv*_CameraSize);
                float thickness = fwidth(gridUv);
                gridUv = frac(gridUv);
                fixed2 gridD = step(thickness, gridUv);
                float griddst = min(gridD.x, gridD.y);
                return griddst;
            }

            fixed2 uv_board(fixed2 uv)
            {
                // "worldspace"
                fixed2 boardUv = uv * _CameraSize * _CellSize;

                // moves to camera position
                boardUv += _CameraPos;

                // moves to origin (bottom left)
                boardUv -= _CameraSize/2. -_BoardSize * 0.5 * _CellSize;

                // normalizes
                boardUv /= _BoardSize*_CellSize;
                return boardUv;
            }

            fixed4 txtr_board(fixed2 board_uv)
            {
                fixed4 col = tex2D(_Board, board_uv);
                return col;
            }

            float dst_board_border(fixed2 board_uv)
            {
                fixed2 stp  = step(1.00, board_uv) + step(board_uv, 0.00); 
                float sqr1 = max(stp.x, stp.y);
                fixed2 stp2 = step(1.02, board_uv) + step(board_uv, -0.02);
                float sqr2 = max(stp2.x, stp2.y);
                return sqr1 - sqr2;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uv_brd = uv_board(i.uv);
                fixed4 col = txtr_board(saturate(uv_brd));

                float grid = dst_grid(uv_brd);
                fixed4 clr = max(col, _GridColor*(1.-grid));

                float2 clickPos = uv_board(_MousePos.xy);
                clickPos = _ClickPos;

                float c = click(uv_board(i.uv), clickPos, _ClickTime);
                clr = lerp(clr, fixed4(1.,1.,1.,1.), c);

                float hvr = hover(uv_brd, _SelectedCell);
                clr += fixed4(1.,1.,1.,1.)*hvr;

                fixed bordr = dst_board_border(uv_brd);
                clr.xy += _BorderColor * bordr;
                // clr += _BorderColor * bordr;

                //clr = fixed4(i.uv.x, i.uv.y, 0., 1.);
                //clr = fixed4(uv_brd.x, uv_brd.y, 0., 1.);
                //clr = fixed4(_MousePos.)
                return clr;
            }
            ENDCG
        }
    }
}
