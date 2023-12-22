sampler uImage0 : register(S0);
sampler uImage1 : register(S1);

float progress;

texture tex;
sampler endTex = sampler_state 
{
	texture = tex;
};

float4 ImageLerp(float2 coords : TEXCOORD) : COLOR
{
	float4 t1 = tex2D(uImage0, coords);
	float4 t2 = tex2D(endTex, coords);
	
	float4 result = lerp(t2, t1, progress);
	result *= t1.a;

	return result;
}

technique technique1 {
	pass ImageLerp {
		PixelShader = compile ps_2_0 ImageLerp();
	}
}