//Matrix used by this shader pass
float4x4 World;
float4x4 View;
float4x4 Projection;

texture Heat;
sampler heatSampler = sampler_state
{
    Texture = Heat;
    MAGFILTER = Linear;
    MINFILTER = Linear;
    MIPFILTER = Linear;
};

texture Gradient;
sampler gradientSampler = sampler_state
{
    Texture = Gradient;
    MAGFILTER = Linear;
    MINFILTER = Linear;
    MIPFILTER = Linear;
};

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
    float4 Color : COLOR0;
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
	float4 tex = tex2D(heatSampler, input.UV);
	float strength = clamp(tex.r / 6, 0, 0.99);	
	strength = pow(strength, 0.4);
	
	float4 gradient = tex2D(gradientSampler, strength);
	output.Color.rgb = gradient.rgb  * strength;
	output.Color.a = strength * 0.5f;
	
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