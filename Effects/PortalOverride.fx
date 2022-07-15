sampler2D target : register(s0);
texture portalTexture;
sampler2D portalSampler : sampler_2D
{
    Texture = <portalTexture>;
};
float2 screenPosition;
float2 screenSize;

float4 MainPS(float2 uv : TEXCOORD0) : COLOR0
{
    float4 t = tex2D(portalSampler, uv + (screenPosition / screenSize));
    return tex2D(portalSampler, uv) * (t.r + t.g + t.b) / 3;
}
Technique technique1
{
    pass Default
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}