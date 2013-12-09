sampler ColorMapSampler : register(s0);

float4 PixelShaderFunction(float2 Tex: TEXCOORD0) : COLOR0
{
	float4 Color = tex2D(ColorMapSampler, Tex);
	Color.rgb = dot(Color.rgb, float3(0.3, 0.59, 0.11));
	return Color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}