sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

static int PerlinSubtract = 1;
static float2 COARDs : TEXCOORD0;
float4 UVShader(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float TotalAlpha = uColor.x;
    float Radius = uColor.y + 16;
    float Timer = uColor.z;
    float Center = pow((coords[0] - targetCoords[0]) * uScreenResolution[0], 2) / pow(2 * sin(coords[0] * Timer) + Radius, 2) + pow((coords[1] - targetCoords[1]) * uScreenResolution[1], 2) / pow(2 * cos(coords[1] * Timer) + Radius, 2);

    //float PerlinY = coords[1] % Timer;
    /*if (PerlinY > 1) 
    {
        PerlinY -= PerlinSubtract;
        if (coords[1] == 1)
            PerlinSubtract += 1;
    }*/
    COARDs = coords;
    COARDs.y = 1 % coords.y - (uTime / 55);
    if (Center >  0.98f && Center < 1) 
    {
        return float4(255, 255, 255, 255);
    }
    if (Center <= 1)
    {
        //aa
        return tex2D(uImage0, coords) + (float4(uSecondaryColor, 0.5) * Center + (tex2D(uImage1, COARDs) * 0.5f)) * TotalAlpha;//clamp(Center - 0.5, 0, 1);
    }
    else 
    {
        return tex2D(uImage0, coords);
    }
}

technique Technique1
{
    pass UVShader
    {
        PixelShader = compile ps_2_0 UVShader();
    }
}