sampler2D input : register(s0);
float progress;

float4 MainPS(float2 uv : TEXCOORD) : COLOR
{
    float2 center = float2(0.5, 0.5);
    float2 dir = uv - center;
    float rot = atan2(dir.y, dir.x) + radians(180);
    float fadeAmount = 0.2;
    float mult = 1 + fadeAmount;
    if (rot < progress * mult * radians(360) - fadeAmount)
    {
        float4 color = tex2D(input, uv);
        return color;
    }
    else if (rot < progress * mult * radians(360))
    {
        float p = rot - progress * mult * radians(360) + fadeAmount;
        float alpha = 1 - p * (1 / fadeAmount);
        float4 color = tex2D(input, uv);
        return color * alpha;
    }
    return float4(0, 0, 0, 0);
}

Technique technique1
{
    pass RadialFade
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}