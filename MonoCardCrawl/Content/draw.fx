float4x4 World;
float4x4 View;
float4x4 Projection;

float4x4 WorldInverseTranspose;

//float3 DiffuseLightDirection = float3(0.3, -0.4, -0.5);
float4 DiffuseColor = float4(1, 1, 1, 1);
/*float DiffuseIntensity = 1.0;
float3 FillLightDirection = float3(-0.6,0.5,0.2);
float4 FillColor = float4(0.5,0.5,1,0.5);
float FillIntensity = 0.5;
*/

float3 LightPosition[16];// = { float3(2, 2, 2), float3(4,4,4) };
float LightFalloff[16];
float3 LightColor[16];
float ActiveLights = 16;


texture Texture;
texture NormalMap;


sampler normalMapSampler = sampler_state
{
	Texture = (NormalMap);
	MAGFILTER = POINT;
	MINFILTER = POINT;
	MIPFILTER = POINT;
	AddressU = Wrap;
	AddressV = Wrap;
};

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
	float3 Tangent : TEXCOORD1;
	float3 BiNormal : TEXCOORD2;
};

struct TexturedVertexShaderOutput
{
	float4 Position : SV_Position0;
	float2 Texcoord : TEXCOORD0;
	float3 Normal : TEXCOORD2;
	float3 WorldPos : TEXCOORD3;
	float3 Tangent : TEXCOORD4;
	float3 BiNormal : TEXCOORD5;
};


TexturedVertexShaderOutput TexturedVertexShaderFunction(TexturedVertexShaderInput input)
{
    TexturedVertexShaderOutput output;
    input.Position.w = 1.0f;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.WorldPos = worldPosition;

	output.Normal = normalize(mul(input.Normal, World));
	output.Tangent = normalize(mul(input.Tangent, World));
	output.BiNormal = normalize(mul(input.BiNormal, World));
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
	float4 texColor = tex2D(diffuseSampler, input.Texcoord);
	float4 normalMap = tex2D(normalMapSampler, input.Texcoord);
	output.Color = float4(0,0,0,1);

	normalMap = (normalMap * 2.0f) - 1.0f;

    // Calculate the normal from the data in the bump map.
    float3 bumpNormal = (normalMap.x * input.Tangent) + (normalMap.y * input.BiNormal) + (normalMap.z * input.Normal);
	
    // Normalize the resulting bump normal.
    bumpNormal = normalize(bumpNormal);

//output.Color = float4(bumpNormal * 255, 1.0f);
//output.Color.a = 1.0f;
//return output;

	for (int i = 0; i < ActiveLights; ++i)
	{
		float lightAttenuation = 1 - (length(input.WorldPos - LightPosition[i]) / LightFalloff[i]);
		float lightIntensity = clamp(dot(bumpNormal, input.WorldPos - LightPosition[i]), 0, 1);
		output.Color += saturate(texColor * DiffuseColor * float4(LightColor[i],1) * lightAttenuation * lightIntensity);
	}
	//output.Color = DiffuseColor * (1 - (lightIntensity / LightFalloff));
		 /*output.Color = saturate(
		 (DiffuseColor * lightIntensity * diffuse)
		 + (FillColor * dot(normal, FillLightDirection)));*/
		 //output.Color = diffuse;// *0.1;
	output.Color.a = texColor.a;
	clip(texColor.a - 0.1);

	output.Color.r = floor(output.Color.r * 16) / 16;
	output.Color.g = floor(output.Color.g * 16) / 16;
	output.Color.b = floor(output.Color.b * 16) / 16;

    return output;
}

PixelShaderOutput PSTexturedColorNoLight(TexturedVertexShaderOutput input) : COLOR0
{
    PixelShaderOutput output;
	float4 texColor = tex2D(diffuseSampler, input.Texcoord);
    output.Color = texColor * DiffuseColor;
	output.Color.a = texColor.a;
    return output;
}

technique DrawTextured
{
    pass Pass1
    {
		AlphaBlendEnable = true;
		BlendOp = Add;
		SrcBlend = One;
		DestBlend = InvSrcAlpha;
		//////ZEnable = false;
		//////ZWriteEnable = true;
		//////ZFunc = LessEqual;
		//CullMode = None;
		
        VertexShader = compile vs_4_0 TexturedVertexShaderFunction();
        PixelShader = compile ps_4_0 PSTexturedColor();
    }
}

technique DrawNoLight
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 TexturedVertexShaderFunction();
		PixelShader = compile ps_4_0 PSTexturedColorNoLight();
	}
}

