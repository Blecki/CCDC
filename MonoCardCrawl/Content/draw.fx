float4x4 World;
float4x4 View;
float4x4 Projection;

float4x4 WorldInverseTranspose;

float3 DiffuseLightDirection = float3(0.3, -0.4, -0.5);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;
float3 FillLightDirection = float3(-0.6,0.5,0.2);
float4 FillColor = float4(0.5,0.5,1,0.5);
float FillIntensity = 0.5;

texture Texture;

sampler diffuseSampler = sampler_state
{
    Texture = (Texture);
    MAGFILTER = POINT;
    MINFILTER = POINT;
    MIPFILTER = POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct TexturedVertexShaderInput
{
	float4 Position : SV_Position0;
	float3 Normal : NORMAL0;
	float2 Texcoord : TEXCOORD0;
};

struct TexturedVertexShaderOutput
{
	float4 Position : SV_Position0;
	float2 Texcoord : TEXCOORD0;
	float3 Normal : TEXCOORD2;
};


TexturedVertexShaderOutput TexturedVertexShaderFunction(TexturedVertexShaderInput input)
{
    TexturedVertexShaderOutput output;
    input.Position.w = 1.0f;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.Normal = input.Normal;
	output.Texcoord = input.Texcoord;
    return output;
}

struct PixelShaderOutput
{
    float4 Color : COLOR0;
};

PixelShaderOutput PSTexturedColor(TexturedVertexShaderOutput input) : COLOR0
{
    PixelShaderOutput output;
	float4 normal = float4(normalize(mul(input.Normal, WorldInverseTranspose)));
		float4 texColor = tex2D(diffuseSampler, input.Texcoord);
		float4 diffuse = texColor;
		float lightIntensity = dot(normal, DiffuseLightDirection);
		 output.Color = saturate(
		 (DiffuseColor * lightIntensity * diffuse)
		 + (FillColor * dot(normal, FillLightDirection)));
		 //output.Color = diffuse;// *0.1;
	output.Color.a = texColor.a;
    return output;
}

PixelShaderOutput PSMousePick(TexturedVertexShaderOutput input) : COLOR0
{
	PixelShaderOutput output;
	output.Color = DiffuseColor;
	return output;
}

PixelShaderOutput PSTexturedColorNolight(TexturedVertexShaderOutput input) : COLOR0
{
    PixelShaderOutput output;
	float4 texColor = tex2D(diffuseSampler, input.Texcoord);
    output.Color = texColor;
	output.Color.a = texColor.a;
	clip(texColor.a - 0.1);
    return output;
}

technique DrawTextured
{
    pass Pass1
    {
		//AlphaBlendEnable = true;
		//BlendOp = Add;
		//SrcBlend = One;
		//DestBlend = InvSrcAlpha;
		//////ZEnable = false;
		//////ZWriteEnable = true;
		//////ZFunc = LessEqual;
		//CullMode = None;
		
        VertexShader = compile vs_4_0 TexturedVertexShaderFunction();
        PixelShader = compile ps_4_0 PSTexturedColor();
    }
}

technique DrawMousePick
{
    pass Pass1
    {
		AlphaBlendEnable = false;
		BlendOp = Add;
		SrcBlend = One;
		DestBlend = InvSrcAlpha;
		
        VertexShader = compile vs_4_0 TexturedVertexShaderFunction();
        PixelShader = compile ps_4_0 PSMousePick();
    }
}


