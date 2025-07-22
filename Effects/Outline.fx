sampler2D uImage0 : register(s0);
sampler2D uImage1 : register(s1);
float2 PixelSize;
float4 OutlineColor;
float4 FillColor;
bool UseFillColor;

float4 OutlineShader(float2 uv : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float4 texColor = tex2D(uImage0, uv);
    
    if (texColor.a == 0.0) {
        return float4(0, 0, 0, 0);
    }
    
    float2 offset = PixelSize * 2.0;
    
    float4 up = tex2D(uImage0, uv + float2(0, offset.y));
    float4 down = tex2D(uImage0, uv + float2(0, -offset.y));
    float4 left = tex2D(uImage0, uv + float2(-offset.x, 0));
    float4 right = tex2D(uImage0, uv + float2(offset.x, 0));
    
    // Check if any neighbor is transparent
    if (up.a == 0.0 || down.a == 0.0 || left.a == 0.0 || right.a == 0.0) {
        return OutlineColor * sampleColor.a;
    }
    
    float4 fillResult = UseFillColor ? FillColor : tex2D(uImage1, uv);
    return fillResult * sampleColor.a;
}

Technique technique1
{
    pass OutlinePass
    {
        PixelShader = compile ps_2_0 OutlineShader();
    }
}