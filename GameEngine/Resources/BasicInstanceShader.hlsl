struct VS_IN
{
	float4 pos : POSITION;
	float4 instancePos : INSTANCEPOS;
	float4 instanceCol : INSTANCECOL;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

float4x4 worldViewProj;


PS_IN VS(VS_IN input)
{
	PS_IN output;

	//input.instancePos.w = 1;
	float4 pos = input.pos + input.instancePos;
	pos.w = 1;

	output.pos = mul(pos, worldViewProj);

	output.col = input.instanceCol;

	return output;
}

float4 PS(PS_IN input) : SV_TARGET
{
	return input.col;
}