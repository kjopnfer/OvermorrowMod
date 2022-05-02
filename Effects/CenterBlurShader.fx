struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};
struct PSInput
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
    float2 PixelPosition : TEXCOORD1;
};

matrix WVP;
sampler img0 : register(s0);
float2 blurOrigin;
float blurRadius;
float blurIntensity;

PSInput MainVS(in VSInput input)
{
    PSInput output = (PSInput)0;
    output.Position = mul(input.Position, WVP);
    output.PixelPosition = input.Position.xy;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 MainPS(PSInput input) : COLOR
{
    float dist = length(blurOrigin - input.PixelPosition.xy) / blurRadius;
    float intensity = clamp(pow(dist, blurIntensity), 0, 1);

    float4 color = tex2D(img0, input.TexCoord);
    return float4(color.rgb, color.a * intensity);
}

Technique technique1
{
    pass Blur
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
}