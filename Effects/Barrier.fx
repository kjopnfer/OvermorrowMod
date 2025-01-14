sampler2D input : register(s0);
sampler2D noiseInput : register(s1);

/// <summary>Chooses a single row from the noise map</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
float NoiseSeed : register(C0);

/// <summary>Ratio of noise to original image</summary>
/// <minValue>-1</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>.1</defaultValue>
float Ratio : register(C1);

/// <summary>Time value for pulsating effect</summary>
/// <defaultValue>0</defaultValue>
float Time : register(C2);

/// <summary>Color tint for the barrier</summary>
/// <defaultValue>float4(0.3, 0.6, 1.0, 1.0)</defaultValue>
float4 TintColor : register(C3);

float4 main(float2 uv : TEXCOORD) : COLOR 
{
    // Sample the input texture
    float4 color = tex2D(input, uv);

    // Ignore fully transparent pixels
    if (color.a < 0.01)
    {
        return color; // Return the original transparent pixel
    }

    // Sample noise texture
    float4 noiseSample = tex2D(noiseInput, float2(uv.x, NoiseSeed));

    // Calculate pulsating glow effect
    float glow = abs(sin(Time * 3.0)) * 0.5 + 0.5;

    // Combine the noise and original color based on Ratio
    color.rgb = lerp(noiseSample.rgb, color.rgb, Ratio);

    // Blend the pulsating tint color with the current color
    color.rgb = lerp(color.rgb, TintColor.rgb, glow * 0.4f);

    return color;
}

technique BarrierEffect
{
    pass P0
    {
        PixelShader = compile ps_2_0 main();
    }
}
