//Matrix used by this shader pass
float4x4 World;
float4x4 View;
float4x4 Projection;

float Strength = 1;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
};

struct PixelShaderOutput
{
    // Position
    float4 Intensity : COLOR0;
};

//A simple vertex shader
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4x4 worldViewProjection = mul( mul(World, View), Projection);
    output.Position = mul(input.Position, worldViewProjection);
    output.UV = input.UV;
 
    return output;
}

//This pixel shader write to the G-Buffer using MRT
PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;
	
	float2 UV = input.UV;	
	float2 center = float2(0.5, 0.5);
	
	float radius = 0.5 * 0.80;
	float dist = distance(UV, center) / radius;
	float a = 1 - dist * dist;
	output.Intensity = clamp(a, 0, 1);
	
	return output;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}