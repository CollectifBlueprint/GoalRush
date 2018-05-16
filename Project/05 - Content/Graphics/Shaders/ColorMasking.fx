//Matrix used by this shader pass
float4x4 World;
float4x4 View;
float4x4 Projection;

float3 Color1;
float3 Color2;
float3 Color3;
float3 Color4;

float Alpha;

texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
    MAGFILTER = Linear;
    MINFILTER = Linear;
    MIPFILTER = Linear;
};

texture Mask;
sampler MaskSampler = sampler_state
{
    Texture = <Mask>;
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

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;
	float4 tex = tex2D(TextureSampler, input.UV);
	float4 mask = tex2D(MaskSampler, input.UV);

	float normalizingConst = 1.0 / (float)(mask.r + mask.g + mask.b + (1 - mask.a));

	//Subjective luminance
	float texLight = (0.299 * tex.r) + (0.587 * tex.g) + (0.114 * tex.b);

	float texMask = (1 - mask.r * normalizingConst - mask.g * normalizingConst - mask.b * normalizingConst - (1-mask.a) * normalizingConst);
	texMask = clamp(0, 1, texMask);

	bool useLightness = true;
	if(!useLightness)
		texLight = 1;
	
	//float3 baseColor = tex.rgb * texMask;

	float3 maskColors = Color1 * mask.r * normalizingConst
					  + Color2 * mask.g * normalizingConst
					  + Color3 * mask.b * normalizingConst
					  + Color4 * (1-mask.a) * normalizingConst;
	
	float3 result = tex.rgb * texMask + maskColors * texLight;

	output.Color.rgb = result;
	output.Color.a = tex.a * Alpha;
	
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