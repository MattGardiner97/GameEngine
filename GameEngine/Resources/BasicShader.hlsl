struct VS_IN
{
	float4 pos : POSITION;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

cbuffer matrixStruct : register(b0)
{
	float4x4 worldViewProj;
};

cbuffer colorStruct : register(b1)
{
	float4 color;
}

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;
	input.pos.w = 1;

	output.pos = mul(input.pos,worldViewProj);
	output.col = color;

	return output;
}

float4 PS(PS_IN input) : SV_TARGET
{
	return input.col;
}