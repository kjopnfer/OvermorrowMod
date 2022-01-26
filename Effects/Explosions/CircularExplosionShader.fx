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
int tick;
int tickSize;
float degPerTick;
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
    
    return float4(sample.r, sample.r, sample.r, sample.r);
    
    // return sample;
    
    // return tex2D(img0, coords);
    
    // return float4(len, pTick, 0, 1.0f);
    
    // float4 sample = tex2D(img0, float2(pTick, len));
    // return float4(pTick, 0, 0, 1.0f);
    // return float4(tex2D(img0, float2(pTick, len)).r, 0, 0, 1.0f);
    // return float4(sample.r, 0, 0, 1.0f);
    
    // if (len > 1.0f) return float4(0, 0, 1.0f, 1.0f);
    // if (pTick > 1.0f) return float4(0, 1.0f, 0, 1.0f);
    
    // if (sample.r > 0) {
    //    return float4(1.0f, 0, 0, 1.0f);
    //}
    // return float4(0, 0, 0, 0);
    
    // if (len < pTick) {
    //     return float4(1.0f, 0, 0, 1.0f);
    // }
    // else {
    //     return float4(0, 0, 0, 0);
    // }
}

float4 TestPS(PSInput input) : COLOR
{
    // if (input.Center.x > input.PixelPosition.x) {
    //     return float4(1.0f, 0, 0, 1.0f);
    // }
    // else {
    //    return float4(0, 1.0f, 0, 1.0f);
    // }

    float len = abs(length(input.PixelPosition - input.Center));
    return float4((input.PixelPosition - input.Center) / radius, 0, 1.0f);
}

Technique technique1
{
    pass Circular
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
}