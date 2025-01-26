sampler2D uImage0 : register(s0);
float3 ColorFillColor;
float ColorFillProgress;
float3 uColor;

float4 ColorFill(float2 uv : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
	float4 color = tex2D(uImage0, uv);
	if (!any(color)) return color;
	color = lerp(color, float4(ColorFillColor, color.a), ColorFillProgress);

	return color * sampleColor.a;
}

Technique technique1
{
	pass ColorFill
	{
		PixelShader = compile ps_2_0 ColorFill();
	}
}