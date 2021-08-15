
float3 HUEtoRGB(in float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R,G,B));
}

float Epsilon = 1e-10;

float3 RGBtoHCV(in float3 RGB)
{
    // Based on work by Sam Hocevar and Emil Persson
    float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
    float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
    float C = Q.x - min(Q.w, Q.y);
    float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
    return float3(H, C, Q.x);
}

float3 RGBtoHSV(in float3 RGB)
{
    float3 HCV = RGBtoHCV(RGB);
    float S = HCV.y / (HCV.z + Epsilon);
    return float3(HCV.x, S, HCV.z);
}

float3 HSVtoRGB(in float3 HSV)
{
    float3 RGB = HUEtoRGB(HSV.x);
    return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

float btn_play(fixed2 uv)
{
    //uv = (uv-.5)*2.;
    uv.y *= 2.;
    uv *= 1.5;
    uv.x += .4;

    float a = radians(35.);
    float a2 = radians(-35.);
    float tp = step(0.0, 1.-dot(uv, fixed2(cos(a), sin(a)))); 
    float bt = step(0.0, 1.-dot(uv, fixed2(cos(a2), sin(a2))));

    return tp * bt * step(-.4, uv.x); // (bt, tp);
}

float _square(fixed2 uv, fixed2 sz)
{
    float w = sz.x;
    float h = sz.y;
    float r = w*.5;
    float l = -w*.5;
    float t = h*.5;
    float b = -h*.5;
    fixed2 stp = step(uv, fixed2(r, t)) * step(fixed2(l,b), uv);
    return min(stp.x, stp.y);
}

float _tri(fixed2 uv)
{
    // uv = (uv-.5)*2.;
    float h = .75;
    uv.y -= h/2;
    float a  = radians(35.);
    float d  = -dot(uv, fixed2(cos(a), sin(a)));
    float d2 = dot(uv, fixed2(cos(-a), sin(-a)));

    float w = fwidth(uv.x);
    //d = smoothstep(-w, w, d);
    d = step(0., d);
    d2 = smoothstep(-w, w, d2);
    d2 = 1.-step(d2, 0.);
    float b = step(0., uv.y+h);

    return d*d2*b;
}

float _circle(fixed2 uv)
{
    return step(length(uv), 0.5);
}

float btn_pause(fixed2 uv)
{
    fixed2 sz = fixed2(.375, 1.5);
    return _square(uv - fixed2(-sz.x, 0.), sz)
        +  _square(uv - fixed2(sz.x, 0.), sz);
}

float btn_step(fixed2 uv)
{
    float btn = btn_play(uv);
    return max(0., btn - btn_play(uv+fixed2(0.45, 0.)));
}

float icon_exit(fixed2 uv)
{
    //uv = (uv-.5)*2.;
    float scl = 1.95;
    //uv *= 1.95;
    uv.y -= 0.1;

    // frame
    float w = .75*1.5;
    float h = .85*1.5;
    float frame = _square(uv, fixed2(w, h));
    frame -= _square(uv+fixed2(0., h*.075), fixed2(w*.85, h));

    //uv.y += 0.045;
    //uv.y *= 0.925;

    uv -= fixed2(-1.305, -1.);
    uv *= .5;
    uv.x *= 0.75;

    float a = radians(65.);
    float dr = 1.-step(0.5, dot(uv-.25, fixed2(cos(a), sin(a))));
    dr *= step(0.5, dot(uv+.15, fixed2(cos(a), sin(a))));
    dr *= step(0.325, uv.x);

    a = a+radians(70.);
    dr *= 1.-step(.275, dot(uv, fixed2(cos(a), sin(a))));
    dr *= step(uv.x, 0.65);

    dr *= 1.-step(dot(uv, fixed2(cos(a), sin(a))), -0.40);

    uv.y += uv.x*0.15;
    dr -= 1.-step(0.03, length(uv - fixed2(0.55, 0.425)));

    //door *= dr;

    return max(frame , dr);
}

float icon_replay(fixed2 uv)
{
    //uv=(uv-.5)*2.;
    uv *= 0.75;
    //uv *= sin(_Time.y)*2.;

    float thickness = .15;
    float circle = step(0.5-thickness, length(uv)) * step(length(uv), 0.5);

    float a    = radians(-45.);
    float d    = dot(uv, fixed2(cos(a), sin(a)));
    float fw   = fwidth(uv.x);
    float thck = .175;
    float lin  = smoothstep(-fw-thck, fw-thck, d)
               - smoothstep(-fw+thck, fw+thck, d);

    circle = saturate(circle-lin);

    // upper triangle
    fixed2 offset = fixed2(1.215, 0.); // triangle offset


    a = radians(-30.);
    float2x2 r = float2x2(cos(a), -sin(a), sin(a), cos(a));
    fixed2 uvr = uv*3.;
    uvr = mul(r,uvr);
    float t = _tri(uvr - offset);

    // bottom triangle
    a = radians(-35. + 180.);
    r = float2x2(cos(a), -sin(a), sin(a), cos(a));
    uvr = uv*3.;
    uvr = mul(r,uvr);
    float t2 = _tri(uvr-offset);

    float tris = max(t, t2);

    return max(circle-tris, tris);
}

float _triside(fixed2 uv, float a)
{
    a = radians(a);
    float sa = dot(uv, fixed2(cos(a), sin(a)));
    sa = 1.-step(-0.0, sa);
    return sa;
}

float _crown_tri(fixed2 uv)
{
    float sa = _triside(uv-fixed2(0., -.3), 40.);
    float sb = 1.-_triside(uv-fixed2(-0.6,0.), 5.);
    //return sa;

    return sa*sb;
}

float icon_win(fixed2 uv)
{
    uv -= fixed2(0., 0.1);
    uv.x *= 0.95;
    float crown_l = _crown_tri(uv);

    uv.x *= -1.;
    float crown_r = _crown_tri(uv);

    //uv.x = 1.-uv.x;
    //uv.x -= .5;
    uv.x = abs(uv.x);
    float crown_m = _triside(uv-fixed2(0., .65), 20.);
    float c = crown_l;
    c = max(crown_l, crown_r); 
    c = max(crown_m, c);
    c *= step(-0.7, uv.y);

    fixed2 uvc = uv*3.5;
    float crc = _circle(uvc-fixed2(2.25,1.5));
    c -= _circle(uvc-fixed2(2.15,1.35));
    c = max(c, crc);

    c -= _circle(uvc-fixed2(0., 1.85));
    c = max(c, _circle(uvc-fixed2(0., 2.0)));

    fixed2 uvb = uv;
    uvb.y += .695;
    uvb.x *= 1.115;
    float bt = step(0.5, (1.-abs(uvb.y*3.5))-abs(uvb.x*uvb.x));

    uvb.y -= .035;
    uvb.x *= 1.1;
    c -= step(0.5, (1.-abs(uvb.y*3.5))-abs(uvb.x*uvb.x));
    return max(c, bt); 
}

float icon_menu(fixed2 uv)
{
    return _square(uv, fixed2(1.25, 0.2))
         + _square(fixed2(uv.x, abs(uv.y) - .4), fixed2(1.25, 0.2));
}

float icon_close(fixed2 uv)
{
    float a = radians(45.);
    float2x2 rot = float2x2(
        cos(a), -sin(a),
        sin(a), cos(a)
    );
    uv = mul(rot,uv.xy);
    return max(_square(uv.yx, fixed2(1.25, 0.25)),
               _square(uv.xy, fixed2(1.25, 0.25)));
}

float draw_icon(float icon, fixed2 uv)
{
    if (icon < 1.)
        return btn_play(uv);
    else if (icon < 2.0)
        return btn_step(uv);
    else if (icon < 3.)
        return btn_pause(uv);
    else if (icon < 4.)
        return icon_exit(uv);
    else if (icon < 5.)
        return icon_replay(uv);
    else if (icon < 6.)
        return icon_win(uv);
    else if (icon < 7.)
        return icon_menu(uv);
    else if (icon < 8.)
        return icon_close(uv);
    return 0.;
}