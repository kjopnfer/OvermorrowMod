sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
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

float4 Flash(float2 coords : TEXCOORD0) : COLOR0 {
    float2 origin = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float4 color = tex2D(uImage0, coords);
    float2 dir = (coords - origin) * 0.1;

    for (float i = 0; i < 1; i = i + 0.1) {
        color += tex2D(uImage0, coords + dir * i) * uIntensity * (1 - i);
        color += tex2D(uImage0, coords + dir * -i) * uIntensity * (1 - i);
    }

    return color;
}

Technique Technique1
{
    pass ScreenFlash
    {
        PixelShader = compile ps_2_0 Flash();
    }
}