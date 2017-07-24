sampler2D S : register(s0);

float4 main(float2 uv: TEXCOORD) : COLOR
{
	float4 C = tex2D(S, uv);

	float x = (C.r + C.g + C.b) / 3;

	return float4(x, x, x, C.a);
}
