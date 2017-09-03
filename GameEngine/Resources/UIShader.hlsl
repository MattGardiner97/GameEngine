struct VS_INPUT
{
	float4 pos : POSITION;
	float4 col : COLOR;
};

struct PS_INPUT
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

PS_INPUT VS(VS_INPUT input)
{
	PS_INPUT result = (PS_INPUT)0;

	result.pos = input.pos;
	result.col = input.col;

	return result;
}

float4 PS(PS_INPUT input) : SV_TARGET
{
	return input.col;
}