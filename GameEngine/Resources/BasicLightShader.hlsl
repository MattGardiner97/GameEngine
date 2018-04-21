struct Light
{
	float3 dir;
	float4 ambient;
	float4 diffuse;
};

cbuffer BufferPerFrame
{
	Light light;
};

cbuffer BufferPerObject
{
	float4x4 mvp;
	float4x4 model;
};


struct PS_INPUT
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float3 normal : NORMAL;
};

PS_INPUT VS(float4 inPos : POSITION, float4 col : COLOR, float3 normal : NORMAL)
{
	PS_INPUT output;

	output.pos = mul(inPos, mvp);
	output.normal = mul(normal,model);
	output.col = col;

	return output;
}

float4 PS(PS_INPUT input) : SV_TARGET
{
	input.normal = normalize(input.normal);

	float4 diffuse = input.col;

	float3 finalColor;

	finalColor = diffuse * light.ambient;
	finalColor += saturate(dot(light.dir, input.normal)*light.diffuse * diffuse);

	return float4(finalColor, diffuse.a);
}