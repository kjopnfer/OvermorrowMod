struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
};
struct PSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
};
matrix WVP;
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

PSInput MainVS(in VSInput input)
{
    PSInput output = (PSInput)0;
    output.Position = mul(input.Position, WVP);
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;
    return output;
}
float4 MainPS(PSInput input) : COLOR
{
    float4 color = input.Color;
    return color;
}
float4 MainTexture(PSInput input) : COLOR
{
    float4 color1 = input.Color;
    float4 a = tex2D(uImage0, input.TexCoord);
    return color1 * a.r;
}
float4 Pallete(PSInput input) : COLOR
{
    float4 color1 = input.Color;
    float4 a = tex2D(uImage0, input.TexCoord);
    return float4(color1.rgb * a.rgb, color1.a);
}

Technique technique1
{
    pass Basic
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
    pass Texture
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainTexture();
    }
}