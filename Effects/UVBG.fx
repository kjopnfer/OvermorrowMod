float4 uShaderSpecificData;

sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity : register(C0);
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uScreenResolution;
float2 uScreenPosition;

//static float2 targetCoords : TEXCOORD0;
//static float2 targetCoords : TEXCOORD0;
static float2 ImagePos : TEXCOORD0;
static float2 COORD : TEXCOORD0;
float4 UVBG(float2 coords : TEXCOORD0) : COLOR0
{
    float thingx = uSecondaryColor.x;
    float thingy = uSecondaryColor.y;
    //return (255, 255, 255, 255);
    //return (tex2D(uImage1, coords));
    ImagePos = (uSecondaryColor.x, uSecondaryColor.y);
    float2 targetCoords = (0.5f, 0.5f);
    float TotalAlpha = uColor.x;
    float Radius = uColor.y + 16;
    float Timer = uColor.z;
    float Center = pow((coords[0] - targetCoords[0]) * 1920, 2) / pow(2 * sin(coords[0] * Timer) + Radius, 2) + pow((coords[1] - targetCoords[1]) * 1080, 2) / pow(2 * cos(coords[1] * Timer) + Radius, 2);
    /*COARDs = coords;
    COARDs.y = 1 % coords.y - (uTime / 55);*/
    /*if (Center >  0.98f && Center < 1)
    {
        return float4(255, 255, 255, 255);
    }*/
    /*if (coords[0] > 0.5f)
        return float4(0, 255, 0, 128);
    return float4(255, 0, 0, 128);*/
    //COORD = coords + (uSecondaryColor.x, uSecondaryColor.y) - (0.5f, 0.5f);
    if (Center <= 1)
    {
        return tex2D(uImage0, coords + float2(thingx, thingy));
        //float4(0, 255, 0, 255);//(tex2D(uImage1, targetCoords)) * 1;//clamp(Center - 0.5, 0, 1);
    }
    else 
    {
        return tex2D(uImage1, coords + float2(thingx, thingy));
    }
}

technique Technique1
{
    pass UVBG
    {
        PixelShader = compile ps_2_0 UVBG();
    }
}