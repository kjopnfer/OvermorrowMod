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
float rotationArea;
float rotation;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    //float2 origin = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 origin = float2(0.5, 0.5);
    float4 color = tex2D(uImage0, coords);
    float2 dir2 = (coords - origin);
    float2 dir = dir2 * 0.1;
    float rot = atan2(dir.y, dir.x) + radians(180);
    float bound1 = rotation - rotationArea;
    float bound2 = rotation + rotationArea;
    if (((bound1 > 0 && bound2 < radians(360) && (rot < bound2 && rot > bound1))) || ((bound1 < 0 && (rot > bound1 + radians(360) || rot < bound2)) || (bound2 > radians(360) && (rot < bound2 - radians(360) || rot > bound1))))
    {
        for (float i = 0.; i < 1; i += 0.5)
        {
            color += tex2D(uImage0, coords + dir * i) * uIntensity * (1 - i);
            color += tex2D(uImage0, coords + dir * -i) * uIntensity * (1 - i);
        }
    }

    return color;
}

technique Technique1
{
    pass ModdersToolkitShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}