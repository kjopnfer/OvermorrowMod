struct VSInput
{
    float4 Position : POSITION0;
    float2 Center : TEXCOORD0;
    int Tick : TEXCOORD1;
};
struct PSInput
{
    float4 Position : POSITION;
    float2 PixelPosition : TEXCOORD1;
    float2 Center : TEXCOORD0;
    int Tick : TEXCOORD2;
};

matrix WVP;
sampler img0 : register(s0);
float2 screenPosition;
float4 color;
int tick;
int texWidth;
float radius;

PSInput MainVS(in VSInput input)
{
    PSInput output = (PSInput)0;
    output.Position = mul(input.Position - float4(screenPosition, 0, 0), WVP);
    output.PixelPosition = input.Position;
    output.Center = input.Center;
    output.Tick = input.Tick;
    return output;
}

float4 MainPS(PSInput input) : COLOR
{
    float len = length(input.Center - input.PixelPosition.xy) / radius;
    float pTick = (float)(tick - input.Tick) / (float)texWidth;
    
    if (len > 1.0f || pTick > 1.0f) return float4(0, 0, 0, 0);
    
    float4 sample = tex2D(img0, float2(pTick, 1.0f - len));
    
    return float4(sample.r, sample.r, sample.r, sample.r) * color;
}

Technique technique1
{
    pass Circular
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
}