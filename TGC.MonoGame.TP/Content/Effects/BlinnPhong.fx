#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 WorldViewProjection;
float4x4 World;
float4x4 InverseTransposeWorld;

float3 ambientColor; // Light's Ambient Color
float3 diffuseColor; // Light's Diffuse Color
float3 specularColor; // Light's Specular Color
float KAmbient;
float KDiffuse;
float KSpecular;
float shininess;
float3 lightPosition;
float3 eyePosition; // Camera position

float3 lightDirection; // La dirección de la luz (hacia adelante)
float cutoffAngle; // Ángulo de corte para la luz focal (en radianes)

texture ModelTexture;
sampler2D textureSampler = sampler_state
{
	Texture = (ModelTexture);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL;
	float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinates : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float4 Normal : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput) 0;

	output.Position = mul(input.Position, WorldViewProjection);
	output.WorldPosition = mul(input.Position, World);
    output.Normal = mul( /*input.Normal*/float4(normalize(input.Normal.xyz), 1.0), InverseTransposeWorld);
	output.TextureCoordinates = input.TextureCoordinates;
	
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 normal = normalize(input.Normal.xyz);
    // Base vectors
	float3 lightDir = normalize(lightPosition - input.WorldPosition.xyz);
	float3 viewDirection = normalize(eyePosition - input.WorldPosition.xyz);
	float3 halfVector = normalize(lightDir + viewDirection);

	// Get the texture texel
	float4 texelColor = tex2D(textureSampler, input.TextureCoordinates.xy);

    // Calculate spotlight effect
	float spotEffect = dot(lightDir, lightDirection); // Producto escalar entre dirección luz y dirección objetivo
	spotEffect = saturate((spotEffect - cos(cutoffAngle)) / (1 - cos(cutoffAngle))); // Atenuación dentro del ángulo
	
	// Calculate the diffuse light
    float NdotL = saturate(dot(normal, lightDir)) * spotEffect;
	float3 diffuseLight = KDiffuse * diffuseColor * NdotL;

	// Calculate the specular light
	float NdotH = dot(normal, halfVector);
	float3 specularLight = sign(NdotL) * KSpecular * specularColor * pow(saturate(NdotH), shininess) * spotEffect;
    
    // Final calculation
	float4 finalColor = float4(saturate(ambientColor * KAmbient + diffuseLight) * texelColor.rgb + specularLight, texelColor.a);
	
	return finalColor;
}


technique BasicColorDrawing
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};