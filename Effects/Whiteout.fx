sampler2D uImage0 : register(s0);
float3 WhiteoutColor;
float WhiteoutProgress;

float4 Whiteout(float2 uv : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(uImage0, uv);
	if (!any(color)) return color;
	color = lerp(color, float4(WhiteoutColor, color.a), WhiteoutProgress);
	return color;
}

Technique technique1
{
	pass Whiteout
	{
		PixelShader = compile ps_2_0 Whiteout();
	}
}