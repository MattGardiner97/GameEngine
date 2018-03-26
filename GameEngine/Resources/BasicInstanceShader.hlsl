struct VS_IN
{
	float4 pos : POSITION;
	uint instanceID : INSTANCEID;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

cbuffer colorStruct : register(b0)
{
	float4 mainColor;
};

cbuffer matrixStruct : register(b1)
{
	float4x4 worldViewProj[1024];
};


PS_IN VS(VS_IN input)
{
	PS_IN output;

	input.pos.w = 1;

	output.pos = mul(input.pos, worldViewProj[input.instanceID]);

	output.col = mainColor;
	

	return output;
}

float4 PS(PS_IN input) : SV_TARGET
{
	return input.col;
}